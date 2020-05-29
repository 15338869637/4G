using Fujica.com.cn.Context.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.Context.IContext
{

    /// <summary>
    /// 车辆进出管理
    /// </summary>
    public interface ICarInOutContext
    { 
        /// <summary>
        /// 车道闸口锁定
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool SetGateKeep(GateKeepListModel model);

        /// <summary>
        /// 读取车道闸口锁定集合
        /// </summary>
        /// <param name="parkingCode">停车场编码</param>
        /// <returns></returns>
        GateKeepListModel GetGateKeep(string parkingCode);

        /// <summary>
        /// 获取车道拦截异常数据
        /// </summary>
        /// <param name="devicemacaddress">车道相机标识</param>
        /// <returns></returns>
        GateCatchDetailModel GetGateCatch(string devicemacaddress);

        /// <summary>
        /// 获取车道车辆进出抓拍数据
        /// </summary>
        /// <param name="parkingCode">车场编号</param>
        /// <param name="deviceMacAddress">车道相机标识</param>
        /// <returns></returns>
        CaptureInOutModel GetGateData(string parkingCode, string deviceMacAddress);

    }
}
