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
    /// 车道管理
    /// </summary>
    public class DrivewayContext : IBasicContext, IDrivewayContext
    {
        /// <summary>
        /// redis操作类
        /// </summary>
        private IBaseRedisOperate<DrivewayModel> _redisoperate = null; 
        /// <summary>
        /// 数据库操作类
        /// </summary>
        private IBaseDataBaseOperate<DrivewayModel> _databaseoperate = null;

        /// <summary>
        /// 车道连接状态 redis操作类
        /// </summary>
        private IBaseRedisOperate<DrivewayConnStatusModel> _redisOperateDrivewayConnStatus = null;

        /// <summary>
        /// 扩展的redis操作类
        /// </summary>
        private IExtenRedisOperate<DrivewayModel> _extenredisoperate = null;
        /// <summary>
        /// 扩展的数据库操作类
        /// </summary>
        private IExtenDataBaseOperate<DrivewayModel> _extendatabaseoperate = null; 

        public DrivewayContext(IBaseRedisOperate<DrivewayModel> redisoperate, IBaseDataBaseOperate<DrivewayModel> databaseoperate, IBaseRedisOperate<DrivewayConnStatusModel> redisOperateDrivewayConnStatus)
        {
            _redisoperate = redisoperate;
            _databaseoperate = databaseoperate;
            _extenredisoperate = redisoperate as IExtenRedisOperate<DrivewayModel>;
            _extendatabaseoperate = databaseoperate as IExtenDataBaseOperate<DrivewayModel>;
            _redisOperateDrivewayConnStatus = redisOperateDrivewayConnStatus;
        }
        /// <summary>
        /// 添加车道
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddDriveway(DrivewayModel model)
        {
            _redisoperate.model = model;
            bool flag = _databaseoperate.SaveToDataBase(model);
            if (flag)
            {
                flag = _redisoperate.SaveToRedis();
                if (flag)
                {
                    flag = _extenredisoperate.KeyLinkGuid(); //关联设备MAC与车道GUID
                    
                }
            }
            return flag;
        }

        /// <summary>
        /// 修改车道
        /// </summary>
        /// <param name="model"></param>
        /// <param name="oldDeviceMacAddress">修改前的相机mac地址，用于判断相机是否替换</param>
        /// <returns></returns>
        public bool ModifyDriveway(DrivewayModel model)
        {
            //读取旧的车道信息
            DrivewayModel oldModel = GetDriveway(model.Guid);
            string oldMacAddress = oldModel == null ? "" : oldModel.DeviceMacAddress;

            _redisoperate.model = model;
            bool flag = _databaseoperate.SaveToDataBase(model);
            if (flag)
            {
                //此时不需要判断redis是否成功，因为修改时redis一定会返回false
                _redisoperate.SaveToRedis();

                //当前的设备MAC与redis中保存的mac地址不一致，删除掉旧的mac地址
                if (oldMacAddress != model.DeviceMacAddress)
                {
                    //更新mac地址
                    _extenredisoperate.KeyLinkGuid();
                    if (!string.IsNullOrEmpty(oldMacAddress))
                    {
                        //删除旧的mac地址
                        model.DeviceMacAddress = oldMacAddress;
                        _extenredisoperate.KeyUnLinkGuid();
                    }
                }
            }
            return flag;
            
        }

        /// <summary>
        /// 删除车道
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool DeleteDriveway(DrivewayModel model)
        {
            _redisoperate.model = model;
            bool flag = _databaseoperate.DeleteInDataBase(model);
            if (flag)
            {
                flag = _extenredisoperate.KeyUnLinkGuid(); //解除关联设备MAC与车道GUID
                if (flag)
                {
                    flag = _redisoperate.DeleteInRedis();
                }
            }
            return flag; 
        }

        /// <summary>
        /// 根据车道类型获取车场的相机数量
        /// </summary>
        /// <param name="parkingCode">停车场编号</param>
        /// <param name="type">车道类型</param>
        /// <returns></returns>
        private int GetCameraCount(string parkingCode, DrivewayType type)
        {
            //停车场所有车道 
            List<DrivewayModel> list = AllDriveway(parkingCode);
            if (list == null)
                return 0;
            return list.Where(m => m.Type == type).Count();
        }

        /// <summary>
        /// 某车场的所有车道
        /// </summary>
        /// <param name="parkcode"></param>
        /// <returns></returns>
        public List<DrivewayModel> AllDriveway(string parkcode)
        {
            //批量数据都从数据库获取
            List<DrivewayModel> model = _databaseoperate.GetMostFromDataBase(parkcode) as List<DrivewayModel>;
            return model;
        }
        
        /// <summary>
        /// 获取某车道
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public DrivewayModel GetDriveway(string guid)
        {
            DrivewayModel model = null;
            _redisoperate.model = new DrivewayModel() { Guid = guid };
            model = _redisoperate.GetFromRedis();

            //从数据库读
            if (model == null)
            {
                model = _databaseoperate.GetFromDataBase(guid);
                //缓存到redis
                if (model != null)
                {
                    _redisoperate.model = model;
                    _redisoperate.SaveToRedis();
                }
            }

            return model;
        }

        /// <summary>
        /// 通过车道设备MAC地址获取车道实体
        /// </summary>
        /// <param name="macaddress"></param>
        /// <returns></returns>
        public DrivewayModel GetDrivewayByMacAddress(string macaddress)
        {
            DrivewayModel model = null;
            _redisoperate.model = new DrivewayModel() { DeviceMacAddress = macaddress };
            model = _extenredisoperate.GetModelByKey();

            //从数据库读
            if (model == null)
            {
                string Guid = _extendatabaseoperate.GetGuidByKey(macaddress);
                if (!string.IsNullOrWhiteSpace(Guid))
                {
                    model = GetDriveway(Guid);
                }
                if (model != null)
                {
                    _redisoperate.model = model;
                    _extenredisoperate.KeyLinkGuid();
                }
            } 
            return model;
        }


        /// <summary>
        /// 获取车场所有车道的连接状态
        /// </summary>
        /// <param name="parkingCode">停车场编号</param>
        /// <returns></returns>
        public IList<DrivewayConnStatusModel> AllDrivewayConnStatus(string parkingCode)
        {
            _redisOperateDrivewayConnStatus.model = new DrivewayConnStatusModel() { ParkingCode = parkingCode };
            return _redisOperateDrivewayConnStatus.GetAllFromRedis();
        }
    }
}
