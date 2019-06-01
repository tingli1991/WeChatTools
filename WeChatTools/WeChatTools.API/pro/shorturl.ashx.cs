
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
    /// 短链接生成接口--正式使用接口
    /// </summary>
    public class shorturl : IHttpHandler
    {
        private const int DURATION = 24 * 60;
        
        protected const string GET = "GET";
        public void ProcessRequest(HttpContext context)
        {
            string result = string.Empty;
            if (context.Request.HttpMethod.ToUpper().Equals(GET))
            {
                string url = context.Request["url"];
                string key = context.Request["key"]; //key ,md5值
                string type = context.Request["type"]; //key ,md5值
                context.Response.ContentType = "text/plain";

                if (!string.IsNullOrEmpty(url) && !string.IsNullOrEmpty(key) && key.Length == 32)
                {
                    if (!IsRedis(context, key))
                    {
                        result = "{\"State\":false,\"Code\":\"003\",\"Data\":\"" + key + "\",\"Msg\":\"当天请求上限,请明天再试,需要讨论技术,联系管理员qq:391502069!\"}";
                    }
                    else
                    {
                        ServiceApiClient SpVoiceObj = null;
                        try
                        {
                            if (type.ToUpper() != "DWZCN") { url = System.Web.HttpUtility.UrlEncode(url); }

                            string json2 = "{\"Mode\":\"ShortUrl\",\"Param\":\"{\'CheckUrl\':\'" + url + "\',\'type\':\'" + type + "\',\'UserKey\':\'" + key + "\'}\"}";
                            
                            SpVoiceObj = new ServiceApiClient("NetTcpBinding_IServiceApi");
                            SpVoiceObj.Open();
                            result = SpVoiceObj.Api(json2);
                            SpVoiceObj.Close();


                            if (!string.IsNullOrEmpty(context.Request.QueryString["callback"]))
                            {
                                string callBack = context.Request.QueryString["callback"].ToString(); //回调
                                result = callBack + "(" + result + ")";
                            }
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
                            result = "{\"State\":false,\"Code\",\"003\",\"Data\":\"" + url + "\",\"Msg\":\"请求操作在配置的超时,请联系管理员!\"}";
                            LogTools.WriteLine( key + ":" + ex.Message);
                        }
                    }


                }
                else
                {
                    result = "{\"State\":false,\"Code\",\"003\",\"Data\":\"" + url + "\",\"Msg\":\"参数错误,联系管理员qq:391502069!\"}";

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


        //每个key 24小时只能请求2000次
        public static bool IsRedis(HttpContext context, string key)
        {
            if (context.Request.Browser.Crawler) return false;
            key = "keycount:" + key;
            bool check = RedisCacheTools.Exists(key);
            if (check)
            {
                RedisCacheTools.Incr(key);
                int hit = RedisCacheTools.Get<int>(key);
                if (hit > 5000) return false;
            }
            else
            {
                DateTime dt = DateTime.Now.AddDays(1);
                RedisCacheTools.Incr(key);

                RedisCacheTools.Expire(key, dt);
            }

            return true;
        }
    }
}