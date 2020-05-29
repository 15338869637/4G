﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fujica.com.cn.Interface.Management.Models.OutPut
{
    /// <summary>
    /// 账户信息响应实体
    /// </summary>
    public class GatherAccountResponse
    {
        /// <summary>
        /// 停车场编码
        /// </summary>
        public string ParkingCode { get; set; }

        /// <summary>
        /// 账户标识
        /// </summary>
        public string Guid { get; set; }

        /// <summary>
        /// 账户名
        /// </summary>
        public string AccountName { get; set; }

        /// <summary>
        /// 支付宝appid
        /// </summary>
        public string AliPayAppid { get; set; }

        /// <summary>
        /// 支付宝parter
        /// </summary>
        public string AliPayPartner { get; set; }

        /// <summary>
        /// 支付宝seller
        /// </summary>
        public string AliPaySeller { get; set; }

        /// <summary>
        /// 支付宝私钥
        /// </summary>
        public string AliPayPriverkey { get; set; }

        /// <summary>
        /// 支付宝公钥
        /// </summary>
        public string AliPayPublickey { get; set; }

        /// <summary>
        /// 支付宝使用V2版本?
        /// </summary>
        public bool AliPayUseV2 { get; set; }

        /// <summary>
        /// 微信appid
        /// </summary>
        public string WeChatAppid { get; set; }

        /// <summary>
        /// 微信商户id
        /// </summary>
        public string WeChatMchid { get; set; }

        /// <summary>
        /// 微信子商户id
        /// </summary>
        public string WeChatSubmchid { get; set; }

        /// <summary>
        /// 微信私钥
        /// </summary>
        public string WeChatPrivatekey { get; set; }

        /// <summary>
        /// 微信secert
        /// </summary>
        public string WeChatSecert { get; set; }

        /// <summary>
        /// 微信开启子商户
        /// </summary>
        public bool WeChatUseSubmch { get; set; }
    }
}