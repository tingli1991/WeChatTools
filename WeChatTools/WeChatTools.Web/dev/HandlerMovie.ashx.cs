using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using WeChatTools.Core;

namespace WeChatTools.Web.dev
{
    /// <summary>
    /// HandlerMovie 的摘要说明
    /// </summary>
    public class HandlerMovie : IHttpHandler
    {

        private string gotoRedirectUrl = "/404.html";//最终用户访问的网址

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";
            string jump = QueryString("key");//参数1：授权key
            string getJump = jump;//
            string getUrl = QueryString("qqmap");//参数1：用户传当前推广的网址,url需要编码 
            if (string.IsNullOrEmpty(getJump) || string.IsNullOrEmpty(getUrl))
            {

                context.Response.Redirect(gotoRedirectUrl);
            }
            else
            {
                getJump = getJump + "!" + getUrl;
                string domainLeft = "https://";
                string html = string.Empty;
                try
                {
                    string domainCenter = GetRandHostUrl();
                    gotoRedirectUrl = domainLeft + domainCenter + "/HomeMovie.ashx";
                    // string xxx =PostHtml(gotoRedirectUrl, getJump);

                    html = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "dev/sply.html");
                    html = html.Replace("$newsView", gotoRedirectUrl).Replace("$newsValue", getJump);
                    //  LogTools.WriteLine(getJump);
                }
                catch (Exception ex)
                {
                    context.Response.Redirect("/404.html");
                }
                context.Response.Write(html);
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





        public string PostHtml(string URL, string postData)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] data = encoding.GetBytes(postData);//Encoding.UTF8.GetBytes(postData);

            HttpWebRequest httpWebRequest;
            HttpWebResponse webResponse;
            Stream getStream;
            StreamReader streamReader;

            httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(URL);
            httpWebRequest.Accept = "application/json, text/javascript, */*; q=0.01";
            httpWebRequest.Referer = URL;
            // httpWebRequest.TransferEncoding = "gzip, deflate";

            httpWebRequest.ContentLength = data.Length;
            httpWebRequest.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            //  httpWebRequest.Host = "183.6.175.51:8000";
            //  httpWebRequest.UserAgent = GetUserAgent();
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


        private string GetRandHostUrl()
        {
            Random ran = new Random();

            string randUrl = "";

            string hosturl = ConfigTool.ReadVerifyConfig("Domain", "JumpDomain");//这些域名都需要指向用户最终要访问的站点
            string[] sArray = hosturl.Split(',');

            int RandKey1 = ran.Next(0, sArray.Length);//随机选中域名
            randUrl = sArray[RandKey1];

            return randUrl;
        }


        private static char[] constant =   
         {   
            '0','1','2','3','4','5','6','7','8','9',  
            'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z'   
         };

        private static string GenerateRandomNumber(int Length)
        {
            System.Text.StringBuilder newRandom = new System.Text.StringBuilder(36);
            Random rd = new Random();
            for (int i = 0; i < Length; i++)
            {
                newRandom.Append(constant[rd.Next(36)]);
            }
            return newRandom.ToString();
        }

        /// <summary>
        /// url请求里的参数
        /// 过滤非法字符
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string QueryString(string param)
        {
            if (HttpContext.Current.Request[param] == null || HttpContext.Current.Request[param].ToString().Trim() == "")
            {
                return "";
            }
            string ret = HttpContext.Current.Request[param].ToString().Trim();
            ret = ret.Replace(",", "");
            ret = ret.Replace("'", "");
            ret = ret.Replace(";", "");

            return ret;
        }

    }
}