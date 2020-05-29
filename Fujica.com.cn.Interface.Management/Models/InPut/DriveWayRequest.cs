using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Fujica.com.cn.Interface.Management.Models.InPut
{
    /// <summary>
    /// 车道请求实体
    /// </summary>
    public class DriveWayRequest : RequestBase
    {
        /// <summary>
        /// 停车场编码
        /// </summary>
        [Required(ErrorMessage = "停车场编码不能为空")]
        public string ParkingCode { get; set; }

        /// <summary>
        /// 车道名
        /// </summary>
        [Required(ErrorMessage = "车道名不能为空")]
        public string DriveWayName { get; set; }

        /// <summary>
        /// 车道类型(模式) 0=入口车道 1=出口车道
        /// </summary>
        public int DrivewayType { get; set; }

        /// <summary>
        /// 设备名称 自定义就好
        /// </summary>
        [Required(ErrorMessage = "设备名不能为空")]
        public string DeviceName { get; set; }

        /// <summary>
        /// 设备标识 如MAC地址等
        /// </summary>
        [Required(ErrorMessage = "设备物理地址不能为空")]
        public string DeviceMACAddress { get; set; }

        /// <summary>
        /// 设备登录地址
        /// </summary>
        public string DeviceEntranceURI { get; set; }

        /// <summary>
        /// 设备账户
        /// </summary>
        public string DeviceAccount { get; set; }

        /// <summary>
        /// 设备验证口令
        /// </summary>
        public string DeviceVerification { get; set; }
    }

    /// <summary>
    /// 车道修改请求实体
    /// </summary>
    public class ModifyDriveWayRequest : DriveWayRequest
    {
        /// <summary>
        /// 车道标识
        /// </summary>
        [Required(ErrorMessage = "车道标识不能为空")]
        public string Guid { get; set; }
    }

    /// <summary>
    /// 车道删除请求实体
    /// </summary>
    public class DeleteDriveWayRequest : RequestBase
    {
        /// <summary>
        /// 停车场编码
        /// </summary>
        [Required(ErrorMessage = "停车场编码不能为空")]
        public string ParkingCode { get; set; }

        /// <summary>
        /// 车道标识
        /// </summary>
        [Required(ErrorMessage = "车道标识不能为空")]
        public string Guid { get; set; }
    }
}