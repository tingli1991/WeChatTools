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
    /// 短链接生成接口--测试接口
    /// </summary>
    public class shorturl3 : IHttpHandler
    {
         
        private static string userIP = "127.0.0.1";
        protected const string POST = "POST";
        private string shorturlkey = ConfigTool.ReadVerifyConfig("shorturlKey", "WeChatCheck");
        private TimeSpan _strWorkingDayAM = DateTime.Parse("08:00").TimeOfDay;//工作时间上午08:00
        private TimeSpan _strWorkingDayPM = DateTime.Parse("21:00").TimeOfDay;

        public void ProcessRequest(HttpContext context)
        {
            string result = string.Empty;
            if (context.Request.HttpMethod.ToUpper().Equals(POST))
            {
                string url = context.Request["url"];                   
                string type = context.Request["type"]; //key ,md5值
                context.Response.ContentType = "text/plain";
                TimeSpan dspNow = DateTime.Now.TimeOfDay;

                if (IsInTimeInterval(dspNow, _strWorkingDayAM, _strWorkingDayPM) && !string.IsNullOrEmpty(url))
                {
                    if (!IsRedis(context))
                    {
                        result = "{\"State\":false,\"Code\":\"003\",\"Data\":\"https://url.cn/5mfnDv7\",\"Msg\":\"当天请求上限,请明天再试,需要讨论技术,联系管理员qq:391502069!\"}";
                    }
                    else
                    {
                        ServiceApiClient SpVoiceObj = null;
                        try
                        {
                            
                            if (type.ToUpper() != "URLCN" && type.ToUpper() != "WURLCN")
                            {
                                type = "URLCN";
                            }
                            url = System.Web.HttpUtility.UrlEncode(url);
                            string json2 = "{\"Mode\":\"ShortUrl\",\"Param\":\"{\'CheckUrl\':\'" + url + "\',\'type\':\'" + type + "\',\'UserKey\':\'" + shorturlkey + "\'}\"}";
                            
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
                            result = "{\"State\":false,\"Code\":\"003\",\"Data\":\"https://url.cn/5mfnDv7\",\"Msg\":\"请求操作在配置的超时,请联系管理员!\"}";
                            LogTools.WriteLine(shorturlkey + ":" + ex.Message);
                        }
                    }


                }
                else
                {
                    result = "{\"State\":false,\"Code\":\"003\",\"Data\":\"https://url.cn/5mfnDv7\",\"Msg\":\"测试接口,请在每天(08:00-21:00)时间段进行测试,需要讨论技术,联系管理员qq:391502069.\"}";

                }
            }
            else
            {
                result = "{\"State\":false,\"Code\":\"003\",\"Data\":\"https://url.cn/5mfnDv7\",\"Msg\":\"参数错误,联系管理员qq:391502069!\"}";
            }
            context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
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
                if (hit > 5) return false;
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
    }
}