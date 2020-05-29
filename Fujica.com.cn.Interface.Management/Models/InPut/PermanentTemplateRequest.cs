using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Fujica.com.cn.Interface.Management.Models.InPut
{
    /// <summary>
    /// 固定车模板请求实体
    /// </summary>
    public class PermanentTemplateRequest : RequestBase
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

        ///// <summary>
        ///// 月数
        ///// </summary>
        //public uint Months { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 操作员
        /// </summary>
        public string OperateUser { get; set; }
    }

    /// <summary>
    /// 固定车模板修改实体
    /// </summary>
    public class ModifyPermanentTemplateRequest:PermanentTemplateRequest
    {
        /// <summary>
        /// 固定车模板唯一标识
        /// </summary>
        [Required(ErrorMessage = "固定车模板唯一标识不能为空")]
        public string Guid { get; set; }
    }
}