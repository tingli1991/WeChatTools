
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using WeChatTools.Core;

namespace WeChatTools.API
{
    /// <summary>
    /// 微信域名检测接口--免费的
    /// </summary>
    public class WXUrlCheck : IHttpHandler
    {
        private const int DURATION = 24 * 60;
        private static string userIP = "127.0.0.1";
        protected const string GET = "GET";
        private string wxCheckApiKey = ConfigTool.ReadVerifyConfig("wxCheckApiKey", "WeChatCheck");
        private TimeSpan _strWorkingDayAM = DateTime.Parse("08:00").TimeOfDay;//工作时间上午08:00
        private TimeSpan _strWorkingDayPM = DateTime.Parse("21:00").TimeOfDay;
        public void ProcessRequest(HttpContext context)
        {
            string result = string.Empty;
            if (context.Request.HttpMethod.ToUpper().Equals(GET))
            {

                //PostHtml();
                //lppsd.zq6kcwhbpvg2twb.com
                //vftkt.n06th8owuihzhhs.com
                context.Response.Headers.Add("Access-Control-Allow-Origin", "http://wx.rrbay.com");
                context.Response.Headers.Add("Access-Control-Allow-Methods", "GET");

                userIP = GetWebClientIp(context);
                context.Response.ContentType = "text/plain";
                TimeSpan dspNow = DateTime.Now.TimeOfDay;
                 
                string urlCheck = string.Empty;
                string callBack = string.Empty;
                if (IsInTimeInterval(dspNow, _strWorkingDayAM, _strWorkingDayPM))
                {
                    string referrer = context.Request.UrlReferrer != null ? context.Request.UrlReferrer.Host.ToLower() : "";
                    callBack = string.IsNullOrEmpty(context.Request.QueryString["callback"]) ? "" : context.Request.QueryString["callback"].ToString(); //回调
                    if (!string.IsNullOrEmpty(context.Request["url"]) && string.IsNullOrEmpty(callBack) && (string.IsNullOrEmpty(referrer) || referrer.ToLower().Contains("v2ex.com") || referrer.ToLower().Contains("hexun.com") || referrer.ToLower().Contains("cnblogs.com") || referrer.ToLower().Contains("zhihu.com") || referrer.ToLower().Contains("csdn.net") || referrer.ToLower().Contains("rrbay.xyz")))
                    {
                        //需要检测的网址
                        urlCheck = context.Request["url"]; //检测的值
                        string[] sArray = urlCheck.Split('.');
                        if (sArray.Length == 3 && sArray[1].Length == 15)
                        {
                            result = "{\"State\":false,\"Code\":\"002\",\"Data\":\"" + urlCheck + "\",\"Msg\":\"歇一歇,访问太快了,联系管理员qq:391502069\"}";
                        }
                        else
                        {
                            if (!IsRedis(context))
                            {
                                result = "{\"State\":false,\"Code\":\"003\",\"Data\":\"" + userIP + "\",\"Msg\":\"当天请求上限,请明天再试,需要讨论技术,联系管理员qq:391502069!\"}";
                            }
                            else
                            {


                                if (!string.IsNullOrEmpty(context.Request["key"]) && context.Request["key"].Length == 32)
                                {
                                    wxCheckApiKey = context.Request["key"]; //key ,md5值
                                }

                                ServiceApiClient SpVoiceObj2 = null;
                                //    ServiceApiClient SpVoiceObj = null;
                                try
                                {

                                    bool isTrue = urlCheck.StartsWith("http");
                                    if (!isTrue) { urlCheck = "http://" + urlCheck; }
                                    urlCheck = System.Web.HttpUtility.UrlEncode(urlCheck);

                                    string json2 = "{\"Mode\":\"AuthKey\",\"Param\":\"{\'CheckUrl\':\'" + urlCheck + "\',\'UserKey\':\'" + wxCheckApiKey + "\'}\"}";

                                    SpVoiceObj2 = new ServiceApiClient("NetTcpBinding_IServiceApi");
                                    SpVoiceObj2.Open();
                                    result = SpVoiceObj2.Api(json2);
                                    SpVoiceObj2.Close();
                                    //JsonObject.Results aup = JsonConvert.DeserializeObject<JsonObject.Results>(result);

                                    //if (aup.State == true)
                                    //{
                                    //    string json = "{\"Mode\":\"WXCheckUrl\",\"Param\":\"{\'CheckUrl\':\'" + urlCheck + "\',\'UserKey\':\'" + wxCheckApiKey + "\'}\"}";
                                    //    SpVoiceObj = new ServiceApiClient("NetTcpBinding_IServiceApi");
                                    //    SpVoiceObj.Open();
                                    //    result = SpVoiceObj.Api(json);
                                    //    SpVoiceObj.Close();

                                    //}
                                    Logger.WriteLogggerTest("#################################################");
                                    Logger.WriteLogggerTest(wxCheckApiKey + ":" + userIP + ":" + result);
                                    Logger.WriteLogggerTest(wxCheckApiKey + ":" + context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"]);
                                   


                                }
                                catch (System.ServiceModel.CommunicationException)
                                {
                                    //  if (SpVoiceObj != null) SpVoiceObj.Abort();
                                    if (SpVoiceObj2 != null) SpVoiceObj2.Abort();
                                }
                                catch (TimeoutException)
                                {
                                    //   if (SpVoiceObj != null) SpVoiceObj.Abort();
                                    if (SpVoiceObj2 != null) SpVoiceObj2.Abort();
                                }
                                catch (Exception ex)
                                {
                                    //   if (SpVoiceObj != null) SpVoiceObj.Abort();
                                    if (SpVoiceObj2 != null) SpVoiceObj2.Abort();
                                    result = "{\"State\":false,\"Code\":\"003\",\"Data\":\"" + urlCheck + "\",\"Msg\":\"请求操作在配置的超时,请联系管理员!\"}";
                                    LogTools.WriteLine(userIP + ":" + wxCheckApiKey + ":" + ex.Message);
                                }

                            }
                        }
                    }
                    else
                    {
                        result = "{\"State\":false,\"Code\":\"003\",\"Data\":\"" + userIP + "\",\"Msg\":\"参数错误,联系管理员qq:391502069!\"}";

                    }
                }
                else
                {
                    result = "{\"State\":false,\"Code\":\"003\",\"Data\":\"" + userIP + "\",\"Msg\":\"测试接口,请在每天(08:00-21:00)时间段进行测试,需要讨论技术,联系管理员qq:391502069.\"}";
                }
                if (!string.IsNullOrEmpty(callBack))
                {
                    result = callBack + "(" + result + ")";
                }
            }
            else
            {
                result = "{\"State\":false,\"Code\",\"003\",\"Data\":\"QQ:391502069 \",\"Msg\":\"参数错误,联系管理员qq:391502069!\"}";
            }
            context.Response.Write(result);
            context.Response.End();
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        CookieContainer _cc = new CookieContainer();

        public string PostHtml()
        {
            Random Rdm = new Random();
            //产生0到100的随机数
            int iRdm = Rdm.Next(423456789, 499999999);

            string wxuin = iRdm.ToString();
            string pass_ticket = "1KLZo5j/85JCXGrbyj5vH6Wn2Ek1qDqjqj2U5tik1232P47mLxmwM+avvXgfWjy5";
            string appmsg_token = "954_1hcOzNorDwiokamoRrnyUm3rQ1fVwUQk-ZDN0s061MxSYtM-BH5313uQ0n5bDgdUat4FJSVA7RrhkCIN";

            string postData = "action=vote&__biz=MzA4MDIzOTQ5OQ%3D%3D&uin=777&key=777&pass_ticket=" + pass_ticket + "&appmsg_token=" + appmsg_token + "&f=json&json=%7B%22super_vote_item%22%3A%5B%7B%22vote_id%22%3A495474521%2C%22item_idx_list%22%3A%7B%22item_idx%22%3A%5B%2216%22%5D%7D%7D%2C%7B%22vote_id%22%3A495474522%2C%22item_idx_list%22%3A%7B%22item_idx%22%3A%5B%2219%22%5D%7D%7D%5D%2C%22super_vote_id%22%3A495474497%7D&idx=2&mid=2653078119&wxtoken=777";
            string cookieStr = "rewardsn=; wxuin=" + wxuin + "; devicetype=android-23; version=26060636; lang=zh_CN; pass_ticket=" + pass_ticket + "; wap_sid2=CLfb39UBElw3ZDlXaU5iNlVsYzB0UVlia3NvZktSWHpoM3FfVl9udFhBWlhJdlRrV0N4NVVwTUZ3V2ZCYW5aWUZrTkxMSVBZYlZyc2xUbTc0THZmWE16ZDNBWEkxYm9EQUFBfjCyyoTXBTgNQAE=; wxtokenkey=777";
            string RefererURl = "https://mp.weixin.qq.com/mp/newappmsgvote?action=show&__biz=MzA4MDIzOTQ5OQ==&supervoteid=495474497&uin=777&key=777&pass_ticket=" + pass_ticket + "&wxtoken=777&mid=2653078119&idx=2&appmsg_token=" + appmsg_token;
            string URL = "https://mp.weixin.qq.com/mp/newappmsgvote";

            string[] cookstr = cookieStr.Split(';');
            foreach (string str in cookstr)
            {
                string[] cookieNameValue = str.Split('=');
                Cookie ck = new Cookie(cookieNameValue[0].Trim().ToString(), cookieNameValue[1].Trim().ToString());
                ck.Domain = "mp.weixin.qq.com";
                _cc.Add(ck);
            }


            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] data = encoding.GetBytes(postData);//Encoding.UTF8.GetBytes(postData);

            HttpWebRequest httpWebRequest;
            HttpWebResponse webResponse;
            Stream getStream;
            StreamReader streamReader;

            httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(URL);
            httpWebRequest.Host = "mp.weixin.qq.com";
            httpWebRequest.ContentLength = data.Length;
            httpWebRequest.Accept = "application/json";
            httpWebRequest.Headers.Add("Origin", "https://mp.weixin.qq.com");
            httpWebRequest.Headers.Add("X-Requested-With", "XMLHttpRequest");
            httpWebRequest.UserAgent = "Mozilla/5.0 (Linux; Android 6.0; 1501-A02 Build/MRA58K; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/57.0.2987.132 MQQBrowser/6.2 TBS/044028 Mobile Safari/537.36 MicroMessenger/6.6.6.1300(0x26060636) NetType/WIFI Language/zh_CN";
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.Referer = RefererURl;
            httpWebRequest.Headers.Add("Accept-Encoding", "gzip, deflate");
            httpWebRequest.Headers.Add("Accept-Language", "zh-CN,en-US;q=0.8");
            httpWebRequest.Headers.Add("Q-UA2", "QV=3&PL=ADR&PR=WX&PP=com.tencent.mm&PPVN=6.6.6&TBSVC=43607&CO=BK&COVC=044028&PB=GE&VE=GA&DE=PHONE&CHID=0&LCID=9422&MO= 1501-A02 &RL=720*1280&OS=6.0&API=23");
            httpWebRequest.Headers.Add("Q-GUID", "04dd8ef186bdc34bdedac5a413b788cb");
            httpWebRequest.Headers.Add("Q-Auth", "31045b957cf33acf31e40be2f3e71c5217597676a9729f1b");
            httpWebRequest.CookieContainer = _cc;
            httpWebRequest.Method = "POST";




            // httpWebRequest.AllowAutoRedirect = true;
            Stream pReqStream = httpWebRequest.GetRequestStream();
            // Send the data.
            pReqStream.Write(data, 0, data.Length);
            pReqStream.Close();

            webResponse = (HttpWebResponse)httpWebRequest.GetResponse();

            getStream = webResponse.GetResponseStream();
            streamReader = new StreamReader(getStream, Encoding.UTF8);
            string getString = "";
            getString = streamReader.ReadToEnd();

            streamReader.Close();
            getStream.Close();
            webResponse.Close();

            return getString;
        }


        /// <summary>  
        /// 将c# DateTime时间格式转换为Unix时间戳格式  
        /// </summary>  
        /// <param name="time">时间</param>  
        /// <returns>long</returns>  
        public static long ConvertDateTimeToInt()
        {
            System.DateTime time = DateTime.Now;
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            long t = (time.Ticks - startTime.Ticks) / 10000;   //除10000调整为13位      
            return t;
        }

        private bool IsInTimeInterval(TimeSpan time, TimeSpan startTime, TimeSpan endTime)
        {
            //判断时间段开始时间是否小于时间段结束时间,如果不是就交换
            if (startTime > endTime)
            {
                TimeSpan tempTime = startTime;
                startTime = endTime;
                endTime = tempTime;
            }

            if (time > startTime && time < endTime)
            {
                return true;
            }
            return false;
        }

        /// <summary>  
        /// 获取外网ip地址  
        /// </summary>  
        public static string GetExtenalIpAddress()
        {
            String url = "http://city.ip138.com/ip2city.asp";
            string IP = "未获取到外网ip";
            try
            {
                //从网址中获取本机ip数据    
                System.Net.WebClient client = new System.Net.WebClient();
                client.Encoding = System.Text.Encoding.Default;
                string str = client.DownloadString(url);
                client.Dispose();

                //提取外网ip数据 [218.104.71.178]    
                int i1 = str.IndexOf("["), i2 = str.IndexOf("]");

                if (!str.Equals("")) IP = str.Substring(i1 + 1, i2 - 1 - i1);
                else IP = GetExtenalIpAddress_0();
            }
            catch (Exception) { }

            return IP;
        }

        /// <summary>  
        /// 获取外网ip地址  
        /// </summary>  
        public static string GetExtenalIpAddress_0()
        {
            var tempIp = "";
            try
            {
                WebRequest wr = (HttpWebRequest)WebRequest.Create("http://ip.chinaz.com/getip.aspx");
                var stream = wr.GetResponse().GetResponseStream();
                var sr = new StreamReader(stream, Encoding.GetEncoding("gb2312"));
                var all = sr.ReadToEnd();
                //读取网站的数据
                int start = all.IndexOf("[") + 1;
                int end = all.IndexOf("]", start);
                tempIp = all.Substring(start, end - start);
                sr.Close();
                stream.Close();
            }
            catch
            {
                // ignored
            }
            return tempIp;
        }


        public static string GetWebClientIp(HttpContext httpContext)
        {
            string customerIP = "127.0.0.1";

            if (httpContext == null || httpContext.Request == null || httpContext.Request.ServerVariables == null) return customerIP;

            customerIP = httpContext.Request.ServerVariables["HTTP_CDN_SRC_IP"];

            if (String.IsNullOrWhiteSpace(customerIP) || "unknown".Equals(customerIP.ToLower()))
            {

                customerIP = httpContext.Request.ServerVariables["Proxy-Client-IP"];
            }
            if (String.IsNullOrWhiteSpace(customerIP) || "unknown".Equals(customerIP.ToLower()))
            {

                customerIP = httpContext.Request.ServerVariables["WL-Proxy-Client-IP"];
            }
            /*
            if (String.IsNullOrWhiteSpace(customerIP) || "unknown".Equals(customerIP.ToLower()))
            {

                customerIP = httpContext.Request.ServerVariables["HTTP_VIA"];
            }
            */
            if (String.IsNullOrWhiteSpace(customerIP) || "unknown".Equals(customerIP.ToLower()))
            {

                customerIP = httpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (!String.IsNullOrWhiteSpace(customerIP) && customerIP.Contains(","))
                {
                    string[] xx = customerIP.Split(new char[] { ',' });
                    if (xx.Length > 1)
                    {
                        customerIP = xx[xx.Length - 2].Trim();
                    }
                    else
                    {
                        customerIP = xx[0];

                    }
                }
            }
            if (String.IsNullOrWhiteSpace(customerIP))
            {

                customerIP = httpContext.Request.ServerVariables["HTTP_CLIENT_IP"];
                if (!String.IsNullOrWhiteSpace(customerIP) && customerIP.Contains(","))
                {
                    customerIP = customerIP.Split(new char[] { ',' })[0];
                }
            }
            if (String.IsNullOrWhiteSpace(customerIP))
            {

                customerIP = httpContext.Request.ServerVariables["REMOTE_ADDR"];
                if (!String.IsNullOrWhiteSpace(customerIP) && customerIP.Contains(","))
                {
                    customerIP = customerIP.Split(new char[] { ',' })[0];
                }

            }

            if (!IsIP(customerIP))
            {
                customerIP = "127.0.0.1";
            }
            return customerIP;
        }

        public static string GetRealIP(string CustomerIP)
        {
            string result = String.Empty;

            result = CustomerIP;

            //可能有代理   
            if (!string.IsNullOrWhiteSpace(result))
            {
                //没有"." 肯定是非IP格式  
                if (result.IndexOf(".") == -1)
                {
                    result = null;
                }
                else
                {
                    //有",",估计多个代理.取第一个不是内网的IP.  
                    if (result.IndexOf(",") != -1)
                    {
                        result = result.Replace(" ", string.Empty).Replace("\"", string.Empty);

                        string[] temparyip = result.Split(",;".ToCharArray());

                        if (temparyip != null && temparyip.Length > 0)
                        {
                            for (int i = 0; i < temparyip.Length; i++)
                            {
                                //找到不是内网的地址  
                                if (IsIP(temparyip[i])
                                    && temparyip[i].Substring(0, 3) != "10."
                                    && temparyip[i].Substring(0, 7) != "192.168"
                                    && temparyip[i].Substring(0, 7) != "172.16.")
                                {
                                    return temparyip[i];
                                }
                            }
                        }
                    }
                    //代理即是IP格式  
                    else if (IsIP(result))
                    {
                        return result;
                    }
                    //代理中的内容非IP  
                    else
                    {
                        result = "";
                    }
                }
            }


            return result;
        }

        /// <summary>
        /// 检查IP地址格式
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsIP(string ip)
        {
            if (!String.IsNullOrWhiteSpace(ip))
            {
                return System.Text.RegularExpressions.Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
            }
            else
            {
                return false;
            }

        }

        public static bool IsValid(HttpContext context)
        {
            if (context.Request.Browser.Crawler) return false;
            string key = userIP;

            int hit = (Int32)(context.Cache[key] ?? 0);
            if (hit > 3) return false;
            else hit++;

            if (hit == 1)
            {
                context.Cache.Add(key, hit, null, DateTime.Now.AddMinutes(DURATION), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Normal, null);

            }
            else
            {
                context.Cache[key] = hit;
            }
            return true;
        }

        public static bool IsRedis(HttpContext context)
        {
            if (context.Request.Browser.Crawler) return false;
            string key = userIP;
            bool check = RedisCacheTools.Exists(key);
            if (check)
            {
                RedisCacheTools.Incr(key);
                int hit = RedisCacheTools.Get<int>(key);
                if (hit > 16) return false;
                /*
                    $redis->incr($key);
                    $count = $redis->get($key);
                    if($count > 5){
                        exit('请求太频繁，请稍后再试！');
                    }
                  */
            }
            else
            {
                DateTime dt = DateTime.Now.AddDays(1);
                RedisCacheTools.Incr(key);
                /*
                    $redis->incr($key);
	                //限制时间为60秒 
	                $redis->expire($key,60)  
                */
                RedisCacheTools.Expire(key, dt);
            }

            return true;
        }
    }
}