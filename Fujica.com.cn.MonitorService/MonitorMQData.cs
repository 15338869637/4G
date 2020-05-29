using fujica.com.cn.RabbitMQFactory.Helpers;
using fujica.com.cn.RabbitMQFactory.Services;
using fujica.com.cn.RabbitMQFactory.Utils;
using Fujica.com.cn.Business.Base;
using Fujica.com.cn.Context.Model;
using Fujica.com.cn.Logger;
using Fujica.com.cn.MonitorService.Business;
using Fujica.com.cn.Tools;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Fujica.com.cn.MonitorService
{
    public static class MonitorMQData
    {
        #region 基础参数配置

        /// <summary>
        /// mq连接地址
        /// </summary>
        private static string MQ_UrlParam = ConfigurationManager.AppSettings["mq_url"].ToString();
        /// <summary>
        /// mq用户名
        /// </summary>
        private static string MQ_UserNameParam = "name_FuJiCaHostName_Administrator";
        /// <summary>
        /// mq密码
        /// </summary>
        private static string MQ_PwdParam = "name_FujicaYun11101110110101100000011101101000";
        /// <summary>
        /// 独立项目编号配置
        /// </summary>
        private static string MQManufacturerCode = ConfigurationManager.AppSettings["MQManufacturerCode"];

        private static ILogger m_ilogger = new Logger.Logger();

        private static ISerializer m_serializer = new JsonSerializer(m_ilogger);

        #endregion

        public static void Execute()
        {

            //初始化mq连接参数对象
            MQConfig config = new MQConfig()
            {
                Url = MQ_UrlParam,
                UserName = MQ_UserNameParam.Substring(5),
                Pwd = string.Concat(MQ_PwdParam.Substring(5, 9), System.Convert.ToInt64(MQ_PwdParam.Substring(14), 2).ToString()),
            };

            MQServcieManager mqManager = new MQServcieManager();

            //循环读取所有已存在的区号列表
            List<string> citycodeList = CityCodeDataManager.GetAllCityCodeList();
            foreach (var item in citycodeList)
            {
                InitMonitorCityCode(mqManager, config, item);
            }

            //监听“创建新地区”的队列
            MonitorNewParking(mqManager, config);

            //监听“付款数据”的队列
            MonitorPayData(mqManager, config);

            //日志记录
            mqManager.OnShowClientAction = OnActionOutputLog;

            //动态添加新地区的回调
            mqManager.AddClientBusAction = OnActionAddClientBus;

            //开始执行
            mqManager.Start();

            Thread tMain = new Thread(() => ThreadMain(mqManager, config));
            tMain.IsBackground = true;
            tMain.Start();


        }


        /// <summary>
        /// 日志记录
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        private static void OnActionOutputLog(MQMessageLogLevel level, string message, Exception ex)
        {
            m_ilogger.LogInfo(LoggerLogicEnum.Tools, "", "", "", "Fujica.com.cn.MonitorService.MonitorMQData.OnActionOutput", "接收MQ日志信息：" + message, Convert.ToString(ex));

        }

        /// <summary>
        /// 显示动态添加的消费者
        /// </summary>
        /// <param name="mQAddExcahngeType"></param>
        /// <param name="msg"></param>
        private static void OnActionAddClientBus(MQAddExcahngeType mQAddExcahngeType, string msg)
        {
            //等李兵确认返回参数（成功/失败）
            m_ilogger.LogInfo(LoggerLogicEnum.Tools, "", "", "", "Fujica.com.cn.MonitorService.MonitorMQData.OnActionAddClientBus", "动态添加消费者日志信息：" + msg);
        }

        /// <summary>
        /// 初始化每个地区创建独立监听队列
        /// （区号 如：0755、0731）
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="config"></param>
        private static void InitMonitorCityCode(MQServcieManager mqManager, MQConfig config, string cityCode)
        {
            //接收数据队列
            MQQueueInfo mQQueueInfo1 = new MQQueueInfo()
            {
                QueueName = MQManufacturerCode + cityCode + "YunParkCameraQueue",
                RoutingKey = cityCode,
                ExchangeType = RabbitMQ.Client.ExchangeType.Topic
            };

            IMQService mQService = new CreateMQService(config, mQQueueInfo1);
            mQService.Exchange = MQYunCameraExchange.TopicFuJiCaYunCameraParkPushExchange;
            mqManager.AddServices(mQService);
        }

        /// <summary>
        /// 动态创建地区监听队列
        /// </summary>
        /// <param name="mqManager"></param>
        /// <param name="config"></param>
        /// <param name="cityCode"></param>
        private static void DynamicAddMonitorCityCode(MQServcieManager mqManager, MQConfig config, string cityCode)
        {

            MQQueueInfo mQQueueInfo = new MQQueueInfo()
            {
                QueueName = MQManufacturerCode + cityCode + "YunParkCameraQueue",
                RoutingKey = cityCode,
                ExchangeType = RabbitMQ.Client.ExchangeType.Topic
            };

            IMQService mQService = new CreateMQService(config, mQQueueInfo);
            mQService.Exchange = MQYunCameraExchange.TopicFuJiCaYunCameraParkPushExchange;

            MQChannel m_MQChannel3 = new MQChannel(mQQueueInfo.ExchangeType, mQQueueInfo.ExchangeName, mQQueueInfo.QueueName, mQQueueInfo.RoutingKey);
            Dictionary<IMQService, MQChannel> IMQServiceAndMQChannel3 = new Dictionary<IMQService, MQChannel>();
            IMQServiceAndMQChannel3.Add(mQService, m_MQChannel3);

            mqManager.DynamicAddMQChannel.Enqueue(IMQServiceAndMQChannel3);
        }

        /// <summary>
        /// 监听“创建新地区”的队列
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="config"></param>
        private static void MonitorNewParking(MQServcieManager mqManager, MQConfig config)
        {
            MQQueueInfo mQQueueInfo11 = new MQQueueInfo()
            {
                QueueName = MQManufacturerCode + "NewCityCodeQueue",
                RoutingKey = "NewCityCode4007004008",
                ExchangeType = RabbitMQ.Client.ExchangeType.Direct,
                ExchangeName = MQYunCameraExchange.DirectDynamicAddNewCityCode
            };

            IMQService mQService = new CreateMQService(config, mQQueueInfo11);
            mqManager.AddServices(mQService);
        }

        /// <summary>
        /// 监听“付款数据”的队列
        /// </summary>
        /// <param name="mqManager"></param>
        /// <param name="config"></param>
        private static void MonitorPayData(MQServcieManager mqManager, MQConfig config)
        {
            MQQueueInfo mQQueueInfo11 = new MQQueueInfo()
            {
                QueueName = MQManufacturerCode + "PayDataQueue.YunPark",
                RoutingKey = "PayData4007004008",
                ExchangeType = RabbitMQ.Client.ExchangeType.Topic,
                ExchangeName = MQYunCameraExchange.TopicPushPayDataToYunParkExchange
            };

            IMQService mQService = new CreateMQService(config, mQQueueInfo11);
            mqManager.AddServices(mQService);
        }

        /// <summary>
        /// 主线程
        /// 控制多线程
        /// </summary>
        /// <param name="mqManager"></param>
        /// <param name="config"></param>
        private static void ThreadMain(MQServcieManager mqManager, MQConfig config)
        {
            int threadCount = 1;
            Int32.TryParse(ConfigurationManager.AppSettings["ThreadCount"], out threadCount);

            for (int i = 0; i < threadCount; i++)
            {
                Thread t = new Thread(() => MonitorResult(mqManager, config));
                t.Start();
            }
        }

        /// <summary>
        /// 子线程
        /// 接收MQ返回的结果
        /// </summary>
        /// <param name="mqManager"></param>
        /// <param name="config"></param>
        /// <param name="e"></param>
        private static void MonitorResult(MQServcieManager mqManager, MQConfig config)
        {
            while (true)
            {
                Thread.Sleep(300);

                while (MQOffLineCameraMsg.MQOffLineCameraMsgQueue != null && 
                    MQOffLineCameraMsg.MQOffLineCameraMsgQueue.Count > 0)
                {
                    //循环读取返回数据
                    string mqMessage = string.Empty;

                    MQOffLineCameraMsg.MQOffLineCameraMsgQueue.TryDequeue(out mqMessage);

                    if (!string.IsNullOrEmpty(mqMessage))
                    {
                        m_ilogger.LogInfo(LoggerLogicEnum.Tools, "", "", "", "Fujica.com.cn.MonitorService.MonitorMQData.MonitorResult", "接收到MQ的发送的命令：" + mqMessage);

                        try
                        {
                            //Match routingkeyMatch = Regex.Match(mqMessage.RoutingKey, @"\.(?<cityCode>\d{4})\.(?<command>.*)");
                            //string cityCode = routingkeyMatch.Success ? routingkeyMatch.Groups["cityCode"].Value : "";
                            //string commandStr = routingkeyMatch.Success ? routingkeyMatch.Groups["command"].Value : "";

                            CommandEntity<object> mqCityCodeModel = m_serializer.Deserialize<CommandEntity<object>>(mqMessage);

                            if (mqCityCodeModel != null && mqCityCodeModel.message != null)
                            {
                                string messageContent = Convert.ToString(mqCityCodeModel.message);
                                if (mqCityCodeModel.command == BussineCommand.Broadcast)
                                {
                                    //入场数据
                                    EntryDataManager.RedisSetData(Convert.ToString(mqCityCodeModel.message), m_ilogger);
                                }
                                else if (mqCityCodeModel.command == BussineCommand.BroadcastOut)
                                {
                                    //出场数据
                                    ExitDataManager.RedisSetData(Convert.ToString(mqCityCodeModel.message), m_ilogger);
                                }
                                else if (mqCityCodeModel.command == BussineCommand.Inductance)
                                {
                                    //压地感数据
                                    ExitPayDataManager.RedisSetData(Convert.ToString(mqCityCodeModel.message), m_ilogger);
                                }
                                else if (mqCityCodeModel.command == BussineCommand.GateCatch)
                                {
                                    //车道拦截数据
                                    GateCatchDataManager.RedisSetData(Convert.ToString(mqCityCodeModel.message), m_ilogger);
                                }
                                else if (mqCityCodeModel.command == BussineCommand.HeartBeat)
                                {
                                    //相机心跳数据
                                    HeartBeatDataManager.RedisSetData(Convert.ToString(mqCityCodeModel.message), m_ilogger);
                                }
                                //else if (mqMessage.RoutingKey.IndexOf(BussineCommand.NewCityCode.ToString()) > 0)
                                //{
                                //    //创建新地区，则创建消费者监听该新地区的数据
                                //    string newCityCode = mqMessage.Content;
                                //    DynamicAddMonitorCityCode(mqManager, config, newCityCode);
                                //}
                                else
                                {
                                    m_ilogger.LogError(LoggerLogicEnum.Tools, "", "", "", "Fujica.com.cn.MonitorService.MonitorMQData.MonitorResult", "接收MQ无法识别命令类型：" + mqMessage);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            //考虑redis挂掉，建立副数据库连接
                            m_ilogger.LogFatal(LoggerLogicEnum.Tools, "", "", "", "Fujica.com.cn.MonitorService.MonitorMQData.MonitorResult", "接收MQ返回的结果进行解析异常", ex.ToString());
                        }
                    }
                }

                while (MQOnLineMsg.MQSaveCityCodeMsgQueue != null && MQOnLineMsg.MQSaveCityCodeMsgQueue.Count > 0)
                {
                    string mqMessage = string.Empty;
                    MQOnLineMsg.MQSaveCityCodeMsgQueue.TryDequeue(out mqMessage);

                    if (!string.IsNullOrEmpty(mqMessage))
                    {
                        MQPackongVehicleEntry mqModel = m_serializer.Deserialize<MQPackongVehicleEntry>(mqMessage);
                        if (mqModel != null)
                        {
                            if (mqModel.rabbitMQType == RabbitMQProduserMsgType.YunParkPayData_topic)
                            {
                                //支付数据（月卡续费、储值卡充值）
                                PayDataManager.RedisSetData(mqModel.json, m_ilogger);
                            }
                            else if (mqModel.rabbitMQType == RabbitMQProduserMsgType.Other01Type_direct)
                            {
                                CommandEntity<CityCodeModel> mqCityCodeModel = m_serializer.Deserialize<CommandEntity<CityCodeModel>>(mqModel.json);
                                if (mqCityCodeModel != null && mqCityCodeModel.message != null)
                                {
                                    if (!string.IsNullOrEmpty(mqCityCodeModel.message.CodeID))
                                    {
                                        //创建新地区，则创建消费者监听该新地区的数据
                                        DynamicAddMonitorCityCode(mqManager, config, mqCityCodeModel.message.CodeID);
                                    }
                                }
                            }
                        }
                    }
                }

            }
        }
    }
}
