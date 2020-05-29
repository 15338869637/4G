using Fujica.com.cn.BaseConnect;
using Fujica.com.cn.Logger;
using StackExchange.Redis;
/***************************************************************************************
 * *
 * *        File Name        : ExitPayDataManager.cs
 * *        Creator          : llp
 * *        Create Time      : 2019-10-18 
 * *        Remark           : 临时车出口压地感扫码缴费
 * *
 * *  Copyright (c) 深圳市富士智能系统有限公司.  All rights reserved. 
 * ***************************************************************************************/
namespace Fujica.com.cn.MonitorService.Business
{
    /// <summary>
    /// 临时车出口压地感扫码缴费 数据管理
    /// </summary>
    public static class ExitPayDataManager
    {
        public static bool RedisSetData(string setContent, ILogger ilogger)
        {
            bool result = false;
            try
            {
                IDatabase db;
                db = RedisHelper.GetDatabase(4);

                long resultCount = db.ListRightPush("MQ_GroundSense", setContent);

                result = resultCount > 0;

                if (result)
                    ilogger.LogInfo(LoggerLogicEnum.Tools, "", "", "", "Fujica.com.cn.MonitorService.Business.ExitPayDataManager.RedisSetData", "接收MQ返回压地感缴费数据保存至Redis成功");
                else
                    ilogger.LogError(LoggerLogicEnum.Tools, "", "", "", "Fujica.com.cn.MonitorService.Business.ExitPayDataManager.RedisSetData", "接收MQ返回压地感缴费数据保存至Redis失败");

                return result;

            }
            catch (System.Exception ex)
            {
                ilogger.LogFatal(LoggerLogicEnum.Tools, "", "", "", "Fujica.com.cn.MonitorService.Business.ExitPayDataManager.RedisSetData", "接收MQ返回压地感缴费数据保存至Redis异常", ex.ToString());
            }
            return result;
        }
    }
}
