using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fujica.com.cn.RabbitMQFactory.Helpers
{
    /// <summary>
    /// RabbitMQ 连接类
    /// </summary>
    public class MQConnection
    {
        private string vhost = string.Empty;
        private IConnection connection = null;
        private MQConfig config = null;
        /// <summary>
        /// MQ连接配置初始化 
        /// </summary>
        /// <param name="config"></param>
        /// <param name="vhost"></param>
        public MQConnection(MQConfig config,string vhost)
        {
            this.config = config;
            this.vhost = vhost;

        }
        /// <summary>
        /// MQ连接配置初始化 
        /// </summary>
        /// <param name="mQConfig"></param>
        public MQConnection(MQConfig mQConfig)
        {
            this.config = mQConfig;

        }


        /// <summary>
        /// 创建MQ连接 ConnectionFactory
        /// </summary>
        public IConnection Connection
        {
            get
            {
                if (connection == null)
                {
                    ConnectionFactory factory = new ConnectionFactory
                    {
                        UserName = config.UserName,
                        Password = config.Pwd,
                        HostName = config.Url,
                        VirtualHost = config.Vhost,
                        AutomaticRecoveryEnabled = true,
                      
                    };
                    connection = factory.CreateConnection();
                }
                return connection;
            }

        }
    }
}
