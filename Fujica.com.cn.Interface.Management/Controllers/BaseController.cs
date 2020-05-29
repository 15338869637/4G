using Fujica.com.cn.Context.Model;
using Fujica.com.cn.Interface.Management.Filter;
using Fujica.com.cn.Logger;
using Fujica.com.cn.Security.AdmissionControl;
using Fujica.com.cn.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

/***************************************************************************************
* *
* *        File Name        : BaseController.cs
* *        Creator          : Ase
* *        Create Time      : 2019-09-17 
* *        Remark           : API接口的基础Controller类
* *
* *  Copyright (c) 深圳市富士智能系统有限公司.  All rights reserved. 
* ***************************************************************************************/
namespace Fujica.com.cn.Interface.Management.Controllers
{
    /// <summary>
    /// API接口的基础Controller
    /// </summary>
    /// <remarks>
    /// 2019.09.17: 修改 命名格式. Ase <br/>       
    /// </remarks>
    [VerifyRequest]
    public class BaseController : ApiController
    {
        /// <summary>
        /// 日志记录器
        /// </summary>
        public readonly ILogger Logger;

        /// <summary>
        /// 序列化器
        /// </summary>
        public readonly ISerializer Serializer;

        /// <summary>
        /// api接入控制
        /// </summary>
        internal readonly APIAccessControl Apiaccesscontrol;

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="logger">日志器</param>
        /// <param name="serializer">序列化器</param>
        /// <param name="apiaccesscontrol">权限控制器</param>
        public BaseController(ILogger logger, ISerializer serializer, APIAccessControl apiaccesscontrol)
        {
            Logger = logger;
            Serializer = serializer;
            Apiaccesscontrol = apiaccesscontrol;
        }
    }
}
