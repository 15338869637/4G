using Fujica.com.cn.Logger;
using Fujica.com.cn.Tools;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Fujica.com.cn.Communication.RabbitMQ
{
    public class RabbitMQPeeker
    {
        //https://www.cnblogs.com/piaolingzxh/p/5448927.html

        ILogger m_logger;
        ISerializer m_serializer;

        public RabbitMQPeeker(ILogger logger, ISerializer serializer)
        {
            m_logger = logger;
            m_serializer = serializer;
        }

        /// <summary>
        /// 通过RabbitMQ获取外界消息
        /// </summary>
        public void PeekMessageByRabbitMQ(string queue, string type, Func<string, RabbitMQAction> CallBackFunction)
        {
            Again:
            RabbitMQEntity model = new RabbitMQEntity()
            {
                json = "",
                expireMinutes = 0,
                routingKey = type,
                exchange = "amq.direct",
                queue = queue
            };
            try
            {
                string hostname = "localhost"; //hsot
                string username = "byl";//用户名
                string password = "bylizp";//密码
                int port = AmqpTcpEndpoint.UseDefaultPort;//默认端口5672
                string virtualhost = "/";//默认值
                IProtocol protocol = Protocols.DefaultProtocol;//默认协议

                //初始化交换机
                ConnectionFactory conn_factory = new ConnectionFactory()
                {
                    HostName = hostname,
                    VirtualHost = virtualhost,
                    UserName = username,
                    Password = password,
                    Port = port,
                    Protocol = protocol
                };//定义一个连接工厂
                IConnection conn = conn_factory.CreateConnection(); //创建连接
                IModel channel = conn.CreateModel(); //创建信道
                channel.ExchangeDeclare(model.exchange, ExchangeType.Direct, true, false, null); //声明交换机
                channel.QueueDeclare(model.queue,true, false, false, null);//声明消息队列 持久化队列，非私有
                channel.QueueBind(model.queue, model.exchange, model.routingKey); //绑定消息队列到指定的交换机与路由上
                channel.BasicQos(0, 1, false);//公平分发 防止某个消费者任务过重

                //订阅消息
                //IBasicConsumer
                QueueingBasicConsumer consumer = new QueueingBasicConsumer(channel); //声明一个消费者
                string consumer_tag = channel.BasicConsume(model.queue, false, consumer); //消费者标志
                while (true)
                {
                    //循环读数据
                    BasicDeliverEventArgs evt_args = consumer.Queue.Dequeue();// 阻塞式取出一条消息
                    
                    string msg_body = Encoding.UTF8.GetString(evt_args.Body);//消息体
                    RabbitMQAction result = CallBackFunction(msg_body);
                    if (result == RabbitMQAction.ACCEPT)
                    {
                        channel.BasicAck(evt_args.DeliveryTag, false);//进行应答，但不批量应答
                    } else if (result == RabbitMQAction.REJECT)
                    {
                        channel.BasicNack(evt_args.DeliveryTag, false, false);//进行拒绝，但不批量拒绝，消息不回队列
                    }
                    else if (result == RabbitMQAction.RETRY)
                    {
                        channel.BasicNack(evt_args.DeliveryTag, false, true);//进行拒绝，但不批量拒绝，消息重回队列
                    }
                    Thread.Sleep(10);
                }
            }
            catch (Exception ex)
            {
                if (ex.ToString().Contains("Already closed") || ex.ToString().Contains("远程主机强迫关闭了一个现有的连接"))
                {
                    goto Again;//跳回去重新连接并发送，以保证可靠
                }
            }
        }
    }
}
