using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Fujica.com.cn.Interface.Management.Models.InPut
{
    /// <summary>
    /// 设备操作请求实体
    /// </summary>
    public class DeviceRequest:RequestBase
    {
        /// <summary>
        /// 停车场编码
        /// </summary>
        [Required(ErrorMessage = "停车场编码不能为空")]
        public string ParkingCode { get; set; }

        /// <summary>
        /// 设备地址
        /// </summary>
        [Required(ErrorMessage = "设备地址不能为空")]
        public string DeviceMacAddress { get; set; }

        /// <summary>
        /// 进出类型 0入 2出
        /// </summary> 
        public int EntranceType { get; set; }

        /// <summary>
        /// 通道名称
        /// </summary>
        public string ThroughName { get; set; }
        /// <summary>
        /// 识别相机
        /// </summary>
        public string DiscernCamera { get; set; }
        /// <summary> 
        /// 通行方式  1 自动开闸、2 手动开闸、3 免费开闸
        /// </summary>
        public int ThroughType { get; set; } 
        /// <summary>
        /// 开闸原因
        /// </summary>
        public string OpenGateReason { get; set; }
        /// <summary>
        /// 值班人员
        /// </summary>
        public string OpenGateOperator { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 图片地址
        /// </summary>
        public string ImageUrl { get; set; }

    }
}