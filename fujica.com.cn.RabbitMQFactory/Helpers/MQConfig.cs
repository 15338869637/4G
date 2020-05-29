using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fujica.com.cn.RabbitMQFactory.Helpers
{
    /// <summary>
    /// RabbitMQ 连接配置信息
    /// fujica_yuncamera_bus 下发个云车场项目使用
    /// </summary>
    public class MQConfig
    {
        /// <summary>
        /// Virtual host  default =  "fujica_yuncamera_bus"    云停车项目使用
        /// </summary>
        private string vhost = "fujica_yuncamera_bus";
        /// <summary>
        /// RabbitMQ monitoring  连接地址     Virtual host  is  fujica_yuncamera_bus 
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// Virtual host  default =  "fujica_yuncamera_bus"
        /// </summary>
        public string Vhost { get { return vhost; } }
        /// <summary>
        /// RabbitMQ monitoring  用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// RabbitMQ monitoring  密码
        /// </summary>
        public string Pwd { get; set; }

    }
}
