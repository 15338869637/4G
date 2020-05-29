/***************************************************************************************
 * *
 * *        File Name        : ExitDataManager.cs
 * *        Creator          : llp
 * *        Create Time      : 2019-09-21 
 * *        Remark           : 车辆出场数据管理
 * *
 * *  Copyright (c) 深圳市富士智能系统有限公司.  All rights reserved. 
 * ***************************************************************************************/
using Fujica.com.cn.BaseConnect;
using Fujica.com.cn.Business.Base;
using Fujica.com.cn.Business.Model;
using Fujica.com.cn.Communication.RabbitMQ;
using Fujica.com.cn.Context.Model;
using Fujica.com.cn.Logger;
using Fujica.com.cn.Tools;
using Fujica.com.cn.WCF.DistributeClient;
using Fujica.com.cn.WCF.Model.Park;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Data.Common;

namespace Fujica.com.cn.MonitorServiceClient.Business
{
    /// <summary>
    /// 车辆出场数据管理.
    /// </summary>
    /// <remarks>
    /// 2019.09.21: 日志参数完善. llp <br/> 
    /// 2019.10.08: 修改日志记录. Ase <br/> 
    /// 2019.10.08: 修复删除redis在场车辆跨月bug. Ase <br/> 
    /// </remarks>  
    public class ExitDataManager : BaseBusiness
    {
        private static string mq_ListKey = "MQ_OutParking";

