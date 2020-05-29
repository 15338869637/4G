/***************************************************************************************
 * *
 * *        File Name        : GateCatchDataManager.cs
 * *        Creator          : llp
 * *        Create Time      : 2019-09-21 
 * *        Remark           : 车道拦截数据管理
 * *
 * *  Copyright (c) 深圳市富士智能系统有限公司.  All rights reserved. 
 * ***************************************************************************************/
using Fujica.com.cn.BaseConnect;
using Fujica.com.cn.Business.Model;
using Fujica.com.cn.Context.Model;
using Fujica.com.cn.Logger;
using Fujica.com.cn.Tools;
using StackExchange.Redis;
using System;
using System.Collections.Generic;

namespace Fujica.com.cn.MonitorServiceClient.Business
{
    /// <summary>
    /// 车道拦截数据管理.
    /// </summary>
    /// <remarks>
    /// 2019.09.21: 日志参数完善. llp <br/> 
    /// 2019.10.08: 修改日志记录. Ase <br/> 
    /// 2019.11.06：遥控手动开闸日志记录功能. Ase <br/>
    /// </remarks>   
    public class GateCatchDataManager
    {
        private static string mq_ListKey = "MQ_GateCatch";
        public static ResponseCommon DataHandle(ILogger m_ilogger, ISerializer m_serializer)
        {
            ResponseCommon response = new ResponseCommon()
            {
                IsSuccess = false,
                MsgType = MsgType.GateCatch
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

            GateCatchModel catchModel = m_serializer.Deserialize<GateCatchModel>(redisContent);
            if (catchModel == null)
            {
                response.MessageContent = "redis数据库读取值转换成实体失败：";
                return response;
            }
            if (string.IsNullOrEmpty(catchModel.DeviceIdentify)
                || string.IsNullOrEmpty(catchModel.ImgUrl)
                //|| string.IsNullOrEmpty(catchModel.CarNo)
                //|| string.IsNullOrEmpty(catchModel.CarTypeGuid)
                )
            {
                response.MessageContent = "redis数据转换成实体后必要参数缺失";
                return response;
            }

            db = RedisHelper.GetDatabase(0);
            string drivewayguid = db.HashGet("DrivewayLinkMACList", catchModel.DeviceIdentify);
            DrivewayModel drivewaymodel = m_serializer.Deserialize<DrivewayModel>(db.HashGet("DrivewayList", drivewayguid ?? ""));
            if (drivewaymodel == null)
            {
                response.MessageContent = "根据车道相机设备MAC地址，读取车道模型为空";
                m_ilogger.LogError(LoggerLogicEnum.Tools, "", "", catchModel.CarNo, "Fujica.com.cn.MonitorServiceClient.GateCatchDataManager", "根据车道相机设备MAC地址，读取车道模型为空");
                return response;
            }

            ParkLotModel parklotmodel = m_serializer.Deserialize<ParkLotModel>(db.HashGet("ParkLotList", drivewaymodel.ParkCode));
            if (parklotmodel == null)
            {
                response.MessageContent = "根据停车场编码，读取车场模型为空";
                m_ilogger.LogError(LoggerLogicEnum.Tools, "", "", catchModel.CarNo, "Fujica.com.cn.MonitorServiceClient.GateCatchDataManager", "根据停车场编码，读取车场模型为空");
                return response;
            }

            //500错误为“非法开闸”
            if (catchModel.ErrorCode == 500)
            {
                //将非法开闸数据发送给Fujica Api存入“异常开闸记录”报表
                if (!AddOpenGateRecord(catchModel, drivewaymodel, m_ilogger))
                {
                    response.MessageContent = "遥控手动非法开闸记录发送Fujica Api失败";
                    m_ilogger.LogError(LoggerLogicEnum.Tools, "", parklotmodel.ParkCode, catchModel.CarNo, "Fujica.com.cn.MonitorServiceClient.GateCatchDataManager", "遥控手动非法开闸记录发送Fujica Api失败");
                    return response;
                }
                else
                {
                    response.IsSuccess = true;
                    response.MessageContent = "遥控手动非法开闸记录成功";
                    return response;
                }
            }

            CarTypeModel cartypemodel = null;

            VehicleEntryDetailModel entrymodel = new VehicleEntryDetailModel(); //入场记录

            //无压地感车辆、无牌车数据
            if (catchModel.ErrorCode == 407 || catchModel.ErrorCode == 404)
            {
                cartypemodel = new CarTypeModel();
            }

            /// 13-无入场记录
            /// 14-临时车未缴费
            /// 400-黑名单
            /// 401-通行限制
            /// 402-月卡被锁
            /// 403-月卡过期
            /// 404-禁止无牌车
            /// 405-手动开闸
            /// 406-满车位
            /// 407-无压地感车辆
            /// 408是储值卡余额不足  
            else
            { 
                cartypemodel = m_serializer.Deserialize<CarTypeModel>(db.HashGet("CarTypeList", catchModel.CarTypeGuid));
                if (cartypemodel == null)
                {
                    response.MessageContent = "根据车类Guid，读取车类模型为空";
                    m_ilogger.LogError(LoggerLogicEnum.Tools, "", "", catchModel.CarNo, "Fujica.com.cn.MonitorServiceClient.EntryDataManager", "根据车类Guid，读取车类模型为空");
                    return response;
                }
                if (catchModel.ErrorCode==14||catchModel.ErrorCode == 402 || catchModel.ErrorCode == 403|| catchModel.ErrorCode == 408||catchModel.ErrorCode==405)
                { 
                    db = RedisHelper.GetDatabase(Common.GetDatabaseNumber(catchModel.CarNo));
                    //入场实体内容  
                    entrymodel = m_serializer.Deserialize<VehicleEntryDetailModel>(db.HashGet(catchModel.CarNo, drivewaymodel.ParkCode));
                }

            }

            GateCatchDetailModel detailModel = new GateCatchDetailModel()
            {
                CarNo = catchModel.CarNo,
                ParkingCode = drivewaymodel.ParkCode,
                DrivewayName = drivewaymodel.DrivewayName,
                DriveWayMAC = catchModel.DeviceIdentify,
                CarType = cartypemodel.CarType,
                CarTypeGuid = cartypemodel.Guid,
                CarTypeName = cartypemodel.CarTypeName,
                ImgUrl = catchModel.ImgUrl
            };

            //存到redis
            try
            {
                db = RedisHelper.GetDatabase(0);
                db.HashSet("GateCatchList", detailModel.DriveWayMAC, m_serializer.Serialize(detailModel)); //存储车道拦截数据 
                bool flag = db.HashExists("GateCatchList", detailModel.DriveWayMAC);

                #region 移动岗亭数据推送
                CaptureInOutModel capturModel = new CaptureInOutModel();
                capturModel.Guid = drivewaymodel.Guid;
                capturModel.ParkCode = detailModel.ParkingCode;
                capturModel.Exit = detailModel.DrivewayName;
                capturModel.DriveWayMAC = detailModel.DriveWayMAC;
                capturModel.CarNo = detailModel.CarNo;
                if (drivewaymodel.Type == DrivewayType.In) //入场 异常数据
                {
                    capturModel.InImgUrl = detailModel.ImgUrl;
                    capturModel.InTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    capturModel.EntryType = "0";
                }
                else
                {
                    capturModel.OutImgUrl = detailModel.ImgUrl;
                    capturModel.OutTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    capturModel.EntryType = "1";
                    if (catchModel.ErrorCode > 13 && (entrymodel.CarNo != null || entrymodel.CarNo != ""))//有入场记录
                    {
                        capturModel.InImgUrl = entrymodel.InImgUrl;
                        capturModel.Entrance = entrymodel.Entrance;
                        capturModel.InTime = Convert.ToDateTime(entrymodel.BeginTime);
                    }

                }  
                capturModel.CarType = Convert.ToInt32( detailModel.CarType);
                capturModel.CarTypeName = detailModel.CarTypeName;
                capturModel.CarTypeGuid = detailModel.CarTypeGuid;
                capturModel.Remark = "异常数据";
                capturModel.ErrorCode = catchModel.ErrorCode.ToString(); 
                //推送到客户端（实现移动岗亭功能）
                SendDataToClient(capturModel);

                //存储一份在redis中
                GateDataToRedis(capturModel, m_serializer);
                #endregion

                if (!flag)
                {
                    response.MessageContent = "车道拦截数据添加redis失败";
                    return response;
                }
                else
                {
                    response.IsSuccess = true;
                    response.MessageContent = "车道拦截数据添加redis成功";
                    return response;
                }

            }
            catch (Exception ex)
            {
                m_ilogger.LogFatal(LoggerLogicEnum.Tools, "", detailModel.ParkingCode, detailModel.CarNo, "Fujica.com.cn.MonitorServiceClient.Business.GateCatchDataManager.DataHandle", "保存车道拦截数据到redis异常", ex.ToString());

                response.MessageContent = "车道拦截数据数据发生异常：" + ex.ToString();
                return response;
            }
        }

        /// <summary>
        /// 出场抓拍数据推送到客户端（实现移动岗亭功能）
        /// </summary>
        /// <param name="model"></param> 
        /// <returns></returns> 
        private static bool SendDataToClient(CaptureInOutModel model)
        {
            RequestFujicaMvcApi request = new RequestFujicaMvcApi();
            //请求方法
            string servername = "Capture/CaptureSend";
            //请求参数
            Dictionary<string, object> dicParam = new Dictionary<string, object>();
            dicParam["Guid"] = model.Guid;//编号
            dicParam["ParkCode"] = model.ParkCode;//停车场编码
            dicParam["DriveWayMAC"] = model.DriveWayMAC;//车道设备地址
            dicParam["CarNo"] = model.CarNo;//车牌号
            dicParam["EntryType"] = model.EntryType;//出入场类型 
            dicParam["Entrance"] = model.Entrance;//入口地址 
            dicParam["Exit"] = model.Exit;//出口地址 
            dicParam["InImgUrl"] = model.InImgUrl;//入场图片地址  
            dicParam["OutImgUrl"] = model.OutImgUrl;//出场图片地址 
            dicParam["InTime"] = model.InTime;//车辆的入场时间 
            dicParam["OutTime"] = model.OutTime;//车辆的出场时间  
            dicParam["CarTypeName"] = model.CarTypeName;//车类
            dicParam["CarType"] = model.CarType;//车类
            dicParam["CarTypeGuid"] = model.CarTypeGuid;
            dicParam["Remark"] = model.Remark;//备注 
            dicParam["ErrorCode"] = model.ErrorCode; 
            return request.RequestInterfaceMvc(servername, dicParam);
        }

        /// <summary>
        /// 车道进出抓拍数据进出数据保存到redis中
        /// </summary>
        /// <param name="model">车道实体</param>
        /// <param name="m_serializer">序列化对象</param>
        /// <returns></returns>
        private static bool GateDataToRedis(CaptureInOutModel model, ISerializer m_serializer)
        {
            IDatabase db = RedisHelper.GetDatabase(0);
            string redisKey = $"GateDataList:{model.ParkCode}";
            db.HashSet(redisKey, model.DriveWayMAC, m_serializer.Serialize(model));
            db.KeyExpire(redisKey, DateTime.Now.AddDays(1).Date);
            return db.HashExists(redisKey, model.DriveWayMAC);
        }

        /// <summary>
        /// 遥控手动开闸发送数据
        /// 添加到异常记录报表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private static bool AddOpenGateRecord(GateCatchModel catchModel, DrivewayModel drivewaymodel, ILogger m_ilogger)
        {
            bool result = false;
            try
            {
                Dictionary<string, object> dicParam = new Dictionary<string, object>();
                RequestFujicaStandard requestFujica = new RequestFujicaStandard();
                string servername = "Park/UnderLineAddOpenGateRecord";
                dicParam["ParkingCode"] = drivewaymodel.ParkCode;
                dicParam["EntranceType"] = (Int32)drivewaymodel.Type;
                dicParam["EquipmentCode"] = drivewaymodel.DeviceMacAddress;
                dicParam["ThroughName"] = drivewaymodel.DrivewayName;
                dicParam["DiscernCamera"] = drivewaymodel.DeviceName;
                dicParam["ThroughType"] = 2;
                dicParam["OpenGateTime"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                dicParam["OpenGateReason"] = "遥控开闸";
                dicParam["OpenGateOperator"] = "";
                dicParam["Remarks"] = "";
                dicParam["ImageUrl"] = catchModel.ImgUrl;
                result = requestFujica.RequestInterfaceV2(servername, dicParam);
                return result;
            }

            catch (Exception ex)
            {
                m_ilogger.LogFatal(LoggerLogicEnum.Tools, "", drivewaymodel.ParkCode, catchModel.CarNo, "Fujica.com.cn.MonitorServiceClient.Business.GateCatchDataManager.AddOpenGateRecord", "添加到异常记录报表时发生异常", ex.ToString());
                return false;
            }
        }
    }
}
