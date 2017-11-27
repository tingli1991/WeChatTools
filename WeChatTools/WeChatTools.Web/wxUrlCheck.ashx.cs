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
        public void ProcessRequest(HttpContext context)
        {
            string key = GetExtenalIpAddress_0();
            //string key = context.Request.ServerVariables.Get("Remote_Addr").ToString();
            //  string key = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"]; ;
            if (!IsValid(context))
            {

                context.Response.Write(key + ":当天请求上限,请明天再试,需要对接接口的，请联系技术人员 QQ:308463776");
                context.Response.End();
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
                context.Response.Write(key + ":" + result);
                context.Response.End();
            }
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


        public static bool IsValid(HttpContext context)
        {
            if (context.Request.Browser.Crawler) return false;
            string key = context.Request.UserHostAddress;

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