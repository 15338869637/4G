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
    /// 停车场车位管理
    /// </summary>
    public class SpaceNumberContext : IBasicContext, ISpaceNumberContext
    { 
        /// <summary>
        /// redis操作类
        /// </summary>
        private IBaseRedisOperate<SpaceNumberModel> redisoperate = null;


        public SpaceNumberContext(IBaseRedisOperate<SpaceNumberModel> _redisoperate)
        {
            redisoperate = _redisoperate;
        }

        /// <summary>
        /// 获取车场的剩余车位数
        /// </summary>
        /// <param name="parkcode"></param>
        /// <returns></returns>
        public SpaceNumberModel GetRemainingSpace(string parkcode)
        {
            SpaceNumberModel model = null;
            redisoperate.model = new SpaceNumberModel() { ParkCode = parkcode };
            model = redisoperate.GetFromRedis(); 
            return model;
        }


        /// <summary>
        /// 设置剩余车位数
        /// </summary>
        /// <returns></returns>
        public bool SetRemainingSpace(SpaceNumberModel model)
        {
            redisoperate.model = model;
            bool redisResult = redisoperate.SaveToRedis(); 
            return redisResult;
        }


    }
}
