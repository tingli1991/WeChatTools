using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeChatTools.Core
{
    public class JsonObject
    {
        #region 接口参数
        public class Parameters
        {
            public string Mode { get; set; }

            public string Param { get; set; }
        }
        #endregion

        #region 接口返回
        public class Results
        {
            public bool State { get; set; }

            public string Data { get; set; }

            public string Msg { get; set; }
        }
        #endregion

        #region 微信域名检测
        public class WeiXinUrl
        {
            
            /// <summary>
            /// 加密的userId
            /// </summary>
            public string UserId { get; set; }

            /// <summary>
            /// 司机Id
            /// </summary>
            public string UserKey { get; set; }
            
            /// <summary>
            /// 货主Id
            /// </summary>
            public string CheckUrl { get; set; }

        }
        #endregion
         
         
    }
}
