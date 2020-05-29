using System;
using System.IO;
/***************************************************************************************
 * *
 * *        File Name        : FujicaMvcApiHelper.cs
 * *        Creator          : llp
 * *        Create Time      : 2019-10-17 
 * *        Remark           : 基础类，读取富士主平台的标准接口配置文件封装
 * *
 * *  Copyright (c) 深圳市富士智能系统有限公司.  All rights reserved. 
 * ***************************************************************************************/
namespace Fujica.com.cn.BaseConnect
{
    /// <summary>
    /// 请求主平台接口帮助类
    /// </summary>
    /// <remarks> 
    /// 2019.10.17: 修改: 接口命名修改. llp <br/>  
    /// </remarks> 
    public class StandardApiHelper
    {
        public static StandardApiConnectionConfigInfo GetConfig()
        {
           return StandardApiConfigs.GetConfig();
        }
    }

    public class StandardApiConfigs
    {
        private static System.Timers.Timer redisConfigTimer = new System.Timers.Timer(600000);//间隔为10分钟

        private static StandardApiConnectionConfigInfo m_configinfo = null;

        private static object m_lockHelper = new object();

        private static DateTime m_fileoldchange = default(DateTime);

        static StandardApiConfigs()
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
            string filename = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config", "fujicaApi.config");
            if (File.Exists(filename))
            {
                lock (m_lockHelper)
                {
                    DateTime m_filenewchange = File.GetLastWriteTime(filename);

                    //当程序运行中config文件发生变化时则对config重新赋值
                    if (m_fileoldchange != m_filenewchange)
                    {
                        m_fileoldchange = m_filenewchange;
                        m_configinfo = (StandardApiConnectionConfigInfo)XmlSerialization.Load(typeof(StandardApiConnectionConfigInfo), filename);
                    }
                }
            }
        }

        /// <summary>
        /// 获取配置类实例
        /// </summary>
        /// <returns></returns>
        public static StandardApiConnectionConfigInfo GetConfig()
        {
            return m_configinfo;
        }
    }

    public class StandardApiConnectionConfigInfo
    {
        /// <summary>
        /// 富士接口通用appid
        /// </summary>
        public string Fujica_appid
        {
            get;
            set;
        }
        /// <summary>
        /// 富士接口通用secret
        /// </summary>
        public string Fujica_secret
        {
            get;
            set;
        }
        /// <summary>
        /// 富士接口通用privatekey
        /// </summary>
        public string Fujica_privatekey
        {
            get;
            set;
        }
        /// <summary>
        /// 接口请求地址
        /// </summary>
        public string Fujica_url
        {
            get;
            set;
        }
    }
}
