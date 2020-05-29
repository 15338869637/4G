using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fujica.com.cn.RabbitMQFactory.Helpers
{
    /// <summary>
    /// 创建一个 RabbitMQ 通道管理类，用于创建通道，
    /// 只有一个公共方法 CreateReceiveChannel，
    /// 传入相关参数，创建一个 MQChannel 对象
    /// </summary>
    public class MQChannelManager
    {
        public MQConnection GetMQConnection { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        public MQChannelManager(MQConnection connection)
        {
            this.GetMQConnection = connection;
        }

        /// <summary>
        /// 创建消息通道 channel
        /// </summary>
        /// <param name="exchangeType">交换器类型</param>
        /// <param name="exchangeName">交换器名称</param>
        /// <param name="queue">队列名称</param>
        /// <param name="routekey">路由键</param>
        /// <returns></returns>
        public MQChannel CreateReceiveChannel(string exchangeType, string exchangeName, string queue, string routekey)
        {
            MQChannel channel = null;
            
            switch (exchangeType)
            {
                case ExchangeType.Headers:
                    break;
                case ExchangeType.Fanout:
                    break;
                case ExchangeType.Topic:
                    if (!routekey.Contains("*."))
                    {
                        routekey = string.Format(@"*.{0}.#", routekey);
                    }
                    break;
                case ExchangeType.Direct:
                    break;
                default:
                    break;
            }

            IModel model = this.CreateModel(exchangeType, exchangeName, queue, routekey);
            //model.BasicQos(0, 10, false);//一次最多取一条消息
            EventingBasicConsumer consumer = this.CreateConsumer(model);
            channel = new MQChannel(exchangeType, exchangeName, queue, routekey)
            {
                Connection = this.GetMQConnection.Connection,
                Consumer = consumer
            };
            consumer.Received += channel.Receive;

            model.BasicConsume(queue: queue, autoAck: false, consumer: consumer);

            return channel;
        }


        /// <summary>
        ///  创建一个通道，包含交换机/队列/路由，并建立绑定关系
        /// </summary>
        /// <param name="exchangeType">交换机类型</param>
        /// <param name="exchangeName">交换机名称</param>
        /// <param name="queue">队列名称</param>
        /// <param name="routeKey">路由名称</param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        private IModel CreateModel(string exchangeType, string exchangeName, string queue, string routeKey, IDictionary<string, object> arguments = null)
        {            
            IModel model = this.GetMQConnection.Connection.CreateModel();
            model.ExchangeDeclare(exchange: exchangeName, type: exchangeType, durable: true, autoDelete: false, arguments: arguments);
            model.QueueDeclare(queue:queue, durable:true, exclusive:false, autoDelete:false, arguments:arguments);
            model.QueueBind(queue:queue, exchange:exchangeName, routingKey:routeKey);
            return model;
        }

        /// <summary>
        ///  接收消息到队列中
        /// </summary>
        /// <param name="model">消息通道</param>
        /// <param name="queue">队列名称</param>
        /// <param name="callback">订阅消息的回调事件</param>
        /// <returns></returns>
        private EventingBasicConsumer CreateConsumer(IModel model)
        {
            EventingBasicConsumer consumer = new EventingBasicConsumer(model);            
            return consumer;
        }







    }
}