        public static ResponseCommon DataHandle(ILogger m_ilogger, ISerializer m_serializer)
        {
            ResponseCommon response = new ResponseCommon()
            {
                IsSuccess = false,
                MsgType = MsgType.OutParking
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
            m_ilogger.LogInfo(LoggerLogicEnum.Tools, "", "", "", "Fujica.com.cn.MonitorServiceClient.ExitDataManager", "车辆出场数据接收成功.原始数据：" + redisContent);

            VehicleOutModel outmodel = m_serializer.Deserialize<VehicleOutModel>(redisContent);
            if (outmodel == null)
            {
                response.MessageContent = "redis数据库读取值转换成实体失败";
                return response;
            }
            if (string.IsNullOrEmpty(outmodel.Guid)
                || string.IsNullOrEmpty(outmodel.DriveWayMAC)
                || string.IsNullOrEmpty(outmodel.CarNo)
                //|| string.IsNullOrEmpty(outmodel.ImgUrl)
                || outmodel.OutTime == DateTime.MinValue
                || string.IsNullOrEmpty(outmodel.CarTypeGuid))
            {
                response.MessageContent = "redis数据转换成实体后必要参数缺失";
                return response;
            }

            //不为空说明是出场数据
            db = RedisHelper.GetDatabase(0);
            string drivewayguid = db.HashGet("DrivewayLinkMACList", outmodel.DriveWayMAC);
            DrivewayModel drivewaymodel = m_serializer.Deserialize<DrivewayModel>(db.HashGet("DrivewayList", drivewayguid ?? ""));
            if (drivewaymodel == null)
            {
                response.MessageContent = "根据车道相机设备MAC地址，读取车道模型为空";
                m_ilogger.LogError(LoggerLogicEnum.Tools, outmodel.Guid, "", outmodel.CarNo, "Fujica.com.cn.MonitorServiceClient.ExitDataManager", "根据车道相机设备MAC地址，读取车道模型为空");
                return response;
            }
            CarTypeModel cartypemodel = m_serializer.Deserialize<CarTypeModel>(db.HashGet("CarTypeList", outmodel.CarTypeGuid));
            if (cartypemodel == null)
            {
                response.MessageContent = "根据车类Guid，读取车类模型为空";
                m_ilogger.LogError(LoggerLogicEnum.Tools, outmodel.Guid, drivewaymodel.ParkCode, outmodel.CarNo, "Fujica.com.cn.MonitorServiceClient.ExitDataManager", "根据车类Guid，读取车类模型为空");
                return response;
            }
            ParkLotModel parklotmodel = m_serializer.Deserialize<ParkLotModel>(db.HashGet("ParkLotList", drivewaymodel.ParkCode));
            if (parklotmodel == null)
            {
                response.MessageContent = "根据停车场编码，读取车场模型为空";
                m_ilogger.LogError(LoggerLogicEnum.Tools, outmodel.Guid, drivewaymodel.ParkCode, outmodel.CarNo, "Fujica.com.cn.MonitorServiceClient.ExitDataManager", "根据停车场编码，读取车场模型为空");
                return response;
            }
            VehicleExitDetailModel exitmodel = new VehicleExitDetailModel()
            {
                RecordGuid = outmodel.Guid,
                ParkingName = parklotmodel.ParkName,
                ParkingCode = parklotmodel.ParkCode,
                CarNo = outmodel.CarNo,
                OutImgUrl = outmodel.ImgUrl,
                LeaveTime = outmodel.OutTime,
                Description = outmodel.Remark,
                Exit = drivewaymodel.DrivewayName,
                ExitCamera = drivewaymodel.DeviceName,
                DriveWayMAC = outmodel.DriveWayMAC,
                OpenType = outmodel.OpenType,
                Operator = outmodel.Operator
            };
            //redis操作
            try
            {
                db = RedisHelper.GetDatabase(Common.GetDatabaseNumber(exitmodel.CarNo));
                //删除前先拿到实体内容
                VehicleEntryDetailModel entrymodel = m_serializer.Deserialize<VehicleEntryDetailModel>(db.HashGet(exitmodel.CarNo, exitmodel.ParkingCode));

                if (entrymodel == null)
                {
                    response.MessageContent = "未找到入场记录";
                    return response;
                }

                bool flag = db.HashDelete(exitmodel.CarNo, exitmodel.ParkingCode);

                //1号db移除该在场车牌
                if (flag)
                {
                    db = RedisHelper.GetDatabase(1);
                    string hashKey = entrymodel.ParkingCode + ":" + DateTime.Now.ToString("yyyyMM");
                    flag = db.SortedSetRemove(hashKey, entrymodel.CarNo);
                    //如果当月集合中不存在，则找最近的二个月的数据
                    if(!flag)
                        for (int i = 1; i < 3; i++)
                        {
                            hashKey = entrymodel.ParkingCode + ":" + DateTime.Now.AddMonths(-i).ToString("yyyyMM");
                            flag = db.SortedSetRemove(hashKey, entrymodel.CarNo);
                            if (flag) break;
                        }
                }
                //2号db移除存储入场数据（相机上传原样数据），出场再删掉（用于相机数据同步）
                if (flag)
                {
                    db = RedisHelper.GetDatabase(2);
                    string hashKey = entrymodel.ParkingCode + ":" + DateTime.Now.ToString("yyyyMM");
                    flag = db.HashDelete(hashKey, entrymodel.CarNo);
                    //如果当月集合中不存在，则找最近的二个月的数据
                    if (!flag)
                        for (int i = 1; i < 3; i++)
                        {
                            hashKey = entrymodel.ParkingCode + ":" + DateTime.Now.AddMonths(-i).ToString("yyyyMM");
                            flag = db.HashDelete(hashKey, entrymodel.CarNo);
                            if (flag) break;
                        }
                }

                if (flag)
                {
                    m_ilogger.LogInfo(LoggerLogicEnum.Tools, exitmodel.RecordGuid, exitmodel.ParkingCode, exitmodel.CarNo, "Fujica.com.cn.MonitorServiceClient.ExitDataManager", entrymodel.CarNo + "车辆出场数据删除redis数据成功");

                    //剩余车位数控制
                    db = RedisHelper.GetDatabase(0);
                    db.ListRightPush("SpaceNumberList:" + entrymodel.ParkingCode, 1);
                    //调用mq给相机发送当前车位数
                    int remainingNumber = Convert.ToInt32(db.ListLength("SpaceNumberList:" + entrymodel.ParkingCode));
                    SpaceNumberToCamera(remainingNumber, entrymodel.ParkingCode, m_ilogger, m_serializer);
                    //修改redis中停车场的剩余车位数
                    parklotmodel.RemainingSpace = (uint)remainingNumber;
                    db = RedisHelper.GetDatabase(0);
                    db.HashSet("ParkLotList", parklotmodel.ParkCode, m_serializer.Serialize(parklotmodel));
                    //修改mysql中停车场的剩余车位数
                    SaveSpaceNumberToDB(parklotmodel, m_ilogger, m_serializer);


                    #region 移动岗亭数据推送
                    CaptureInOutModel capturModel = new CaptureInOutModel()
                    {
                        Guid = exitmodel.RecordGuid,
                        ParkCode = exitmodel.ParkingCode,
                        Entrance=entrymodel.Entrance,
                        Exit = exitmodel.Exit,
                        DriveWayMAC = exitmodel.DriveWayMAC,
                        RemainingNumber = remainingNumber.ToString(),
                        CarNo = exitmodel.CarNo,
                        EntryType = "1",
                        CarType =Convert.ToInt32( entrymodel.CarType.ToString()),
                        CarTypeName = entrymodel.CarTypeName,
                        CarTypeGuid = entrymodel.CarTypeGuid,
                        InTime = Convert.ToDateTime(entrymodel.BeginTime),
                        OutTime= exitmodel.LeaveTime,
                        OutImgUrl= exitmodel.OutImgUrl,
                        InImgUrl = entrymodel.InImgUrl,
                        Remark=exitmodel.Description,
                        ErrorCode = "-1" //错误类型(异常时使用,正常数据默认是-1)
                    }; 

                    //推送到客户端（实现移动岗亭功能）
                    SendOutDataToClient(capturModel);

                    //存储一份在redis中
                    GateDataToRedis(capturModel, m_serializer);
                    #endregion

                    //往主平台Fujica补发出场数据                
                    bool fujicaResult = ExitDataToFujica(exitmodel, entrymodel, cartypemodel.Idx);
                    if (fujicaResult)
                    {
                        //出场分发服务
                        DistributeExitData(exitmodel, entrymodel, cartypemodel.Idx, m_ilogger);

                        response.IsSuccess = true;
                        response.MessageContent = entrymodel.CarNo + "车辆出场数据删除redis和补发fujica出场数据成功";
                        m_ilogger.LogInfo(LoggerLogicEnum.Tools, exitmodel.RecordGuid, exitmodel.ParkingCode, exitmodel.CarNo, "Fujica.com.cn.MonitorServiceClient.ExitDataManager", entrymodel.CarNo + "车辆出场数据删除redis和补发fujica出场数据成功");
                        return response;
                    }
                    else
                    {
                        response.IsSuccess = true;
                        response.MessageContent = entrymodel.CarNo + "车辆出场数据删除redis成功；补发fujica出场数据失败";
                        m_ilogger.LogError(LoggerLogicEnum.Tools, exitmodel.RecordGuid, exitmodel.ParkingCode, exitmodel.CarNo, "Fujica.com.cn.MonitorServiceClient.ExitDataManager", entrymodel.CarNo + "车辆出场数据删除redis成功；补发fujica出场数据失败");
                        return response;
                    }

                }
                else
                {
                    response.MessageContent = entrymodel.CarNo + "车辆出场数据删除redis数据失败";
                    m_ilogger.LogError(LoggerLogicEnum.Tools, exitmodel.RecordGuid, exitmodel.ParkingCode, exitmodel.CarNo, "Fujica.com.cn.MonitorServiceClient.ExitDataManager", entrymodel.CarNo + "车辆出场数据删除redis数据失败");
                    return response;
                }
            }
            catch (Exception ex)
            {
                m_ilogger.LogFatal(LoggerLogicEnum.Tools, exitmodel.RecordGuid, exitmodel.ParkingCode, exitmodel.CarNo, "Fujica.com.cn.MonitorServiceClient.Business.ExitDataManager.DataHandle", "在redis移除停车数据异常", ex.ToString());

                response.MessageContent = "车辆出场数据发生异常：" + ex.ToString();
                return response;
            }

        }


        /// <summary>
        /// 补发出场数据给主平台Fujica
        /// </summary>
        /// <param name="exitmodel">出场记录实体</param>
        /// <param name="entrymodel">入场记录实体</param>
        /// <param name="carType">车类</param>
        /// <returns>true:补发成功  false:补发失败</returns>
        private static bool ExitDataToFujica(VehicleExitDetailModel exitmodel, VehicleEntryDetailModel entrymodel, string carType)
        {
            RequestFujicaStandard requestFujica = new RequestFujicaStandard();
            //请求方法
            string servername = "/Park/ReVehicleOutRecordV2";
            //请求参数
            Dictionary<string, object> dicParam = new Dictionary<string, object>();
            dicParam["InEquipmentCode"] = entrymodel.DriveWayMAC;//入口设备机号
            dicParam["OutEquipmentCode"] = exitmodel.DriveWayMAC;//出口设备机号
            dicParam["InDiscernCamera"] = entrymodel.EntranceCamera;//入口识别相机
            dicParam["OutDiscernCamera"] = exitmodel.ExitCamera;//出口识别相机
            dicParam["InThroughType"] = entrymodel.OpenType;//入口通行方式
            dicParam["OutThroughType"] = exitmodel.OpenType;//出口通行方式
            dicParam["CarNo"] = exitmodel.CarNo;//车牌号
            dicParam["ParkingCode"] = exitmodel.ParkingCode;//停车场编码 
            dicParam["LongStop"] = (Int32)(exitmodel.LeaveTime - entrymodel.BeginTime).TotalMinutes;
            //LongStop  停驶时间（分钟）
            dicParam["Entrance"] = entrymodel.Entrance;//入口名
            dicParam["Export"] = exitmodel.Exit;//出口名
            dicParam["CustomDate"] = DateTime.Now;//客户端时间
            dicParam["AppearanceDate"] = exitmodel.LeaveTime;//出场时间
            dicParam["AdmissionDate"] = entrymodel.BeginTime;//入场时间
            dicParam["InImgUrl"] = entrymodel.InImgUrl;//入场车辆图片地址
            dicParam["OutImgUrl"] = exitmodel.OutImgUrl;//出场车辆图片地址
            dicParam["LineRecordCode"] = exitmodel.RecordGuid;//线下停车记录编号
            dicParam["CarType"] = carType;//车类
            dicParam["CardType"] = entrymodel.CarType == 0 ? 3 : (entrymodel.CarType == 3 ? 1 : entrymodel.CarType);//fujica停车卡类型 1月卡 2储值卡 3 临时卡
            dicParam["Description"] = exitmodel.Description;
            dicParam["InParkingOperator"] = entrymodel.Operator; //入场值班人员
            dicParam["OutParkingOperator"] = exitmodel.Operator;//出场值班人员

             

            //返回fujica api补发车辆入场记录 接口的结果
            return requestFujica.RequestInterfaceV2(servername, dicParam);
        }

        /// <summary>
        /// 发送剩余车位数量给相机
        /// </summary>
        /// <param name="remainingNumber">剩余车位数</param>
        /// <param name="parkingCode">停车场编号</param>
        /// <param name="m_ilogger"></param>
        /// <param name="m_serializer"></param>
        /// <returns></returns>
        private static bool SpaceNumberToCamera(int remainingNumber, string parkingCode, ILogger m_ilogger, ISerializer m_serializer)
        {
            CommandEntity<int> entity = new CommandEntity<int>()
            {
                command = BussineCommand.RemainingSpace,
                idMsg = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                message = remainingNumber
            };
            RabbitMQSender rabbitMq = new RabbitMQSender(m_ilogger, m_serializer);
            return rabbitMq.SendMessageForRabbitMQ("发送剩余车位数", m_serializer.Serialize(entity), entity.idMsg, parkingCode);
        }

        /// <summary>
        /// 出场抓拍数据推送到客户端（实现移动岗亭功能）
        /// </summary>
        /// <param name="model"></param> 
        /// <returns></returns>
        private static bool SendOutDataToClient(CaptureInOutModel model)
        {
            RequestFujicaMvcApi request = new RequestFujicaMvcApi();
            //请求方法
            string servername = "Capture/CaptureSend";
            //请求参数
            Dictionary<string, object> dicParam = new Dictionary<string, object>();
            dicParam["Guid"] = model.Guid;//编号
            dicParam["ParkCode"] = model.ParkCode;//停车场编码
            dicParam["DriveWayMAC"] = model.DriveWayMAC;//车道设备地址
            dicParam["RemainingNumber"] = model.RemainingNumber;
            dicParam["CarNo"] = model.CarNo;//车牌号
            dicParam["EntryType"] = 1;//出场 
            dicParam["Entrance"] = model.Entrance;//入口地址 
            dicParam["Exit"] = model.Exit;//出口地址 
            dicParam["InImgUrl"] = model.InImgUrl;//入场图片地址  
            dicParam["OutImgUrl"] = model.OutImgUrl;//出场图片地址 
            dicParam["InTime"] = model.InTime;//车辆的入场时间 
            dicParam["OutTime"] = model.OutTime;//车辆的出场时间  
            dicParam["CarTypeName"] = model.CarTypeName; 
            dicParam["CarType"] = model.CarType; 
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
        /// 修改mysql中停车场的剩余车位数
        /// </summary>
        /// <param name="model">停车场实体</param>
        /// <param name="m_ilogger"></param>
        /// <param name="m_serializer"></param>
        /// <returns></returns>
        private static bool SaveSpaceNumberToDB(ParkLotModel model, ILogger m_ilogger, ISerializer m_serializer)
        {
            try
            {
                string commandtext = @"UPDATE t_parklot SET entityContent=@entityContent WHERE projectGuid=@projectGuid and parkCode=@parkCode";

                DbParameter projectGuid = dbhelper.factory.CreateParameter();
                projectGuid.ParameterName = "@projectGuid";
                projectGuid.Value = model.ProjectGuid;

                DbParameter parkCode = dbhelper.factory.CreateParameter();
                parkCode.ParameterName = "@parkCode";
                parkCode.Value = model.ParkCode;

                DbParameter entityContent = dbhelper.factory.CreateParameter();
                entityContent.ParameterName = "@entityContent";
                entityContent.Value = m_serializer.Serialize(model);

                DbParameter[] parameter = new DbParameter[] { projectGuid, parkCode, entityContent };
                return dbhelper.ExecuteNonQuery(commandtext, parameter) > 0 ? true : false;
            }
            catch (Exception ex)
            {
                m_ilogger.LogFatal(LoggerLogicEnum.Tools, "", model.ParkCode, "", "Fujica.com.cn.MonitorServiceClient.ExitDataManager.SaveSpaceNumberToDB", string.Format("保存停车场剩余车位数发生异常，入参:{0}", m_serializer.Serialize(model)), ex.ToString());
            }
            return false;
        }

        /// <summary>
        /// 分发出场数据
        /// </summary>
        /// <param name="entrymodel"></param>
        /// <param name="carType"></param>
        private static void DistributeExitData(VehicleExitDetailModel exitmodel, VehicleEntryDetailModel entrymodel, string carType, ILogger m_ilogger)
        {
            try
            {
                var instance = Distribute.GetInstance();
                VehicleOutRecordRequest exitReuqest = new VehicleOutRecordRequest()
                {
                    CarNo = entrymodel.CarNo,
                    ParkingCode = entrymodel.ParkingCode,
                    CardNo = "",
                    LongStop = (Int32)(exitmodel.LeaveTime - entrymodel.BeginTime).TotalMinutes,
                    Entrance = entrymodel.Entrance,
                    Export = exitmodel.Exit,
                    CustomDate = DateTime.Now,
                    AppearanceDate = exitmodel.LeaveTime,
                    Description = exitmodel.Description,
                    AdmissionDate = entrymodel.BeginTime,
                    ParkingCard = "",
                    InImgUrl = entrymodel.InImgUrl,
                    OutImgUrl = exitmodel.OutImgUrl,
                    LineRecordCode = entrymodel.RecordGuid,
                    GroupCar = 0,
                    CarType = carType,
                    CardType = entrymodel.CarType == 0 ? 3 : (entrymodel.CarType == 3 ? 1 : entrymodel.CarType)
                };
                instance.DistributeOutDataAsync(exitReuqest);
                m_ilogger.LogInfo(LoggerLogicEnum.Tools, entrymodel.RecordGuid, entrymodel.ParkingCode, entrymodel.CarNo, "Fujica.com.cn.MonitorServiceClient.ExitDataManager.DistributeExitData", entrymodel.CarNo + "分发出场数据成功");
            }
            catch (Exception ex)
            {
                m_ilogger.LogFatal(LoggerLogicEnum.Tools, entrymodel.RecordGuid, entrymodel.ParkingCode, entrymodel.CarNo, "Fujica.com.cn.MonitorServiceClient.ExitDataManager.DistributeExitData", entrymodel.CarNo + "分发出场数据异常", ex.ToString());

            }
        }
    }
}
