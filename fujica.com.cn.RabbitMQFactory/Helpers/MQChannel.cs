using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Framing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//internal 只能在同一程序集访问

//广播是向所有绑定fanout 交换器的队列发送消息

//topic  *.区号.# 

namespace fujica.com.cn.RabbitMQFactory.Helpers
{
    /// <summary>
    /// 创建一个通道类，用于订阅、发布消息，同时提供一个关闭通道连接的方法 Stop
    /// </summary>
    public class MQChannel
    {
        /// <summary>
        /// 交换器类型名称  使用ExchangeType.
        /// </summary>
        public string ExchangeType { get; set; }
        /// <summary>
        /// 交换器名称 
        /// </summary>
        public string ExchangeName { get; set; }
        /// <summary>
        /// 队列名称
        /// </summary>
        public string Queue { get; set; }
        /// <summary>
        /// 路由键名称
        /// </summary>
        public string RoutingKey { get; set; }
        /// <summary>
        /// 连接
        /// </summary>
        public IConnection Connection { get; set; }
        /// <summary>
        /// 消费者
        /// </summary>
        public EventingBasicConsumer Consumer { get; set; }
        /// <summary>
        /// 外部订阅消费者通知委托
        /// </summary>
        public Action<MQMessageBody> OnReceiveCallBack { get; set; }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="exchangeType">交换器类型 </param>
        /// <param name="exchange">交换器名称</param>
        /// <param name="queue">队列名称</param>
        /// <param name="routingKey">路由键</param>
        public MQChannel(string exchangeType, string exchange, string queue, string routingKey)
        {
            this.ExchangeType = exchangeType;
            this.ExchangeName = exchange;
            this.Queue = queue;
            this.RoutingKey = routingKey;

        }

        /// <summary>
        /// 向当前队列发送消息[当前连接的队列发送消息]
        /// </summary>
        /// <param name="content">发送消息内容</param>
        public void PulishMySelf(string content)
        {
            byte[] body = Encoding.UTF8.GetBytes(content);
            IBasicProperties basicProperties = new BasicProperties();
            basicProperties.DeliveryMode = 2;
            basicProperties.ContentType = "text/plain";
            basicProperties.Expiration = "3600000";//60分zong
            //Consumer.Model.BasicPublish(ExchangeName, RoutingKey, false, basicProperties, body);
            //Consumer.Model.BasicPublish(ExchangeName, "FUJICA.0755.inpark.camera", false, basicProperties, body); //test 
            //Consumer.Model.BasicPublish(ExchangeName, "FUJICA.0200.inpark.camera", false, basicProperties, body); //test 
            //Consumer.Model.BasicPublish(ExchangeName, "FUJICA.0210.inpark.camera", false, basicProperties, body); //test 
            //Consumer.Model.BasicPublish(ExchangeName, "FUJICA.0290.inpark.camera", false, basicProperties, body); //test 
        }

