using Fujica.com.cn.Context.Model;
using System.ComponentModel.DataAnnotations;
/***************************************************************************************
 * *
 * *        File Name        : CarTypeRequest.cs
 * *        Creator          : llp
 * *        Create Time      : 2019-09-24 
 * *        Remark           : 车类请求实体
 * *
 * *  Copyright (c) 深圳市富士智能系统有限公司.  All rights reserved. 
 * ***************************************************************************************/

namespace Fujica.com.cn.Interface.Management.Models.InPut
{
    /// <summary>
    /// 车类请求实体.
    /// </summary>
    /// <remarks>
    /// 2019.09.24: 创建. llp <br/> 
    /// 2019.09.24:  去除TypeName 验证.  llp<br/>
    /// </remarks> 
    public class CarTypeRequest : RequestBase
    {
        /// <summary>
        /// 停车场编码
        /// </summary>
        [Required(ErrorMessage = "停车场编码不能为空")]
        public string ParkingCode { get; set; }

        /// <summary>
        /// 车类名
        /// </summary>
       // [Required(ErrorMessage = "车类名不能为空")]
        public string TypeName { get; set; }
        
        /// <summary>
        /// 缴费模式  0=时租卡 1=月租卡 2=储值卡 3=贵宾卡
        /// </summary>
        public CarTypeEnum PaymentMode { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enable { get; set; }
    }

    /// <summary>
    /// 车类修改请求实体
    /// </summary>
    public class ModifyCarTypeRequest: CarTypeRequest
    {
        /// <summary>
        /// 车类标识
        /// </summary>
        [Required(ErrorMessage = "车类标识不能为空")]
        public string Guid { get; set; }
    }
}