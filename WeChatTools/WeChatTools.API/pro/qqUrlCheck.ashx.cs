using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using WeChatTools.Core;

namespace WeChatTools.API.pro
{
    /// <summary>
    /// qq域名检测 - 免费的
    /// </summary>
    public class qqUrlCheck : IHttpHandler
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

                                    string json2 = "{\"Mode\":\"AuthQQKey\",\"Param\":\"{\'CheckUrl\':\'" + urlCheck + "\',\'UserKey\':\'" + wxCheckApiKey + "\'}\"}";

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

        //防止恶意请求
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
    }
}