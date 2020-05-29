using Fujica.com.cn.Context.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fujica.com.cn.Interface.Management.Models.OutPut
{
    /// <summary>
    /// 车类模板输出实体
    /// </summary>
    public class PermanentTemplateResponse: PermanentTemplateModel
    {
        /// <summary>
        /// 车类名
        /// </summary>
        public string CarTypeName { get; set; }
    }
}