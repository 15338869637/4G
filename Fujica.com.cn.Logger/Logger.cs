using System;
using System.Threading.Tasks;

/***************************************************************************************
 * *
 * *        File Name        : Logger.cs
 * *        Creator          : llp
 * *        Create Time      : 2019-09-21 
 * *        Remark           : 基础类，全局枚举、特性封装
 * *
 * *  Copyright (c) 深圳市富士智能系统有限公司.  All rights reserved. 
 * ***************************************************************************************/
namespace Fujica.com.cn.Logger
{
    /// <summary>
    /// 基础类.
    /// </summary>
    /// <remarks> 
    /// 2019.10.17: 修改: 新增参数注释信息. llp <br/>  
    /// </remarks>  
    public class Logger : ILogger
    {
        private string _contextcode = Guid.NewGuid().ToString();
        /// <summary>
        /// 一次请求生命周期中的日志编码，一次请求响应使用一个编码
        /// </summary>
        private string ContextCode
        {
            get { return _contextcode; }
        }

        public Logger()
        {
        }
        /// <summary>
        /// 公共函数日志(该类型的日志直接存储)
        /// </summary>
        /// <param name="projectInfo">执行该日志的项目函数根路径(命名空间.类名.函数名)</param>
        /// <param name="message">请求的参数或者自定义错误信息</param>
        /// <param name="exception">异常信息</param>
        public void LogUtils(string projectInfo, object message, Exception exception)
        {
            Task.Factory.StartNew(() =>
            {
                LogHelper.SetLoggerInfo(ContextCode, LoggerLogicEnum.Tools, LevelEnum.Utils, "", "", "", projectInfo, Convert.ToString(message), Convert.ToString(exception));
            });
        }
        /// <summary>
        /// 调试日志
        /// </summary>
        /// <param name="logicType">项目逻辑枚举类型</param>
        /// <param name="loggerCode">日志编码，填写线下记录编码linerecordcode</param>
        /// <param name="parkingCode">停车场编码</param>
        /// <param name="carNoOrCardNo">车牌号或者卡号</param>
        /// <param name="projectInfo">执行该日志的项目函数根路径(命名空间.类名.函数名)</param>
        /// <param name="message">请求的参数或者实体json</param>
        /// <param name="exception">错误信息或者异常信息</param>
        public void LogDebug(LoggerLogicEnum logicType, string loggerCode, string parkingCode, string carNoOrCardNo, string projectInfo, string message, string exception)
        {
            //Console.WriteLine(message);

            Task.Factory.StartNew(() =>
            {
                LogHelper.SetLoggerInfo(ContextCode, logicType, LevelEnum.Debug, loggerCode, parkingCode, carNoOrCardNo, projectInfo, message, exception);
            });
        }
        /// <summary>
        /// 跟踪日志
        /// </summary>
        /// <param name="logicType">项目逻辑枚举类型</param>
        /// <param name="loggerCode">日志编码，填写线下记录编码linerecordcode</param>
        /// <param name="parkingCode">停车场编码</param>
        /// <param name="carNoOrCardNo">车牌号或者卡号</param>
        /// <param name="projectInfo">执行该日志的项目函数根路径(命名空间.类名.函数名)</param>
        /// <param name="message">请求的参数或者实体json</param>
        /// <param name="exception">错误信息或者异常信息</param>
        public void LogTrace(LoggerLogicEnum logicType, string loggerCode, string parkingCode, string carNoOrCardNo, string projectInfo, string message, string exception)
        {
            Task.Factory.StartNew(() =>
            {
                LogHelper.SetLoggerInfo(ContextCode, logicType, LevelEnum.Trace, loggerCode, parkingCode, carNoOrCardNo, projectInfo, message, exception);
            });
        }
        /// <summary>
        /// 逻辑日志
        /// </summary>
        /// <param name="logicType">项目逻辑枚举类型</param>
        /// <param name="loggerCode">日志编码，填写线下记录编码linerecordcode</param>
        /// <param name="parkingCode">停车场编码</param>
        /// <param name="carNoOrCardNo">车牌号或者卡号</param>
        /// <param name="projectInfo">执行该日志的项目函数根路径(命名空间.类名.函数名)</param>
        /// <param name="message">请求的参数或者实体json</param>
        /// <param name="exception">错误信息或者异常信息</param>
        public void LogLogic(LoggerLogicEnum logicType, string loggerCode, string parkingCode, string carNoOrCardNo, string projectInfo, string message, string exception)
        {
            Task.Factory.StartNew(() =>
            {
                LogHelper.SetLoggerInfo(ContextCode, logicType, LevelEnum.Logic, loggerCode, parkingCode, carNoOrCardNo, projectInfo, message, exception);
            });
        }
        /// <summary>
        /// 一般信息日志
        /// </summary>
        /// <param name="logicType">项目逻辑枚举类型</param>
        /// <param name="loggerCode">日志编码，填写线下记录编码linerecordcode</param>
        /// <param name="parkingCode">停车场编码</param>
        /// <param name="carNoOrCardNo">车牌号或者卡号</param>
        /// <param name="projectInfo">执行该日志的项目函数根路径(命名空间.类名.函数名)</param>
        /// <param name="message">请求的参数或者实体json</param>
        /// <param name="exception">错误信息或者异常信息</param>
        public void LogInfo(LoggerLogicEnum logicType, string loggerCode, string parkingCode, string carNoOrCardNo, string projectInfo, string message, string exception)
        {
            Task.Factory.StartNew(() =>
            {
                LogHelper.SetLoggerInfo(ContextCode, logicType, LevelEnum.Info, loggerCode, parkingCode, carNoOrCardNo, projectInfo, message, exception);
            });
        }
        /// <summary>
        /// 警告日志
        /// </summary>
        /// <param name="logicType">项目逻辑枚举类型</param>
        /// <param name="loggerCode">日志编码，填写线下记录编码linerecordcode</param>
        /// <param name="parkingCode">停车场编码</param>
        /// <param name="carNoOrCardNo">车牌号或者卡号</param>
        /// <param name="projectInfo">执行该日志的项目函数根路径(命名空间.类名.函数名)</param>
        /// <param name="message">请求的参数或者实体json</param>
        /// <param name="exception">错误信息或者异常信息</param>
        public void LogWarn(LoggerLogicEnum logicType, string loggerCode, string parkingCode, string carNoOrCardNo, string projectInfo, string message, string exception)
        {
            Task.Factory.StartNew(() =>
            {
                LogHelper.SetLoggerInfo(ContextCode, logicType, LevelEnum.Warn, loggerCode, parkingCode, carNoOrCardNo, projectInfo, message, exception);
            });
        }
        /// <summary>
        /// 错误日志
        /// </summary>
        /// <param name="logicType">项目逻辑枚举类型</param>
        /// <param name="loggerCode">日志编码，填写线下记录编码linerecordcode</param>
        /// <param name="parkingCode">停车场编码</param>
        /// <param name="carNoOrCardNo">车牌号或者卡号</param>
        /// <param name="projectInfo">执行该日志的项目函数根路径(命名空间.类名.函数名)</param>
        /// <param name="message">请求的参数或者实体json</param>
        /// <param name="exception">错误信息或者异常信息</param>
        public void LogError(LoggerLogicEnum logicType, string loggerCode, string parkingCode, string carNoOrCardNo, string projectInfo, string message, string exception)
        {
            //Console.ForegroundColor = ConsoleColor.Red;
            //Console.WriteLine(message + "\r\n" + exception);
            //Console.ResetColor();

            Task.Factory.StartNew(() =>
            {
                LogHelper.SetLoggerInfo(ContextCode, logicType, LevelEnum.Error, loggerCode, parkingCode, carNoOrCardNo, projectInfo, message, exception);
            });
        }

        /// <summary>
        /// 致命日志
        /// </summary>
        /// <param name="logicType">项目逻辑枚举类型</param>
        /// <param name="loggerCode">日志编码，填写线下记录编码linerecordcode</param>
        /// <param name="parkingCode">停车场编码</param>
        /// <param name="carNoOrCardNo">车牌号或者卡号</param>
        /// <param name="projectInfo">执行该日志的项目函数根路径(命名空间.类名.函数名)</param>
        /// <param name="message">请求的参数或者实体json</param>
        /// <param name="exception">错误信息或者异常信息</param>
        public void LogFatal(LoggerLogicEnum logicType, string loggerCode, string parkingCode, string carNoOrCardNo, string projectInfo, string message, string exception)
        {
            Task.Factory.StartNew(() =>
            {
                LogHelper.SetLoggerInfo(ContextCode, logicType, LevelEnum.Fatal, loggerCode, parkingCode, carNoOrCardNo, projectInfo, message, exception);
            });
        }
    }
}
