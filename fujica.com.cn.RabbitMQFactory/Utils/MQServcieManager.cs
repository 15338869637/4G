using fujica.com.cn.RabbitMQFactory.Helpers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace fujica.com.cn.RabbitMQFactory.Utils
{
    /// <summary>
    /// RabbitMQ连接管理《除了自带的自动重连机制，还要定时检测连接是否健康》
    /// </summary>
    public class MQServcieManager : IDisposable
    {
        private const string nameSpaceStr = "fujica.com.cn.RabbitMQFactory.Utils.MQServcieManager.";

        public int Timer_tick { get; set; } = 60 * 1000;
        public Timer timer = null;


        Thread RabbitMQConnPoolTh = null;
        Thread DynamicAddMQChannelTh = null;


        /// <summary>
        /// 记录消息日志函数
        /// </summary>
        public Action<MQMessageLogLevel, string, Exception> OnShowClientAction = null;
        /// <summary>
        /// 记录动态添加topic 消费者
        /// </summary>
        public Action<MQAddExcahngeType,string> AddClientBusAction = null;

        public List<IMQService> MQServices { get; set; } = new List<IMQService>();
        /// <summary>
        /// 动态添加新车场 车场按停车场编码的区号分
        /// </summary>
        public ConcurrentQueue<Dictionary<IMQService, MQChannel>> DynamicAddMQChannel { get; set; } = new ConcurrentQueue<Dictionary<IMQService, MQChannel>>();

        /// <summary
        /// 初始化
        /// </summary>
        public MQServcieManager()
        {            

            RabbitMQConnPoolTh = new Thread(new ThreadStart(RabbitMQConnPool))
            {
                IsBackground = true
            }; RabbitMQConnPoolTh.Start();

            DynamicAddMQChannelTh = new Thread(new ThreadStart(DynamicAddMQChannelPool))
            {
                IsBackground = true
            }; DynamicAddMQChannelTh.Start();



            /* 发送消息线程
            Thread SendMsgTh = new Thread(new ThreadStart(RabbitMQSendMsgPool))
            {
                IsBackground = true
            }; SendMsgTh.Start();
            */
        }

        /// <summary>
        /// 添加 IMQService
        /// </summary>
        /// <param name="mQService"></param>
        public void AddServices(IMQService mQService)
        {
            MQServices.Add(mQService);

        }

        /// <summary>
        /// 定时检测RabbitMQ连接状态，
        /// </summary>        
        private void RabbitMQConnPool()
        {

            while (true)
            {
                Thread.Sleep(5 * 60 * 1000);
                try
                {
                    int error = 0, reconnect = 0;
                    OnShowClientAction?.Invoke(MQMessageLogLevel.Debug, "开始检测连接状态", null);
                    foreach (var item in this.MQServices)
                    {
                        for (int i = 0; i < item.List_Channels.Count; i++)
                        {
                            var c = item.List_Channels[i];
                            if (c.Connection == null || !c.Connection.IsOpen)
                            {
                                error++;
                                OnShowClientAction?.Invoke(MQMessageLogLevel.Fatal, $"{c.ExchangeName} {c.ExchangeType} {c.Queue} {c.RoutingKey} 重新创建连接", null);
                                try
                                {
                                    c.CloseMQConnection();
                                    var channel = item.CreateChannel(c.Queue, c.RoutingKey, c.ExchangeType);
                                    item.List_Channels.Remove(c);
                                    item.List_Channels.Add(channel);

                                    OnShowClientAction?.Invoke(MQMessageLogLevel.Info, $"{c.ExchangeName}{c.ExchangeType} {c.Queue} {c.RoutingKey} 重新创建连接完成", null);
                                    reconnect++;

                                }
                                catch (Exception ex)
                                {
                                    OnShowClientAction?.Invoke(MQMessageLogLevel.Error, ex.Message, ex);
                                }
                            }
                            else
                            {
                                //--这些发送消息是用来测试的
                                c.PulishMySelf(i + string.Format(@"hello:{0} c.PulishMySelf {1} queueName={2} ", DateTime.Now, Guid.NewGuid().ToString(), c.Queue + "---" + c.ExchangeName + "---" + c.ExchangeType));
                            }

                        }
                    }
                    OnShowClientAction?.Invoke(MQMessageLogLevel.Info, $"{DateTime.Now} 重连次数完成，错误数：{error}，重连成功数：{reconnect}", null);

                }
                catch (Exception ex)
                {
                    OnShowClientAction?.Invoke(MQMessageLogLevel.Error, $"{nameSpaceStr}RabbitMQConnPool 连接池抛出异常:{ex.ToString()}", null);
                }


            }
        }
        /// <summary>
        /// 动态添加信道 add pool
        /// </summary>
        /// <param name="sender"></param>
        private void DynamicAddMQChannelPool()
        {
            while (true)
            {
                //Thread.Sleep(5 * 60 * 1000);
                Thread.Sleep(10 * 1000);
                try
                {
                    while (DynamicAddMQChannel.Count > 0)
                    {
                        Dictionary<IMQService, MQChannel> mQChannel = null;
                        DynamicAddMQChannel.TryDequeue(out mQChannel);
                        if (mQChannel != null)
                        {
                            foreach (KeyValuePair<IMQService, MQChannel> item in mQChannel)
                            {
                                var channel = item.Key.CreateChannel(item.Value.Queue, item.Value.RoutingKey, item.Value.ExchangeType);
                                item.Key.List_Channels.Add(channel);
                                //这里记录日志 添加新车场成功
                                this.AddServices(item.Key);
                                AddClientBusAction?.Invoke(MQAddExcahngeType.topic, $"{DateTime.Now}:[动态添加新增车场信息] |Queue={item.Value.Queue} | RoutingKey={item.Value.RoutingKey} |ExchangeName={item.Value.ExchangeName}");
                            }

                            //根据区号添加新停车场  长度14位
                             //使用topic 模式 创建  routingkey  使用4位的[区域编码]


                        }                        
                    }
                }
                catch (Exception ex)
                {
                    OnShowClientAction?.Invoke(MQMessageLogLevel.Error, $"{nameSpaceStr}DynamicAddMQChannelPool() 连接池抛出异常:{ex.ToString()}", null);
                }
            }
        }


        /// <summary>
        /// 发送消息
        /// </summary>
        private void RabbitMQSendMsgPool()
        {
            while (true)
            {
                Thread.Sleep(500);
                try
                {
                    while (MQOffLineCameraMsg.MQSendCameraMsgQueue.Count > 0)
                    {
                        string strMsg = "";
                        MQOffLineCameraMsg.MQSendCameraMsgQueue.TryDequeue(out strMsg);
                        foreach (var item in this.MQServices)
                        {
                            for (int i = 0; i < item.List_Channels.Count; i++)
                            {
                                var c = item.List_Channels[i];
                                if (c.Connection == null || !c.Connection.IsOpen)
                                {
                                    continue;
                                }
                                else
                                {
                                    //--这些发送消息是用来测试的
                                    //c.PulishMySelf(i + string.Format(@"hello:{0} c.PulishMySelf {1} queueName={2} ", DateTime.Now, Guid.NewGuid().ToString(), c.QueueName + "---" + c.ExchangeName + "---" + c.ExchangeTypeName));
                                    c.PulishMySelf(strMsg);
                                    OnShowClientAction?.Invoke(MQMessageLogLevel.Info, DateTime.Now + "发送消息" + strMsg, null);
                                    continue;
                                }
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    //日志 
                    OnShowClientAction?.Invoke(MQMessageLogLevel.Error, $"函数{nameSpaceStr}.RabbitMQSendMsgPool() 发送消息发生异常", ex);
                }
            }
        }
        /// <summary>
        /// 管理 开启订阅
        /// </summary>
        public void Start()
        {
            foreach (var item in this.MQServices)
            {
                try
                {
                    item.Start();
                }
                catch (Exception ex)
                {
                    OnShowClientAction?.Invoke(MQMessageLogLevel.Error, $"函数{nameSpaceStr}.Start() 开启订阅发生异常", ex);
                }
            }

        }

        /// <summary>
        /// 管理 停止订阅
        /// </summary>
        public void Stop()
        {
            try
            {
                foreach (var item in this.MQServices)
                {
                    item.Stop();

                }
                lock (MQServices)
                {
                    MQServices.Clear();
                }
                Dispose();
            }
            catch (Exception ex)
            {
                //日志 
                OnShowClientAction?.Invoke(MQMessageLogLevel.Error, $"函数{nameSpaceStr}.Stop() 停止订阅发生异常", ex);
            }

        }

        public void Dispose()
        {
            RabbitMQConnPoolTh = null;
            DynamicAddMQChannelTh = null;
        }
    }
}
