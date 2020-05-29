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
    /// 车道连接状态缓存
    /// </summary>
    public class DrivewayConnStatusRedisCache : IBaseRedisOperate<DrivewayConnStatusModel>
    {
        /// <summary>
        /// 日志记录器
        /// </summary>
        internal readonly ILogger m_logger;

        /// <summary>
        /// 序列化器
        /// </summary>
        internal readonly ISerializer m_serializer = null;

        public DrivewayConnStatusRedisCache(ILogger _logger, ISerializer _serializer)
        {
            m_logger = _logger;
            m_serializer = _serializer;
        }

        public DrivewayConnStatusModel model
        {
            get;

            set;
        }

        public bool DeleteInRedis()
        {
            throw new NotImplementedException();
        }

        public IList<DrivewayConnStatusModel> GetAllFromRedis()
        {
            List<DrivewayConnStatusModel> list = null;
            try
            {
                IDatabase db = RedisHelper.GetDatabase(0);
                HashEntry[] hashResult = db.HashGetAll("HeartBeatList:" + model.ParkingCode);
                if (hashResult != null && hashResult.Length > 0)
                {
                    list = new List<DrivewayConnStatusModel>();
                    foreach (var item in hashResult)
                    {
                        list.Add(new DrivewayConnStatusModel() { ParkingCode = model.ParkingCode, DeviceMacAddress = item.Name, ConnDate = Convert.ToDateTime(item.Value) });
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.DataService, "", "", "", "Fujica.com.cn.DataService.RedisCache.DrivewayConnStatusRedisCache.GetAllFromRedis", "读取相机连接状态数据异常", ex.ToString());
                return list;
            }
        }

        public DrivewayConnStatusModel GetFromRedis()
        {
            throw new NotImplementedException();
        }

        public bool SaveToRedis()
        {
            throw new NotImplementedException();
        }
    }
}
