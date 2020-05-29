using Fujica.com.cn.Business.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Fujica.com.cn.Interface.Management.Models.InPut
{
    /// <summary>
    /// 车场信息请求实体
    /// </summary>
    public class ParkLotRequest:RequestBase
    {
        /// <summary>
        /// 停车场编码
        /// </summary>
        [Required(ErrorMessage = "停车场编码不能为空")]
        public string ParkingCode { get; set; }

        /// <summary>
        /// 停车场名称
        /// </summary>
        [Required(ErrorMessage ="停车场名称不能为空")]
        public string ParkingName { get; set; }

        /// <summary>
        /// 车牌前缀 如:"粤,B"
        /// </summary>
        [Required(ErrorMessage = "车牌前缀不能为空")]
        public string CarNoPrefix { get; set; }

        /// <summary>
        /// 车位数
        /// </summary>
        public uint ParkingSpacesNumber { get; set; }

        /// <summary>
        /// 剩余车位数
        /// </summary>
        public uint RemainingSpace { get; set; }

        /// <summary>
        /// 车场类型 0=商业 1=小区
        /// </summary>
        public int ParkingType { get; set; }

        /// <summary>
        /// 车场地址
        /// </summary>
        [Required(ErrorMessage = "停车场名称不能为空")]
        public string ParkingSiteAddress { get; set; }

        /// <summary>
        /// 收款账户编码
        /// </summary>
        //public string GatherAccountGuid { get; set; }

        /// <summary>
        /// 车场存续标志
        /// </summary>
        public bool Existence { get; set; }

        /// <summary>
        /// 当前操作员角色guid（可选参数）
        /// </summary>
        public string RoleGuid { get; set; }
    }

    /// <summary>
    /// 车场信息删除请求实体
    /// </summary>
    public class DeleteParkLotRequest: RequestBase
    {
        /// <summary>
        /// 停车场编码
        /// </summary>
        [Required(ErrorMessage = "停车场编码不能为空")]
        public string ParkingCode { get; set; }
    }

    /// <summary>
    /// 在场车场删除信息请求实体
    /// </summary>
    public class InVehicleDeleteRequest  
    {
        /// <summary>
        /// 停车场编码
        /// </summary>
        [Required(ErrorMessage = "停车场编码不能为空")]
        public string ParkingCode { get; set; }

        /// <summary>
        /// 停车记录编码
        /// </summary>
        [Required(ErrorMessage = "停车记录编码不能为空")]
        public string RecordGuid { get; set; }

        /// <summary>
        /// 车牌
        /// </summary>
        [Required(ErrorMessage = "车牌不能为空")]
        public string CarNo { get; set; } 
       
        /// <summary>
        ///入场照片
        /// </summary>
        public string ImgUrl { get; set; } 
        /// <summary>
        /// 当前操作员
        /// </summary>
        [Required(ErrorMessage = "操作人员不能为空")]
        public string Operator { get; set; } 
    }


}