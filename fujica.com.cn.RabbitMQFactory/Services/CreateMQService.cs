using fujica.com.cn.RabbitMQFactory.Helpers;
using fujica.com.cn.RabbitMQFactory.Utils;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fujica.com.cn.RabbitMQFactory.Services
{
    public class CreateMQService : MQServiceBase
    {

        private string vhost = "fujica_yuncamera_bus";

        private string exchange = "FuJiCaYunCameraParkPushData.topic";

        public Action<MQMessageLogLevel, string, Exception> OnAction = null;

        /// <summary>
        /// 传入联系参数
        /// </summary>
        /// <param name="mQConfig"></param>
        public CreateMQService(MQConfig mQConfig) : base(mQConfig)
        {
            //base.ListMqQueue.Add(new MQQueueInfo
            //{
            //    QueueName = "03Test19000100100203546",
            //    RoutingKey = "0319000100100203546",
            //    ExchangeType = ExchangeType.Direct,
            //    OnReceived = this.OnReceived

            //});

        }

        /// <summary>
        /// 添加队列信息
        /// </summary>
        /// <param name="mQConfig"></param>
        /// <param name="mQQueueInfo"></param>
        public CreateMQService(MQConfig mQConfig, MQQueueInfo mQQueueInfo) : base(mQConfig)
        {
            //mQQueueInfo.OnReceived = this.OnReceived;
            base.ListMqQueue.Add(mQQueueInfo);
        }

        public override string VHost { get { return vhost; } set { vhost = value; } }
        public override string Exchange { get { return exchange; } set { exchange = value; } }
        
        /// <summary>
        /// 回调  不使用
        /// </summary>
        /// <param name="mQMessageBody"></param>
        public override void OnReceived(MQMessageBody mQMessageBody)
        {
            try
            {                
               // Console.WriteLine($"22消费者接收到消息{mQMessageBody.Content}");
            }
            catch (Exception ex)
            {
                OnAction?.Invoke(MQMessageLogLevel.Error, ex.Message, ex);
            }
           // mQMessageBody.Consumer.Model.BasicAck(deliveryTag:mQMessageBody.BasicDeliver.DeliveryTag, multiple: true);
        }
    }
}
