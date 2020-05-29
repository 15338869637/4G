using Fujica.com.cn.BaseConnect;
using Fujica.com.cn.Business.Base;
using Fujica.com.cn.Business.Model;
using Fujica.com.cn.Business.ParkLot;
using Fujica.com.cn.Context.Model;
using Fujica.com.cn.Logger;
using Fujica.com.cn.Tools;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fujica.com.cn.Business.Parking
{
    //曾维04月09日注释，将该类移到ParkLot中
    ///// <summary>
    ///// 车进出场类
    ///// </summary>
    //public class VehicleInOutManager:IBaseBusiness
    //{
    //    /// <summary>
    //    /// 日志记录器
    //    /// </summary>
    //    internal readonly ILogger m_logger;

    //    /// <summary>
    //    /// 序列化器
    //    /// </summary>
    //    internal readonly ISerializer m_serializer = null;

    //    /// <summary>
    //    /// 车场管理器
    //    /// </summary>
    //    private ParkLotManager parklotmanager;

    //    /// <summary>
    //    /// 车类管理器
    //    /// </summary>
    //    private CarTypeManager cartypemanager;

    //    /// <summary>
    //    /// 车道管理器
    //    /// </summary>
    //    private DrivewayManager drivewaymanager;

    //    public string LastErrorDescribe
    //    {
    //        get
    //        {
    //            throw new NotImplementedException();
    //        }

    //        set
    //        {
    //            throw new NotImplementedException();
    //        }
    //    }
    //    public VehicleInOutManager(ILogger _logger, ISerializer _serializer, 
    //        ParkLotManager _parklotmanager, CarTypeManager _cartypemanager,
    //        DrivewayManager _drivewaymanager)
    //    {
    //        m_logger = _logger;
    //        m_serializer = _serializer;
    //        parklotmanager = _parklotmanager;
    //        cartypemanager = _cartypemanager;
    //        drivewaymanager = _drivewaymanager;
    //    }

    //    /// <summary>
    //    /// 进场
    //    /// </summary>
    //    /// <returns></returns>
    //    public bool Enter(VehicleInOutModel model,string broadcastData)
    //    {
    //        if (model == null) return false;

    //        VehicleEntryDetailModel content = new VehicleEntryDetailModel();
    //        content.RecordGuid = model.Guid;
    //        content.CarNo = model.CarNo;
    //        content.BeginTime = model.EventTime;
    //        content.InImgUrl = model.ImgUrl;
    //        content.CarTypeGuid = model.CarTypeGuid;
    //        content.Description = model.Remark;
    //        content.DriveWayMAC = model.DriveWayMAC;

    //        CarTypeModel cartypecontent = cartypemanager.GetCarType(model.CarTypeGuid);
    //        if (cartypecontent != null)
    //        {
    //            content.CarType = cartypecontent.carType;
    //            content.CarTypeName = cartypecontent.carTypeName;
    //            ParkLotModel parklotcontent = parklotmanager.GetParkLot(cartypecontent.parkCode);
    //            if (parklotcontent != null) {
    //                content.ParkingName = parklotcontent.parkName;
    //                content.ParkingCode = parklotcontent.parkCode;
    //            }
    //            DrivewayModel drivewaycontent = drivewaymanager.GetDrivewayByMacAddress(model.DriveWayMAC);
    //            if (drivewaycontent != null)
    //            {
    //                content.Entrance = drivewaycontent.drivewayName;
    //                //存到redis
    //                try
    //                {
    //                    IDatabase db = FollowRedisHelper.GetDatabase(GetDatabaseNumber(model.CarNo));
    //                    db.HashSet(content.CarNo, content.ParkingCode, m_serializer.Serialize(content)); //存储入场数据
    //                    bool flag = db.HashExists(content.CarNo, content.ParkingCode);

    //                    //存储成功后在1号db存储在场车牌 在2号db存储待广播的车牌
    //                    ////3号db缓存半小时该进场数据
    //                    if (flag)
    //                    {
    //                        db = FollowRedisHelper.GetDatabase(1); 
    //                        db.HashSet(content.ParkingCode, content.CarNo, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")); //存储车场在场车牌索引
    //                        flag = db.HashExists(content.ParkingCode, content.CarNo);
    //                    }
    //                    //存储成功后在2号db存储待广播的车牌
    //                    if (flag)
    //                    {
    //                        db = FollowRedisHelper.GetDatabase(2);
    //                        db.HashSet(content.ParkingCode, content.CarNo, broadcastData);
    //                        flag = db.HashExists(content.ParkingCode, content.CarNo);
    //                    }
    //                    //存储成功后在3号db存储入场车数据半小时
    //                    if (flag)
    //                    {
    //                        db = FollowRedisHelper.GetDatabase(3);
    //                        string key = string.Format("{0}_{1}", drivewaycontent.guid, model.CarNo);
    //                        db.StringSet(key, m_serializer.Serialize(model), TimeSpan.FromSeconds(0.5 * 3600));
    //                        flag = db.KeyExists(key);
    //                    }
    //                    return flag;
    //                }
    //                catch (Exception ex)
    //                {
    //                    m_logger.LogFatal(LoggerLogicEnum.Bussiness, "", content.ParkingCode, "", "Fujica.com.cn.Business.Parking.VehicleInOutManager.Enter", "保存停车数据到redis异常", ex.ToString());
    //                    return false;
    //                }
    //            }
    //        }
    //        return false;
    //    }

    //    /// <summary>
    //    /// 出场
    //    /// </summary>
    //    /// <returns></returns>
    //    public bool Exit(VehicleInOutModel model)
    //    {
    //        if (model == null) return false;

    //        VehicleExitDetailModel content = new VehicleExitDetailModel();
    //        content.RecordGuid = model.Guid;
    //        content.CarNo = model.CarNo;
    //        content.LeaveTime = model.EventTime;
    //        content.OutImgUrl = model.ImgUrl;
    //        content.Description = model.Remark;

    //        CarTypeModel cartypecontent = cartypemanager.GetCarType(model.CarTypeGuid);
    //        if (cartypecontent != null)
    //        {
    //            ParkLotModel parklotcontent = parklotmanager.GetParkLot(cartypecontent.parkCode);
    //            if (parklotcontent != null)
    //            {
    //                content.ParkingName = parklotcontent.parkName;
    //                content.ParkingCode = parklotcontent.parkCode;
    //            }
    //            DrivewayModel drivewaycontent = drivewaymanager.GetDrivewayByMacAddress(model.DriveWayMAC);
    //            if (drivewaycontent != null)
    //            {
    //                content.Exit = drivewaycontent.drivewayName;
    //                try
    //                {
    //                    IDatabase db = FollowRedisHelper.GetDatabase(GetDatabaseNumber(model.CarNo));
    //                    bool flag = db.HashDelete(content.CarNo, content.ParkingCode);
    //                    //存储成功后在1号db移除该在场车牌,在2号db移除待广播的车牌(如果未广播的情况下)
    //                    //3号db缓存半小时该出场数据
    //                    if (flag)
    //                    {
    //                        //1号
    //                        db = FollowRedisHelper.GetDatabase(1);
    //                        db.HashDelete(content.ParkingCode, content.CarNo);
    //                        //2号
    //                        db = FollowRedisHelper.GetDatabase(1);
    //                        db.HashDelete(content.ParkingCode, content.CarNo);
    //                        //3号
    //                        db = FollowRedisHelper.GetDatabase(3);
    //                        string key = string.Format("{0}_{1}", drivewaycontent.guid, model.CarNo);
    //                        db.StringSet(key, m_serializer.Serialize(model), TimeSpan.FromSeconds(0.5 * 3600));
    //                        return true;
    //                    }else
    //                    {
    //                        return false;
    //                    }
    //                }
    //                catch (Exception ex)
    //                {
    //                    m_logger.LogFatal(LoggerLogicEnum.Bussiness, "", content.ParkingCode, "", "Fujica.com.cn.Business.Parking.VehicleInOutManager.Exit", "在redis移除停车数据异常", ex.ToString());
    //                    return false;
    //                }
    //            }
    //        }
    //        return false;
    //    }

    //    /// <summary>
    //    /// 获取在场车辆列表
    //    /// </summary>
    //    /// <param name="parklotcode">车场编码</param>
    //    /// <returns></returns>
    //    public List<PresenceOfVehicleModel> AllPresenceOfVehicle(string parklotcode)
    //    {
    //        List<PresenceOfVehicleModel> result = new List<PresenceOfVehicleModel>();
    //        try
    //        {
    //            IDatabase db = FollowRedisHelper.GetDatabase(1); //在场车辆缓存索引
    //            string[] allCarNo = db.HashKeys(parklotcode).ToStringArray();
    //            foreach (var carNo in allCarNo)
    //            {
    //                db = FollowRedisHelper.GetDatabase(GetDatabaseNumber(carNo));
    //                VehicleEntryDetailModel model = m_serializer.Deserialize<VehicleEntryDetailModel>(db.HashGet(carNo, parklotcode));
    //                if (model != null)
    //                {
    //                    if (model != default(VehicleEntryDetailModel))
    //                    {
    //                        TimeSpan stoptime = DateTime.Now - model.BeginTime;
    //                        result.Add(new PresenceOfVehicleModel()
    //                        {
    //                            parkCode = model.ParkingCode,
    //                            parkName = model.ParkingName,
    //                            guid = model.RecordGuid,
    //                            entranceName = model.Entrance,
    //                            carNo = model.CarNo,
    //                            inImgUrl = model.InImgUrl,
    //                            inTime = model.BeginTime,
    //                            stopLong = string.Format("{0}天{1}小时{2}分钟", stoptime.Days, stoptime.Hours, stoptime.Minutes),
    //                            carTypeGuid = model.CarTypeGuid,
    //                            carTypeName = model.CarTypeName,
    //                            remark = model.Description
    //                        });
    //                    }
    //                }
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            m_logger.LogFatal(LoggerLogicEnum.Bussiness, "", parklotcode, "", "Fujica.com.cn.Business.Parking.VehicleInOutManager.ALlPresenceOfVehicle", "从redis获取在场车数据异常", ex.ToString());
    //        }
    //        return result;
    //    }

    //    /// <summary>
    //    /// 获取入场车辆原始数据列表
    //    /// </summary>
    //    /// <param name="parkingCode"></param>
    //    /// <returns></returns>
    //    public Dictionary<string,string> AllPresenceOfVehicleByOriginal(string parkingCode)
    //    {
    //        Dictionary<string, string> dic = null;
    //        try
    //        {
    //            IDatabase db = FollowRedisHelper.GetDatabase(2);
    //            HashEntry[] hashResult = db.HashGetAll(parkingCode);
    //            if (hashResult != null && hashResult.Length > 1)
    //            {
    //                dic = new Dictionary<string, string>();
    //                foreach (var item in hashResult)
    //                {
    //                    dic.Add(item.Name, item.Value);
    //                }
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            m_logger.LogFatal(LoggerLogicEnum.Bussiness, "", parkingCode, "", "Fujica.com.cn.Business.Parking.VehicleInOutManager.AllPresenceOfVehicleByOriginal", "从redis获取原始入场数据异常", ex.ToString());
    //        }
    //        return dic;
    //    }

    //    /// <summary>
    //    /// 车道半小时内过的车
    //    /// </summary>
    //    /// <returns></returns>
    //    public List<VehicleInOutModel> DriveWayHalfHourCarsRecord(string drivewayguid)
    //    {
    //        List<VehicleInOutModel> result = new List<VehicleInOutModel>();
    //        try
    //        {
    //            IDatabase db = FollowRedisHelper.GetDatabase(3); //通过车道的半小时数据的db
    //            IServer srv = FollowRedisHelper.GetCurrentServer();
    //            IEnumerable<RedisKey> allRecordKey = srv.Keys(3);

    //            foreach (var RecordKey in allRecordKey)
    //            {
    //                if (RecordKey.ToString().IndexOf(drivewayguid) > -1)
    //                {
    //                    VehicleInOutModel content = m_serializer.Deserialize<VehicleInOutModel>(db.StringGet(RecordKey));
    //                    if (content != null) result.Add(content);
    //                }
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            m_logger.LogFatal(LoggerLogicEnum.Bussiness, "", "", "", "Fujica.com.cn.Business.Parking.VehicleInOutManager.DriveWayHalfHourCarsRecord", "从redis获取车道半小时数据异常", ex.ToString());
    //        }
    //        return result;
    //    }

    //    /// <summary>
    //    /// 获取某车的进场记录
    //    /// </summary>
    //    /// <param name="carno"></param>
    //    /// <returns></returns>
    //    public VehicleInOutModel GetEntryRecord(string carno,string parkcode)
    //    {
    //        VehicleInOutModel result = new VehicleInOutModel();
    //        try
    //        {
    //            IDatabase db = FollowRedisHelper.GetDatabase(GetDatabaseNumber(carno));

    //            VehicleEntryDetailModel content = m_serializer.Deserialize<VehicleEntryDetailModel>(db.HashGet(carno, parkcode));
    //            VehicleInOutModel inmodel = new VehicleInOutModel()
    //            {
    //                Guid = content.RecordGuid,
    //                CarNo = content.CarNo,
    //                CarTypeGuid = content.CarTypeGuid,
    //                EventTime = content.BeginTime,
    //                ImgUrl = content.InImgUrl,
    //                DriveWayMAC = content.DriveWayMAC,
    //                Remark = content.Description
    //            };
    //            if (content != null) result = inmodel;
    //        }
    //        catch (Exception ex)
    //        {
    //            m_logger.LogFatal(LoggerLogicEnum.Bussiness, "", "", "", "Fujica.com.cn.Business.Parking.VehicleInOutManager.GetRecord", "从redis获取进场数据异常", ex.ToString());
    //        }
    //        return result;
    //    }

    //    /// <summary>
    //    /// 散列车辆在场数据到不同db(10-15)
    //    /// </summary>
    //    /// <returns></returns>
    //    private int GetDatabaseNumber(string carNo)
    //    {
    //        byte[] byts = Encoding.ASCII.GetBytes(carNo);
    //        int length = byts.Length;
    //        int sum = 0;
    //        for (int i = 0; i < length; i++)
    //        {
    //            sum += byts[i];
    //        }
    //        return (sum % 6) + 10; //6代表分配到6个db里,10代表从第10个db开始
    //    }
    //}
}