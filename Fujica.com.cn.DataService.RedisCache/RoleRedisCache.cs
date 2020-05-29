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
    /// 角色权限缓存
    /// </summary>
    public class RoleRedisCache : IBaseRedisOperate<RolePermissionModel>
    {
        /// <summary>
        /// 日志记录器
        /// </summary>
        internal readonly ILogger m_logger;

        /// <summary>
        /// 序列化器
        /// </summary>
        internal readonly ISerializer m_serializer = null;

        public RoleRedisCache(ILogger _logger, ISerializer _serializer)
        {
            m_logger = _logger;
            m_serializer = _serializer;
        }

        public RolePermissionModel model
        {
            get;
            set;
        }

        public bool DeleteInRedis()
        {
            try
            {
                IDatabase db = RedisHelper.GetDatabase(0);
                return db.HashDelete("RoleList", model.Guid);
            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.DataService, "", "", "", "Fujica.com.cn.DataService.RedisCache.RoleRedisCache.DeleteInRedis", "删除角色异常", ex.ToString());
                return false;
            }
        }

        public IList<RolePermissionModel> GetAllFromRedis()
        {
            throw new NotImplementedException();
        }

        public RolePermissionModel GetFromRedis()
        {
            try
            {
                IDatabase db = RedisHelper.GetDatabase(0);
                this.model = m_serializer.Deserialize<RolePermissionModel>(db.HashGet("RoleList", model.Guid));
                return this.model;
            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.DataService, "", "", "", "Fujica.com.cn.DataService.RedisCache.RoleRedisCache.GetFromRedis", "获取角色异常", ex.ToString());
                return null;
            }
        }

        public bool SaveToRedis()
        {
            try
            {
                IDatabase db = RedisHelper.GetDatabase(0);
                return db.HashSet("RoleList", model.Guid, m_serializer.Serialize(model));
            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.DataService, "", "", "", "Fujica.com.cn.DataService.RedisCache.RoleRedisCache.SaveToRedis", "保存角色异常", ex.ToString());
                return false;
            }
        }
    }
}
