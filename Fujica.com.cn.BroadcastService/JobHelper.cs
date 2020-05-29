using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.BroadcastService
{
    public class JobHelper
    {
        /// <summary>
        /// 创建一个作业调度池
        /// </summary>
        /// <returns></returns>
        public static IScheduler SchedulerCreate()
        {
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            return scheduler;
        }

        /// <summary>
        /// 创建一个具体的作业
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IJobDetail JobCreate<T>()
        {
            IJobDetail job = JobBuilder.Create(typeof(T)).Build();
            return job;
        }

        /// <summary>
        /// 创建一个触发器
        /// </summary>
        /// <param name="cron"></param>
        /// <returns></returns>
        public static ITrigger TriggerCreate(string cron)
        {
            ITrigger trigger = TriggerBuilder.Create()
                .WithCronSchedule(cron)
                .Build();

            return trigger;
        }
    }
}
