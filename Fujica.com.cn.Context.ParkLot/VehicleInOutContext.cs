using Fujica.com.cn.Context.IContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fujica.com.cn.Context.Model;
using Fujica.com.cn.DataService.RedisCache;
using Fujica.com.cn.DataService.DataBase;
using Fujica.com.cn.BaseConnect; 
using Fujica.com.cn.Business.Model;
using Fujica.com.cn.Tools;
using StackExchange.Redis;

namespace Fujica.com.cn.Context.ParkLot
{

    /// <summary>
    /// 车进出场类
    /// </summary>
    public class VehicleInOutContext : IBasicContext, IVehicleInOutContext
    { 
        /// <summary>
        /// 序列化器
        /// </summary>
        internal readonly ISerializer m_serializer = null;


        public VehicleInOutContext(ISerializer _serializer)
        {
            m_serializer = _serializer;
        }
 
 
        ///// <summary>
        ///// 获取在场车辆列表
        ///// </summary>
        ///// <param name="parklotcode">车场编码</param>
        ///// <returns></returns>
        //public List<PresenceOfVehicleModel> AllPresenceOfVehicle(string parklotcode)
        //{
        //    List<PresenceOfVehicleModel> result = new List<PresenceOfVehicleModel>();
        //    try
        //    {
        //        IDatabase db = FollowRedisHelper.GetDatabase(1); //在场车辆缓存索引
        //        string[] allCarNo = db.HashKeys(parklotcode).ToStringArray();
        //        foreach (var carNo in allCarNo)
        //        {
        //            db = FollowRedisHelper.GetDatabase(GetDatabaseNumber(carNo));
        //            VehicleEntryDetailModel model = m_serializer.Deserialize<VehicleEntryDetailModel>(db.HashGet(carNo, parklotcode)); 
        //            if (model != null)
        //            {
        //                if (model != default(VehicleEntryDetailModel))
        //                {
        //                    TimeSpan stoptime = DateTime.Now - model.BeginTime;
        //                    result.Add(new PresenceOfVehicleModel()
        //                    {
        //                        ParkCode = model.ParkingCode,
        //                        ParkName = model.ParkingName,
        //                        Guid = model.RecordGuid,
        //                        EntranceName = model.Entrance,
        //                        CarNo = model.CarNo,
        //                        InImgUrl = model.InImgUrl,
        //                        InTime = model.BeginTime,
        //                        StopLong = string.Format("{0}天{1}小时{2}分钟", stoptime.Days, stoptime.Hours, stoptime.Minutes),
        //                        CarTypeGuid = model.CarTypeGuid,
        //                        CarTypeName = model.CarTypeName,
        //                        Remark = model.Description
        //                    });
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result = null;
        //    }

        //    return result.OrderByDescending(a =>a.InTime).ToList();//降序
              
        //}

