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
    /// 车道拦截对象缓存
    /// </summary>
    public class GateCatchRedisCache : IBaseRedisOperate<GateCatchDetailModel>
    {
        /// <summary>
        /// 日志记录器
        /// </summary>
        internal readonly ILogger m_logger;

        /// <summary>
        /// 序列化器
        /// </summary>
        internal readonly ISerializer m_serializer = null;

        public GateCatchRedisCache(ILogger _logger, ISerializer _serializer)
        {
            m_logger = _logger;
            m_serializer = _serializer;
        }

        public GateCatchDetailModel model
        {
            get;

            set;
        }

        public bool DeleteInRedis()
        {
            throw new NotImplementedException();
        }

        public IList<GateCatchDetailModel> GetAllFromRedis()
        {
            throw new NotImplementedException();
        }

        public GateCatchDetailModel GetFromRedis()
        {
            try
            {
                IDatabase db = RedisHelper.GetDatabase(0);
                this.model = m_serializer.Deserialize<GateCatchDetailModel>(db.HashGet("GateCatchList", model.DriveWayMAC));
                return this.model;
            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.DataService, "", "", "", "Fujica.com.cn.DataService.RedisCache.GateCatchRedisCache.GetFromRedis", "获取车道拦截异常", ex.ToString());
                return null;
            }
        }

        public bool SaveToRedis()
        {
            throw new NotImplementedException();
        }
    }
}
