using fujica.com.cn.RabbitMQFactory.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fujica.com.cn.RabbitMQFactory.Utils
{
    /// <summary>
    /// 定义对象接口
    /// </summary>
    public interface IMQService
    {
        /// <summary>
        ///  创建通道
        /// </summary>
        /// <param name="queue">队列名称</param>
        /// <param name="routeKey">路由名称</param>
        /// <param name="exchangeType">交换机类型</param>
        /// <param name="exchangeName">交换器名称</param>
        /// <param name="vhost">新建云车场Vhost</param>
        /// <returns></returns>
        MQChannel CreateChannel(string queue, string routeKey, string exchangeType);
        //MQChannel CreateChannel(string queue, string routeKey, string exchangeType,string exchangeName,string vhost);
        /// <summary>
        ///  开启订阅
        /// </summary>
        void Start();
        /// <summary>
        ///  停止订阅
        /// </summary>
        void Stop();
        /// <summary>
        ///  通道列表
        /// </summary>
        List<MQChannel> List_Channels { get; set; }
        /// <summary>
        ///  消息队列中定义的虚拟机   Virtual host  fujica_yuncamera_bus
        /// </summary>
        string VHost { get; set; }
        /// <summary>
        ///  消息队列中定义的交换机名称
        /// </summary>
        string Exchange { get; set; }
    }
}
