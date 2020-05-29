using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Fujica.com.cn.Interface.Management.Models.InPut
{
    /// <summary>
    /// 免费放行请求实体
    /// </summary>
    public class FreeOpenGateRequest 
    { 
        /// <summary>
        /// 停车场编码
        /// </summary> 
        [Required(ErrorMessage = "车场编码不能为空")]
        public string ParkingCode { get; set; }
        /// <summary>
        /// 设备标识
        /// </summary> 
        [Required(ErrorMessage = "设备标识不能为空")]
        public string DeviceIdentify { get; set; }
        /// <summary>
        /// 车牌号
        /// </summary>
        [Required(ErrorMessage = "车牌号不能为空")]
        public string CarNo { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 操作员
        /// </summary> 
        [Required(ErrorMessage = "操作员不能为空")]
        public string TolloPerator { get; set; }
    }

    
}