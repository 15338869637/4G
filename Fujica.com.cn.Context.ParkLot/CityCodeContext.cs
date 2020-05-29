using Fujica.com.cn.Context.IContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fujica.com.cn.Context.Model;
using Fujica.com.cn.DataService.RedisCache;
using Fujica.com.cn.DataService.DataBase;

namespace Fujica.com.cn.Context.ParkLot
{

    /// <summary>
    /// 城市区号管理
    /// </summary>
    public class CityCodeContext : IBasicContext, ICityCodeContext
    { 
        /// <summary>
        /// redis操作类
        /// </summary>
        private IBaseRedisOperate<CityCodeModel> redisoperate = null;


        public CityCodeContext(IBaseRedisOperate<CityCodeModel> _redisoperate)
        {
            redisoperate = _redisoperate;
        }

        /// <summary>
        /// 添加城市
        /// </summary>
        /// <returns></returns>
        public bool AddCityCode(CityCodeModel model)
        {
            redisoperate.model = model;
            bool redisResult = redisoperate.SaveToRedis(); 
            return redisResult;
        }


    }
}
