 
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Fujica.com.cn.Bussiness.Entity
{
    /// <summary>
    /// 线下临时卡缴费
    /// </summary>
    public class OfflineTemporaryCardPayModel 
    {
        /// <summary>
        /// 车类别(关联计费类型)
        /// </summary>
        public string CarType { get; set; }
        /// <summary>
        /// 入场时间
        /// </summary>
        public DateTime? AdmissionDate { get; set; }
        /// <summary>
        /// 计费开始时间 （算费接口返回的 最后一次缴费时间或入场时间）
        /// </summary>
        public DateTime? BillingStartTime { get; set; }
        /// <summary>
        /// 计费截至时间 （算费接口返回的计费时间）
        /// </summary>
        public DateTime BillingDeadline { get; set; }
        /// <summary>
        /// 人工免费
        /// </summary>
        public decimal FreeAdmission { get; set; }
        /// <summary>
        /// 计费操作员
        /// </summary>
        public string TollOperator { get; set; }


        /// <summary>
        /// 缴费金额
        /// </summary>
        public decimal ActualAmount { get; set; }
        /// <summary>
        /// 缴费时间
        /// </summary>
        public DateTime AmountTime = DateTime.Now;
        /// <summary>
        /// 车牌号
        /// </summary>
        public string CarNo { get; set; }
        /// <summary>
        /// 停车场编码
        /// </summary>
        [Required]
        public string ParkingCode { get; set; }
        /// <summary>
        /// 临时卡卡号
        /// </summary>
        public string CardNo { get; set; }
        /// <summary>
        /// 线下停车记录编码
        /// </summary>
        public string LineRecordCode { get; set; }
        /// <summary>
        /// 客户端时间
        /// </summary>
        public DateTime CustomDate = DateTime.Now;
        /// <summary>
        /// 线下交易编号
        /// </summary>
        [Required]
        public string DealNo { get; set; }
        /// <summary>
        /// 金额(停车费)
        /// </summary>
        public decimal Amount { get; set; }
        ///<summary>
        ///抵扣金额
        ///</summary>
        public decimal DeductionAmount { get; set; }
        ///<summary>
        ///抵扣原因
        ///</summary>
        public string Reason { get; set; }
        /// <summary>
        /// 使用的优惠小时数 通过hosting获取优惠金额后会返回该字段的值
        /// </summary>
        public float UseTimeCount { get; set; }
        /// <summary>
        /// 发票领取状态1已领2未领
        /// </summary>
        public int InvoiceStatus { get; set; }
        /// <summary>
        /// 发票领取时间
        /// </summary>
        public DateTime InvoiceTime { get; set; }
        /// <summary>
        /// 缴费后当前储值卡金额
        /// </summary>
        public decimal PayBackamt { get; set; }
        /// <summary>
        /// 更新后的已收取费用(单位：元)
        /// 取值：
        ///     未跨段 取当次停车费；
        ///     跨段 取跨段后的新的费用。
        /// </summary>
        public decimal NewChargedFee { get; set; }
        /// <summary>
        /// 跨段后的时间分割点(格式：2017-08-01 12:32:00)
        /// 时间分割点为当次的入场时间或者跨段后的缴费时间
        /// </summary>
        public string NewSplitTime { get; set; }
        /// <summary>
        /// 优惠券信息
        /// </summary>
        public List<CouponCalculateView> CouponList { get; set; }
    }
    public class CouponCalculateView
    {
        /// <summary>
        /// 优惠券编码
        /// </summary>
        public string CouponCode { get; set; }
        /// <summary>
        /// 优惠券类型1 纸质券 2 电子券
        /// </summary>
        public int FavorableType { get; set; }

    }

}
