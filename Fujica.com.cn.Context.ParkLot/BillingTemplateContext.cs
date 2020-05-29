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
    /// 计费模板管理
    /// </summary>
    public class BillingTemplateContext : IBasicContext,IBillingTemplateContext
    {
        /// <summary>
        /// redis操作类
        /// </summary>
        private IBaseRedisOperate<BillingTemplateModel> redisoperate = null;

        /// <summary>
        /// 数据库操作类
        /// </summary>
        private IBaseDataBaseOperate<BillingTemplateModel> databaseoperate = null;

        public BillingTemplateContext(IBaseRedisOperate<BillingTemplateModel> _redisoperate, IBaseDataBaseOperate<BillingTemplateModel> _databaseoperate)
        {
            redisoperate = _redisoperate;
            databaseoperate = _databaseoperate;
        }

        /// <summary>
        /// 添加计费模板
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddBillingTemplate(BillingTemplateModel model)
        {
            redisoperate.model = model;
            bool flag = databaseoperate.SaveToDataBase(model);
            if (!flag) return false; //数据库不成功就不要往下执行了
            return redisoperate.SaveToRedis(); 
       
        }

        /// <summary>
        /// 修改计费模板
        /// </summary>
        /// <returns></returns>
        public bool ModifyBillingTemplate(BillingTemplateModel model)
        {
            redisoperate.model = model;
            bool flag = databaseoperate.SaveToDataBase(model);
            if (flag) redisoperate.SaveToRedis(); //此时不需要判断redis是否成功，因为修改时redis一定会返回false
            return flag;
        }

        /// <summary>
        /// 删除计费模板
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool DeleteBillingTemplate(BillingTemplateModel model)
        {
            redisoperate.model = model;
            bool flag = databaseoperate.DeleteInDataBase(model);
            if (!flag) return false; //数据库不成功就不要往下执行了
            return redisoperate.DeleteInRedis();
        }


        /// <summary>
        /// 获取某计费模板
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public BillingTemplateModel GetBillingTemplate(string carTypeGuid)
        {
            BillingTemplateModel model = null;
            redisoperate.model = new BillingTemplateModel() { CarTypeGuid = carTypeGuid };
            model = redisoperate.GetFromRedis(); 
            //从数据库读
            if (model == null)
            {
                model = databaseoperate.GetFromDataBase(carTypeGuid);
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
