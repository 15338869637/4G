using Fujica.com.cn.BaseConnect;
using Fujica.com.cn.Business.Model;
using Fujica.com.cn.Communication.RabbitMQ;
using Fujica.com.cn.Context.Model;
using Fujica.com.cn.EntryDataService;
using Fujica.com.cn.Interface.DataUpload.Models.InPut;
using Fujica.com.cn.Logger;
using Fujica.com.cn.Tools;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Fujica.com.cn.EntryDataService
{
    public partial class EntryDataService : ServiceBase
    {
        ILogger m_ilogger;
        Task mainTask;

        public EntryDataService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            m_ilogger = new Logger.Logger();
            try
            {
                MonitorEntryData.Execute();
                m_ilogger.LogInfo(LoggerLogicEnum.Tools, "", "", "", "Fujica.com.cn.EntryDataService.EntryDataService.OnStart", "服务启动完毕");
            }
            catch (Exception ex)
            {
                m_ilogger.LogFatal(LoggerLogicEnum.Tools, "", "", "", "Fujica.com.cn.EntryDataService.EntryDataService.OnStart", "服务启动出现异常", ex.ToString());
            }
        }

        protected override void OnStop()
        {
            m_ilogger.LogInfo(LoggerLogicEnum.Tools, "", "", "", "Fujica.com.cn.EntryDataService.EntryDataService.OnStop", "服务停止完毕");
        }

        /// <summary>
        /// 散列车辆在场数据到不同db(10-15)
        /// </summary>
        /// <returns></returns>
        private int GetDatabaseNumber(string carNo)
        {
            byte[] byts = Encoding.UTF8.GetBytes(carNo);
            int length = byts.Length;
            int sum = 0;
            for (int i = 0; i < length; i++)
            {
                sum += byts[i];
            }
            return (sum % 6) + 10; //6代表分配到6个db里,10代表从第10个db开始
        }
    }
}
