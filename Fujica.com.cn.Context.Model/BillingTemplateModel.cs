using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.Context.Model
{
    /// <summary>
    /// 计费模板模型
    /// </summary>
    public class BillingTemplateModel : IBaseModel
    {

        /// <summary>
        /// 项目编码
        /// </summary>
        public string ProjectGuid { get; set; }
        
        /// <summary>
        /// 停车场编码
        /// </summary>
        public string ParkCode { get; set; }

        /// <summary>
        /// 卡类型(对应的车类型的guid)
        /// </summary>
        public string CarTypeGuid { get; set; }

        /// <summary>
        /// 计费方式1-10 共10种
        /// 1=小时算费 2=分段算费 3=深圳算费 4=按半小时算费 5=简易分段算费 
        /// 6=分段按小时算费 7=分段按半小时算费 8=半小时算费(可分段) 9=新分段收费标准
        /// 10=分段按15分钟算费
        /// </summary>
        public int ChargeMode { get; set; }

        /// <summary>
        /// 模板实体json字符串
        /// </summary>
        public string TemplateJson { get; set; }
    }

    /// <summary>
    /// 具体计费方式的基础模型
    /// </summary>
    public class BillingTemplateChargeModel : IBaseModel
    {

        /// <summary>
        /// 卡类型(对应的车类型的guid)
        /// </summary>
        public string CarTypeGuid { get; set; }

        /// <summary>
        /// 计费方式1-10 共10种
        /// 1=小时算费 2=分段算费 3=深圳算费 4=按半小时算费 5=简易分段算费 
        /// 6=分段按小时算费 7=半小时算费(可分段) 8=分段按半小时算费 9=新分段收费标准
        /// 10=分段按15分钟算费
        /// </summary>
        public int ChargeMode { get; set; }

        /// <summary>
        /// 开始时间-时间段1
        /// </summary>
        public string BeginTime1 { get; set; }
        /// <summary>
        /// 开始时间-时间段2
        /// </summary>
        public string BeginTime2 { get; set; }
        /// <summary>
        /// 开始时间-时间段3
        /// </summary>
        public string BeginTime3 { get; set; }

        /// <summary>
        /// 结束时间-时间段1
        /// </summary>
        public string EndTime1 { get; set; }
        /// <summary>
        /// 结束时间-时间段2
        /// </summary>
        public string EndTime2 { get; set; }
        /// <summary>
        /// 结束时间-时间段3
        /// </summary>
        public string EndTime3 { get; set; }
        /// <summary>
        /// 免费分钟数-时间段1
        /// </summary>
        public int FreeMinutes1 { get; set; }
        /// <summary>
        /// 免费分钟数-时间段2
        /// </summary>
        public int FreeMinutes2 { get; set; }
        /// <summary>
        /// 免费分钟数-时间段3
        /// </summary>
        public int FreeMinutes3 { get; set; }

        /// <summary>
        /// 离场超时分钟-时间段1
        /// 缴费后可享有的免费时间
        /// </summary>
        public int LeaveTimeout1 { get; set; }
        /// <summary>
        /// 离场超时分钟-时间段2
        /// 缴费后可享有的免费时间
        /// </summary>
        public int LeaveTimeout2 { get; set; }
        /// <summary>
        /// 离场超时分钟-时间段3
        /// 缴费后可享有的免费时间
        /// </summary>
        public int LeaveTimeout3 { get; set; }
    }

    /// <summary>
    /// 按小时算费模型
    /// </summary>
    public class HourlyTollModel
        {
            /// <summary>
            /// 免费分钟数
            /// </summary>
            public int FreeMinutes { get; set; }

            /// <summary>
            /// 离场超时分钟，缴费后可享有的免费时间
            /// </summary>
            public int LeaveTimeout { get; set; }

            /// <summary>
            /// 1小时金额
            /// </summary>
            public decimal h1 { get; set; }
            /// <summary>
            /// 2小时金额
            /// </summary>
            public decimal h2 { get; set; }
            /// <summary>
            /// 3小时金额
            /// </summary>
            public decimal h3 { get; set; }
            /// <summary>
            /// 4小时金额
            /// </summary>
            public decimal h4 { get; set; }
            /// <summary>
            /// 5小时金额
            /// </summary>
            public decimal h5 { get; set; }
            /// <summary>
            /// 6小时金额
            /// </summary>
            public decimal h6 { get; set; }
            /// <summary>
            /// 7小时金额
            /// </summary>
            public decimal h7 { get; set; }
            /// <summary>
            /// 8小时金额
            /// </summary>
            public decimal h8 { get; set; }
            /// <summary>
            /// 9小时金额
            /// </summary>
            public decimal h9 { get; set; }
            /// <summary>
            /// 10小时金额
            /// </summary>
            public decimal h10 { get; set; }
            /// <summary>
            /// 11小时金额
            /// </summary>
            public decimal h11 { get; set; }
            /// <summary>
            /// 12小时金额
            /// </summary>
            public decimal h12 { get; set; }
            /// <summary>
            /// 13小时金额
            /// </summary>
            public decimal h13 { get; set; }
            /// <summary>
            /// 14小时金额
            /// </summary>
            public decimal h14 { get; set; }
            /// <summary>
            /// 15小时金额
            /// </summary>
            public decimal h15 { get; set; }
            /// <summary>
            /// 16小时金额
            /// </summary>
            public decimal h16 { get; set; }
            /// <summary>
            /// 17小时金额
            /// </summary>
            public decimal h17 { get; set; }
            /// <summary>
            /// 18小时金额
            /// </summary>
            public decimal h18 { get; set; }
            /// <summary>
            /// 19小时金额
            /// </summary>
            public decimal h19 { get; set; }
            /// <summary>
            /// 20小时金额
            /// </summary>
            public decimal h20 { get; set; }
            /// <summary>
            /// 21小时金额
            /// </summary>
            public decimal h21 { get; set; }
            /// <summary>
            /// 22小时金额
            /// </summary>
            public decimal h22 { get; set; }
            /// <summary>
            /// 23小时金额
            /// </summary>
            public decimal h23 { get; set; }
            /// <summary>
            /// 24小时金额
            /// </summary>
            public decimal h24 { get; set; }

            /// <summary>
            /// 每天最大金额
            /// </summary>
            public decimal DayAmountTopLimit { get; set; }

            /// <summary>
            /// 每次最大金额
            /// </summary>
            public decimal AmountTopLimit { get; set; }

            /// <summary>
            /// 单位 0=角 1=元
            /// </summary>
            public int MonetaryUnit { get; set; }
        }

    /// <summary>
    /// 分段算费模型
    /// </summary>
    public class SegmentTollModel
    {
        /// <summary>
        /// MonetaryUnit
        /// </summary>
        public int MonetaryUnit { get; set; }
        /// <summary>
        /// LeaveTimeout
        /// </summary>
        public int LeaveTimeout { get; set; }
        /// <summary>
        /// MaxDayMoney
        /// </summary>
        public decimal MaxDayMoney { get; set; }
        /// <summary>
        /// FreeMiniteAll
        /// </summary>
        public int FreeMiniteAll { get; set; }
        /// <summary>
        /// BeginTime1
        /// </summary>
        public DateTime BeginTime1 { get; set; }
        /// <summary>
        /// EndTime1
        /// </summary>
        public DateTime EndTime1 { get; set; }
        /// <summary>
        /// BeginTime2
        /// </summary>
        public DateTime BeginTime2 { get; set; }
        /// <summary>
        /// EndTime2
        /// </summary>
        public DateTime EndTime2 { get; set; }
        /// <summary>
        /// BeginTime3
        /// </summary>
        public DateTime BeginTime3 { get; set; }
        /// <summary>
        /// EndTime3
        /// </summary>
        public DateTime EndTime3 { get; set; }
        /// <summary>
        /// FreeMinute1
        /// </summary>
        public int FreeMinute1 { get; set; }
        /// <summary>
        /// FreeMinute2
        /// </summary>
        public int FreeMinute2 { get; set; }
        /// <summary>
        /// FreeMinute3
        /// </summary>
        public int FreeMinute3 { get; set; }
        /// <summary>
        /// Fee1
        /// </summary>
        public decimal Fee1 { get; set; }
        /// <summary>
        /// Fee2
        /// </summary>
        public decimal Fee2 { get; set; }
        /// <summary>
        /// Fee3
        /// </summary>
        public decimal Fee3 { get; set; }
        /// <summary>
        /// SameTimeFreeMinute
        /// </summary>
        public int SameTimeFreeMinute { get; set; }
        /// <summary>
        /// SameTimeMoney1
        /// </summary>
        public decimal SameTimeMoney1 { get; set; }
        /// <summary>
        /// SameTimeMoney2
        /// </summary>
        public decimal SameTimeMoney2 { get; set; }
        /// <summary>
        /// SameTimeMoney3
        /// </summary>
        public decimal SameTimeMoney3 { get; set; }
        /// <summary>
        /// SpanFreeMinute
        /// </summary>
        public int SpanFreeMinute { get; set; }
        /// <summary>
        /// SpanAccount
        /// </summary>
        public int SpanAccount { get; set; }
        /// <summary>
        /// SpanNoOut
        /// </summary>
        public int SpanNoOut { get; set; }
        /// <summary>
        /// SpanNoOutMoney
        /// </summary>
        public decimal SpanNoOutMoney { get; set; }
        /// <summary>
        /// SpanOut
        /// </summary>
        public int SpanOut { get; set; }
        /// <summary>
        /// SpanOutMoney
        /// </summary>
        public decimal SpanOutMoney { get; set; }
        /// <summary>
        /// OverFlag
        /// </summary>
        public int OverFlag { get; set; }
        /// <summary>
        /// OverFreeMinute
        /// </summary>
        public int OverFreeMinute { get; set; }
        /// <summary>
        /// OverMaxValue
        /// </summary>
        public int OverMaxValue { get; set; }
        /// <summary>
        /// OverNoOut
        /// </summary>
        public int OverNoOut { get; set; }
        /// <summary>
        /// OverNoOutMoney
        /// </summary>
        public decimal OverNoOutMoney { get; set; }
        /// <summary>
        /// OverOut
        /// </summary>
        public int OverOut { get; set; }
        /// <summary>
        /// OverOutMoney
        /// </summary>
        public decimal OverOutMoney { get; set; }
    }

    /// <summary>
    /// 深圳算费模型
    /// </summary>
    public class ShenZhengTollModel
    {
        /// <summary>
        /// FreeMinutes
        /// </summary>
        public int FreeMinutes { get; set; }
        /// <summary>
        /// RHBeginTime
        /// </summary>
        public DateTime RHBeginTime { get; set; }
        /// <summary>
        /// RHEndTime
        /// </summary>
        public DateTime RHEndTime { get; set; }
        /// <summary>
        /// RHFirstHour
        /// </summary>
        public decimal RHFirstHour { get; set; }
        /// <summary>
        /// RHEveryHalfHour
        /// </summary>
        public decimal RHEveryHalfHour { get; set; }
        /// <summary>
        /// NRHFirstHour
        /// </summary>
        public decimal NRHFirstHour { get; set; }
        /// <summary>
        /// NRHEveryHour
        /// </summary>
        public decimal NRHEveryHour { get; set; }
        /// <summary>
        /// NWDFirstHour
        /// </summary>
        public decimal NWDFirstHour { get; set; }
        /// <summary>
        /// NWDEveryHour
        /// </summary>
        public decimal NWDEveryHour { get; set; }
        /// <summary>
        /// NWDEveryDay
        /// </summary>
        public decimal NWDEveryDay { get; set; }
        /// <summary>
        /// WDEveryDay
        /// </summary>
        public decimal WDEveryDay { get; set; }
        /// <summary>
        /// EveryDay
        /// </summary>
        public decimal EveryDay { get; set; }
        /// <summary>
        /// AmountTopLimit
        /// </summary>
        public decimal AmountTopLimit { get; set; }
        /// <summary>
        /// Timeout
        /// </summary>
        public int Timeout { get; set; }
        /// <summary>
        /// MonetaryUnit
        /// </summary>
        public int MonetaryUnit { get; set; }
        /// <summary>
        /// TollFlag
        /// </summary>
        public int TollFlag { get; set; }
        /// <summary>
        /// 离场超时分钟，缴费后可享有的免费时间
        /// </summary>
        public int LeaveTimeout { get; set; }
    }

    /// <summary>
    /// 按半小时算费模型
    /// </summary>
    public class HalfHourTollModel
    {
        /// <summary>
		/// 白天免费分钟
        /// </summary>
        public int DayFreeMinutes { get; set; }

        /// <summary>
		/// 晚上免费分钟
        /// </summary>
        public int NightFreeMinutes { get; set; }

        /// <summary>
        /// 白天时段 开始时间
        /// </summary>
        public DateTime DayBeginTime { get; set; }

        /// <summary>
        /// 白天时段截止时间
        /// </summary>
        public DateTime DayEndTime { get; set; }
        /// <summary>
        /// 白天段首N个半
        /// </summary>
        public int DayFirstHour { get; set; }

        /// <summary>
        /// 白天时段首N个半小时收费
        /// </summary>
        public decimal DayFirstMoney { get; set; }

        /// <summary>
        /// 白天段 后M小时
        /// </summary>
        public int DayNextHour { get; set; }

        /// <summary>
        /// 后M小时收费
        /// </summary>
        public decimal DayNextMoney { get; set; }

        /// <summary>
        /// 白天段 超时时间 30以内
        /// </summary>
        public int DayTimeout { get; set; }

        /// <summary>
        /// 晚上段 每M半小时
        /// </summary>
        public int NightFirstHour { get; set; }

        /// <summary>
        /// 晚上段 每M半小时 收费
        /// </summary>
        public decimal NightFirstMoney { get; set; }

        /// <summary>
        /// 晚上段 每M半小时N个半小时
        /// </summary>
        public int NightNextHour { get; set; }

        /// <summary>
        /// 晚上段 每M半小时N个半小时收费
        /// </summary>
        public decimal NightNextMoney { get; set; }

        /// <summary>
        /// 晚上段 超时时间 30以内
        /// </summary>
        public int NightTimeout { get; set; }

        /// <summary>
        /// 每天最高收费
        /// </summary>
        public decimal MaxDayMoney { get; set; }

        /// <summary>
        /// 本次最高收费
        /// </summary>
        public decimal MaxMoney { get; set; }

        /// <summary>
        /// 收费单位
        /// </summary>
        public int MonetaryUnit { get; set; }

        /// <summary>
        /// 离场超时分钟，缴费后可享有的免费时间
        /// </summary>
        public int LeaveTimeout { get; set; }
    }

    /// <summary>
    /// 简易分段算费模型
    /// </summary>
    public class SimpleSegmentTollModel
    {
        /// <summary>
        /// 免费分钟
        /// </summary>
        public int FreeMinutes { get; set; }
        /// <summary>
        /// 白天开始时间
        /// </summary>
        public DateTime DayBeginTime { get; set; }
        /// <summary>
        /// 白天截止日期
        /// </summary>
        public DateTime DayEndTime { get; set; }
        /// <summary>
        /// 白天收费
        /// </summary>
        public decimal DayMoney { get; set; }
        /// <summary>
        /// 晚上段收费
        /// </summary>
        public decimal NightMoney { get; set; }
        /// <summary>
        /// 每天最高收费
        /// </summary>
        public decimal DayMaxMoney { get; set; }
        /// <summary>
        /// 白天段 分钟内
        /// </summary>
        public int DayMinute { get; set; }
        /// <summary>
        /// 白天段开始分钟内收费金额
        /// </summary>
        public decimal DayFirstMoney { get; set; }
        /// <summary>
        /// 晚上段时间内
        /// </summary>
        public int NightMinute { get; set; }
        /// <summary>
        /// 晚上段开始收费金额
        /// </summary>
        public decimal NightFirstMoney { get; set; }
        /// <summary>
        /// 跨段处理
        /// </summary>
        public int SpanProcessing { get; set; }
        /// <summary>
        /// 计费单位
        /// </summary>
        public int MonetaryUnit { get; set; }
        /// <summary>
        /// 离场超时分钟，缴费后可享有的免费时间
        /// </summary>
        public int LeaveTimeout { get; set; }
    }

    /// <summary>
    /// 分段按小时算费模型
    /// </summary>
    public class SegmentHourlyTollModel
    {
        /// <summary>
        /// 免费分钟
        /// </summary>
        public int FreeMinutes { get; set; }
        /// <summary>
        /// 白天开始时间
        /// </summary>
        public DateTime DayBeginTime { get; set; }
        /// <summary>
        /// 白天结束时间
        /// </summary>
        public DateTime DayEndTime { get; set; }
        /// <summary>
        /// 白天N小时内
        /// </summary>
        public int DayFirstHour { get; set; }
        /// <summary>
        /// 每M小时
        /// </summary>
        public int DayPerHour { get; set; }
        /// <summary>
        /// 白天收费
        /// </summary>
        public decimal DayFirstMoney { get; set; }
        /// <summary>
        /// 超过N小时后的消费
        /// </summary>
        public decimal DayNextMoney { get; set; }
        /// <summary>
        /// 晚上小时内
        /// </summary>
        public int NightFirstHour { get; set; }
        /// <summary>
        /// 每N个小时
        /// </summary>
        public int NightPerHour { get; set; }
        /// <summary>
        /// NightFirstMoney
        /// </summary>
        public decimal NightFirstMoney { get; set; }
        /// <summary>
        /// NightNextMoney
        /// </summary>
        public decimal NightNextMoney { get; set; }
        /// <summary>
        /// 超时时间
        /// </summary>
        public int Timeout { get; set; }
        /// <summary>
        /// 每天最高收费
        /// </summary>
        public decimal MaxDayMoney { get; set; }
        /// <summary>
        /// 跨段处理
        /// </summary>
        public int SpanProcessing { get; set; }
        /// <summary>
        /// 收费单位
        /// </summary>
        public int MonetaryUnit { get; set; }
        /// <summary>
        /// 离场超时分钟，缴费后可享有的免费时间
        /// </summary>
        public int LeaveTimeout { get; set; }
    }

    /// <summary>
    /// 无分段按半小时收费模型
    /// </summary>
    public class SegmentNoneHalfHourTollModel
    {
        /// <summary>
        /// 收费单位
        /// </summary>
        public int MonetaryUnit { get; set; }
        /// <summary>
        /// 免费分钟
        /// </summary>
        public int FreeMinutes { get; set; }
        /// <summary>
        /// 首几小时数
        /// </summary>
        public int FirstHalfHour { get; set; }
        /// <summary>
        /// 首几小时收费数
        /// </summary>
        public decimal FirstMoney { get; set; }
        /// <summary>
        /// 以后几小时数
        /// </summary>
        public int NextHalfHour { get; set; }
        /// <summary>
        /// 以后几小时收费数
        /// </summary>
        public decimal NextMoney { get; set; }
        /// <summary>
        /// 超时限制分钟
        /// </summary>
        public int Timeout { get; set; }
        /// <summary>
        /// 离场超时分钟，缴费后可享有的免费时间
        /// </summary>
        public int LeaveTimeout { get; set; }
    }

    /// <summary>
    /// 按半小时算费(可分段)
    /// </summary>
    public class SegmentHalfHourTollModel
    {
        /// <summary>
        /// 计费单位
        /// </summary>
        public int MonetaryUnit { get; set; }
        /// <summary>
        /// 免费分钟
        /// </summary>
        public int FreeMinutes { get; set; }
        /// <summary>
        /// 白天段开始
        /// </summary>
        public DateTime DayBeginTime { get; set; }
        /// <summary>
        /// 白天段结束
        /// </summary>
        public DateTime DayEndTime { get; set; }
        /// <summary>
        /// 白天首几半小时数
        /// </summary>
        public int DayFirstHalfHour { get; set; }
        /// <summary>
        /// 白天首几半小时收费数
        /// </summary>
        public decimal DayFirstMoney { get; set; }
        /// <summary>
        /// 白天首几半小时之后数
        /// </summary>
        public int DayNextHalfHour { get; set; }
        /// <summary>
        /// 白天首几半小时之后收费数
        /// </summary>
        public decimal DayNextMoney { get; set; }
        /// <summary>
        /// 白天最高收费
        /// </summary>
        public decimal DayMaxMoney { get; set; }
        /// <summary>
        /// 晚上首几半小时数
        /// </summary>
        public int NightFirstHalfHour { get; set; }
        /// <summary>
        /// 晚上首几半小时收费数
        /// </summary>
        public decimal NightFirstMoney { get; set; }
        /// <summary>
        /// 晚上首几半小时之后数
        /// </summary>
        public int NightNextHalfHour { get; set; }
        /// <summary>
        /// 晚上首几半小时之后收费数
        /// </summary>
        public decimal NightNextMoney { get; set; }
        /// <summary>
        /// 晚上最高收费
        /// </summary>
        public decimal NightMaxMoney { get; set; }
        /// <summary>
        /// 单次最高收费
        /// </summary>
        public decimal AmountTopLimit { get; set; }
        /// <summary>
        /// 24小时最高收费
        /// </summary>
        public decimal DayTopLimit { get; set; }
        /// <summary>
        /// 超过1天后，剩余不足1天时间晚上无前1小时收费，直接按每1个半小时收费
        /// </summary>
        public int FeeSet1 { get; set; }
        /// <summary>
        /// 超过1天后，剩余不足1天时间晚上无前1小时收费，直接按每1个半小时收费
        /// </summary>
        public int FeeSet2 { get; set; }
        /// <summary>
        /// 去掉免费分钟后再计算费用
        /// </summary>
        public int FeeSet3 { get; set; }
        /// <summary>
        /// 无分段，按白天段处理
        /// </summary>
        public int FeeSet4 { get; set; }
        /// <summary>
		/// 超时分钟
        /// </summary>
        public int TimeOut { get; set; }
        /// <summary>
        /// 离场超时分钟，缴费后可享有的免费时间
        /// </summary>
        public int LeaveTimeout { get; set; }
    }

    /// <summary>
    /// 新分段算费模型
    /// </summary>
    public class NewSegmentTollModel
    {
        /// <summary>
        /// 计费单位  1 元 0 角
        /// </summary>
        public int MonetaryUnit { get; set; }
        /// <summary>
        /// 收费开始时间 08:00
        /// </summary>
        public string BeginTime { get; set; }
        /// <summary>
        /// 收费截至时间 20:00
        /// </summary>
        public string EndTime { get; set; }
        /// <summary>
		/// 每天最大金额
        /// </summary>
        public decimal DayTopLimit { get; set; }
        /// <summary>
        /// 白天段最大收费金额
        /// </summary>
        public decimal DayTimeMaxAmount { get; set; }
        /// <summary>
        /// 夜间段最大收费金额
        /// </summary>
        public decimal NightTimeMaxAmount { get; set; }
        /// <summary>
        /// 免费分钟
        /// </summary>
        public int FreeMinutes { get; set; }
        /// <summary>
        /// 离场超时分钟，缴费后可享有的免费时间
        /// </summary>
        public int LeaveTimeout { get; set; }

        /// <summary>
        /// 停车时间跨段处理方式  0 跨段时将两段停留时间分别计算，并将计算结果相加 1 跨段时按停车时间逐段计算，最后将计算结果相加
        /// </summary>
        public int CrossSegmentsType { get; set; }
        /// <summary>
        /// 无分段时按白天段处理 0 不用 1 启用
        /// </summary>
        public int NoSegmentedIsEnable { get; set; }
        /// <summary>
        /// 免费时间的处理方式 0 不用 1 启用
        /// </summary>
        public int FreeTreatTypeIsEnable { get; set; }
        /// <summary>
        /// 免费时间处理方式 0总时间去掉免费时间再计算 1 分段处理免费时间
        /// </summary>
        public int FreeTreatType { get; set; }
        /// <summary>
        /// 跨段免费时间处理方式 0 白天段和晚上段都有免费分钟，但计算时不去掉免费分钟 1 白天段和晚上段都有免费分钟，但计算时去掉免费分钟
        /// </summary>
        public int CrossSegmentsFreeType { get; set; }
        /// <summary>
        /// 白天开始N小时
        /// </summary>
        public int DayTimeSt1 { get; set; }
        /// <summary>
        /// 白天开始N小时每M个半小时
        /// </summary>
        public int DayTimeFt1 { get; set; }
        /// <summary>
        /// 白天开始M个半小时收取费用
        /// </summary>
        public decimal DayTimeFee1 { get; set; }
        /// <summary>
        /// 白天超过N小时但在P小时内
        /// </summary>
        public int DayTimeSt2 { get; set; }
        /// <summary>
        /// 白天超过N小时在P小时内每M个半小时
        /// </summary>
        public int DayTimeFt2 { get; set; }
        /// <summary>
        /// 白天超过N小时在P小时内每M个半小时的收取费用
        /// </summary>
        public decimal DayTimeFee2 { get; set; }
        /// <summary>
        /// 白天停车时间大于P小时
        /// </summary>
        public int DayTimeSt3 { get; set; }
        /// <summary>
        /// 白天停车时间大于P小时每M个半小时
        /// </summary>
        public int DayTimeFt3 { get; set; }
        /// <summary>
        /// 白天停车时间大于P小时每M个半小时收取费用
        /// </summary>
        public decimal DayTimeFee3 { get; set; }

        /// <summary>
        /// 夜间开始N小时
        /// </summary>
        public int NightTimeSt1 { get; set; }
        /// <summary>
        /// 夜间开始N小时每M个半小时
        /// </summary>
        public int NightTimeFt1 { get; set; }
        /// <summary>
        /// 夜间开始M个半小时收取费用
        /// </summary>
        public decimal NightTimeFee1 { get; set; }
        /// <summary>
        ///夜间超过N小时但在P小时内
        /// </summary>
        public int NightTimeSt2 { get; set; }
        /// <summary>
        ///夜间超过N小时在P小时内每M个半小时
        /// </summary>
        public int NightTimeFt2 { get; set; }
        /// <summary>
        ///夜间超过N小时在P小时内每M个半小时的收取费用
        /// </summary>
        public decimal NightTimeFee2 { get; set; }
        /// <summary>
        ///夜间停车时间大于P小时
        /// </summary>
        public int NightTimeSt3 { get; set; }
        /// <summary>
        ///夜间停车时间大于P小时每M个半小时
        /// </summary>
        public int NightTimeFt3 { get; set; }
        /// <summary>
        ///夜间停车时间大于P小时每M个半小时收取费用
        /// </summary>
        public decimal NightTimeFee3 { get; set; }
        /// <summary>
        /// 超时分钟
        /// </summary>
        public int Timeout { get; set; }
    }

    /// <summary>
    /// 按15分钟收费(可分段)
    /// </summary>
    public class SegmentQuarterHourTollModel
    {
        /// <summary>
        /// 1 元 0 角 默认 元 2分
        /// </summary>
        public int MonetaryUnit { get; set; }
        /// <summary>
        /// 白天时段开始
        /// </summary>
        public DateTime DayBeginTime { get; set; }
        /// <summary>
        /// 白天时段结束
        /// </summary>
        public DateTime DayEndTime { get; set; }
        /// <summary>
        /// 白天免费分钟
        /// </summary>
        public int DayFreeMinutes { get; set; }
        /// <summary>
        /// 白天首几个15分时间段数
        /// </summary>
        public int DayFirstTime { get; set; }
        /// <summary>
        /// 白天首几个15分时间段收费数
        /// </summary>
        public decimal DayFirstMoney { get; set; }
        /// <summary>
        /// 白天首几个15分之后时间段数
        /// </summary>
        public int DayNextTime { get; set; }
        /// <summary>
        /// 白天首几个15分之后时间段收费数
        /// </summary>
        public decimal DayNextMoney { get; set; }
        /// <summary>
        /// 白天超时时间
        /// </summary>
        public int DayTimeOut { get; set; }
        /// <summary>
        /// 白天最高收费
        /// </summary>
        public decimal DayMaxMoney { get; set; }
        /// <summary>
        /// 晚上免费分钟
        /// </summary>
        public int NightFreeMinutes { get; set; }
        /// <summary>
        /// 晚上首几个15分时间段数
        /// </summary>
        public int NightFirstTime { get; set; }
        /// <summary>
        /// 晚上首几个15分时间段收费数
        /// </summary>
        public decimal NightFirstMoney { get; set; }
        /// <summary>
        /// 晚上首几个15分之后时间段数
        /// </summary>
        public int NightNextTime { get; set; }
        /// <summary>
        /// 晚上首几个15分之后时间段收费数
        /// </summary>
        public decimal NightNextMoney { get; set; }
        /// <summary>
        /// 晚上超时时间
        /// </summary>
        public int NightTimeOut { get; set; }
        /// <summary>
		/// 晚上最高收费
        /// </summary>
        public decimal NightMaxMoney { get; set; }
        /// <summary>
        /// 本次最高收费
        /// </summary>
        public decimal AmountTopLimit { get; set; }
        /// <summary>
        /// 每天最高收费
        /// </summary>
        public decimal DayTopLimit { get; set; }
        /// <summary>
        /// 无分段 按白天段处理
        /// </summary>
        public int FeeSet1 { get; set; }
        /// <summary>
        /// 几个免费分钟
        /// </summary>
        public int FeeSet2 { get; set; }
        /// <summary>
        /// 计算时是否去掉免费分钟
        /// </summary>
        public int FeeSet3 { get; set; }
        /// <summary>
        /// 收费金额精确到 元 角 分
        /// </summary>
        public int FeeSet4 { get; set; }
        /// <summary>
        /// 跨段收费规则
        /// </summary>
        public int FeeSet5 { get; set; }
        /// <summary>
        ///  超过1天后，剩余不足1天时间白天段无前N小时收费，直接按每M个15分钟收费
        /// </summary>
        public int FeeSet6 { get; set; }
        /// <summary>
        ///  超过1天后，剩余不足1天时间晚上段无前N小时收费，直接按每M个15分钟收费
        /// </summary>
        public int FeeSet7 { get; set; }
        /// <summary>
        /// FeeSet7
        /// </summary>
        public int FeeSet8 { get; set; }
        /// <summary>
        /// FeeSet8
        /// </summary>
        public int FeeSet9 { get; set; }
        /// <summary>
        /// FeeSet10 超时分钟 按10进制 或 16进制
        /// </summary>
        public int FeeSet10 { get; set; }
        /// <summary>
        /// 离场超时分钟，缴费后可享有的免费时间
        /// </summary>
        public int LeaveTimeout { get; set; }
    }
}
