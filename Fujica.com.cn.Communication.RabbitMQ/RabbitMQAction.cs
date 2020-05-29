using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.Communication.RabbitMQ
{
    public enum RabbitMQAction
    {
        /// <summary>
        /// 处理成功
        /// </summary>
        ACCEPT,  // 处理成功
        /// <summary>
        /// 可以重试的错误
        /// </summary>
        RETRY,   // 可以重试的错误
        /// <summary>
        /// 无需重试的错误
        /// </summary>
        REJECT,  // 无需重试的错误
    }
}
