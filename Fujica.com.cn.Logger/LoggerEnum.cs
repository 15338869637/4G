/***************************************************************************************
 * *
 * *        File Name        : Logger.cs
 * *        Creator          : llp
 * *        Create Time      : 2019-09-03 
 * *        Remark           : 日志需要的公用枚举类
 * *
 * *  Copyright (c) 深圳市富士智能系统有限公司.  All rights reserved. 
 * ***************************************************************************************/

namespace Fujica.com.cn.Logger
{

    /// <summary>
    /// 项目逻辑枚举.
    /// </summary>
    /// <remarks> 
    /// 2019.09.20:新增注释 llp <br/>  
    /// </remarks> 
    public enum LoggerLogicEnum
    {
        /// <summary>
        /// 工具层
        /// </summary>
        Tools,
        /// <summary>
        /// 数据服务层
        /// </summary>
        DataService,
        /// <summary>
        /// 业务层
        /// </summary>
        Bussiness,
        /// <summary>
        /// 通信层
        /// </summary>
        Communication,
        /// <summary>
        /// 接口层
        /// </summary>
        Interface,
        /// <summary>
        /// 交互层
        /// </summary>
        Interactive,
        /// <summary>
        /// 拦截器
        /// </summary>
        Filter,

    }


    /// <summary>
    /// 日志级别.
    /// </summary>
    /// <remarks> 
    /// 2019.09.20: 修改 和主平台日志级别枚举类保持一致  llp <br/>  
    /// </remarks>  
    public enum LevelEnum
    {
        #region
        ///// <summary>
        ///// 调试
        ///// </summary>
        //Debug = 0,
        ///// <summary>
        ///// 跟踪
        ///// </summary>
        //Trace = 1,
        ///// <summary>
        ///// 逻辑
        ///// </summary>
        //Logic = 2,
        ///// <summary>
        ///// 一般信息 
        ///// </summary>
        //Info = 3,
        ///// <summary>
        ///// 警告
        ///// </summary>
        //Warn = 4,
        ///// <summary>
        ///// 错误
        ///// </summary>
        //Error = 5,
        ///// <summary>
        ///// 致命
        ///// </summary>
        //Fatal = 6,
        ///// <summary>
        ///// 公共函数逻辑异常
        ///// </summary>
        //Utils = 7,
        #endregion

        /// <summary>
        /// 调试
        /// </summary>
        Debug = 0,
        /// <summary>
        /// 一般信息 
        /// </summary>
        Info = 1,
        /// <summary>
        /// 警告
        /// </summary>
        Warn = 2,
        /// <summary>
        /// 错误
        /// </summary>
        Error = 3,
        /// <summary>
        /// 致命
        /// </summary>
        Fatal = 4,
        /// <summary>
        /// 跟踪
        /// </summary>
        Trace = 5,
        /// <summary>
        /// 逻辑
        /// </summary>
        Logic = 6,
        /// <summary>
        /// 公共函数逻辑异常
        /// </summary>
        Utils = 7,
    }
}
