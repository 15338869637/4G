using Fujica.com.cn.Context.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.Context.IContext
{

    /// <summary>
    /// 车进出场类
    /// </summary>
    public interface IVehicleInOutContext
    { 
        ///// <summary>
        ///// 获取在场车辆列表
        ///// </summary>
        ///// <param name="parklotcode">车场编码</param>
        ///// <returns></returns>
        //List<PresenceOfVehicleModel> AllPresenceOfVehicle(string parklotcode);

        /// <summary>
        /// 获取所有入场车辆原始数据列表
        /// </summary>
        /// <param name="parkingCode"></param>
        /// <returns></returns>
        List<VehicleInModel> AllPresenceOfVehicleByOriginal(string parkingCode);

        ///// <summary>
        ///// 车道半小时内过的车
        ///// </summary>
        ///// <returns></returns>
        //List<VehicleInOutModel> DriveWayHalfHourCarsRecord(string drivewayguid, string carNo);

        /// <summary>
        /// 获取某车的进场记录
        /// </summary>
        /// <param name="carno"></param>
        /// <returns></returns>
        VehicleInOutModel GetEntryRecord(string carno, string parkcode);

        /// <summary>
        /// 散列车辆在场数据到不同db(10-15)
        /// </summary>
        /// <returns></returns>
        int GetDatabaseNumber(string carNo);
    }
}
