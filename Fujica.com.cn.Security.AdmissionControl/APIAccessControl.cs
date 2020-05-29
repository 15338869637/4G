using Fujica.com.cn.Context.Model;
using Fujica.com.cn.DataService.DataBase;
using Fujica.com.cn.DataService.RedisCache;
using Fujica.com.cn.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.Security.AdmissionControl
{
    /// <summary>
    /// API接入控制
    /// </summary>
    public class APIAccessControl
    {
        /// <summary>
        /// 日志记录器
        /// </summary>
        private readonly ILogger Logger;

        /// <summary>
        /// redis操作类
        /// </summary>
        private IBaseRedisOperate<APIAccessModel> redisoperate = null;

        /// <summary>
        /// 数据库操作类
        /// </summary>
        private IBaseDataBaseOperate<APIAccessModel> databaseoperate = null;

        /// <summary>
        /// 工程名称,由命名空间名称以及类名组成,用于记录日志
        /// </summary>
        private const string const_projectName = "Fujica.com.cn.Security.AdmissionControl.APIAccessControl.";
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="enterpriseInfoContext"></param>
        /// <param name="logger">日志记录对象</param>
        /// <param name="toolContainer">公共工具类容器对象</param>
        public APIAccessControl(ILogger logger, IBaseRedisOperate<APIAccessModel> _redisoperate, IBaseDataBaseOperate<APIAccessModel> _databaseoperate)
        {
            Logger = logger;
            redisoperate = _redisoperate;
            databaseoperate = _databaseoperate;
        }

        public APIAccessModel Get(string appid)
        {
            APIAccessModel model = null;
            redisoperate.model = new APIAccessModel() { AppID = appid };
            model = redisoperate.GetFromRedis();

            //从数据库读
            if (model == null)
            {
                model = databaseoperate.GetFromDataBase(appid);
                //缓存到redis
                if (model != null)
                {
                    redisoperate.model = model;
                    redisoperate.SaveToRedis();
                }
            }
            return model;
        }
    }
}
