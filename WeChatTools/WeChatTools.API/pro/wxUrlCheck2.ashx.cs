
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
    /// 微信域名检测工具--正式使用接口
    /// </summary>
    public class WXUrlCheck2 : IHttpHandler
    {
        private const int DURATION = 24 * 60;
        private static string userIP = "127.0.0.1";
        private string wxCheckApiKey = ConfigTool.ReadVerifyConfig("wxCheckApiKey", "WeChatCheck");
        protected const string GET = "GET";
        public void ProcessRequest(HttpContext context)
        {
            string result = string.Empty;
            if (context.Request.HttpMethod.ToUpper().Equals(GET))
            {
                //正式用
                userIP = GetWebClientIp(context);
                string urlCheck = string.Empty;
                context.Response.ContentType = "text/plain";

                if (!string.IsNullOrEmpty(context.Request["url"]) && !string.IsNullOrEmpty(context.Request["key"]) && context.Request["key"].Length == 32)
                {
                    string userKey = context.Request["key"]; //key ,md5值

                    if (userKey.Trim() == wxCheckApiKey)
                    {
                        result = "{\"State\":false,\"Code\",\"003\",\"Data\":\"" + urlCheck + "\",\"Msg\":\"参数错误,联系管理员qq:391502069!\"}";
                    }
                    else
                    {

                        if (!IsRedis(context, userKey))
                        {
                            result = "{\"State\":false,\"Code\":\"003\",\"Data\":\"" + userKey + "\",\"Msg\":\"当天此key超过间隔24小时100万次请求上限,请稍后再试或者购买新的key!\"}";
                        }
                        else
                        {

                            ServiceApiClient SpVoiceObj2 = null;
                            //  ServiceApiClient SpVoiceObj = null;
                            try
                            {
                                //需要检测的网址
                                urlCheck = context.Request["url"]; //检测的值
                                bool isTrue = urlCheck.StartsWith("http");
                                if (!isTrue) { urlCheck = "http://" + urlCheck; }
                                urlCheck = System.Web.HttpUtility.UrlEncode(urlCheck);

                                string json2 = "{\"Mode\":\"AuthKey\",\"Param\":\"{\'CheckUrl\':\'" + urlCheck + "\',\'UserKey\':\'" + userKey + "\'}\"}";

                                SpVoiceObj2 = new ServiceApiClient("NetTcpBinding_IServiceApi");
                                SpVoiceObj2.Open();
                                result = SpVoiceObj2.Api(json2);
                                SpVoiceObj2.Close();


                                if (!string.IsNullOrEmpty(context.Request.QueryString["callback"]))
                                {
                                    string callBack = context.Request.QueryString["callback"].ToString(); //回调
                                    result = callBack + "(" + result + ")";
                                }
                            }
                            catch (System.ServiceModel.CommunicationException)
                            {
                                //   if (SpVoiceObj != null) SpVoiceObj.Abort();
                                if (SpVoiceObj2 != null) SpVoiceObj2.Abort();
                            }
                            catch (TimeoutException)
                            {
                                // if (SpVoiceObj != null) SpVoiceObj.Abort();
                                if (SpVoiceObj2 != null) SpVoiceObj2.Abort();
                            }
                            catch (Exception ex)
                            {
                                //   if (SpVoiceObj != null) SpVoiceObj.Abort();
                                if (SpVoiceObj2 != null) SpVoiceObj2.Abort();
                                result = "{\"State\":false,\"Code\",\"003\",\"Data\":\"" + urlCheck + "\",\"Msg\":\"请求操作在配置的超时,请联系管理员!\"}";
                                LogTools.WriteLine(userIP + ":" + userKey + ":" + ex.Message);
                            }

                        }
                    }
                }
                else
                {
                    result = "{\"State\":false,\"Code\",\"003\",\"Data\":\"" + urlCheck + "\",\"Msg\":\"参数错误,联系管理员qq:391502069!\"}";

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
            if (String.IsNullOrWhiteSpace(customerIP))
            {

                customerIP = httpContext.Request.ServerVariables["HTTP_CLIENT_IP"];
                if (!String.IsNullOrWhiteSpace(customerIP) && customerIP.Contains(","))
                {
                    customerIP = customerIP.Split(new char[] { ',' })[0];
                }
            }

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


        //每个key 每天最多只能请求700000次
        public static bool IsRedis(HttpContext context, string key)
        {
            if (context.Request.Browser.Crawler) return false;
            if (RedisCacheTools.Exists(key))
            {
                string keycount = "keycount:" + key;
                bool check = RedisCacheTools.Exists(keycount);
                if (check)
                {
                    RedisCacheTools.Incr(keycount);
                    int hit = RedisCacheTools.Get<int>(keycount);
                    if (hit > 700000) return false;
                }
                else
                {
                    DateTime dt = DateTime.Now.Date.AddDays(1);
                    RedisCacheTools.Incr(keycount);

                    RedisCacheTools.Expire(keycount, dt);
                }
            }
            else
            {
                return false;
            }
            return true;
        }



    }
}