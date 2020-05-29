using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Fujica.com.cn.Interface.Management.Models.InPut
{
    /// <summary>
    /// 开闸原因请求实体
    /// </summary>
    public class OpenGateReasonRequest : RequestBase
    {
        /// <summary>
        /// 开闸类型 0=手动 1=免费
        /// </summary>
        public int OpenType { get; set; }

        /// <summary>
        /// 开闸原因注明
        /// </summary>
        [Required(ErrorMessage = "开闸原因不能为空")]
        public string ReasonRemark { get; set; }
    }

    /// <summary>
    /// 开闸原因修改请求实体
    /// </summary>
    public class ModifyOpenGateReasonRequest : OpenGateReasonRequest
    {
        /// <summary>
        /// 标识
        /// </summary>
        [Required(ErrorMessage = "原因标识不能为空")]
        public string Guid { get; set; }
    }

    /// <summary>
    /// 开闸原因修改请求实体
    /// </summary>
    public class DeleteOpenGateReasonRequest : RequestBase
    {
        /// <summary>
        /// 标识
        /// </summary>
        [Required(ErrorMessage = "原因标识不能为空")]
        public string Guid { get; set; }
    }
}