/***************************************************************************************
 * *
 * *        File Name        : HeartBeatDataManager.cs
 * *        Creator          : llp
 * *        Create Time      : 2019-09-21 
 * *        Remark           :监控相机心跳数据
 * *
 * *  Copyright (c) 深圳市富士智能系统有限公司.  All rights reserved. 
 * ***************************************************************************************/

using Fujica.com.cn.BaseConnect;
using Fujica.com.cn.Logger;
using Fujica.com.cn.MonitorServiceClient.Model;
using Fujica.com.cn.Tools;
using StackExchange.Redis;
using System;
using System.Collections.Generic;

namespace Fujica.com.cn.MonitorServiceClient.Business
{
    /// <summary>
    /// 监控相机心跳数据
    /// </summary>
    /// <remarks>
    /// 2019.09.21: 日志参数完善. llp <br/> 
    /// 2019.09.23：因相机和服务器时间同步问题，暂时不使用相机时间进行数据更新. Ase <br/>
    /// </remarks>
    public class HeartBeatDataManager : BaseBusiness
    {
        private static string mq_ListKey = "MQ_HeartBeat";
        public static ResponseCommon DataHandle(ILogger m_ilogger, ISerializer m_serializer)
        {
            ResponseCommon response = new ResponseCommon()
            {
                IsSuccess = false,
                MsgType = MsgType.HeartBeat
            };
            IDatabase db;
            db = RedisHelper.GetDatabase(4);
            string redisContent = db.ListLeftPop(mq_ListKey);
            if (string.IsNullOrEmpty(redisContent))
            {
                response.MessageContent = "redis数据库读取值为空";
                return response;
            }
            response.RedisContent = redisContent;
            m_ilogger.LogInfo(LoggerLogicEnum.Tools, "", "", "", "Fujica.com.cn.MonitorServiceClient.HeartBeatDataManager", "相机心跳数据接收成功。原始数据：" + redisContent);


            HeartBeatModel heartModel = m_serializer.Deserialize<HeartBeatModel>(redisContent);
            if (heartModel == null)
            {
                response.MessageContent = "redis数据库读取值转换成实体失败：";
                return response;
            }
            if (string.IsNullOrEmpty(heartModel.TimeStamp)
                || string.IsNullOrEmpty(heartModel.DeviceIdentify)
                || string.IsNullOrEmpty(heartModel.ParkingCode)
                )
            {
                response.MessageContent = "redis数据转换成实体后必要参数缺失";
                return response;
            }

            string hashKey = "HeartBeatList:" + heartModel.ParkingCode;
            db = RedisHelper.GetDatabase(0);
            //因相机和服务器时间同步问题，暂时不使用相机时间进行数据更新
            //db.HashSet(hashKey, heartModel.DeviceIdentify, Regex.Replace(heartModel.TimeStamp, @"(\d{4})(\d{2})(\d{2})(\d{2})(\d{2})(\d{2})", "$1-$2-$3 $4:$5:$6"));
            db.HashSet(hashKey, heartModel.DeviceIdentify, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            db.KeyExpire(hashKey, DateTime.Now.AddDays(1));//一天后自动过期
            bool flag = db.HashExists(hashKey, heartModel.DeviceIdentify);

            if (flag)
            {
                response.IsSuccess = true;
                response.MessageContent = heartModel.DeviceIdentify + "相机心跳数据添加redis成功";
                m_ilogger.LogInfo(LoggerLogicEnum.Tools, "", "", "", "Fujica.com.cn.MonitorServiceClient.HeartBeatDataManager", heartModel.DeviceIdentify + "相机心跳数据添加redis成功");

                #region 相机心跳数据发送至页面
                HeartBeatModel model = new HeartBeatModel()
                {
                    TimeStamp= heartModel.TimeStamp,
                    ParkingCode= heartModel.ParkingCode,
                    DeviceIdentify= heartModel.DeviceIdentify
                };
                SendHeartBeatToClient(model);
                #endregion

                return response;
            } 
            else
            {
                response.MessageContent = heartModel.DeviceIdentify + "相机心跳数据添加redis失败";
                m_ilogger.LogInfo(LoggerLogicEnum.Tools, "", heartModel.ParkingCode, "", "Fujica.com.cn.MonitorServiceClient.HeartBeatDataManager", heartModel.DeviceIdentify + "相机心跳数据添加redis失败");
                return response;
            }
        }

        /// <summary>
        /// 相机心跳数据发送至页面
        /// </summary>
        /// <param name="model"></param> 
        /// <returns></returns>
        private static bool SendHeartBeatToClient(HeartBeatModel model)
        {
            RequestFujicaMvcApi request = new RequestFujicaMvcApi();
            //请求方法
            string servername = "Capture/HeartBeatSend";
            //请求参数
            Dictionary<string, object> dicParam = new Dictionary<string, object>();
            dicParam["TimeStamp"] = model.TimeStamp;//时间戳
            dicParam["ParkingCode"] = model.ParkingCode;//停车场编码
            dicParam["DeviceIdentify"] = model.DeviceIdentify;//相机设备地址 
            return request.RequestInterfaceMvc(servername, dicParam);
        }

    }
}
