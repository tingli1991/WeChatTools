using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace WeChatTools.Web.test
{
    /// <summary>
    /// HdNlsqpy 的摘要说明
    /// </summary>
    public class HdNlsqpy : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string html = string.Empty;
            if (context.Request.UrlReferrer != null)
            {
                 html = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "test/2.html");
            }
            else
            {
                 html = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "test/3.html");
            }
            context.Response.Write(html);
            context.Response.End();
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}