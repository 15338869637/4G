using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Fujica.com.cn.Interface.Management.Models.InPut
{
    /// <summary>
    /// 车牌修正请求实体
    /// </summary>
    public class ModifyCarNoRequest:RequestBase
    {
        /// <summary>
        /// 停车场编码
        /// </summary>
        [Required(ErrorMessage = "停车场编码不能为空")]
        public string ParkingCode { get; set; }

        /// <summary>
        /// 原车牌
        /// </summary>
        [Required(ErrorMessage = "原车牌不能为空")]
        public string OldCarNo { get; set; }
        /// <summary>
        /// 新车牌
        /// </summary>
        [Required(ErrorMessage = "新车牌不能为空")]
        public string NewCarNo { get; set; }
        /// <summary>
        /// 操作员
        /// </summary>
        public string Operator { get; set; }

        /// <summary>
        /// 入场照片
        /// </summary> 
        public string InImgUrl { get; set; }

        /// <summary>
        /// 入场时间
        /// </summary> 
        public DateTime InTime { get; set; }
        /// <summary>
        /// 入场通道
        /// </summary> 
        public string ThroughName { get; set; }
        /// <summary>
        /// 车类
        /// </summary>
        public string CarType { get; set; }
        /// <summary>
        /// 入口设备号
        /// </summary> 
        public string DriveWayMAC { get; set; } 
        /// <summary>
        /// 停车记录编码
        /// </summary> 
         public string ParkingRecordCode { get; set; } 
        ///// <summary>
        ///// 新车牌
        ///// </summary>
        //[Required(ErrorMessage = "新车牌不能为空")]
        //public string NewCarNo { get; set; }
        ///// <summary>
        ///// 操作员
        ///// </summary>
        //public string Operator { get; set; }
    }
}