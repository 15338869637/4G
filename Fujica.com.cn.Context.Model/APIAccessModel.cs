using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.Context.Model
{
    /// <summary>
    /// API接入信息模型
    /// </summary>
    public class APIAccessModel:IBaseModel
    {
        /// <summary>
        /// 是否允许第三方接入api,0-否,1-是
        /// </summary>
        public int Enable { get; set; }

        /// <summary>
        /// appid
        /// </summary>
        public string AppID { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Secret { get; set; }

        /// <summary>
        /// 公钥
        /// </summary>
        public string PublicKey { get; set; }

        /// <summary>
        /// 私钥
        /// </summary>
        public string PrivateKey { get; set; }

        /// <summary>
        /// 私钥PKCS8对齐
        /// </summary>
        public string PrivateKeyPKCS8 { get; set; }

        /// <summary>
        /// 是否需要验证,1 时间戳验证、2 签名验证、 4 接口权限验证、 8参数验证
        /// </summary>
        public int NeedVerify { get; set; }
    }
}
