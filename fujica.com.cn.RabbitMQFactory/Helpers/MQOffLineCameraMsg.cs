using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fujica.com.cn.RabbitMQFactory.Helpers
{
    /// <summary>
    /// 接收线下相机推送的数据
    /// </summary>
    public sealed class MQOffLineCameraMsg
    {
        /// <summary>
        /// 保存线下上推的数据到中心服务  相机==》中心服务
        /// </summary>
        public static ConcurrentQueue<MQMessageBody> MQOffLineCameraMsgQueue  = new ConcurrentQueue<MQMessageBody>();

        /// <summary>
        /// 中心服务发送消息到每个相机  中心服务==》相机
        /// </summary>
        public static ConcurrentQueue<string> MQSendCameraMsgQueue = new ConcurrentQueue<string>();


    }
}
