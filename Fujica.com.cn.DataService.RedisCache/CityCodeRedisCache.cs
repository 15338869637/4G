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
    /// 城市区号对象缓存
    /// </summary>
    public class CityCodeRedisCache : IBaseRedisOperate<CityCodeModel>
    {
        /// <summary>
        /// 日志记录器
        /// </summary>
        internal readonly ILogger m_logger;

        /// <summary>
        /// 序列化器
        /// </summary>
        internal readonly ISerializer m_serializer = null;

        public CityCodeRedisCache(ILogger _logger, ISerializer _serializer)
        {
            m_logger = _logger;
            m_serializer = _serializer;
        }

        public CityCodeModel model
        {
            get;

            set;
        }

        public bool DeleteInRedis()
        {
            throw new NotImplementedException();
        }

        public IList<CityCodeModel> GetAllFromRedis()
        {
            throw new NotImplementedException();
        }

        public CityCodeModel GetFromRedis()
        {
            throw new NotImplementedException();
        }

        public bool SaveToRedis()
        {
            try
            {
                IDatabase db = RedisHelper.GetDatabase(0);
                //先检测是否有重复，重复则不添加
                bool codeExists = db.SetContains("CityCodeList", model.CodeID);
                if (codeExists) return false;
                return db.SetAdd("CityCodeList", model.CodeID);
            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.DataService, "", "", "", "Fujica.com.cn.DataService.RedisCache.CityCodeRedisCache.SaveToRedis", "保存城市区号异常", ex.ToString());
                return false;
            }
        }
    }
}
