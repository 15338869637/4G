using Fujica.com.cn.Context.IContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fujica.com.cn.Context.Model;
using Fujica.com.cn.DataService.RedisCache;
using Fujica.com.cn.DataService.DataBase;

namespace Fujica.com.cn.Context.ParkLot
{
    /// <summary>
    /// 车辆进出管理
    /// </summary>
    public class CarInOutContext : IBasicContext, ICarInOutContext
    { 
        /// <summary>
        /// 车道锁定redis操作类
        /// </summary>
        private IBaseRedisOperate<GateKeepListModel> _gateKeepRedisOperate = null;

        /// <summary>
        /// 车道拦截redis操作类
        /// </summary>
        private IBaseRedisOperate<GateCatchDetailModel> _gateCatchRedisOperate = null;

        /// <summary>
        /// 车道进出车辆数据redis操作类
        /// </summary>
        private IBaseRedisOperate<CaptureInOutModel> _gateDataRedisOperate = null;

        public CarInOutContext(IBaseRedisOperate<GateKeepListModel> redisoperate,IBaseRedisOperate<GateCatchDetailModel> gateCatchRedisOperate, IBaseRedisOperate<CaptureInOutModel> gateDataRedisOperate)
        {
            _gateKeepRedisOperate = redisoperate;
            _gateCatchRedisOperate = gateCatchRedisOperate;
            _gateDataRedisOperate = gateDataRedisOperate;
        }


        /// <summary>
        /// 车道闸口锁定
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool SetGateKeep(GateKeepListModel model)
        { 
            _gateKeepRedisOperate.model = model;
            //因修改的时候始终是false，所以暂不参考redis返回值
            _gateKeepRedisOperate.SaveToRedis(); 
            return true;
        }

        /// <summary>
        /// 读取车道闸口锁定集合
        /// </summary>
        /// <param name="parkingCode">停车场编码</param>
        /// <returns></returns>
        public GateKeepListModel GetGateKeep(string parkingCode)
        {
            GateKeepListModel model = null;
            _gateKeepRedisOperate.model = new GateKeepListModel() { ParkingCode = parkingCode };
            model = _gateKeepRedisOperate.GetFromRedis();
            return model;
        }

        /// <summary>
        /// 获取车道拦截异常数据
        /// </summary>
        /// <param name="devicemacaddress">车道相机标识</param>
        /// <returns></returns>
        public GateCatchDetailModel GetGateCatch(string devicemacaddress)
        {
            GateCatchDetailModel model = null;
            _gateCatchRedisOperate.model = new GateCatchDetailModel() { DriveWayMAC = devicemacaddress };
            model = _gateCatchRedisOperate.GetFromRedis();
            return model;
        }

        /// <summary>
        /// 获取车道车辆进出抓拍数据
        /// </summary>
        /// <param name="parkingCode">车场编号</param>
        /// <param name="deviceMacAddress">车道mac地址</param>
        /// <returns></returns>
        public CaptureInOutModel GetGateData(string parkingCode, string deviceMacAddress)
        {
            CaptureInOutModel model = null;
            _gateDataRedisOperate.model = new CaptureInOutModel() { ParkCode = parkingCode, DriveWayMAC = deviceMacAddress };
            model = _gateDataRedisOperate.GetFromRedis();
            return model;
        }

    }
}
