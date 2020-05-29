using Fujica.com.cn.BaseConnect;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Script.Serialization;
/***************************************************************************************
 * *
 * *        File Name        : LogHelper.cs
 * *        Creator          : llp
 * *        Create Time      : 2019-09-21 
 * *        Remark           : 工具类
 * *
 * *  Copyright (c) 深圳市富士智能系统有限公司.  All rights reserved. 
 * ***************************************************************************************/
namespace Fujica.com.cn.Logger
{
    /// <summary>
    /// 工具类.
    /// </summary>
    /// <remarks> 
    /// 2019.10.17: 修改: SetLoggerInfo,新增参数注释信息. llp <br/>  
    /// </remarks>  
    public class LogHelper
    {
        static JavaScriptSerializer jsonserializationer = new JavaScriptSerializer();
        public LogHelper()
        {
            //db = FollowRedisHelper.GetDatabase(9);
        }

        /// <summary>
        /// info
        /// </summary>
        /// <param name="contextCode">会话标识，线上：一次请求生命周期中的日志编码，一次请求响应使用一个编码</param>
        /// <param name="logicType">项目逻辑枚举类型</param>
        /// <param name="level">日志级别</param>
        /// <param name="loggerCode">日志编码，填写线下记录编码linerecordcode</param>
        /// <param name="parkingCode">停车场编码</param>
        /// <param name="carNoOrCardNo">车牌号或卡号</param>
        /// <param name="projectInfo">项目标识</param>
        /// <param name="message">请求的参数或者实体json</param>
        /// <param name="exception"> 错误信息或者异常信息</param>
        public static void SetLoggerInfo(
            string contextCode,
            LoggerLogicEnum logicType,
            LevelEnum level,
            string loggerCode,
            string parkingCode,
            string carNoOrCardNo,
            string projectInfo,
            string message,
            string exception)
        {
            Dictionary<string, object> arguments = new Dictionary<string, object>();
            arguments["LogicType"] = logicType.ToString();
            arguments["ContextCode"] = contextCode;
            arguments["LoggerCode"] = loggerCode;
            arguments["Level"] = level;
            arguments["ParkingCode"] = parkingCode;
            arguments["CarNoOrCardNo"] = carNoOrCardNo;
            arguments["ProjectInfo"] = projectInfo;
            arguments["Message"] = message;
            arguments["Exception"] = exception;
            arguments["CreateTime"] = DateTime.Now.ToString();
            arguments["Ip"] = Dns.GetHostAddresses(Dns.GetHostName()).First(p => p.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToString();
            try
            {
                IDatabase db = RedisHelper.GetDatabase(9);
                string key = string.Format("ApiLoggerInfo_{0}", DateTime.Now.Minute);
                db.ListRightPush(key, jsonserializationer.Serialize(arguments));
                db.KeyExpireAsync(key, DateTime.Now.AddMinutes(5)); //5分钟过期
            }
            catch { }
            finally { }
        }

    }
}
