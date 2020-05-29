/***************************************************************************************
 * *
 * *        File Name        : EntryDataManager.cs
 * *        Creator          : llp
 * *        Create Time      : 2019-09-21 
 * *        Remark           : 车辆入场数据管理
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
    /// 车辆入场数据管理.
    /// </summary>
    /// <remarks>
    /// 2019.09.21: 日志参数完善. llp <br/> 
    /// 2019.10.08: 修改日志记录. Ase <br/> 
    /// </remarks> 
    public class EntryDataManager : BaseBusiness
    {
        private static string mq_ListKey = "MQ_InParking";
        public static ResponseCommon DataHandle(ILogger m_ilogger, ISerializer m_serializer)
        {
            ResponseCommon response = new ResponseCommon()
            {
                IsSuccess = false,
                MsgType = MsgType.InParking
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
            m_ilogger.LogInfo(LoggerLogicEnum.Tools, "", "", "", "Fujica.com.cn.MonitorServiceClient.EntryDataManager", "车辆入场数据接收成功.原始数据：" + redisContent);

            VehicleInModel inmodel = m_serializer.Deserialize<VehicleInModel>(redisContent);
            if (inmodel == null)
            {
                response.MessageContent = "redis数据库读取值转换成实体失败：";
                return response;
            }
            if (string.IsNullOrEmpty(inmodel.Guid)
                || string.IsNullOrEmpty(inmodel.DriveWayMAC)
                || string.IsNullOrEmpty(inmodel.CarNo)
                //|| string.IsNullOrEmpty(inmodel.ImgUrl)//补发入场车辆，没有照片
                || inmodel.InTime == DateTime.MinValue
                || string.IsNullOrEmpty(inmodel.CarTypeGuid)
                //|| string.IsNullOrEmpty(inmodel.Remark)
                )
            {
                response.MessageContent = "redis数据转换成实体后必要参数缺失";
                return response;
            }

            //不为空说明是入场数据  
            db = RedisHelper.GetDatabase(0);
            string drivewayguid = db.HashGet("DrivewayLinkMACList", inmodel.DriveWayMAC);
            DrivewayModel drivewaymodel = m_serializer.Deserialize<DrivewayModel>(db.HashGet("DrivewayList", drivewayguid ?? ""));
            if (drivewaymodel == null)
            {
                response.MessageContent = "根据车道相机设备MAC地址，读取车道模型为空";
                m_ilogger.LogError(LoggerLogicEnum.Tools, inmodel.Guid,"", inmodel.CarNo, "Fujica.com.cn.MonitorServiceClient.EntryDataManager", "根据车道相机设备MAC地址，读取车道模型为空");
                return response;
            }
            CarTypeModel cartypemodel = m_serializer.Deserialize<CarTypeModel>(db.HashGet("CarTypeList", inmodel.CarTypeGuid));
            if (cartypemodel == null)
            {
                response.MessageContent = "根据车类Guid，读取车类模型为空";
                m_ilogger.LogError(LoggerLogicEnum.Tools, inmodel.Guid, drivewaymodel.ParkCode, inmodel.CarNo, "Fujica.com.cn.MonitorServiceClient.EntryDataManager", "根据车类Guid，读取车类模型为空");
                return response;
            }
            ParkLotModel parklotmodel = m_serializer.Deserialize<ParkLotModel>(db.HashGet("ParkLotList", cartypemodel.ParkCode));
            if (parklotmodel == null)
            {
                response.MessageContent = "根据停车场编码，读取车场模型为空";
                m_ilogger.LogError(LoggerLogicEnum.Tools, inmodel.Guid, drivewaymodel.ParkCode, inmodel.CarNo, "Fujica.com.cn.MonitorServiceClient.EntryDataManager", "根据停车场编码，读取车场模型为空");
                return response;
            }
            VehicleEntryDetailModel entrymodel = new VehicleEntryDetailModel()
            {
                RecordGuid = inmodel.Guid,
                ParkingName = parklotmodel.ParkName,
                ParkingCode = parklotmodel.ParkCode,
                CarNo = inmodel.CarNo,
                InImgUrl = inmodel.ImgUrl,
                BeginTime = inmodel.InTime,
                CarTypeGuid = inmodel.CarTypeGuid,
                Description = inmodel.Remark,
                CarType = (Int32)cartypemodel.CarType,
                CarTypeName = cartypemodel.CarTypeName,
                Entrance = drivewaymodel.DrivewayName,
                EntranceCamera = drivewaymodel.DeviceName,
                DriveWayMAC = inmodel.DriveWayMAC,
                OpenType = inmodel.OpenType,
                Operator = inmodel.Operator
            };
            //存到redis
            try
            {
                db = RedisHelper.GetDatabase(Common.GetDatabaseNumber(entrymodel.CarNo));
                //查询车辆是否已在场内（是否重复入场）
                bool repeatEntry = db.HashExists(entrymodel.CarNo, entrymodel.ParkingCode);

                //散列储存车辆入场数据
                db.HashSet(entrymodel.CarNo, entrymodel.ParkingCode, m_serializer.Serialize(entrymodel));
                db.KeyExpire(entrymodel.CarNo, DateTime.Now.AddDays(1 - DateTime.Now.Day).Date.AddMonths(3).AddSeconds(-1));//三个月自动过期或出场删除
                bool flag = db.HashExists(entrymodel.CarNo, entrymodel.ParkingCode);

                //在1号db存储在场车牌列表（用于后续业务的分页查询）
                if (flag)
                {
                    db = RedisHelper.GetDatabase(1);
                    string hashKey = entrymodel.ParkingCode + ":" + DateTime.Now.ToString("yyyyMM");
                    db.SortedSetAdd(hashKey, entrymodel.CarNo, Convert.ToDouble(inmodel.InTime.ToString("yyyyMMddHHmmss")));
                    flag = db.KeyExpire(hashKey, DateTime.Now.AddDays(1 - DateTime.Now.Day).Date.AddMonths(3).AddSeconds(-1));//三个月自动过期或出场删除
                }

                //在2号db存储入场数据实体（相机上传原样数据），出场再删掉（用于相机数据同步）
                if (flag)
                {
                    db = RedisHelper.GetDatabase(2);
                    string hashKey = entrymodel.ParkingCode + ":" + DateTime.Now.ToString("yyyyMM");
                    db.HashSet(hashKey, entrymodel.CarNo, redisContent);
                    db.KeyExpire(hashKey, DateTime.Now.AddDays(1 - DateTime.Now.Day).Date.AddMonths(3).AddSeconds(-1));//三个月自动过期或出场删除
                    flag = db.HashExists(hashKey, entrymodel.CarNo);
                }

                //redis存储成功后
                if (flag)
                {
                    m_ilogger.LogInfo(LoggerLogicEnum.Tools, entrymodel.RecordGuid, entrymodel.ParkingCode, entrymodel.CarNo, "Fujica.com.cn.MonitorServiceClient.EntryDataManager", entrymodel.CarNo + "车辆入场数据添加redis成功");

                    //当前车位数量
                    int remainingNumber = Convert.ToInt32(db.ListLength("SpaceNumberList:" + entrymodel.ParkingCode));
                    //如果不是重复入场，则进行车位数更新
                    if (!repeatEntry)
                    {
                        //剩余车位数控制
                        db = RedisHelper.GetDatabase(0);
                        db.ListLeftPop("SpaceNumberList:" + entrymodel.ParkingCode);
                        //调用mq给相机发送当前车位数
                        //最新车位数量
                        remainingNumber = Convert.ToInt32(db.ListLength("SpaceNumberList:" + entrymodel.ParkingCode));
                        SpaceNumberToCamera(remainingNumber, entrymodel.ParkingCode, m_ilogger, m_serializer);
                        //修改redis中停车场的剩余车位数
                        parklotmodel.RemainingSpace = (uint)remainingNumber;
                        db = RedisHelper.GetDatabase(0);
                        db.HashSet("ParkLotList", parklotmodel.ParkCode, m_serializer.Serialize(parklotmodel));
                        //修改mysql中停车场的剩余车位数
                        SaveSpaceNumberToDB(parklotmodel, m_ilogger, m_serializer);
                    }

                    #region 移动岗亭数据推送
                    CaptureInOutModel capturModel = new CaptureInOutModel()
                    {
                        Guid = entrymodel.RecordGuid,
                        ParkCode = entrymodel.ParkingCode,
                        Entrance = entrymodel.Entrance,
                        DriveWayMAC = entrymodel.DriveWayMAC,
                        RemainingNumber = remainingNumber.ToString(),
                        CarNo = entrymodel.CarNo,
                        EntryType = "0",
                        CarType = entrymodel.CarType,
                        CarTypeName = entrymodel.CarTypeName,
                        CarTypeGuid = entrymodel.CarTypeGuid,
                        InTime = Convert.ToDateTime(entrymodel.BeginTime),
                        InImgUrl = entrymodel.InImgUrl,
                        Remark = entrymodel.Description,
                        ErrorCode = "-1" //错误类型(异常时使用,正常数据默认是-1)
                    };

                    //推送到客户端（实现移动岗亭功能）
                    SendInDataToClient(capturModel);

                    //存储一份在redis中
                    GateDataToRedis(capturModel, m_serializer);
                    #endregion

                    //再往主平台Fujica补发入场数据
                    bool fujicaResult = EntryDataToFujica(entrymodel, cartypemodel.Idx);
                    if (fujicaResult)
                    {
                        //入场分发服务
                        DistributeEntryData(entrymodel, cartypemodel.Idx, m_ilogger);

                        response.IsSuccess = true;
                        response.MessageContent = entrymodel.CarNo + "车辆入场数据添加redis和补发fujica入场数据成功";
                        m_ilogger.LogInfo(LoggerLogicEnum.Tools, entrymodel.RecordGuid, entrymodel.ParkingCode, entrymodel.CarNo, "Fujica.com.cn.MonitorServiceClient.EntryDataManager", entrymodel.CarNo + "车辆入场数据添加redis和补发fujica入场数据成功");
                        return response;
                    }
                    else
                    {
                        response.IsSuccess = true;
                        response.MessageContent = entrymodel.CarNo + "车辆入场数据添加redis成功；补发fujica入场数据失败";
                        m_ilogger.LogError(LoggerLogicEnum.Tools, entrymodel.RecordGuid, entrymodel.ParkingCode, entrymodel.CarNo, "Fujica.com.cn.MonitorServiceClient.EntryDataManager", entrymodel.CarNo + "车辆入场数据添加redis成功；补发fujica入场数据失败");
                        return response;
                    }
                }
                else
                {
                    response.MessageContent = entrymodel.CarNo + "车辆入场数据添加redis失败";
                    m_ilogger.LogError(LoggerLogicEnum.Tools, entrymodel.RecordGuid, entrymodel.ParkingCode, entrymodel.CarNo, "Fujica.com.cn.MonitorServiceClient.EntryDataManager", entrymodel.CarNo + "车辆入场数据添加redis失败");
                    return response;
                }
            }
            catch (Exception ex)
            {
                m_ilogger.LogFatal(LoggerLogicEnum.Tools, entrymodel.RecordGuid, entrymodel.ParkingCode, entrymodel.CarNo, "Fujica.com.cn.MonitorServiceClient.EntryDataManager", entrymodel.CarNo + "保存入场数据到redis异常", ex.ToString());

                response.MessageContent = entrymodel.CarNo + "车辆入场数据发生异常：" + ex.ToString();
                return response;
            }
            finally
            {
                //GC.Collect();
            }

        }


        /// <summary>
        /// 补发入场数据给主平台Fujica
        /// </summary>
        /// <param name="entrymodel">入场记录实体</param>
        /// <param name="carType">车类</param>
        /// <returns>true:补发成功  false:补发失败</returns>
        private static bool EntryDataToFujica(VehicleEntryDetailModel entrymodel,string carType)
        {
            RequestFujicaStandard requestFujica = new RequestFujicaStandard();
            //请求方法
            string servername = "/Park/ReVehicleEntryRecordV2";
            //请求参数
            Dictionary<string, object> dicParam = new Dictionary<string, object>();
            dicParam["InEquipmentCode"] = entrymodel.DriveWayMAC;//入口设备机号  
            dicParam["DiscernCamera"] = entrymodel.EntranceCamera;//识别相机 
            dicParam["ThroughType"] = entrymodel.OpenType;//通行方式
            dicParam["CarNo"] = entrymodel.CarNo;//车牌号
            dicParam["ParkingCode"] = entrymodel.ParkingCode;//停车场编码
            dicParam["ParkName"] = entrymodel.ParkingName;//停车场名称
            dicParam["CustomDate"] = DateTime.Now;//客户端时间
            dicParam["LineRecordCode"] = entrymodel.RecordGuid;//线下停车记录编号
            dicParam["OperatorName"] = "入场服务程序";//操作员
            //暂时去掉，后期有可能用到，主平台那边是int类型
            //dicParam["WatchhouseCode"] = driveWayMAC;//出入口设备机号  
            dicParam["Entrance"] = entrymodel.Entrance;//入口名
            dicParam["InImgUrl"] = entrymodel.InImgUrl;//入场车辆图片地址
            dicParam["BeginTime"] = entrymodel.BeginTime;//车辆的入场时间
            dicParam["CarType"] = carType;//车类，对应车辆模板类型
            dicParam["CardType"] = entrymodel.CarType == 0 ? 3 : (entrymodel.CarType == 3 ? 1 : entrymodel.CarType);//fujica停车卡类型 1月卡 2储值卡 3 临时卡
            dicParam["Remark"] = entrymodel.Description;
            dicParam["ParkingOperator"] = entrymodel.Operator;

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
        private static bool SpaceNumberToCamera(int remainingNumber,string parkingCode, ILogger m_ilogger, ISerializer m_serializer)
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
        /// 进场抓拍数据推送到客户端（实现移动岗亭功能）
        /// </summary>
        /// <param name="model"></param> 
        /// <returns></returns>
        private static bool SendInDataToClient(CaptureInOutModel model)
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
            dicParam["EntryType"] = model.EntryType;
            dicParam["Entrance"] = model.Entrance;//入口地址 
            dicParam["InImgUrl"] = model.InImgUrl;//入场车辆图片地址
            dicParam["InTime"] = model.InTime;//车辆的入场时间  
            dicParam["CarTypeName"] = model.CarTypeName;//车类
            dicParam["CarType"] = model.CarType;//车类
            dicParam["CarTypeGuid"] = model.CarTypeGuid;
            dicParam["Remark"] = model.Remark;//备注 
            dicParam["ErrorCode"] = model.ErrorCode;  
            return request.RequestInterfaceMvc(servername, dicParam);
        }

        /// <summary>
        /// 车道进出抓拍数据保存到redis中
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
                m_ilogger.LogFatal(LoggerLogicEnum.Tools, "", model.ParkCode, "", "Fujica.com.cn.MonitorServiceClient.EntryDataManager.SaveSpaceNumberToDB", string.Format("保存停车场剩余车位数发生异常，入参:{0}", m_serializer.Serialize(model)), ex.ToString());
            }
            return false;
        }

        /// <summary>
        /// 分发入场数据
        /// </summary>
        /// <param name="entrymodel"></param>
        /// <param name="carType"></param>
        private static void DistributeEntryData(VehicleEntryDetailModel entrymodel, string carType, ILogger m_ilogger)
        {
            try
            {
                var instance = Distribute.GetInstance();
                VehicleEntryRequest entryReuqest = new VehicleEntryRequest()
                {
                    CarNo = entrymodel.CarNo,
                    CardNo = "",
                    ParkingCode = entrymodel.ParkingCode,
                    ParkName = entrymodel.ParkingName,
                    CustomDate = DateTime.Now,
                    LineRecordCode = entrymodel.RecordGuid,
                    OperatorName = entrymodel.Entrance,
                    IsBigParkCost = true,
                    WatchhouseCode = 0,
                    SpecialCar = 0,
                    Entrance = entrymodel.Entrance,
                    InImgUrl = entrymodel.InImgUrl,
                    ParkingCard = "",
                    BeginTime = entrymodel.BeginTime,
                    CarType = carType,
                    CardType = entrymodel.CarType == 0 ? 3 : (entrymodel.CarType == 3 ? 1 : entrymodel.CarType)
                };
                instance.DistributeEntryDataAsync(entryReuqest);
                m_ilogger.LogInfo(LoggerLogicEnum.Tools, entrymodel.RecordGuid, entrymodel.ParkingCode, entrymodel.CarNo, "Fujica.com.cn.MonitorServiceClient.EntryDataManager.DistributeEntryData", entrymodel.CarNo + "分发入场数据成功");
            }
            catch (Exception ex)
            {
                m_ilogger.LogFatal(LoggerLogicEnum.Tools, entrymodel.RecordGuid, entrymodel.ParkingCode, entrymodel.CarNo, "Fujica.com.cn.MonitorServiceClient.EntryDataManager.DistributeEntryData", entrymodel.CarNo + "分发入场数据异常", ex.ToString());

            }
        }
    }
}
