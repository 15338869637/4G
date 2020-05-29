using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fujica.com.cn.RabbitMQFactory.Helpers;

namespace fujica.com.cn.RabbitMQFactory.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class MQServiceBase : IMQService
    {
        internal bool started = false;
        /// <summary>
        /// 接收消息
        /// </summary>
        /// <param name="mQMessageBody"></param>
        public abstract void OnReceived(MQMessageBody mQMessageBody);
        /// <summary>
        /// 信道list集合
        /// </summary>
        public List<MQChannel> List_Channels { get; set; } = new List<MQChannel>();
        /// <summary>
        /// MQ连接配置
        /// </summary>
        public MQConfig MQ_Config { get; set; }
        /// <summary>
        /// 消息队列中定义的虚拟机
        /// </summary>
        public abstract string VHost { get; set; }
        /// <summary>
        ///  消息队列中定义的交换机名称
        /// </summary>
        public abstract string Exchange { get; set; }
        /// <summary>
        /// 定义的队列集合列表
        /// </summary>
        public List<MQQueueInfo> ListMqQueue { get; } = new List<MQQueueInfo>();

        internal MQServiceBase(MQConfig mQConfig)
        {
            this.MQ_Config = mQConfig;
        }
        
             
        /// <summary>
        /// 创建信道
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="routingKey"></param>
        /// <param name="exchangeType"></param>
        /// <returns></returns>
        public MQChannel CreateChannel(string queue, string routingKey, string exchangeType)
        {
            MQConnection mQConnection = new MQConnection(this.MQ_Config);
            MQChannelManager mQChannelManager = new MQChannelManager(mQConnection);
            MQChannel mQChannel = mQChannelManager.CreateReceiveChannel(exchangeType, this.Exchange, queue, routingKey);
            return mQChannel;
        }
        /// <summary>
        /// 启动订阅
        /// </summary>
        public void Start()
        {
            if (started)
            {
                return;
            }
            MQConnection mQConnection = new MQConnection(this.MQ_Config);
            MQChannelManager mQChannelManager = new MQChannelManager(mQConnection);
            foreach (var item in this.ListMqQueue)
            {
                string selfExchange = "";

                if (string.IsNullOrWhiteSpace(item.ExchangeName))
                {
                    selfExchange = this.Exchange;
                }
                else
                {
                    selfExchange = item.ExchangeName;
                }


                MQChannel mQChannel = mQChannelManager.CreateReceiveChannel(item.ExchangeType, selfExchange, item.QueueName, item.RoutingKey);
                mQChannel.OnReceiveCallBack = item.OnReceived;
                this.List_Channels.Add(mQChannel);
            }
            started = true;
        }

        /// <summary>
        /// 停止订阅
        /// </summary>
        public void Stop()
        {
            foreach (var c in this.List_Channels)
            {
                c.CloseMQConnection();
            }
            this.List_Channels.Clear();
            started = false;
        }




    }
}