        /// <summary>
        /// 发送数据到RabbitMQ
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">发送实体</param>
        /// <param name="parkingCode">停车场编码</param>
        /// <param name="exchangeName">交换器名称</param>
        /// <param name="redisTTL">回写Redis过期分钟数</param>
        /// <param name="MQTTL">MQ队列过期分钟数</param>
        /// <param name="exchangeType">交换器类型   选择一个类型"direct" "fanout" "headers"  "topic"</param>
        /// <returns></returns>
        public bool Pulish<T>(T entity, string msgId, string parkingCode, string exchangeName, int redisTTL, int MQTTL, string exchangeType = "1:direct  2:fanout  3:headers  4:topic")//where T : MQPackongVehicleEntry
        {

            MQPackongVehicleEntry mQPackongVehicleEntry = new MQPackongVehicleEntry()
            {
                json = Newtonsoft.Json.JsonConvert.SerializeObject(entity),
                rabbitMQType = RabbitMQProduserMsgType.YOUBOKESendData,
                expireMinutes = MQTTL,
                idMsg = msgId,
                routingKey = parkingCode,
                exchange = exchangeName,
                parkingCode = parkingCode
            };
            string content = Newtonsoft.Json.JsonConvert.SerializeObject(mQPackongVehicleEntry);
            byte[] body = Encoding.UTF8.GetBytes(content);
            IBasicProperties basicProperties = new BasicProperties();
            basicProperties.DeliveryMode = 2;
            basicProperties.ContentType = "text/plain";
            basicProperties.Expiration = "3600000";
            Consumer.Model.BasicPublish(exchangeName, parkingCode, false, basicProperties, body);

            // 要把 消费者分为这4中情况 direct  fanout headers topic  信道要有这4个工厂来生产数据
            return true;


        }
        /// <summary>
        /// 向线下相机广播消息 入场，出场， 缴费
        /// 返回"200"成功，其他失败
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">发送消息实体</param>
        /// <param name="carmeraRoutingKey">线下相机自定义路由键</param>
        /// <param name="exchange">交换器名称</param>
        /// <param name="exchangeType">交换器类型</param>
        /// <param name="msgTTL"></param>
        /// <returns></returns>
        //public string PublishFanout<T>(T entity,string carmeraRoutingKey,string exchange,string exchangeType,int msgTTL)
        //{
        //    string retStr = string.Empty;
        //    try
        //    {
        //        string content = Newtonsoft.Json.JsonConvert.SerializeObject(entity);
        //        byte[] body = Encoding.UTF8.GetBytes(content);
        //        IBasicProperties basicProperties = new BasicProperties();
        //        basicProperties.DeliveryMode = 1;
        //        Consumer.Model.BasicPublish(MQYunCameraExchange.FanoutCameraInAndOutData, carmeraRoutingKey, false, basicProperties, body);

        //    }
        //    catch (Exception ex)
        //    {
        //        retStr = ex.ToString();
        //    }
        //    return retStr;

        //}


        /// <summary>
        /// 回调接收消息 消费
        /// </summary>
        internal void Receive(object sender, BasicDeliverEventArgs e)
        {
            MQMessageBody msgbody = new MQMessageBody();
            string exchangeStr = "";
            string content = "";
            try
            {
#if DEBUG
                // 接收到消息的其他属性
                string consumerTag = e.ConsumerTag;// The consumer tag of the consumer that the message was delivered to. 消息传递到的使用者的使用者标记。
                ulong deliveryTag = e.DeliveryTag; //The delivery tag for this delivery. See IModel.BasicAck. 此传递的传递标记。看到IModel.BasicAck。
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"consumerTag={consumerTag},deliveryTag={deliveryTag}");
#endif
                content = Encoding.UTF8.GetString(e.Body);
                msgbody.Content = content;
                msgbody.Consumer = (EventingBasicConsumer)sender;
                msgbody.BasicDeliver = e;
                msgbody.RoutingKey = e.RoutingKey;
                exchangeStr = e.Exchange;

            }
            catch (Exception ex)
            {
                msgbody.ErrorMessage = $"Receive Msg 发生错误{ex.Message}";
                msgbody.Exception = ex;
                msgbody.Error = true;
                msgbody.Code = 500;

            }
            Console.WriteLine($"11消费者接收到消息{msgbody.Content}");

            switch (exchangeStr)
            {
                case MQYunCameraExchange.DirectRecOnLinePayData:
                    MQOnLineMsg.MQOnLineMsgQueue.Enqueue(msgbody);
                    break;
                case MQYunCameraExchange.DirectDynamicAddNewCityCode:
                    MQOnLineMsg.MQSaveCityCodeMsgQueue.Enqueue(content);
                    break;
                default:
                    MQOffLineCameraMsg.MQOffLineCameraMsgQueue.Enqueue(msgbody);
                    break;
            }
            //不为null时执行
            //OnReceiveCallBack?.Invoke(body);

        }

        /// <summary>
        /// 设置消息处理完成ACk
        /// </summary>
        /// <param name="consumer"></param>
        /// <param name="deliveryTag"></param>
        /// <param name="multiple"></param>
        public void SetBasicAck(EventingBasicConsumer consumer, ulong deliveryTag, bool multiple)
        {
            consumer.Model.BasicAck(deliveryTag, multiple);
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        public void CloseMQConnection()
        {
            ///this.Connection.IsOpen 测试发现这个判断斌不是即时的
            if (this.Connection != null && this.Connection.IsOpen)
            {
                this.Connection.Close();
                this.Connection.Dispose();

            }
        }



    }
}
