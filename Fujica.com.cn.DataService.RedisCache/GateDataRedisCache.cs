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
    /// 车道进出抓拍数据对象缓存
    /// </summary>
    public class GateDataRedisCache : IBaseRedisOperate<CaptureInOutModel>
    {
        /// <summary>
        /// 日志记录器
        /// </summary>
        internal readonly ILogger m_logger;

        /// <summary>
        /// 序列化器
        /// </summary>
        internal readonly ISerializer m_serializer = null;

        public GateDataRedisCache(ILogger logger, ISerializer serializer)
        {
            m_logger = logger;
            m_serializer = serializer;
        }
        public CaptureInOutModel model
        {
            get;

            set;
        }

        public bool DeleteInRedis()
        {
            throw new NotImplementedException();
        }

        public IList<CaptureInOutModel> GetAllFromRedis()
        {
            throw new NotImplementedException();
        }

        public CaptureInOutModel GetFromRedis()
        {
            try
            {
                IDatabase db = RedisHelper.GetDatabase(0);
                this.model = m_serializer.Deserialize<CaptureInOutModel>(db.HashGet("GateDataList:" + model.ParkCode, model.DriveWayMAC));
                return this.model;
            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.DataService, "", "", "", "Fujica.com.cn.DataService.RedisCache.GateDataRedisCache.GetFromRedis", "获取车道进出抓拍数据异常", ex.ToString());
                return null;
            }
        }

        public bool SaveToRedis()
        {
            throw new NotImplementedException();
        }
    }
}