        /// <summary>
        /// 获取所有入场车辆原始数据列表
        /// </summary>
        /// <param name="parkingCode">停车场编号</param>
        /// <returns></returns>
        public List<VehicleInModel> AllPresenceOfVehicleByOriginal(string parkingCode)
        {
            List<VehicleInModel> list = new List<VehicleInModel>();
            try
            {
                //先读取SortedSet分页
                //3个月内的数据
                for (int i = 0; i < 3; i++)
                {
                    string hashKey = parkingCode + ":" + DateTime.Now.AddMonths(-i).ToString("yyyyMM");
                    int pageIndex = 1;
                    int pageSize = 100;
                    //每次只读取pageSize条
                    while (true)
                    {
                        IDatabase db = RedisHelper.GetDatabase(1);
                        //根据时间倒叙分页读取到车牌列表
                        RedisValue[] carNoList = db.SortedSetRangeByRank(hashKey, (pageIndex - 1) * pageSize, (pageIndex - 1) * pageSize + pageSize - 1, Order.Ascending);
                        if (carNoList.Count() > 0)
                        {
                            db = RedisHelper.GetDatabase(2);
                            //读取hash的实体数据
                            RedisValue[] jsonList = db.HashGet(hashKey, carNoList);
                            foreach (RedisValue item in jsonList)
                            {
                                if (!item.HasValue) continue;
                                list.Add(m_serializer.Deserialize<VehicleInModel>(item));
                            }
                        }
                        if (carNoList.Count() <= pageSize)
                            break;
                        pageIndex++;
                    }
                }
            }
            catch (Exception ex)
            {
                list = null;
            }
            return list;
        } 
        ///// <summary>
        ///// 车道半小时内过的车
        ///// </summary>
        ///// <returns></returns>
        //public List<VehicleInOutModel> DriveWayHalfHourCarsRecord(string drivewayguid, string carNo)
        //{
        //    List<VehicleInOutModel> result = new List<VehicleInOutModel>();
        //    try
        //    {
        //        IDatabase db = FollowRedisHelper.GetDatabase(3); //通过车道的半小时数据的db
        //        IServer srv = FollowRedisHelper.GetCurrentServer();
        //        IEnumerable<RedisKey> allRecordKey = srv.Keys(3);

        //        //改之前的

        //        //    foreach (var RecordKey in allRecordKey)
        //        //{
        //        //    if (RecordKey.ToString().IndexOf(drivewayguid) > -1)
        //        //    {
        //        //        VehicleInOutModel content = m_serializer.Deserialize<VehicleInOutModel>(db.StringGet(RecordKey));
        //        //        if (content != null) result.Add(content);
        //        //    }
        //        //} 

        //        //llp  改为可以兼容传输多个值
        //        string[] sArray = drivewayguid.Split(',');
        //        foreach (string i in sArray)
        //        {
        //            foreach (var RecordKey in allRecordKey)
        //            {
        //                if (RecordKey.ToString().IndexOf(i) > -1)
        //                {
        //                    VehicleInOutModel content = m_serializer.Deserialize<VehicleInOutModel>(db.StringGet(RecordKey));
        //                    if (content != null) result.Add(content);
        //                }
        //            }
        //        }
        //        if (result.Count > 0 && carNo != null)
        //        {
        //            result = result.FindAll(m => m.CarNo.Contains(carNo));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result = null;
        //    }

        //    return result.OrderByDescending(a => a.EventTime).ToList();//降序
        //}

        /// <summary>
        /// 获取某车的进场记录
        /// </summary>
        /// <param name="carno">车牌号码</param>
        /// <param name="parkingCode">停车场编号</param>
        /// <returns></returns>
        public VehicleInOutModel GetEntryRecord(string carno, string parkingCode)
        {
            VehicleInOutModel result = null;
            try
            {
                IDatabase db = RedisHelper.GetDatabase(GetDatabaseNumber(carno));

                VehicleEntryDetailModel content = m_serializer.Deserialize<VehicleEntryDetailModel>(db.HashGet(carno, parkingCode));
                if (content != null)
                {
                    result = new VehicleInOutModel()
                    {
                        Guid = content.RecordGuid,
                        CarNo = content.CarNo,
                        CarTypeGuid = content.CarTypeGuid,
                        EventTime = content.BeginTime,
                        ImgUrl = content.InImgUrl,
                        DriveWayMAC = content.DriveWayMAC,
                        Remark = content.Description,
                        Entrance = content.Entrance
                    };
                }
            }
            catch (Exception ex)
            {
            }
            return result;
        }

        /// <summary>
        /// 散列车辆在场数据到不同db(10-15)
        /// </summary>
        /// <returns></returns>
        public int GetDatabaseNumber(string carNo)
        {
            byte[] byts = Encoding.UTF8.GetBytes(carNo);
            int length = byts.Length;
            int sum = 0;
            for (int i = 0; i < length; i++)
            {
                sum += byts[i];
            }
            return (sum % 6) + 10; //6代表分配到6个db里,10代表从第10个db开始
        }

    }
}
