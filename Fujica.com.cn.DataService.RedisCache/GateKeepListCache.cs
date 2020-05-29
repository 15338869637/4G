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
    /// 车道闸口锁定缓存
    /// </summary>
    public class GateKeepListCache : IBaseRedisOperate<GateKeepListModel>
    {
        /// <summary>
        /// 日志记录器
        /// </summary>
        internal readonly ILogger m_logger;

        /// <summary>
        /// 序列化器
        /// </summary>
        internal readonly ISerializer m_serializer = null;

        public GateKeepListCache(ILogger _logger, ISerializer _serializer)
        {
            m_logger = _logger;
            m_serializer = _serializer;
        }

        public GateKeepListModel model
        {
            get; set;
        }

        public bool DeleteInRedis()
        {
            return false;
        }

        public IList<GateKeepListModel> GetAllFromRedis()
        {
            throw new NotImplementedException();
        }

        public GateKeepListModel GetFromRedis()
        {
            try
            {
                IDatabase db = RedisHelper.GetDatabase(0);
                model = m_serializer.Deserialize<GateKeepListModel>(db.HashGet("GateKeepList", model.ParkingCode));
                return model;
            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.DataService, "", "", "", "Fujica.com.cn.DataService.RedisCache.GateKeepListModel.GetFromRedis", "获取车道闸口锁定异常", ex.ToString());
                return null;
            }
        }

        public bool SaveToRedis()
        {
            try
            {
                IDatabase db = RedisHelper.GetDatabase(0);
                return db.HashSet("GateKeepList", model.ParkingCode, m_serializer.Serialize(model));
            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.DataService, "", "", "", "Fujica.com.cn.DataService.RedisCache.GateKeepListModel.SaveToRedis", "保存车道闸口锁定异常", ex.ToString());
                return false;
            }
        }
    }
}
