using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.Business.Base
{
    /// <summary>
    /// 业务命令
    /// </summary>
    public enum BussineCommand
    {
        /// <summary>
        /// 相机 用于获取相机信息
        /// </summary>
        CameraInfo = 0,
        /// <summary>
        /// 开闸
        /// </summary>
        OpenGate = 1,
        /// <summary>
        /// 拍照
        /// </summary>
        Photograph = 2,
        /// <summary>
        /// 修正 车牌 等数据
        /// </summary>
        Correct = 3,
        /// <summary>
        /// 补录入场
        /// </summary>
        AddRecord = 4,
        /// <summary>
        /// 地感
        /// </summary>
        Inductance = 5,
        /// <summary>
        /// 语音与屏显 提醒命令
        /// </summary>
        Remind = 6,
        /// <summary>
        /// 限行指令
        /// </summary>
        Restrict = 7,
        /// <summary>
        /// 临时车车类指令
        /// </summary>
        CarType = 8,
        /// <summary>
        /// 黑名单指令
        /// </summary>
        BlackList = 9,
        /// <summary>
        /// 临时卡 开卡
        /// </summary>
        TempCar = 10,
        /// <summary>
        /// 月卡  不论开卡，销卡，续费皆为此指令
        /// </summary>
        MonthCar = 11,
        /// <summary>
        /// 储值卡 不论开卡，销卡，续费皆为此指令
        /// </summary>
        ValueCar = 12,
        /// <summary>
        /// 入场数据广播命令
        /// </summary>
        Broadcast = 13,
        /// <summary>
        /// 出场数据广播命令
        /// <summary>
        BroadcastOut = 14,
        /// <summary>
        /// 创建新城市区号指令
        /// </summary>
        NewCityCode = 15,
        /// <summary>
        /// 同步停车数据（临时车、月卡车、储值车等）
        /// </summary>
        SyncParking = 16,
        /// <summary>
        /// 车场满位禁止入场车类 命令
        /// </summary>
        BarredEntryFull = 17,
        /// <summary>
        /// 月租车延期提醒天数
        /// </summary>
        MonthCarExpireRemindDay = 18,
        /// <summary>
        /// 月卡车过期处理 -1=禁止入场 0=看做临时车 大于等于N(N>=1) 过期N天后禁止入场
        /// </summary>
        MonthCarExpireMode = 19,
        /// <summary>
        /// 车道闸口锁定
        /// </summary>
        GateKeep = 20,
        /// <summary>
        /// 车牌修改重推
        /// </summary>
        CarNumberRepush = 21,
        /// <summary>
        /// 相机升级
        /// </summary>
        CameraUpdate = 22,
        /// <summary>
        /// 车道拦截
        /// </summary>
        GateCatch = 23,
        /// <summary>
        /// 新建车场（首次发送）
        /// </summary>
        NewParkLot = 24,
        /// <summary>
        /// 手动开闸车类集合
        /// </summary>
        ManualOpenGate = 25,
        /// <summary>
        /// 剩余车位数量
        /// </summary>
        RemainingSpace = 26,
        /// <summary>
        /// 删除车道时 发送指令
        /// </summary>
        CameraDelete = 27,
        /// <summary>
        /// 修改车道时发送指令
        /// </summary>
        CameraEdit = 28,
        /// <summary>
        /// 相机心跳
        /// </summary>
        HeartBeat = 29,
        /// <summary>
        /// 场内出场删除
        /// </summary>
        FieldDelete = 30


    }

    /// <summary>
    /// 指令数据封装
    /// </summary>
    public class CommandEntity<T> where T : new()
    {
        /// <summary>
        /// 指令
        /// </summary>
        public BussineCommand command { get; set; }

        /// <summary>
        /// 消息id，ACK回复时候的key
        /// </summary>
        public string idMsg { get; set; }

        /// <summary>
        /// 具体指令数据
        /// </summary>
        public T message { get; set; }

    }
}
