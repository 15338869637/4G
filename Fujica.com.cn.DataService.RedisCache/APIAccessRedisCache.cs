using Fujica.com.cn.Context.Model;
using Fujica.com.cn.Logger;
using Fujica.com.cn.Tools;
using Fujica.com.cn.BaseConnect;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.DataService.RedisCache
{
    /// <summary>
    /// api接入控制缓存
    /// </summary>
    public class APIAccessRedisCache : IBaseRedisOperate<APIAccessModel>
    {
        /// <summary>
        /// 日志记录器
        /// </summary>
        internal readonly ILogger m_logger;

        /// <summary>
        /// 序列化器
        /// </summary>
        internal readonly ISerializer m_serializer = null;

        public APIAccessRedisCache(ILogger _logger, ISerializer _serializer)
        {
            m_logger = _logger;
            m_serializer = _serializer;
        }

        public APIAccessModel model { get; set; }

        public bool DeleteInRedis()
        {
            try
            {
                IDatabase db = RedisHelper.GetDatabase(0);
                return db.HashDelete("ApiAccessList", model.AppID);
            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.DataService, "", "", "", "Fujica.com.cn.DataService.RedisCache.APIAccessRedisCache.DeleteInRedis", "删除接入信息异常", ex.ToString());
                return false;
            }
        }

        public IList<APIAccessModel> GetAllFromRedis()
        {
            throw new NotImplementedException();
        }

        public APIAccessModel GetFromRedis()
        {
            try
            {
                IDatabase db = RedisHelper.GetDatabase(0);
                this.model = m_serializer.Deserialize<APIAccessModel>(db.HashGet("ApiAccessList", model.AppID));
                return this.model;
            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.DataService,"","","", "Fujica.com.cn.DataService.RedisCache.APIAccessRedisCache.GetFromRedis", "获取接入信息异常", ex.ToString());
                return null;
            }
        }

        public bool SaveToRedis()
        {
            try
            {
                IDatabase db = RedisHelper.GetDatabase(0);
                return db.HashSet("ApiAccessList", model.AppID, m_serializer.Serialize(model));
            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.DataService, "", "", "", "Fujica.com.cn.DataService.RedisCache.APIAccessRedisCache.SaveToRedis", "保存停车场异常", ex.ToString());
                return false;
            }
        }
    }
}
