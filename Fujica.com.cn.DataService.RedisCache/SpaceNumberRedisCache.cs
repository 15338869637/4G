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
    /// 停车场车位对象缓存
    /// </summary>
    public class SpaceNumberRedisCache : IBaseRedisOperate<SpaceNumberModel>
    {
        /// <summary>
        /// 日志记录器
        /// </summary>
        internal readonly ILogger m_logger;

        /// <summary>
        /// 序列化器
        /// </summary>
        internal readonly ISerializer m_serializer = null;

        public SpaceNumberRedisCache(ILogger _logger, ISerializer _serializer)
        {
            m_logger = _logger;
            m_serializer = _serializer;
        }

        public SpaceNumberModel model
        {
            get;

            set;
        }

        public bool DeleteInRedis()
        {
            throw new NotImplementedException();
        }

        public IList<SpaceNumberModel> GetAllFromRedis()
        {
            throw new NotImplementedException();
        }

        public SpaceNumberModel GetFromRedis()
        {

            try
            {
                IDatabase db = RedisHelper.GetDatabase(0); 
                long spaceCount = db.ListLength("SpaceNumberList:" + model.ParkCode);
                model.RemainingSpace = Convert.ToUInt32(spaceCount);
                return model;
            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.DataService, "", "", "", "Fujica.com.cn.DataService.RedisCache.GateCatchRedisCache.GetFromRedis", "获取车场剩余车位数异常", ex.ToString());
                return null;
            }
              
            
        }

        public bool SaveToRedis()
        {
            try
            {
                IDatabase db = RedisHelper.GetDatabase(0);
                if (model != null && !string.IsNullOrEmpty(model.ParkCode))
                {
                    db.ListRemove("SpaceNumberList:" + model.ParkCode, 1);
                }

                RedisValue[] spaceValue = new RedisValue[model.RemainingSpace];
                for (int i = 0; i < model.RemainingSpace; i++)
                {
                    spaceValue[i] = 1;
                }
                long resultCount = db.ListRightPush("SpaceNumberList:"+ model.ParkCode, spaceValue);
                if (resultCount > 0) 
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.DataService, "", "", "", "Fujica.com.cn.DataService.RedisCache.SpaceNumberRedisCache.SaveToRedis", "保存剩余车位异常", ex.ToString());
                return false;
            }
        }
    }
}
