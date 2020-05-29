using Fujica.com.cn.BaseConnect;
using Fujica.com.cn.Logger;
using StackExchange.Redis;
/***************************************************************************************
 * *
 * *        File Name        : EntryDataManager.cs
 * *        Creator          : llp
 * *        Create Time      : 2019-10-18 
 * *        Remark           : 车辆入场数据管理
 * *
 * *  Copyright (c) 深圳市富士智能系统有限公司.  All rights reserved. 
 * ***************************************************************************************/
namespace Fujica.com.cn.MonitorService.Business
{
    /// <summary>
    /// 车辆入场数据管理
    /// </summary>
    public static class EntryDataManager
    {
        public static bool RedisSetData(string setContent, ILogger ilogger)
        {
            bool result = false;
            try
            {
                IDatabase db;
                db = RedisHelper.GetDatabase(4);

                long resultCount = db.ListRightPush("MQ_InParking", setContent);

                result = resultCount > 0;

                if (result)
                    ilogger.LogInfo(LoggerLogicEnum.Tools, "", "", "", "Fujica.com.cn.MonitorService.Business.EntryDataManager.RedisSetData", "接收MQ返回入场数据保存至Redis成功");
                else
                    ilogger.LogError(LoggerLogicEnum.Tools, "", "", "", "Fujica.com.cn.MonitorService.Business.EntryDataManager.RedisSetData", "接收MQ返回入场数据保存至Redis失败");

                return result;

            }
            catch (System.Exception ex)
            {
                ilogger.LogFatal(LoggerLogicEnum.Tools, "", "", "", "Fujica.com.cn.MonitorService.Business.EntryDataManager.RedisSetData", "接收MQ返回入场数据保存至Redis异常", ex.ToString());
            }
            return result;
        }

    }
}
