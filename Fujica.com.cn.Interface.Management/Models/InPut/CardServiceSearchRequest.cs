using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Fujica.com.cn.Interface.Management.Models.InPut
{
    /// <summary>
    /// 临时卡查询请求实体
    /// </summary>
    /// <remarks>
    /// 2019.10.25: 修改.月卡、临时卡、储值卡分页搜索增加卡类查询条件 Ase <br/> 
    /// </remarks>
    public class TempCardSearchRequest : RequestBase
    {
        /// <summary>
        /// 当前页码
        /// </summary>
        [Required(ErrorMessage = "当前页码不能为空")]
        public int PageIndex { get; set; }

        /// <summary>
        /// 每页数量
        /// </summary>
        [Required(ErrorMessage = "每页数量不能为空")]
        public int PageSize { get; set; }

        /// <summary>
        /// 停车场编码
        /// </summary>
        [Required(ErrorMessage = "停车场编码不能为空")]
        public string ParkingCode { get; set; }

        /// <summary>
        /// 车牌
        /// </summary>
        public string CarNo { get; set; }

        /// <summary>
        /// 车主姓名
        /// </summary>
        public string CarOwnerName { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 开卡日期
        /// </summary>
        public DateTime? ApplyDate { get; set; }

        /// <summary>
        /// 卡状态 0=全部 1=正常 2=报停 3=锁定
        /// </summary>
        public int StatusType { get; set; }

        /// <summary>
        /// 卡类guid
        /// </summary>
        public string CarTypeGuid { get; set; }
    }

    /// <summary>
    /// 月卡查询请求实体
    /// </summary>
    public class MonthCardSearchRequest : RequestBase
    {
        /// <summary>
        /// 当前页码
        /// </summary>
        [Required(ErrorMessage = "当前页码不能为空")]
        public int PageIndex { get; set; }

        /// <summary>
        /// 每页数量
        /// </summary>
        [Required(ErrorMessage = "每页数量不能为空")]
        public int PageSize { get; set; }

        /// <summary>
        /// 停车场编码
        /// </summary>
        [Required(ErrorMessage = "停车场编码不能为空")]
        public string ParkingCode { get; set; }

        /// <summary>
        /// 车牌
        /// </summary>
        public string CarNo { get; set; }

        /// <summary>
        /// 车主姓名
        /// </summary>
        public string CarOwnerName { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 开卡日期
        /// </summary>
        public DateTime? ApplyDate { get; set; }

        /// <summary>
        /// 卡状态 0=全部 1=正常 2=报停 3=锁定
        /// </summary>
        public int StatusType { get; set; }

        /// <summary>
        /// 卡类guid
        /// </summary>
        public string CarTypeGuid { get; set; }
    }

    /// <summary>
    /// 储值卡查询请求实体
    /// </summary>
    public class ValueCardSearchRequest : RequestBase
    {
        /// <summary>
        /// 当前页码
        /// </summary>
        [Required(ErrorMessage = "当前页码不能为空")]
        public int PageIndex { get; set; }

        /// <summary>
        /// 每页数量
        /// </summary>
        [Required(ErrorMessage = "每页数量不能为空")]
        public int PageSize { get; set; }

        /// <summary>
        /// 停车场编码
        /// </summary>
        [Required(ErrorMessage = "停车场编码不能为空")]
        public string ParkingCode { get; set; }

        /// <summary>
        /// 车牌
        /// </summary>
        public string CarNo { get; set; }

        /// <summary>
        /// 车主姓名
        /// </summary>
        public string CarOwnerName { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 开卡日期
        /// </summary>
        public DateTime? ApplyDate { get; set; }

        /// <summary>
        /// 卡状态 0=全部 1=正常 2=报停 3=锁定
        /// </summary>
        public int StatusType { get; set; }

        /// <summary>
        /// 卡类guid
        /// </summary>
        public string CarTypeGuid { get; set; }
    }
}