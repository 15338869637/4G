using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fujica.com.cn.RabbitMQFactory.Helpers
{

    /// <summary>
    /// 消息体对象 MessageBody，用于解析和传递消息到业务系统中，
    /// 在接下来的 MQChannel 类中会用到 2019年2月21日
    /// </summary>
    public class MQMessageBody
    {

        private int code = 200;
        private bool error = false;

        /// <summary>
        /// 消费者触发事件
        /// </summary>
        public RabbitMQ.Client.Events.EventingBasicConsumer Consumer { get; set; }
        /// <summary>
        /// AMQP 传递消息参数
        /// </summary>
        public RabbitMQ.Client.Events.BasicDeliverEventArgs BasicDeliver { get; set; }
        /// <summary>
        /// 路由键 eg: FUJICA.0755.InPark.Camera
        /// </summary>
        public string RoutingKey { get; set; }
        /// <summary>
        /// 200成功
        /// </summary>
        public int Code { get { return code; } set { code = value; } }
        /// <summary>
        /// 消息
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMessage { get; set; }
        /// <summary>
        /// false
        /// </summary>
        public bool Error { get { return error; }  set { error = value; } }
        /// <summary>
        /// 异常
        /// </summary>
        public Exception Exception { get; set; }
    }
}
