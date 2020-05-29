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
    /// 停车场对象缓存
    /// </summary>
    public class ParkLotRedisCache : IBaseRedisOperate<ParkLotModel>
    {
        /// <summary>
        /// 日志记录器
        /// </summary>
        internal readonly ILogger m_logger;

        /// <summary>
        /// 序列化器
        /// </summary>
        internal readonly ISerializer m_serializer = null;

        public ParkLotRedisCache(ILogger _logger, ISerializer _serializer)
        {
            m_logger = _logger;
            m_serializer = _serializer;
        }

        public ParkLotModel model
        {
            get;
            set;
        }

        public bool DeleteInRedis()
        {
            try
            {
                IDatabase db = RedisHelper.GetDatabase(0);
                return db.HashDelete("ParkLotList", model.ParkCode);
            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.DataService, "", "", "", "Fujica.com.cn.DataService.RedisCache.ParkLotRedisCache.DeleteInRedis", "删除停车场异常", ex.ToString());
                return false;
            }
        }

        public IList<ParkLotModel> GetAllFromRedis()
        {
            throw new NotImplementedException();
        }

        public ParkLotModel GetFromRedis()
        {
            try
            {
                IDatabase db = RedisHelper.GetDatabase(0);
                this.model = m_serializer.Deserialize<ParkLotModel>(db.HashGet("ParkLotList", model.ParkCode));
                return this.model;
            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.DataService, "", "", "", "Fujica.com.cn.DataService.RedisCache.ParkLotRedisCache.GetFromRedis", "获取停车场异常", ex.ToString());
                return null;
            }
        }

        public bool SaveToRedis()
        {
            try
            {
                IDatabase db = RedisHelper.GetDatabase(0);
                return db.HashSet("ParkLotList", model.ParkCode, m_serializer.Serialize(model));
            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.DataService, "", "", "", "Fujica.com.cn.DataService.RedisCache.ParkLotRedisCache.SaveToRedis", "保存停车场异常", ex.ToString());
                return false;
            }
        }
    }
}
