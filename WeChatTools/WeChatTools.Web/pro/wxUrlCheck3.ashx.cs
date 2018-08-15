using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using WeChatTools.Core;

namespace WeChatTools.Web
{
    /// <summary>
    /// 微信域名检测工具--内部使用接口
    /// </summary>
    public class WXUrlCheck3 : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            string urlCheck = string.Empty;
            context.Response.ContentType = "text/plain";
            string result = string.Empty;
            if (!string.IsNullOrEmpty(context.Request["url"]) && !string.IsNullOrEmpty(context.Request["key"]) && context.Request["key"].Length == 32)
            {
                string userKey = context.Request["key"]; //key ,md5值
                ServiceApiClient SpVoiceObj = null;
                try
                {
                    SpVoiceObj = new ServiceApiClient("NetTcpBinding_IServiceApi");
                    //需要检测的网址
                    urlCheck = context.Request["url"]; //检测的值
                    bool isTrue = urlCheck.StartsWith("http");
                    if (!isTrue) { urlCheck = "http://" + urlCheck; }
                    urlCheck = System.Web.HttpUtility.UrlEncode(urlCheck);


                    string json = "{\"Mode\":\"WXCheckUrl\",\"Param\":\"{\'CheckUrl\':\'" + urlCheck + "\',\'UserKey\':\'" + userKey + "\'}\"}";

                    SpVoiceObj.Open();
                    result = SpVoiceObj.Api(json);
                    SpVoiceObj.Close();


                }
                catch (System.ServiceModel.CommunicationException)
                {
                    if (SpVoiceObj != null) SpVoiceObj.Abort();
                    
                }
                catch (TimeoutException)
                {
                    if (SpVoiceObj != null) SpVoiceObj.Abort();
                     
                }
                catch (Exception ex)
                {
                    if (SpVoiceObj != null) SpVoiceObj.Abort();                  
                    result = "{\"State\":false,\"Data\":\"" + urlCheck + "\",\"Msg\":\"请求操作在配置的超时,请联系管理员!\"}";
                    LogTools.WriteLine( ex.Message);
                }
                context.Response.Write(result);


            }
            else
            {
                context.Response.Write("参数错误,进qq群交流:41977413!");

            }

            context.Response.End();



        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
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