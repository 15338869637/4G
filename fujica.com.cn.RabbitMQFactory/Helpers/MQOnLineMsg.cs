using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fujica.com.cn.RabbitMQFactory.Helpers
{
    /// <summary>
    /// 线上下发MQ 消息
    /// </summary>
    public sealed class MQOnLineMsg
    {
        /// <summary>
        /// 保存线上下发的数据到中心服务
        /// </summary>
        public static ConcurrentQueue<MQMessageBody> MQOnLineMsgQueue = new ConcurrentQueue<MQMessageBody>();
        /// <summary>
        /// 保存区号队列 添加新车场区号到中心服务
        /// </summary>
        public static ConcurrentQueue<string> MQSaveCityCodeMsgQueue = new ConcurrentQueue<string>();


    }
}
