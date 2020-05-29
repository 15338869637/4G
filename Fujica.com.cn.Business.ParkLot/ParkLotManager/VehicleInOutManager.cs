/***************************************************************************************
 * *
 * *        File Name        : ParkLotManager.cs
 * *        Creator          : llp
 * *        Create Time      : 2019-09-21 
 * *        Remark           : 收费设置(模板)管理 业务逻辑层
 * *
 * *  Copyright (c) 深圳市富士智能系统有限公司.  All rights reserved. 
 * ***************************************************************************************/
using Fujica.com.cn.Business.Base;
using Fujica.com.cn.Context.Model;
using Fujica.com.cn.Logger;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fujica.com.cn.Business.ParkLot
{

    /// <summary>
    ///  车进出场管理器
    /// </summary> 
    /// <remarks>
    /// 2019.09.21: 新增注释信息. llp <br/> 
    /// 2019.09.23: 修改方法“SendAllPresenceOfVehicleByOriginal”的返回参数，数据为空也返回true. Ase <br/> 
    /// </remarks>  
    partial class ParkLotManager
    {
        ///// <summary>
        ///// 获取在场车辆列表
        ///// </summary>
        ///// <param name="parklotcode">车场编码</param>
        ///// <returns></returns>
        //public List<PresenceOfVehicleModel> AllPresenceOfVehicle(string parklotcode)
        //{
        //    List<PresenceOfVehicleModel> list = _iVehicleInOutContext.AllPresenceOfVehicle(parklotcode);
        //    return list;
        //}

        /// <summary>
        /// 获取所有入场车辆原始数据列表
        /// </summary>
        /// <param name="parkingCode"></param>
        /// <returns></returns>
        public List<VehicleInModel> AllPresenceOfVehicleByOriginal(string parkingCode)
        {
            List<VehicleInModel> list = _iVehicleInOutContext.AllPresenceOfVehicleByOriginal(parkingCode);
            return list;
        }
        /// <summary>
        /// 发送所有入场车辆原始数据
        /// </summary>
        /// <param name="parkingCode">停车场编码</param>
        /// <returns></returns>
        public bool SendAllPresenceOfVehicleByOriginal(string parkingCode,string deviceMACAddress)
        {
            try
            {
                List<VehicleInModel> list = AllPresenceOfVehicleByOriginal(parkingCode);
                if (list == null || list.Count <= 0) return true;

                EntryOriginalModel model = new EntryOriginalModel();
                model.DeviceIdentify = deviceMACAddress;

                //每次只发送50条，分批发完
                int itemCount = 50;
                for (int i = 0; i < Math.Ceiling((double)list.Count() / itemCount); i++)
                {
                    model.OriginalList = list.Skip(i * itemCount).Take(itemCount).ToList();
                    CommandEntity<EntryOriginalModel> entity = new CommandEntity<EntryOriginalModel>()
                    {
                        command = BussineCommand.SyncParking,
                        idMsg = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                        message = model
                    };

                    m_rabbitMQ.SendMessageForRabbitMQ("同步所有入场车辆原始数据", m_serializer.Serialize(entity), entity.idMsg, parkingCode);
                }
                return true;
            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.Bussiness, "", parkingCode, "", "Fujica.com.cn.Business.ParkLot.CardServiceManager.SendAllPresenceOfVehicleByOriginal", "下发同步所有入场车辆原始数据时发生异常", ex.ToString());
                return false;
            }
        }

        ///// <summary>
        ///// 车道半小时内过的车
        ///// </summary>
        ///// <returns></returns>
        //public List<VehicleInOutModel> DriveWayHalfHourCarsRecord(string drivewayguid, string parkcode, string carNo)
        //{
        //    List<VehicleInOutModel> list = _iVehicleInOutContext.DriveWayHalfHourCarsRecord(drivewayguid, carNo);

        //      List<CarTypeModel> content =AllCarType(parkcode); 
        //   // CarTypeModel content = AllCarType(parkcode);
        //    foreach (var item in list)
        //    {
        //        CarTypeModel cartypemode = content.Where(o => o.Guid == item.CarTypeGuid).FirstOrDefault();
        //        item.CarTypeName = cartypemode.CarTypeName;
        //    } 
        //    return list;
        //}

        /// <summary>
        /// 获取某车的进场记录
        /// </summary>
        /// <param name="carno"></param>
        /// <returns></returns>
        public VehicleInOutModel GetEntryRecord(string carno, string parkcode)
        {
            VehicleInOutModel model = _iVehicleInOutContext.GetEntryRecord(carno, parkcode);
            return model;
        }

    }
}
