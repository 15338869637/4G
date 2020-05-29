using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Fujica.com.cn.Interface.Management.Models.InPut
{
    /// <summary>
    /// 业务请求基类
    /// </summary>
    public class RequestBase
    {
        /// <summary>
        /// 项目编码
        /// </summary>
        [Required(ErrorMessage = "项目编码不能为空")]
        public string ProjectGuid { get; set; }
    }
}