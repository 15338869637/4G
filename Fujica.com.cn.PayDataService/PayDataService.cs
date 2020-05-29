using Fujica.com.cn.Logger;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.PayDataService
{
    partial class PayDataService : ServiceBase
    {
        ILogger m_ilogger;

        public PayDataService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            m_ilogger = new Logger.Logger();
            try
            {
                MonitorPayData.Execute();
                m_ilogger.LogInfo(LoggerLogicEnum.Tools, "", "", "", "Fujica.com.cn.PayDataService.PayDataService.OnStart", "服务启动完毕");
            }
            catch (Exception ex)
            {
                m_ilogger.LogFatal(LoggerLogicEnum.Tools, "", "", "", "Fujica.com.cn.PayDataService.PayDataService.OnStart", "服务启动出现异常", ex.ToString());
            }
        }

        protected override void OnStop()
        {
            m_ilogger.LogInfo(LoggerLogicEnum.Tools, "", "", "", "Fujica.com.cn.PayDataService.PayDataService.OnStop", "服务停止完毕");
        }
    }
}
