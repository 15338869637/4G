using StackExchange.Redis;
using System;
using System.IO;
/***************************************************************************************
 * *
 * *        File Name        : RedisHelper.cs
 * *        Creator          : llp
 * *        Create Time      : 2019-10-17 
 * *        Remark           : 基础类，读取Redis配置文件封装
 * *
 * *  Copyright (c) 深圳市富士智能系统有限公司.  All rights reserved. 
 * ***************************************************************************************/
namespace Fujica.com.cn.BaseConnect
{
    /// <summary>
    ///redis帮助类
    /// </summary>
    /// <remarks> 
    /// 2019.10.17: 修改: 接口命名修改. llp <br/>  
    /// </remarks> 
    public class RedisHelper
    {
        private static Lazy<ConnectionMultiplexer> followLazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            RedisConnectionConfigInfo configinfo = RedisConfigs.GetConfig();
            return ConnectionMultiplexer.Connect(configinfo.FollowRedisServerIp + ":" + configinfo.FollowRedisServerPort + ",abortConnect=false,ssl=false,password=" + configinfo.FollowRedisServerPassword);
        });

        public static ConnectionMultiplexer Manager
        {
            get
            {
                return followLazyConnection.Value;
            }
        }
        public static IDatabase GetDatabase(int dbIndex)
        {
            return followLazyConnection.Value.GetDatabase(dbIndex);
        }

        public static IServer GetCurrentServer()
        {
            ConnectionMultiplexer _manager = Manager;
            return _manager.GetServer(_manager.GetEndPoints()[0]);
        }
    }


    /// <summary>
    /// 获取配置文件
    /// </summary>
    public class RedisConfigs
    {
        private static System.Timers.Timer redisConfigTimer = new System.Timers.Timer(600000);//间隔为10分钟

        private static RedisConnectionConfigInfo m_configinfo = null;

        private static object m_lockHelper = new object();

        private static DateTime m_fileoldchange = default(DateTime);

        static RedisConfigs()
        {
            LoadingConfig();
            redisConfigTimer.AutoReset = true;
            redisConfigTimer.Enabled = true;
            redisConfigTimer.Elapsed += new System.Timers.ElapsedEventHandler(Timer_Elapsed);
            redisConfigTimer.Start();
        }

        /// <summary>
        /// 定时重新读取配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            LoadingConfig();
        }

        /// <summary>
        /// 载入配置
        /// </summary>
        /// <returns></returns>
        private static void LoadingConfig()
        {
            //redis配置文件路径 C:/..../Config/redis.config
            string filename = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config", "redis.config");
            if (File.Exists(filename))
            {
                lock (m_lockHelper)
                {
                    DateTime m_filenewchange = File.GetLastWriteTime(filename);

                    //当程序运行中config文件发生变化时则对config重新赋值
                    if (m_fileoldchange != m_filenewchange)
                    {
                        m_fileoldchange = m_filenewchange;
                        m_configinfo = (RedisConnectionConfigInfo)XmlSerialization.Load(typeof(RedisConnectionConfigInfo), filename);
                    }
                }
            }
        }

        /// <summary>
        /// 获取配置类实例
        /// </summary>
        /// <returns></returns>
        public static RedisConnectionConfigInfo GetConfig()
        {
            return m_configinfo;
        }
    }

    public class RedisConnectionConfigInfo
    {
        /// <summary>
        /// 主服务器ip
        /// </summary>
        public string MasterRedisServerIp
        {
            get;
            set;
        }
        /// <summary>
        /// 主服务器端口
        /// </summary>
        public int MasterRedisServerPort
        {
            get;
            set;
        }
        /// <summary>
        /// 主服务器密码
        /// </summary>
        public string MasterRedisServerPassword
        {
            get;
            set;
        }
        /// <summary>
        /// 从服务器ip
        /// </summary>
        public string FollowRedisServerIp
        {
            get;
            set;
        }
        /// <summary>
        /// 从服务器端口
        /// </summary>
        public int FollowRedisServerPort
        {
            get;
            set;
        }
        /// <summary>
        /// 从服务器密码
        /// </summary>
        public string FollowRedisServerPassword
        {
            get;
            set;
        }
    }
}
