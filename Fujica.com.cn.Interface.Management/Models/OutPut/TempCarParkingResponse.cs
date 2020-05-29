using Fujica.com.cn.Context.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fujica.com.cn.Interface.Management.Models.OutPut
{
    /// <summary>
    /// 获取临时车停车费用信息实体
    /// </summary>
    public class TempCarParkingFeeResponse : IBaseModel
    {
        public TempCarParkingFeeResponse()
        {

        }
        /// <summary>
        /// 车牌号码
        /// </summary>
        public string CarNumber { get; set; }

        /// <summary>
        /// 停车费用
        /// </summary>
        public decimal ParkingFee { get; set; }

        /// <summary>
        /// 应缴费用（应缴费用=停车费–自动使用的优惠券的优惠费用）
        /// </summary>
        public decimal ActualAmount { get; set; }

        /// <summary>
        /// 优惠金额
        /// </summary>
        public decimal DiscountAmount { get { return ParkingFee - ActualAmount; } }

        /// <summary>
        /// 已缴费用
        /// </summary>
        public decimal PaymentAmount { get; set; }

        /// <summary>
        /// 优惠券列表
        /// </summary>
        public List<MCoupon> CouponList { get; set; }
    }

    /// <summary>
    /// 用户模型
    /// </summary>
    public class MCoupon
    {
        /// <summary>
        /// 商户名称
        /// </summary>
        public string MerchantName { get; set; }

        /// <summary>
        /// 优惠券优惠类型1全免2减免金额3减免时长4减免百分比
        /// </summary>
        public string CouponType { get; set; }

        /// <summary>
        /// 对应优惠券类型的值 金额/时间/折扣
        /// </summary>
        public string CouponValue { get; set; }

        /// <summary>
        /// 优惠券编码
        /// </summary>
        public string CouponCode { get; set; }

        /// <summary>
        /// 优惠券类型1 纸质券 2 电子券 3 活动券 4 新版纸质券
        /// </summary>
        public int FavorableType { get; set; }

        /// <summary>
        ///截至时间
        /// </summary>
        public string EndTime { get; set; }

        /// <summary>
        /// 优惠券描述
        /// </summary>
        public string CouponDescribe { get; set; }

        /// <summary>
        /// 优惠券名称
        /// </summary>
        public string CouponName { get; set; }
        /// <summary>
        ///标识优惠券是线上或线下使用的状态 1只在线上使用 2只在线下使用 3线上线下都可以使用
        /// </summary>
        public int UseStatus { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 操作人员姓名
        /// </summary>
        public string OperatorName { get; set; }
        /// <summary>
        /// 操作人员电话
        /// </summary>
        public string OperatorPhone { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}