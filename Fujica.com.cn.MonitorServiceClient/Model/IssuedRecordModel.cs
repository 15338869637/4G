using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.MonitorServiceClient.Model
{
    /// <summary>
    /// 订单下发实体
    /// </summary>
    public class IssuedRecord
    {
        public IssuedRecord()
        {
            this.CouponList = new List<IssuedCouponModel>();
        }
        /// <summary>
        /// 线上交易编号
        /// </summary>
        public string TradeNo { get; set; } = "";
        /// <summary>
        /// 停车场编码
        /// </summary>
        public string ParkingCode { get; set; } = "";
        /// <summary>
        /// 车牌号码
        /// </summary> 
        public string CarNo { get; set; } = "";
        /// <summary>
        /// 停车卡号
        /// </summary>
        public string CardNo { get; set; } = "";
        /// <summary>
        /// 卡类别（1月卡、2储值卡、3临时卡）
        /// </summary>
        public int CardType { get; set; } = 0;
        /// <summary>
        /// 车类别（对应字典表车类别信息）
        /// </summary>
        public string CarType { get; set; } = "";
        /// <summary>
        /// 交易编号
        /// </summary>
        public string DealNo { get; set; } = "";
        /// <summary>
        /// 交易日期
        /// </summary>
        public DateTime DealDate = new DateTime(1970, 1, 1);
        /// <summary>
        /// 续费日期数目
        /// </summary>
        public int RenewDay { get; set; } = 0;
        /// <summary>
        /// 续费日期类型 1日  2月 3年
        /// </summary> 
        public int RenewDayType { get; set; } = 0;
        /// <summary>
        /// 数据下发状态 0新数据-1下发失败数据
        /// </summary>
        public int Status { get; set; } = 0;
        /// <summary>
        /// 缴费金额
        /// </summary>
        public decimal Price { get; set; } = 0;
        /// <summary>
        /// 免费金额
        /// </summary>
        public decimal FreeMoney { get; set; } = 0;
        /// <summary>
        /// 支付渠道
        /// </summary>
        public string TranceType { get; set; } = "";
        /// <summary>
        /// 服务器时间
        /// </summary>
        public DateTime SystemDate = new DateTime(1970, 1, 1);

        /// <summary>
        /// 线下交易编号
        /// </summary>
        public string OffLineOrderId { get; set; } = "";
        /// <summary>
        /// 优惠券实体
        /// </summary>
        public List<IssuedCouponModel> CouponList { get; set; } = new List<IssuedCouponModel>();
        /// <summary>
        /// 更新后的已收取费用(单位：元)
        /// </summary>
        public decimal NewChargedFee { get; set; } = 0;
        /// <summary>
        /// -- 更新后的时间分割点(格式：2017-08-01 12:32:00)
        ///     8月15号之后 这个字段改成存放分段明细的JSON字符串，
        ///     线下收到需要判断其字符串长度大于 20 方可序列化该JSON字符串，
        ///     线上在支付完成后，将该JSON添加至已存在的时间分割点内 
        /// </summary>
        public string NewSplitTime { get; set; } = "";
        /// <summary>
        /// 车辆入场时间
        /// </summary>
        public DateTime ParkingBeginTime { get; set; } = new DateTime(1970, 1, 1);
        /// <summary>
        /// 线上计费结束时间
        /// </summary>
        public DateTime FeeEndTime { get; set; } = new DateTime(1970, 1, 1);
        /// <summary>
        /// 组车编码
        /// </summary>
        public string GroupParkingCode { get; set; } = "";

        /// <summary>
        /// 分段收费明细
        /// </summary>
        public string FeePartInfo { get; set; } = "";
        /// <summary>
        /// 备注信息
        /// </summary>
        public int Flag { get; set; } = 0;
        /// <summary>
        /// 是否第二次下发
        /// </summary>
        public int IsSecond { get; set; } = -1;
    }


    /// <summary>
    /// 优惠券实体
    /// </summary>
    public class IssuedCouponModel
    {
        /// <summary>
        /// 商户名称
        /// </summary>
        public string MerchantName { get; set; }
        /// <summary>
        /// 优惠券优惠类型1全免2减免金额3减免时长4减免百分比 
        /// </summary>
        public int CouponType { get; set; }
        /// <summary>
        /// 对应优惠券类型的值 金额/时间/折扣
        /// </summary>
        public decimal CouponValue { get; set; }
        /// <summary>
        /// 优惠券编码
        /// </summary>
        public string CouponCode { get; set; }
        /// <summary>
        /// 优惠券类型1 纸质券 2 电子券 3 活动券
        /// </summary>
        public int FavorableType { get; set; }
        /// <summary>
        /// 截至时间
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
        /// 标识优惠券是线上或线下使用的状态 1只在线上使用 2只在线下使用 3线上线下都可以使用
        /// </summary>
        public int UseStatus { get; set; }
        /// <summary>
        /// 是否自动使用优惠券1是2否
        /// </summary>
        public int IsAuto { get; set; }
    }
}
