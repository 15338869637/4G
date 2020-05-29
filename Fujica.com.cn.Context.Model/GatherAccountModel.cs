using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.Context.Model
{
    /// <summary>
    /// 收款账户模型
    /// </summary>
    public class GatherAccountModel : IBaseModel
    {
        /// <summary>
        /// 项目编码
        /// </summary>
        public string ProjectGuid { get; set; }

        /// <summary>
        /// 唯一标识
        /// </summary>
        public string Guid { get; set; }

        /// <summary>
        /// 账户名
        /// </summary>
        public string AccountName { get; set; }

        /// <summary>
        /// 支付宝账号
        /// </summary>
        public AliPayAccountModel AlipayAccount { get; set; }

        /// <summary>
        /// 微信账号
        /// </summary>
        public WeChatAccountModel WechatAccount { get; set; }

        /// <summary>
        /// 所有支付服务商账户模型 支付宝 微信 等
        /// </summary>
        //public List<IBaseModel> paymentISV { get; set; }

        /********************************下面为具体支付服务商账户的模型*******************************/

        /// <summary>
        /// 支付宝账户模型
        /// </summary>
        public class AliPayAccountModel
        {
            public string Appid { get; set; }

            public string Partner { get; set; }

            public string Seller { get; set; }

            public string Priverkey { get; set; }

            public string Publickey { get; set; }

            public bool Usev2 { get; set; }
        }

        /// <summary>
        /// 微信账户模型
        /// </summary>
        public class WeChatAccountModel
        {
            public string Appid { get; set; }

            public string Mchid { get; set; }

            public string Submchid { get; set; }

            public string Privatekey { get; set; }

            public string Secert { get; set; }

            public bool Usesubmch { get; set; }
        }
    }
}
