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
    /// 微信域名检测工具
    /// </summary>
    public class WXUrlCheck : IHttpHandler
    {
        private ServiceApi _service = null;
        private const int DURATION = 24 * 60;
        private static string  userIP="127.0.0.1";
        public void ProcessRequest(HttpContext context)
        {
            userIP = GetWebClientIp();
             
            if (!IsValid(context))
            {

                context.Response.Write(userIP + ":当天请求上限,请明天再试,需要对接接口的，请联系技术人员 QQ:308463776");
              
            }
            else
            {
                _service = new ServiceApi();
                string domain = HttpContext.Current.Request["url"].ToString().Trim();
                domain = domain.Replace("https://", "").Replace("http://", "");
                string json = "{\"Mode\": \"WXCheckUrl\", \"Param\": \"{\'CheckUrl\':\'" + domain + "\'}\"}";
                string result = _service.Api(json);

                Logger.WriteLoggger(result);
                context.Response.ContentType = "text/plain";
                context.Response.Write(result);
              
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


        public static string GetWebClientIp()
        {
            string CustomerIP = "";

            try
            {
                if (System.Web.HttpContext.Current == null
            || System.Web.HttpContext.Current.Request == null
            || System.Web.HttpContext.Current.Request.ServerVariables == null)
                    return "";



                //CDN加速后取到的IP   
                CustomerIP = System.Web.HttpContext.Current.Request.Headers["Cdn-Src-Ip"];
                if (!string.IsNullOrEmpty(CustomerIP) && IsIP(CustomerIP))
                {
                    return CustomerIP;
                }

                CustomerIP = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];


                if (!String.IsNullOrEmpty(CustomerIP) && IsIP(CustomerIP))
                    return CustomerIP;

                if (System.Web.HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null)
                {
                    CustomerIP = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                    if (CustomerIP == null)
                        CustomerIP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }
                else
                {
                    CustomerIP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                }

                if (string.Compare(CustomerIP, "unknown", true) == 0)
                    CustomerIP = System.Web.HttpContext.Current.Request.UserHostAddress;

                if (!IsIP(CustomerIP))
                {
                    CustomerIP = "127.0.0.1";
                }
            }
            catch { }

            return CustomerIP;
        }

        /// <summary>
        /// 检查IP地址格式
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsIP(string ip)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }

        public static bool IsValid(HttpContext context)
        {
            if (context.Request.Browser.Crawler) return false;
            string key = userIP;

            int hit = (Int32)(context.Cache[key] ?? 0);
            if (hit > 15) return false;
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


    }
}