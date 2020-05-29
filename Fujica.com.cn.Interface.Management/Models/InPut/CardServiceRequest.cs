using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Fujica.com.cn.Interface.Management.Models.InPut
{
    /// <summary>
    /// 卡务管理请求基本实体
    /// </summary>
    public class CardServiceBaseRequest : RequestBase
    {
        /// <summary>
        /// 停车场编码
        /// </summary>
        [Required(ErrorMessage = "停车场编码不能为空")]
        public string ParkingCode { get; set; }

        /// <summary>
        /// 车牌
        /// </summary>
        [Required(ErrorMessage = "车牌号不能为空")]
        public string CarNo { get; set; }
        /// <summary>
        /// 操作员
        /// </summary>
        [Required(ErrorMessage = "操作员不能为空")]
        public string RechargeOperator { get; set; }
    }

    /// <summary>
    /// 注销卡请求实体
    /// </summary>
    public class CancelCardServiceRequest : CardServiceBaseRequest
    {

        /// <summary>
        /// 卡上原金额
        /// </summary>
        [Required(ErrorMessage = "卡上金额不能为空")]
        public decimal Balance { get; set; }
        /// <summary>
        /// 退款金额
        /// </summary>
        [Required(ErrorMessage = "退款金额不能为空")]
        public decimal RefundAmount { get; set; }


        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }

    /// <summary>
    /// 开卡请求实体
    /// </summary>
    public class ApplyCardServiceRequest:CardServiceBaseRequest
    {
        /// <summary>
        /// 车主姓名
        /// </summary>
        [Required(ErrorMessage = "车主姓名不能为空")]
        public string CarOwnerName { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        [Required(ErrorMessage = "联系电话不能为空")]
        public string Mobile { get; set; }

        /// <summary>
        /// 授权车道集合,如 b831254e32b94406be2d563dd7efbca5,3c3741986f8241619eb4ca8d9fbd373e
        /// </summary>
        [Required(ErrorMessage = "授权车道不能为空")]
        public string DrivewayGuidList { get; set; }

        /// <summary>
        /// 卡类型 所属车类唯一标识
        /// </summary>
        [Required(ErrorMessage = "卡类型不能为空")]
        public string CarTypeGuid { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 起始日期 月卡时有效
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 截止日期 月卡时有效
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 支付金额
        /// </summary>
        [Required(ErrorMessage = "支付金额不能为空")]
        public decimal PayAmount { get; set; }

        /// <summary>
        /// 支付方式，直接中文描述
        /// </summary>
        [Required(ErrorMessage = "支付方式不能为空")]
        public string PayStyle { get; set; }
        
    }

    /// <summary>
    /// 修改卡请求实体
    /// </summary>
    public class ModifyCardServiceRequest : CardServiceBaseRequest
    {
        /// <summary>
        /// 车主姓名
        /// </summary>
        [Required(ErrorMessage = "车主姓名不能为空")]
        public string CarOwnerName { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        [Required(ErrorMessage = "联系电话不能为空")]
        public string Mobile { get; set; }

        /// <summary>
        /// 授权车道集合,如 b831254e32b94406be2d563dd7efbca5,3c3741986f8241619eb4ca8d9fbd373e
        /// </summary>
        [Required(ErrorMessage = "授权车道不能为空")]
        public string DrivewayGuidList { get; set; }

        /// <summary>
        /// 卡类型 所属车类唯一标识
        /// </summary>
        [Required(ErrorMessage = "卡类型不能为空")]
        public string CarTypeGuid { get; set; }
    }

    /// <summary>
    /// 月卡请求实体
    /// </summary>
    public class MonthCardServiceRequest: CardServiceBaseRequest
    {
        /// <summary>
        /// 起始日期 月卡时有效
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 截止日期 月卡时有效
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 延期之前截止日期 月卡时有效
        /// </summary>
        public DateTime PrimaryEndDate { get; set; }
        

        /// <summary>
        /// 支付金额
        /// </summary>
        [Required(ErrorMessage = "支付金额不能为空")]
        public decimal PayAmount { get; set; }

        /// <summary>
        /// 支付方式，直接中文描述
        /// </summary>
        [Required(ErrorMessage = "支付方式不能为空")]
        public string PayStyle { get; set; }

        /// <summary>
        /// 车类编码不能为空
        /// </summary>
        [Required(ErrorMessage = "车类编码不能为空")]
        public string CarTypeGuid { get; set; }

    }

    /// <summary>
    /// 储值卡请求实体
    /// </summary>
    public class ValueCardServiceRequest: CardServiceBaseRequest
    {
        /// <summary>
        /// 支付金额
        /// </summary>
        [Required(ErrorMessage = "支付金额不能为空")]
        public decimal PayAmount { get; set; }

        /// <summary>
        /// 支付方式，直接中文描述
        /// </summary>
        [Required(ErrorMessage = "支付方式不能为空")]
        public string PayStyle { get; set; }

        /// <summary>
        /// 车类编码不能为空
        /// </summary>
        [Required(ErrorMessage = "车类编码不能为空")]
        public string CarTypeGuid { get; set; }
    }

    /// <summary>
    /// 月卡报停请求实体
    /// </summary>
    public class MonthCardPause:CardServiceBaseRequest
    {
        /// <summary>
        /// 报停开始日期
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 报停结束日期
        /// </summary>
        public DateTime EndDate { get; set; }

    }
}