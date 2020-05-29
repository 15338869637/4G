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
    /// 收款账户缓存
    /// </summary>
    public class GatherAccountRedisCache : IBaseRedisOperate<GatherAccountModel>
    {
        /// <summary>
        /// 日志记录器
        /// </summary>
        internal readonly ILogger m_logger;

        /// <summary>
        /// 序列化器
        /// </summary>
        internal readonly ISerializer m_serializer = null;

        public GatherAccountRedisCache(ILogger _logger, ISerializer _serializer)
        {
            m_logger = _logger;
            m_serializer = _serializer;
        }

        public GatherAccountModel model
        {
            get;
            set;
        }

        public bool DeleteInRedis()
        {
            try
            {
                IDatabase db = RedisHelper.GetDatabase(0);
                return db.HashDelete("GatherAccountList", model.Guid);
            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.DataService, "", "", "", "Fujica.com.cn.DataService.RedisCache.GatherAccountRedisCache.DeleteInRedis", "删除收款账户异常", ex.ToString());
                return false;
            }
        }

        public IList<GatherAccountModel> GetAllFromRedis()
        {
            throw new NotImplementedException();
        }

        public GatherAccountModel GetFromRedis()
        {
            try
            {
                IDatabase db = RedisHelper.GetDatabase(0);
                this.model = m_serializer.Deserialize<GatherAccountModel>(db.HashGet("GatherAccountList", model.Guid));
                return this.model;
            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.DataService, "", "", "", "Fujica.com.cn.DataService.RedisCache.GatherAccountRedisCache.GetFromRedis", "获取收款账户异常", ex.ToString());
                return null;
            }
        }

        public bool SaveToRedis()
        {
            try
            {
                IDatabase db = RedisHelper.GetDatabase(0);
                return db.HashSet("GatherAccountList", model.Guid, m_serializer.Serialize(model));
            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.DataService, "", "", "", "Fujica.com.cn.DataService.RedisCache.GatherAccountRedisCache.SaveToRedis", "保存收款账户异常", ex.ToString());
                return false;
            }
        }
    }
}
