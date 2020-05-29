using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.Context.Model
{
    /// <summary>
    /// 语音指令模型
    /// </summary>
    public class VoiceCommandModel : IBaseModel
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
        /// 对应车道
        /// </summary>
        public string DrivewayGuid { get; set; }

        /// <summary>
        /// 车道相机标识
        /// </summary>
        public string DeviceMacAddress { get; set; }

        /// <summary>
        /// 指令列表
        /// </summary>
        public List<CommandDetialModel> CommandList { get; set; }
    }

    /// <summary>
    /// 指令详情
    /// </summary>
    public class CommandDetialModel
    { 
        /// <summary>
        /// 命令类型(名称)
        /// </summary>
        public VoiceCommand CommandType { get; set; }

        /// <summary>
        /// 语音展示
        /// </summary>
        public string ShowVoice { get; set; }

        /// <summary>
        /// 文字展示
        /// </summary>
        public string ShowText { get; set; }
    }

    /// <summary>
    /// 指令枚举
    /// </summary>
    public enum VoiceCommand
    {
        /// <summary>
        /// 入口空闲
        /// </summary>
        EntranceFree=0,
        /// <summary>
        /// 出口空闲
        /// </summary>
        ExportFree = 1,
        /// <summary>
        /// 临时车入场识别
        /// </summary>
        TempCarIn = 2,
        /// <summary>
        /// 未缴费临时车出场识别
        /// </summary>
        UnPaidTempCarOut = 3,
        /// <summary>
        /// 已缴费临时车出场识别
        /// </summary>
        PaidTempCarOut = 4,
        /// <summary>
        /// 已缴费临时车离场超时出场识别
        /// </summary>
        PaidTempCarOverTime = 5,
        /// <summary>
        /// 月卡车入场识别
        /// </summary>
        MonthCarIn = 6,
        /// <summary>
        /// 月卡车入场识别提醒延期
        /// </summary>
        MonthCarInRemind = 7,
        /// <summary>
        /// 已过期月卡车入场识别
        /// </summary>
        MonthCarOverDate = 8,
        /// <summary>
        /// 月卡车出场识别
        /// </summary>
        MonthCarOut = 9,
        /// <summary>
        /// 月卡车出场识别提醒延期
        /// </summary>
        MonthCarOutRemind = 10,
        /// <summary>
        /// 储值车入场识别
        /// </summary>
        ValueCarIn = 11,
        /// <summary>
        /// 储值车入场识别提醒充值
        /// </summary>
        ValueCarInRemind = 12,
        /// <summary>
        /// 储值车出场识别
        /// </summary>
        ValueCarOut = 13,
        /// <summary>
        /// 储值车出场识别提醒充值
        /// </summary>
        ValueCarOutRemind = 14,
        /// <summary>
        /// 储值车出场余额不足
        /// </summary>
        ValueCarInsufficient = 15,
        /// <summary>
        /// 车道通行限制
        /// </summary>
        DriveWayRestriction = 16,
        /// <summary>
        /// 无入场记录
        /// </summary>
        NoEntryRecord = 17,
        /// <summary>
        /// 手动开闸
        /// </summary>
        ManualOpenGate = 18

    }
}
