using Fujica.com.cn.Context.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.Context.IContext
{
    /// <summary>
    /// 车道管理
    /// </summary>
    public interface IDrivewayContext
    {
        /// <summary>
        /// 添加车道
        /// </summary>
        /// <returns></returns>
        bool AddDriveway(DrivewayModel model);

        /// <summary>
        /// 修改车道
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool ModifyDriveway(DrivewayModel model);

        /// <summary>
        /// 删除车道
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool DeleteDriveway(DrivewayModel model);

        /// <summary>
        /// 某车场的所有车道
        /// </summary>
        /// <param name="parkcode"></param>
        /// <returns></returns>
        List<DrivewayModel> AllDriveway(string parkcode);

        /// <summary>
        /// 获取车场所有车道的连接状态
        /// </summary>
        /// <param name="parkingCode">停车场编号</param>
        /// <returns>车道连接状态实体集合</returns>
        IList<DrivewayConnStatusModel> AllDrivewayConnStatus(string parkingCode);

        /// <summary>
        /// 获取某车道
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        DrivewayModel GetDriveway(string guid);

        /// <summary>
        /// 通过车道设备MAC地址获取车道实体
        /// </summary>
        /// <param name="macaddress"></param>
        /// <returns></returns>
        DrivewayModel GetDrivewayByMacAddress(string macaddress);

    }
        
}
