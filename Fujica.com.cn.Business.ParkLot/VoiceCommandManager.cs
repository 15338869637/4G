/***************************************************************************************
 * *
 * *        File Name        : VoiceCommandManager.cs
 * *        Creator          : llp
 * *        Create Time      : 2019-09-21 
 * *        Remark           : 语音指令管理器 逻辑类
 * *
 * *  Copyright (c) 深圳市富士智能系统有限公司.  All rights reserved. 
 * ***************************************************************************************/
using Fujica.com.cn.Business.Base;
using Fujica.com.cn.Business.Model;
using Fujica.com.cn.Communication.RabbitMQ;
using Fujica.com.cn.Context.IContext;
using Fujica.com.cn.Context.Model;
using Fujica.com.cn.Logger;
using Fujica.com.cn.Tools;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fujica.com.cn.Business.ParkLot
{
    /// <summary>
    /// 语音指令管理器.
    /// </summary> 
    /// <remarks>
    /// 2019.09.21: 日志参数完善. llp <br/> 
    /// </remarks>   
    public class VoiceCommandManager : IBaseBusiness
    {
        /// <summary>
        /// 日志记录器
        /// </summary>
        internal readonly ILogger m_logger;
        /// <summary>
        /// 序列化器
        /// </summary>
        internal readonly ISerializer m_serializer = null;

        private readonly RabbitMQSender m_rabbitMQ;

        /// <summary>
        /// 语音信息操作上下文
        /// </summary>
        private IVoiceCommandContext _iVoiceCommandContext = null;

        public string LastErrorDescribe
        {
            get;
            set;
        }

        public VoiceCommandManager(ILogger _logger, ISerializer _serializer,
            RabbitMQSender _rabbitMQ,
            IVoiceCommandContext iVoiceCommandContext)
        {
            m_logger = _logger;
            m_serializer = _serializer;
            m_rabbitMQ = _rabbitMQ;
            _iVoiceCommandContext = iVoiceCommandContext;
        }

        /// <summary>
        /// 保存指令
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool SaveCommand(VoiceCommandModel model)
        {
            bool flag = _iVoiceCommandContext.SaveCommand(model);
            if (!flag)
            {
                LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_SAVE_VOICECOMMAND.GetDesc();
                return false;
            }
            //同步语音命令给相机
            flag = SendVoiceToMq(model);
            if (!flag)
            {
                LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_MQ_SEND_ERROR.GetDesc();
                return false;
            }
            return flag;
        }

        /// <summary>
        /// 读取指令
        /// </summary>
        /// <param name="drivewayGuid">车道唯一id</param>
        /// <returns></returns>
        public VoiceCommandModel GetCommand(string drivewayGuid)
        {
            return _iVoiceCommandContext.GetCommand(drivewayGuid);
        }

        /// <summary>
        /// 初始化语音指令
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool InitVoiceCommand(VoiceCommandModel model)
        {
            if (model == null) return false;
            //初始化语音指令
            if (model.CommandList == null || model.CommandList.Count <= 0)
            {
                model.CommandList = new List<CommandDetialModel>();
                foreach (VoiceCommand commandItem in Enum.GetValues(typeof(VoiceCommand)))
                {
                    CommandDetialModel command1 = new CommandDetialModel();
                    switch (commandItem)
                    {
                        case VoiceCommand.EntranceFree:
                            command1.CommandType = VoiceCommand.EntranceFree;
                            command1.ShowVoice = "";
                            command1.ShowText = "深圳富士智能系统有限公司研制";
                            break;
                        case VoiceCommand.ExportFree:
                            command1.CommandType = VoiceCommand.ExportFree;
                            command1.ShowVoice = "";
                            command1.ShowText = "深圳富士智能系统有限公司研制";
                            break;
                        case VoiceCommand.TempCarIn:
                            command1.CommandType = VoiceCommand.TempCarIn;
                            command1.ShowVoice = "<p>，欢迎光临";
                            command1.ShowText = "<p>\r\n欢迎光临";
                            break;
                        case VoiceCommand.UnPaidTempCarOut:
                            command1.CommandType = VoiceCommand.UnPaidTempCarOut;
                            command1.ShowVoice = "<p>，请扫码缴费";
                            command1.ShowText = "<p>\r\n请扫码缴费";
                            break;
                        case VoiceCommand.PaidTempCarOut:
                            command1.CommandType = VoiceCommand.PaidTempCarOut;
                            command1.ShowVoice = "<p>，一路顺风";
                            command1.ShowText = "<p>\r\n一路顺风";
                            break;
                        case VoiceCommand.PaidTempCarOverTime:
                            command1.CommandType = VoiceCommand.PaidTempCarOverTime;
                            command1.ShowVoice = "<p>，离场超时，请扫码补缴费用";
                            command1.ShowText = "<p>，离场超时\r\n请扫码补缴费用";
                            break;
                        case VoiceCommand.MonthCarIn:
                            command1.CommandType = VoiceCommand.MonthCarIn;
                            command1.ShowVoice = "<p>，欢迎光临，可用日期<d>天";
                            command1.ShowText = "<p>，欢迎光临\r\n可用日期<d>天";
                            break;
                        case VoiceCommand.MonthCarInRemind:
                            command1.CommandType = VoiceCommand.MonthCarInRemind;
                            command1.ShowVoice = "<p>，欢迎光临，可用日期<d>天，请尽快延期";
                            command1.ShowText = "<p>，欢迎光临\r\n可用日期<d>天，请尽快延期";
                            break;
                        case VoiceCommand.MonthCarOverDate:
                            command1.CommandType = VoiceCommand.MonthCarOverDate;
                            command1.ShowVoice = "<p>，月卡已过期，请续费";
                            command1.ShowText = "<p>\r\n月卡已过期，请续费";
                            break;
                        case VoiceCommand.MonthCarOut:
                            command1.CommandType = VoiceCommand.MonthCarOut;
                            command1.ShowVoice = "<p>，一路顺风，可用日期<d>天";
                            command1.ShowText = "<p>，一路顺风\r\n可用日期<d>天";
                            break;
                        case VoiceCommand.MonthCarOutRemind:
                            command1.CommandType = VoiceCommand.MonthCarOutRemind;
                            command1.ShowVoice = "<p>，一路顺风，可用日期<d>天，请尽快延期";
                            command1.ShowText = "<p>，一路顺风\r\n可用日期<d>天，请尽快延期";
                            break;
                        case VoiceCommand.ValueCarIn:
                            command1.CommandType = VoiceCommand.ValueCarIn;
                            command1.ShowVoice = "<p>，欢迎光临，余额<b>元";
                            command1.ShowText = "<p>，欢迎光临\r\n余额<b>元";
                            break;
                        case VoiceCommand.ValueCarInRemind:
                            command1.CommandType = VoiceCommand.ValueCarInRemind;
                            command1.ShowVoice = "<p>，欢迎光临，余额<b>元，请尽快充值";
                            command1.ShowText = "<p>，欢迎光临\r\n余额<b>元，请尽快充值";
                            break;
                        case VoiceCommand.ValueCarOut:
                            command1.CommandType = VoiceCommand.ValueCarOut;
                            command1.ShowVoice = "<p>，一路顺风，此次收费<c>元，余额<b>元";
                            command1.ShowText = "<p>，一路顺风\r\n此次收费<c>元，余额<b>元";
                            break;
                        case VoiceCommand.ValueCarOutRemind:
                            command1.CommandType = VoiceCommand.ValueCarOutRemind;
                            command1.ShowVoice = "<p>，一路顺风，此次收费<c>元，余额<b>元，请尽快充值";
                            command1.ShowText = "<p>，一路顺风\r\n此次收费<c>元，余额<b>元，请尽快充值";
                            break;
                        case VoiceCommand.ValueCarInsufficient:
                            command1.CommandType = VoiceCommand.ValueCarInsufficient;
                            command1.ShowVoice = "<p>，余额不足，请充值后出场";
                            command1.ShowText = "<p>\r\n余额不足，请充值后出场";
                            break;
                        case VoiceCommand.DriveWayRestriction:
                            command1.CommandType = VoiceCommand.DriveWayRestriction;
                            command1.ShowVoice = "<p>，该时间禁止通行";
                            command1.ShowText = "<p>\r\n该时间禁止通行";
                            break;
                        case VoiceCommand.NoEntryRecord:
                            command1.CommandType = VoiceCommand.NoEntryRecord;
                            command1.ShowVoice = "<p>，无入场记录";
                            command1.ShowText = "<p>\r\n无入场记录";
                            break;
                        case VoiceCommand.ManualOpenGate:
                            command1.CommandType = VoiceCommand.ManualOpenGate;
                            command1.ShowVoice = "此次开闸需确认，请联系管理员开闸";
                            command1.ShowText = "此次开闸需确认，请联系管理员开闸";
                            break;
                    }
                    model.CommandList.Add(command1);
                }
            }
            return SaveCommand(model);
        }

        /// <summary>
        /// 发送语音指令给相机
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private bool SendVoiceToMq(VoiceCommandModel model)
        {
            try
            {
                //需要下发的数据
                List<VoiceCommandDetialModel> sendList = new List<VoiceCommandDetialModel>();
                foreach (var item in model.CommandList)
                {
                    sendList.Add(new VoiceCommandDetialModel()
                    {
                        CommandType = item.CommandType.ToString(),
                        ShowText = item.ShowText,
                        ShowVoice = item.ShowVoice,
                        DeviceIdentify = model.DeviceMacAddress
                    });
                }

                //每次只发送10条，分批发完（缓解相机的压力，担心相机缓冲区不够大）
                int itemCount = 10;

                for (int i = 0; i < Math.Ceiling((double)sendList.Count() / itemCount); i++)
                {
                    CommandEntity<List<VoiceCommandDetialModel>> entity = new CommandEntity<List<VoiceCommandDetialModel>>()
                    {
                        command = BussineCommand.Remind,
                        idMsg = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                        message = sendList.Skip(i * itemCount).Take(itemCount).ToList()
                    };

                    m_rabbitMQ.SendMessageForRabbitMQ("发送语音指令命令", m_serializer.Serialize(entity), entity.idMsg, model.ParkCode);
                }
                return true;

            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.Bussiness, "", model.ParkCode, "", "Fujica.com.cn.Business.SystemSet.VoiceCommandManager.SendVoiceToMq", "下发语音指令数据时发生异常", ex.ToString());
                return false;
            }
        }
    }
}
