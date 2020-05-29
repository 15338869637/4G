using System;

namespace Fujica.com.cn.Logger
{
    public interface ILogger
    {
        /// <summary>
        /// 公共函数日志(该类型的日志直接存储)
        /// </summary>
        /// <param name="projectInfo">执行该日志的项目函数根路径(命名空间.类名.函数名)</param>
        /// <param name="message">请求的参数或者自定义错误信息</param>
        /// <param name="exception">异常信息</param>
        void LogUtils(string projectInfo, object message, Exception exception);
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
        void LogDebug(LoggerLogicEnum logicType, string loggerCode, string parkingCode, string carNoOrCardNo, string projectInfo, string message, string exception = "");

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
        void LogTrace(LoggerLogicEnum logicType, string loggerCode, string parkingCode, string carNoOrCardNo, string projectInfo, string message, string exception = "");

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
        void LogLogic(LoggerLogicEnum logicType, string loggerCode, string parkingCode, string carNoOrCardNo, string projectInfo, string message, string exception = "");

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
        void LogInfo(LoggerLogicEnum logicType, string loggerCode, string parkingCode, string carNoOrCardNo, string projectInfo, string message, string exception = "");

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
        void LogWarn(LoggerLogicEnum logicType, string loggerCode, string parkingCode, string carNoOrCardNo, string projectInfo, string message, string exception = "");

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
        void LogError(LoggerLogicEnum logicType, string loggerCode, string parkingCode, string carNoOrCardNo, string projectInfo, string message, string exception = "");

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
        void LogFatal(LoggerLogicEnum logicType, string loggerCode, string parkingCode, string carNoOrCardNo, string projectInfo, string message, string exception = "");
    }
}
