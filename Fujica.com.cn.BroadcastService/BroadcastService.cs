using Fujica.com.cn.BroadcastService.JobsDetails;
using Fujica.com.cn.Communication.RabbitMQ;
using Fujica.com.cn.Logger;
using Fujica.com.cn.Tools;
using Quartz;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.BroadcastService
{
    public partial class BroadcastService : ServiceBase
    {
        IScheduler scheduler;
        ILogger m_ilogger;

        public BroadcastService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                m_ilogger = new Logger.Logger();
                scheduler = JobHelper.SchedulerCreate();
                scheduler.ScheduleJob(JobHelper.JobCreate<BroadcastEntryData>(), JobHelper.TriggerCreate("*/5 * * * * ?")); //五秒执行一次进场数据广播任务
                scheduler.Start();

                m_ilogger.LogInfo(LoggerLogicEnum.Tools,"","","", "Fujica.com.cn.BroadcastService.BroadcastService.OnStart", "服务启动完毕");
            }
            catch (Exception ex)
            {
                m_ilogger.LogFatal(LoggerLogicEnum.Tools, "", "", "", "Fujica.com.cn.BroadcastService.BroadcastService.OnStart", "服务启动出现异常", ex.ToString());
            }
        }

        protected override void OnStop()
        {
            try
            {
                scheduler.Shutdown();
                m_ilogger.LogInfo(LoggerLogicEnum.Tools, "", "", "", "Fujica.com.cn.BroadcastService.BroadcastService.OnStop", "服务停止完毕");

            }
            catch (Exception ex)
            {
                m_ilogger.LogFatal(LoggerLogicEnum.Tools, "", "", "", "Fujica.com.cn.BroadcastService.BroadcastService.OnStart", "服务停止出现异常", ex.ToString());
            }
        }
    }
}
