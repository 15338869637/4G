using Fujica.com.cn.Logger;
using System;
using System.ServiceProcess;
using System.Threading;

namespace Fujica.com.cn.ExitDataService
{
    public partial class ExitDataService : ServiceBase
    {
        ILogger m_ilogger;
        Thread taskThread;

        public ExitDataService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            m_ilogger = new Logger.Logger();
            try
            {
                MonitorExitData.Execute();
                m_ilogger.LogInfo(LoggerLogicEnum.Tools, "", "", "", "Fujica.com.cn.ExitDataService.ExitDataService.OnStart", "服务启动完毕");
            }
            catch (Exception ex)
            {
                m_ilogger.LogFatal(LoggerLogicEnum.Tools, "", "", "", "Fujica.com.cn.ExitDataService.ExitDataService.OnStart", "服务启动出现异常", ex.ToString());
            }
        }

        protected override void OnStop()
        {
            m_ilogger.LogInfo(LoggerLogicEnum.Tools, "", "", "", "Fujica.com.cn.ExitDataService.ExitDataService.OnStop", "服务停止完毕");
        }
    }
}
