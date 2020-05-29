using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fujica.com.cn.RabbitMQFactory.Utils
{
    /// <summary>
    /// Virtual host 枚举
    /// </summary>
    public enum MQVHostNameType
    {

        ////RabbitMQ Vhost 设置

        // DefaultVHost = "/",

        // Virtual Host=       /                // 线上缴费下发，其他下发业务 到线下， 线下数据上传推送
        // Virtual Host= fujica_internal_bus    // 富士云平台内部数据交互






    }
}
