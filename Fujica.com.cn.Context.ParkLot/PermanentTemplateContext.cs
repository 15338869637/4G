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
    /// 收费设置(模板)管理
    /// </summary>
    public class PermanentTemplateContext : IBasicContext, IPermanentTemplateContext
    { 

        /// <summary>
        /// 数据库操作类
        /// </summary>
        private IBaseDataBaseOperate<PermanentTemplateModel> databaseoperate = null;


        /// <summary>
        /// redis操作类
        /// </summary>
        private IBaseRedisOperate<PermanentTemplateModel> redisoperate = null;


        public PermanentTemplateContext(IBaseRedisOperate<PermanentTemplateModel> _redisoperate, IBaseDataBaseOperate<PermanentTemplateModel> _databaseoperate)
        {
            redisoperate = _redisoperate;
            databaseoperate = _databaseoperate;
        }

        /// <summary>
        /// 设置固定车延期模板
        /// </summary>
        /// <returns></returns>
        public bool SetPermanentTemplate(PermanentTemplateModel model)
        {
            redisoperate.model = model;
            bool flag = databaseoperate.SaveToDataBase(model);
            if (flag)
            {
                //此时不需要判断redis是否成功，因为修改时redis一定会返回false
                redisoperate.SaveToRedis();
            }
            return flag;
        }

        
        /// <summary>
        /// 删除固定车延期模板
        /// </summary>
        /// <returns></returns>
        public bool DeletePermanentTemplate(PermanentTemplateModel model)
        {
            redisoperate.model = model;
            bool flag = databaseoperate.DeleteInDataBase(model);
            if (flag)
            {
                flag = redisoperate.DeleteInRedis();
            } 
            return flag;
        }

        /// <summary>
        /// 某车场所有固定车延期模板
        /// </summary>
        /// <param name="parkcode"></param>
        /// <returns></returns>
        public List<PermanentTemplateModel> AllPermanentTemplate(string parkcode)
        {
            //批量数据都从数据库获取
            List<PermanentTemplateModel> model = databaseoperate.GetMostFromDataBase(parkcode) as List<PermanentTemplateModel>;
            return model.OrderByDescending(a => a.OperateTime).ToList();//降序;
        }

        /// <summary>
        /// 获取固定车延期模板
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public PermanentTemplateModel GetPermanentTemplate(string cartypeguid)
        {
            PermanentTemplateModel model = null;
            redisoperate.model = new PermanentTemplateModel() { CarTypeGuid = cartypeguid };
            model = redisoperate.GetFromRedis();

            //从数据库读
            if (model == null)
            {
                model = databaseoperate.GetFromDataBase(cartypeguid);
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
