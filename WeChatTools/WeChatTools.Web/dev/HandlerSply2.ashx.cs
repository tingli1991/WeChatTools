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
            string u = context.Request.UserAgent;
            /* 
          var isAndroid = u.IndexOf("Android") > -1 || u.IndexOf("Adr") > -1; //android终端  
          
              var isiOS = !!u.match(/\(i[^;]+;( U;)? CPU.+Mac OS X/); //ios终端  
         if(isAndroid){  
              alert("android");  
          }else if(isiOS){  
              alert("ios");  
          }else{  
          }  
          var ua = window.navigator.userAgent.toLowerCase();  
          if (ua.match(/MicroMessenger/i) == 'micromessenger') {  
              alert("微信");  
          } else {  
              alert("非微信");  
          }  
              */

            context.Response.ContentType = "text/plain";
            context.Response.Write(u);
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