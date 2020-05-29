using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Fujica.com.cn.Interface.Management.Models.InPut
{
    /// <summary>
    /// 计费模板请求实体
    /// </summary>
    public class BillingTemplateRequest : RequestBase
    {
        /// <summary>
        /// 停车场编码
        /// </summary>
        [Required(ErrorMessage = "停车场编码不能为空")]
        public string ParkingCode { get; set; }

        /// <summary>
        /// 卡类型(对应的车类型的guid)
        /// </summary>
        [Required(ErrorMessage = "卡类型不能为空")]
        public string CarTypeGuid { get; set; }

        /// <summary>
        /// 计费模式
        /// 1=小时算费 2=分段算费 3=深圳算费 4=按半小时算费 5=简易分段算费 
        /// 6=分段按小时算费 7=半小时算费(可分段) 8=分段按半小时算费 9=新分段收费标准
        /// 10=分段按15分钟算费
        /// </summary>
        public int ChargeMode { get; set; }

        /// <summary>
        /// 模板实体json字符串
        /// </summary>
        public string TemplateJson { get; set; }
    }
}