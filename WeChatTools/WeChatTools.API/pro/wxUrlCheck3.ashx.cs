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
    ///   微信域名检测接口--免费的
    /// </summary>
    public class WXUrlCheck3 : IHttpHandler
    {
        private const int DURATION = 24 * 60;
        private static string userIP = "127.0.0.1";
        protected const string POST = "POST";

        private string wxKey = ConfigTool.ReadVerifyConfig("wxCheckApiKey3", "WeChatCheck");


        public void ProcessRequest(HttpContext context)
        {
            string result = string.Empty;
            if (context.Request.HttpMethod.ToUpper().Equals(POST))
            {
                //PostHtml();

                userIP = GetWebClientIp(context);
                context.Response.ContentType = "text/plain";
                TimeSpan dspNow = DateTime.Now.TimeOfDay;

                string urlCheck = context.Request["url"];

                string model = context.Request["model"];

                // string referrer = context.Request.UrlReferrer != null ? context.Request.UrlReferrer.Host.ToLower() : "";

                if (!string.IsNullOrEmpty(urlCheck) && !string.IsNullOrEmpty(model))
                {

                    if (!string.IsNullOrEmpty(context.Request["key"]) && context.Request["key"].Length == 32)
                    {
                        wxKey = context.Request["key"]; //key ,md5值
                    }


                    ServiceApiClient SpVoiceObj2 = null;
                    //    ServiceApiClient SpVoiceObj = null;
                    try
                    {
                        //需要检测的网址                                
                        bool isTrue = urlCheck.StartsWith("http");
                        if (!isTrue) { urlCheck = "http://" + urlCheck; }
                        urlCheck = System.Web.HttpUtility.UrlEncode(urlCheck);

                        string json2 = "{\"Mode\":\"" + model + "\",\"Param\":\"{\'CheckUrl\':\'" + urlCheck + "\',\'UserKey\':\'" + wxKey + "\'}\"}";

                        SpVoiceObj2 = new ServiceApiClient("NetTcpBinding_IServiceApi");
                        SpVoiceObj2.Open();
                        result = SpVoiceObj2.Api(json2);
                        SpVoiceObj2.Close();


                        Logger.WriteLogggerTest("#################################################");
                        Logger.WriteLogggerTest(wxKey + ":" + userIP + ":" + result);
                        Logger.WriteLogggerTest(wxKey + ":" + context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"]);





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
                        LogTools.WriteLine(userIP + ":" + wxKey + ":" + ex.Message);
                    }


                }
                else
                {
                    result = "{\"State\":false,\"Code\":\"003\",\"Data\":\"" + userIP + "\",\"Msg\":\"参数错误,联系管理员qq:391502069!\"}";

                }


            }
            else
            {
                result = "{\"State\":false,\"Code\":\"003\",\"Data\":\"QQ:391502069 \",\"Msg\":\"参数错误,联系管理员qq:391502069!\"}";
            }
            context.Response.Headers.Add("Access-Control-Allow-Origin", "http://www.rrbay.xyz");
            context.Response.Headers.Add("Access-Control-Allow-Methods", "POST");
            context.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
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