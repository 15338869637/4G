/***************************************************************************************
 * *
 * *        File Name        : ResponseCommon.cs
 * *        Creator          : llp
 * *        Create Time      : 2019-10-18 
 * *        Remark           : 业务响应公共基类 
 * *
 * *  Copyright (c) 深圳市富士智能系统有限公司.  All rights reserved. 
 * ***************************************************************************************/
namespace Fujica.com.cn.MonitorServiceClient.Business
{
    /// <summary>
    /// 业务响应公共基类 
    /// </summary>
    public class ResponseCommon
    {
        /// <summary>
        /// 是否执行成功
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 响应结果描述
        /// </summary>
        public string MessageContent { get; set; }

        /// <summary>
        /// redis读取内容
        /// </summary>
        public string RedisContent { get; set; }

        /// <summary>
        /// 消息类别
        /// </summary>
        public MsgType MsgType { get; set; }
    }

    public enum MsgType
    {
        /// <summary>
        /// 入场数据
        /// </summary>
        InParking,
        /// <summary>
        /// 出场数据
        /// </summary>
        OutParking,
        /// <summary>
        /// 压地感数据
        /// </summary>
        GroundSense,
        /// <summary>
        /// 车道拦截数据
        /// </summary>
        GateCatch,
        /// <summary>
        /// 支付数据
        /// </summary>
        PayCard,
        /// <summary>
        /// 相机心跳数据
        /// </summary>
        HeartBeat
    }
}
