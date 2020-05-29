using fujica.com.cn.RabbitMQFactory.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fujica.com.cn.RabbitMQFactory.Utils
{
    /// <summary>
    /// 队列信息
    /// </summary>
    public partial class MQQueueInfo
    {
        /// <summary>
        /// 队列名称
        /// </summary>
        public string QueueName { get; set; }
        /// <summary>
        /// 路由键 富士云使用停车场编码parkingcode
        /// </summary>
        public string RoutingKey { get; set; }
        /// <summary>
        /// 交换机类型 
        /// </summary>
        public string ExchangeType { get; set; }
        /// <summary>
        /// 交换机名称 根据业务需求设置
        /// </summary>
        public string ExchangeName { get; set; }
        /// <summary>
        /// 接受MQ消息委托 
        /// </summary>
        public Action<MQMessageBody> OnReceived { get; set; }
        /// <summary>
        /// 输出信息到客户端
        /// </summary>
        public Action<MQChannel, string> ShowClientAction { get; set; }



    }
}
