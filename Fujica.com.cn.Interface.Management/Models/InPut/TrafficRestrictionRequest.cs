using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Fujica.com.cn.Interface.Management.Models.InPut
{
    /// <summary>
    /// 限行规则请求实体
    /// </summary>
    public class TrafficRestrictionRequest : RequestBase
    {
        /// <summary>
        /// 停车场编码
        /// </summary>
        [Required(ErrorMessage = "停车场编码不能为空")]
        public string ParkingCode { get; set; }

        /// <summary>
        /// 限制车道集合,逗号分隔
        /// </summary>
        public string DrivewayGuid { get; set; }

        /// <summary>
        /// 限制车类集合,逗号分隔
        /// </summary>
        public string CarTypeGuid { get; set; }

        /// <summary>
        /// 指定的禁止通行周天,长度为7,依次为周一到周末，为1表示禁止,例如 0010010 表示周三周六禁止
        /// </summary>
        public string AssignDays { get; set; }

        /// <summary>
        /// 禁行开始时间 HH:mm:ss
        /// </summary>
        public string StartTime { get; set; }

        /// <summary>
        /// 禁行结束时间 HH:mm:ss
        /// </summary>
        public string EndTime { get; set; }
    }

    /// <summary>
    /// 限行规则修改实体
    /// </summary>
    public class ModifyTrafficRestrictionRequest: TrafficRestrictionRequest
    {
        /// <summary>
        /// 规则标识
        /// </summary>
        [Required(ErrorMessage = "规则标识不能为空")]
        public string Guid { get; set; }
    }
}