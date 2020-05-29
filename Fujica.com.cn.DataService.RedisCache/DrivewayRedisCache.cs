using Fujica.com.cn.BaseConnect;
using Fujica.com.cn.Context.Model;
using Fujica.com.cn.Logger;
using Fujica.com.cn.Tools;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.DataService.RedisCache
{
    /// <summary>
    /// 车道数据缓存
    /// </summary>
    public class DrivewayRedisCache : IBaseRedisOperate<DrivewayModel>, IExtenRedisOperate<DrivewayModel>
    {
        /// <summary>
        /// 日志记录器
        /// </summary>
        internal readonly ILogger m_logger;

        /// <summary>
        /// 序列化器
        /// </summary>
        internal readonly ISerializer m_serializer = null;

        public DrivewayRedisCache(ILogger _logger, ISerializer _serializer)
        {
            m_logger = _logger;
            m_serializer = _serializer;
        }

        public DrivewayModel model
        {
            get;
            set;
        }

        public bool DeleteInRedis()
        {
            try
            {
                IDatabase db = RedisHelper.GetDatabase(0);
                return db.HashDelete("DrivewayList", model.Guid);
            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.DataService, "", "", "", "Fujica.com.cn.DataService.RedisCache.DrivewayRedisCache.DeleteInRedis", "删除车道数据异常", ex.ToString());
                return false;
            }
        }

        public DrivewayModel GetFromRedis()
        {
            try
            {
                IDatabase db = RedisHelper.GetDatabase(0);
                this.model = m_serializer.Deserialize<DrivewayModel>(db.HashGet("DrivewayList", model.Guid));
                return this.model;
            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.DataService, "", "", "", "Fujica.com.cn.DataService.RedisCache.DrivewayRedisCache.GetFromRedis", "获取车道数据异常", ex.ToString());
                return null;
            }
        }

        public bool SaveToRedis()
        {
            try
            {
                IDatabase db = RedisHelper.GetDatabase(0);
                return db.HashSet("DrivewayList", model.Guid, m_serializer.Serialize(model));
            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.DataService, "", "", "", "Fujica.com.cn.DataService.RedisCache.DrivewayRedisCache.SaveToRedis", "保存车道数据异常", ex.ToString());
                return false;
            }
        }

        public IList<DrivewayModel> GetAllFromRedis()
        {
            throw new NotImplementedException();
        }

        public DrivewayModel GetModelByKey()
        {
            try
            {
                IDatabase db = RedisHelper.GetDatabase(0);
                string guid = db.HashGet("DrivewayLinkMACList", model.DeviceMacAddress);
                if (string.IsNullOrEmpty(guid))
                    return null;
                this.model = m_serializer.Deserialize<DrivewayModel>(db.HashGet("DrivewayList", guid));
                return this.model;
            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.DataService, "", "", "", "Fujica.com.cn.DataService.RedisCache.DrivewayRedisCache.GetFromRedis", "通过key获取车道数据异常", ex.ToString());
                return null;
            }
        }

        public bool KeyLinkGuid()
        {
            try
            {
                IDatabase db = RedisHelper.GetDatabase(0);
                return db.HashSet("DrivewayLinkMACList", model.DeviceMacAddress, model.Guid);
            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.DataService, "", "", "", "Fujica.com.cn.DataService.RedisCache.DrivewayRedisCache.KeyLinkGuid", "车道设备关联车道GUID异常", ex.ToString());
                return false;
            }
        }

        public bool KeyUnLinkGuid()
        {
            try
            {
                IDatabase db = RedisHelper.GetDatabase(0);
                return db.HashDelete("DrivewayLinkMACList", model.DeviceMacAddress);
            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.DataService, "", "", "", "Fujica.com.cn.DataService.RedisCache.DrivewayRedisCache.KeyUnLinkGuid", "车道设备解除关联车道GUID异常", ex.ToString());
                return false;
            }
        }

    }
}
