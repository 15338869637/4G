using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fujica.com.cn.RabbitMQFactory.Helpers
{
    public partial class MQPackongVehicleEntry
    {
        /// <summary>
        /// 消息id  ，做为接收字典的key
        /// </summary>
        public string idMsg { get; set; }
        /// <summary>
        /// 过期分钟数
        /// </summary>
        public int expireMinutes { get; set; }
        /// <summary>
        /// 消息类型
        /// </summary>
        public RabbitMQProduserMsgType rabbitMQType { get; set; }
        /// <summary>
        /// 数据 根据枚举 解析获取不同的实体对象
        /// </summary>
        public string json { get; set; }
        /// <summary>
        /// 停车场编码
        /// </summary>
        public string parkingCode { get; set; }
        /// <summary>
        /// 路由键
        /// </summary>
        public string routingKey { get; set; }
        /// <summary>
        /// 信道
        /// </summary>
        public string exchange { get; set; }
        /// <summary>
        ///  交换器类型   选择一个类型"direct" "fanout" "headers"  "topic"
        /// </summary>
        public string exchangeType { get; set; }
    }

    /// <summary>
    /// 生产者发送的消息类型
    /// </summary>
    public enum RabbitMQProduserMsgType : uint
    {
        /// <summary>
        /// 心跳 (在web和api项目中没有心跳类型)
        /// </summary>
        [Description("心跳")]
        RabbitMQHeartBeat = 1000,
        /// <summary>
        /// 添加入场记录
        /// </summary>
        [Description("添加入场记录")]
        RabbitMQAddRecord = 1001,
        /// <summary>
        /// 删除入场记录
        /// </summary>
        [Description("删除入场记录")]
        RabbitMQSubRecord = 1002,
        /// <summary>
        /// 算费业务
        /// </summary>
        [Description("算费业务")]
        RabbitMQFee = 1003,
        /// <summary>
        /// 无车牌进场出场业务
        /// </summary>
        [Description("无车牌进场出场业务")]
        RabbitMQNoneCarNo = 1004,
        /// <summary>
        /// 锁车业务（锁车）
        /// </summary>
        [Description("锁车业务（锁车）")]
        RabbitMQLockCar = 1005,
        /// <summary>
        /// 锁车业务（解锁）
        /// </summary>
        [Description("锁车业务（解锁）")]
        RabbitMQUnLockCar = 1006,
        /// <summary>
        /// 下发缴费
        /// </summary>
        [Description("下发缴费")]
        RabbitMQPay = 1007,
        /// <summary>
        /// 访客
        /// </summary>
        [Description("访客")]
        RabbitVisitor = 1008,
        /// <summary>
        /// 车辆预约（预约车位）
        /// </summary>
        [Description("车辆预约")]
        Reservation = 1009,
        /// <summary>
        /// 电子券
        /// </summary>
        [Description("电子券")]
        RabbitMQElecCoupon = 1010,
        /// <summary>
        /// 纸质券
        /// </summary>
        [Description("纸质券")]
        RabbitMQPaperCoupon = 1011,
        /// <summary>
        /// 新增停车场权限配置
        /// </summary>
        [Description("新增停车场权限配置")]
        RabbitMQParkingLotRule = 1012,
        /// <summary>
        /// 取消车辆预约（取消车位）
        /// </summary>
        [Description("取消车辆预约")]
        CancelReservation = 1013,
        /// <summary>
        /// 修改优惠券状态为已使用
        /// </summary>
        [Description("修改优惠券状态为已使用")]
        RabbitMQUpdateCouponStatus = 1014,
        /// <summary>
        /// 线上车辆出场 2017年3月9日
        /// </summary>
        [Description("线上车辆出场")]
        RabbitMQOnLineAppearanceRecord = 1015,
        /// <summary>
        /// RabbitMQ发送统一通道
        /// </summary>
        [Description("RabbitMQ发送统一通道")]
        RabbitMQUnifyChannel = 1016,
        /// <summary>
        /// APP专用通道
        /// </summary>
        [Description("APP业务消息")]
        AppExchange = 1017,
        /// <summary>
        /// 请求线下算费（附带优惠数据）
        /// </summary>
        [Description("请求线下算费（附带优惠数据）")]
        RabbitMQFeeAndCoupons = 1018,
        /// <summary>
        /// 代扣失败记录下发通道
        /// </summary>
        [Description("代扣失败记录下发通道")]
        RabbitMQWithholdingAmountFailureToRecord = 1019,

        /// <summary>
        /// 无感支付签约解约下发通道
        /// </summary>
        [Description("无感支付签约解约下发通道")]
        RabbitMQNoSensePay = 1020,
        /// <summary>
        /// 创建车组信息
        /// </summary>
        [Description("创建车组信息通道")]
        RabbitMQCreateGroupLot = 1021,
        /// <summary>
        /// 更改车组信息
        /// </summary>
        [Description("更改车组信息通道")]
        RabbitMQEditGroupLot = 1022,
        /// <summary>
        ///车辆退出车组通道
        /// </summary>
        [Description("车辆退出车组通道")]
        RabbitMQRemoveOneCar = 1023,
        /// <summary>
        /// 解散已有车组通道
        /// </summary>
        [Description("解散已有车组通道")]
        RabbitMQDisbandGroupLot = 1024,
        /// <summary>
        ///车辆加入车组通道
        /// </summary>
        [Description("车辆加入车组通道")]
        RabbitMQAppendToGroupLot = 1025,

        #region 推送消息模板
        /// <summary>
        /// 出场推送
        /// </summary>
        [Description("出场推送")]
        VehicleLeave = 1026,
        /// <summary>
        /// 入场推送
        /// </summary>
        [Description("入场推送")]
        VehicleEntry = 1027,
        /// <summary>
        /// 临时车缴费推送
        /// </summary>
        [Description("临时车缴费推送")]
        PaySuccess = 1028,
        #endregion

        #region 优泊客 发送数据
        /// <summary>
        /// 下发数据
        /// </summary>
        [Description("优泊客 数据下发")]
        YOUBOKESendData = 1029,
        /// <summary>
        /// 上推数据
        /// </summary>
        [Description("优泊客 相机数据推送到线上")]
        YOUBOKEPushData = 1030

        #endregion
    }
}
