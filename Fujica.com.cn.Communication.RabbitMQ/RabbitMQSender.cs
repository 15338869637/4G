using Fujica.com.cn.Logger;
using Fujica.com.cn.Tools;
using FuJiCaWcfModel;
using FuJiWcfCaRabbitMQClient;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Fujica.com.cn.Communication.RabbitMQ
{
    /// <summary>
    /// MQ消息发送器
    /// </summary>
    public class RabbitMQSender
    {
        //https://www.cnblogs.com/piaolingzxh/p/5448927.html

        ILogger m_logger;
        ISerializer m_serializer;

        public RabbitMQSender(ILogger logger, ISerializer serializer)
        {
            m_logger = logger;
            m_serializer = serializer;
        }

        /// <summary>
        /// 通过RabbitMQ向外发送消息
        /// </summary>
        /// <param name="desc">业务描述</param>
        /// <param name="entity">数据实体</param>
        /// <param name="messageId">消息id(guid)</param>
        /// <param name="parkingCode">停车场编码</param>
        public bool SendMessageForRabbitMQ(string desc,string entity, string messageId, string parkingCode,string routingKey="", string exchange = "")
        {
            bool flag = false;
            try
            {
                if (string.IsNullOrEmpty(routingKey))
                    routingKey = parkingCode;

                if (string.IsNullOrEmpty(exchange))
                {
                    exchange = parkingCode + "_CameraExchange";
                }

                //if (string.IsNullOrWhiteSpace(parkingCode))
                //{
                //    m_logger.LogInfo(LoggerLogicEnum.Communication, "", parkingCode, "", "Fujica.com.cn.Communication.RabbitMQ.RabbitMQSender.SendMessageForRabbitMQ", "RabbitMQ对象缺少通道或路由键");
                //    return flag;
                //}

                RabbitMQProduserMsgType msgType;
                if (exchange == "FuJiCaYunCameraParkInit.fanout")
                {
                    msgType = RabbitMQProduserMsgType.YunParkDataInitCameraType_fanout;
                }
                else if (exchange == "FuJiCaDynamicAddNewCityCode.direct")
                {
                    exchange = ConfigurationManager.AppSettings["MQManufacturerCode"] + exchange;
                    msgType = RabbitMQProduserMsgType.Other01Type_direct;
                }
                else
                {
                    msgType = RabbitMQProduserMsgType.YunParkDataType_fanout;
                }

                //创建发送的消息体
                PackongVehicleEntryMQ model = new PackongVehicleEntryMQ()
                {
                    json = entity,
                    rabbitMQType = msgType,
                    idMsg = messageId,
                    routingKey = routingKey,
                    exchange = exchange,
                    parkingCode = parkingCode                    
                };
                string jsonMsg = m_serializer.Serialize(model);
                //m_logger.LogInfo(LoggerLogicEnum.Communication, "", parkingCode, "", "Fujica.com.cn.Communication.RabbitMQ.RabbitMQSender.SendMessageForRabbitMQ", "序列化结果" + jsonMsg);
                Startup.AsyncInitSendMQMsgOfYunParkData(jsonMsg);
                m_logger.LogInfo(LoggerLogicEnum.Communication, "", parkingCode, "", "Fujica.com.cn.Communication.RabbitMQ.RabbitMQSender.SendMessageForRabbitMQ", string.Format("MQ下发成功!下发业务：{0}；发送参数：{1}；", desc, jsonMsg));
                flag = true;
            }
            catch (Exception ex)
            {
                m_logger.LogError(LoggerLogicEnum.Communication, "", parkingCode, "", "Fujica.com.cn.Communication.RabbitMQ.RabbitMQSender.SendMessageForRabbitMQ", "RabbitMQ对象创建失败OR通过RabbitMQ发送消息失败", ex.ToString());
            }
            return flag;
        }

        //public bool SendMessageForRabbitMQ(string desc, string entity, string queue, string parkingCode)
        //{
        //    Again:
        //    RabbitMQEntity model = new RabbitMQEntity()
        //    {
        //        json = entity, 
        //        expireMinutes = 0,
        //        routingKey = queue,
        //        exchange = "amq.direct",
        //        queue = queue
        //    };
        //    try
        //    {
        //        string hostname = "localhost"; //hsot
        //        string username = "byl";//用户名
        //        string password = "bylizp";//密码
        //        int port = AmqpTcpEndpoint.UseDefaultPort;//默认端口5672
        //        string virtualhost = "/";//默认值
        //        IProtocol protocol = Protocols.DefaultProtocol;//默认协议

        //        //初始化交换机
        //        ConnectionFactory conn_factory = new ConnectionFactory()
        //        {
        //            HostName = hostname,
        //            VirtualHost = virtualhost,
        //            UserName = username,
        //            Password = password,
        //            Port = port,
        //            Protocol = protocol
        //        };//定义一个连接工厂
        //        IConnection conn = conn_factory.CreateConnection(); //创建连接
        //        IModel channel = conn.CreateModel(); //创建信道
        //        channel.ExchangeDeclare(model.exchange, ExchangeType.Direct, true, false, null); //声明交换机
        //        channel.QueueDeclare(model.queue, false, false, false, null);//声明并新建消息队列 非持久化，非私有
        //        channel.QueueBind(model.queue, model.exchange, model.routingKey); //绑定消息队列到指定的交换机（路由为其绑定关系）

        //        //派送消息
        //        IBasicProperties msg_props = channel.CreateBasicProperties();//创建消息原型
        //        msg_props.DeliveryMode = 1;//不用持久化
        //        msg_props.ContentType = "text/plain"; //内容类型
        //        //msg_props.Expiration = "3600000";//消息过期时间
        //        channel.BasicPublish(model.exchange, model.routingKey, msg_props, Encoding.UTF8.GetBytes(model.json));//发布一条消息
        //        channel.Close();
        //        conn.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        if (ex.ToString().Contains("Already closed") || ex.ToString().Contains("远程主机强迫关闭了一个现有的连接"))
        //        {
        //            goto Again;//跳回去重新连接并发送，以保证可靠
        //        }else
        //        {
        //            return false;
        //        }
        //    }
        //    return true;
        //}
    }
}
