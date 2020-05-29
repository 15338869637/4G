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
    /// 其它设置数据缓存
    /// </summary>
    public class FunctionPointRedisCache : IBaseRedisOperate<FunctionPointModel>
    {
        /// <summary>
        /// 日志记录器
        /// </summary>
        internal readonly ILogger m_logger;

        /// <summary>
        /// 序列化器
        /// </summary>
        internal readonly ISerializer m_serializer = null;

        public FunctionPointRedisCache(ILogger _logger, ISerializer _serializer)
        {
            m_logger = _logger;
            m_serializer = _serializer;
        }

        public FunctionPointModel model
        {
            get;
            set;
        }

        public bool DeleteInRedis()
        {
            return false;
        }

        public IList<FunctionPointModel> GetAllFromRedis()
        {
            throw new NotImplementedException();
        }

        public FunctionPointModel GetFromRedis()
        {
            try
            {
                IDatabase db = RedisHelper.GetDatabase(0);
                this.model = m_serializer.Deserialize<FunctionPointModel>(db.HashGet("FunctionPointList", model.ParkCode));
                return this.model;
            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.DataService, "", "", "", "Fujica.com.cn.DataService.RedisCache.FunctionPointRedisCache.GetFromRedis", "获取其它设置功能点数据异常", ex.ToString());
                return null;
            }
        }

        public bool SaveToRedis()
        {
            try
            {
                IDatabase db = RedisHelper.GetDatabase(0);
                return db.HashSet("FunctionPointList", model.ParkCode, m_serializer.Serialize(model));
            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.DataService, "", "", "", "Fujica.com.cn.DataService.RedisCache.FunctionPointRedisCache.SaveToRedis", "保存其它设置功能点数据异常", ex.ToString());
                return false;
            }
        }
    }
}
