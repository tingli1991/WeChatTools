using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeChatTools.Web.dev
{
    /// <summary>
    /// HandlerSply2 的摘要说明
    /// </summary>
    public class HandlerSply2 : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string agent = context.Request.UserAgent;
            
            if (agent.Contains("Windows NT") || (agent.Contains("Windows NT") && agent.Contains("compatible; MSIE 9.0;")))
            { 
               //https:
            }
            else if (agent.Contains("Macintosh") || agent.Contains("iPhone") || agent.Contains("iPod") | agent.Contains("iPad") | agent.Contains("Windows Phone"))
            {
                //http:
            }
            else
            {
                //https:
            }

            context.Response.ContentType = "text/plain";
            context.Response.Write(agent);
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