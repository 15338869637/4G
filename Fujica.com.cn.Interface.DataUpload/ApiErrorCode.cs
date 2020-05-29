using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Fujica.com.cn.Interface.DataUpload
{

    /// <summary>
    /// 基本错误枚举
    /// </summary>
    public enum ApiBaseErrorCode
    {
        /// <summary>
        /// 请求成功
        /// </summary>
        [Description("接口请求成功")]
        API_SUCCESS=200,
        /// <summary>
        /// 请求失败
        /// </summary>
        [Description("接口请求失败")]
        API_FAIL = 201,
        /// <summary>
        /// 请求未授权
        /// </summary>
        [Description("请求未授权")]
        API_Unauthorized = 202,
        /// <summary>
        /// 服务器出现异常
        /// </summary>
        [Description("服务器出现异常")]
        API_ERROR = 203,
        /// <summary>
        /// 参数验证错误
        /// </summary>
        [Description("参数验证错误")]
        API_PARAM_ERROR = 204,
        /// <summary>
        /// 没有此接口的请求权限
        /// </summary>
        [Description("没有此接口的请求权限")]
        API_INTERFACENAME_ERROR = 205,
    }
}