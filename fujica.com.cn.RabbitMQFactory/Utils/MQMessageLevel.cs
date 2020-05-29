using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fujica.com.cn.RabbitMQFactory.Utils
{
    /// <summary>
    /// 消息日志等级
    /// </summary>
    public enum MQMessageLogLevel
    {
        Debug = 0,
        Info,
        Warn,
        Error,
        Fatal

    }

    /// <summary>
    ///记录新增 topic车场日志信息
    /// </summary>
    public enum MQAddExcahngeType
    {
        /// <summary>
        /// 
        /// </summary>
        topic,
        fanout,
        headers,
        direct
    }



}
