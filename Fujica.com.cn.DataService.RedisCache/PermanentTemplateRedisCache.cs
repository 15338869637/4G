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
    public class PermanentTemplateRedisCache : IBaseRedisOperate<PermanentTemplateModel>
    {
        /// <summary>
        /// 日志记录器
        /// </summary>
        internal readonly ILogger m_logger;

        /// <summary>
        /// 序列化器
        /// </summary>
        internal readonly ISerializer m_serializer = null;

        public PermanentTemplateRedisCache(ILogger _logger, ISerializer _serializer)
        {
            m_logger = _logger;
            m_serializer = _serializer;
        }

        public PermanentTemplateModel model
        {
            get;
            set;
        }

        public bool DeleteInRedis()
        {
            try
            {
                IDatabase db = RedisHelper.GetDatabase(0);
                return db.HashDelete("PermanentTemplateList", model.CarTypeGuid);
            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.DataService, "", "", "", "Fujica.com.cn.DataService.RedisCache.PermanentTemplateRedisCache.DeleteInRedis", "删除固定车模板异常", ex.ToString());
                return false;
            }
        }

        public IList<PermanentTemplateModel> GetAllFromRedis()
        {
            throw new NotImplementedException();
        }

        public PermanentTemplateModel GetFromRedis()
        {
            try
            {
                IDatabase db = RedisHelper.GetDatabase(0);
                this.model = m_serializer.Deserialize<PermanentTemplateModel>(db.HashGet("PermanentTemplateList", model.CarTypeGuid));
                return this.model;
            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.DataService, "", "", "", "Fujica.com.cn.DataService.RedisCache.PermanentTemplateRedisCache.GetFromRedis", "获取固定车模板异常", ex.ToString());
                return null;
            }
        }

        public bool SaveToRedis()
        {
            try
            {
                IDatabase db = RedisHelper.GetDatabase(0);
                return db.HashSet("PermanentTemplateList", model.CarTypeGuid, m_serializer.Serialize(model));
            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.DataService, "", "", "", "Fujica.com.cn.DataService.RedisCache.PermanentTemplateRedisCache.SaveToRedis", "保存固定车模板异常", ex.ToString());
                return false;
            }
        }
    }
}
