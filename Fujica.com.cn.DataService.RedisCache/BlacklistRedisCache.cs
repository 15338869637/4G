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
    /// 黑名单对象缓存
    /// </summary>
    public class BlacklistRedisCache : IBaseRedisOperate<BlacklistModel>
    {
        /// <summary>
        /// 日志记录器
        /// </summary>
        internal readonly ILogger m_logger;

        /// <summary>
        /// 序列化器
        /// </summary>
        internal readonly ISerializer m_serializer = null;

        public BlacklistRedisCache(ILogger _logger, ISerializer _serializer)
        {
            m_logger = _logger;
            m_serializer = _serializer;
        }

        public BlacklistModel model
        {
            get;
            set;
        }

        public bool DeleteInRedis()
        {
            try
            {
                IDatabase db = RedisHelper.GetDatabase(0);
                BlacklistModel content = m_serializer.Deserialize<BlacklistModel>(db.HashGet("BlackLister", model.ParkCode));
                if (content == null) return true;
                foreach (var item in model.List)
                {
                    content.List.Remove(content.List.Find(o => o.CarNo == item.CarNo));
                }
                return db.HashSet("BlackLister", model.ParkCode, m_serializer.Serialize(content));
            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.DataService, "", "", "", "Fujica.com.cn.DataService.RedisCache.BlacklistRedisCache.DeleteInRedis", "删除黑名单异常", ex.ToString());
                return false;
            }
        }

        public IList<BlacklistModel> GetAllFromRedis()
        {
            throw new NotImplementedException();
        }

        public BlacklistModel GetFromRedis()
        {
            try
            {
                IDatabase db = RedisHelper.GetDatabase(0);
                this.model = m_serializer.Deserialize<BlacklistModel>(db.HashGet("BlackLister", model.ParkCode));
                return this.model;
            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.DataService, "", "", "", "Fujica.com.cn.DataService.RedisCache.BlacklistRedisCache.GetFromRedis", "获取黑名单异常", ex.ToString());
                return null;
            }
        }

        public bool SaveToRedis()
        {
            try
            {
                IDatabase db = RedisHelper.GetDatabase(0);
                BlacklistModel content = m_serializer.Deserialize<BlacklistModel>(db.HashGet("BlackLister", model.ParkCode));
                if (content == null) content = new BlacklistModel() { ParkCode = model.ParkCode, ProjectGuid = model.ProjectGuid };
                foreach (var item in model.List)
                {
                    content.List.Remove(content.List.Find(o => o.CarNo == item.CarNo)); //移除可能存在的
                    content.List.Add(item);
                }
                return db.HashSet("BlackLister", content.ParkCode, m_serializer.Serialize(content));
            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.DataService, "", "", "", "Fujica.com.cn.DataService.RedisCache.BlacklistRedisCache.SaveToRedis", "保存黑名单异常", ex.ToString());
                return false;
            }
        }
    }
}
