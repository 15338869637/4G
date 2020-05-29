using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.Communication.RabbitMQ
{
    /// <summary>
    /// MQ数据实体的包装
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RabbitMQEntity
    {
        /// <summary>
        /// 过期分钟数
        /// </summary>
        public int expireMinutes { get; set; }

        /// <summary>
        /// 具体业务数据
        /// </summary>
        public string json { set; get; }

        /// <summary>
        /// 队列 设置号
        /// </summary>
        public string queue { get; set; }

        /// <summary>
        /// 路由 车场编码
        /// </summary>
        public string routingKey { get; set; }

        /// <summary>
        /// 信道
        /// </summary>
        public string exchange { get; set; }
    }
}
