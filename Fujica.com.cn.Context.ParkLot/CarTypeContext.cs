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
    /// 车类管理
    /// </summary>
    public class CarTypeContext : IBasicContext, ICarTypeContext
    {
        /// <summary>
        /// redis操作类
        /// </summary>
        private IBaseRedisOperate<CarTypeModel> redisoperate = null;

        /// <summary>
        /// 数据库操作类
        /// </summary>
        private IBaseDataBaseOperate<CarTypeModel> databaseoperate = null;

        /// <summary>
        /// 数据库扩展操作类
        /// </summary>
        private IExtenDataBaseOperate<CarTypeModel> extendatabaseoperate = null;

        public CarTypeContext(IBaseRedisOperate<CarTypeModel> _redisoperate, IBaseDataBaseOperate<CarTypeModel> _databaseoperate)
        {
            redisoperate = _redisoperate;
            databaseoperate = _databaseoperate;
            extendatabaseoperate = databaseoperate as IExtenDataBaseOperate<CarTypeModel>;
        }

        /// <summary>
        /// 添加车类
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddCarType(CarTypeModel model)
        {
            redisoperate.model = model;
            bool flag = databaseoperate.SaveToDataBase(model);
            if (!flag) return false; //数据库不成功就不要往下执行了
            return redisoperate.SaveToRedis();
        }

        /// <summary>
        /// 修改车类
        /// </summary>
        /// <returns></returns>
        public bool ModifyCarType(CarTypeModel model)
        {
            redisoperate.model = model;
            bool flag = databaseoperate.SaveToDataBase(model);
            if (!flag) return false; //数据库不成功就不要往下执行了
            //此时不需要判断redis是否成功，因为修改时redis一定会返回false
            redisoperate.SaveToRedis();
            return true;
        }

        /// <summary>
        /// 删除车类
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool DeleteCarType(CarTypeModel model)
        {
            redisoperate.model = model;
            bool flag = databaseoperate.DeleteInDataBase(model);
            if (!flag) return false; //数据库不成功就不要往下执行了
            return redisoperate.DeleteInRedis();
        }


        /// <summary>
        /// 获取某车类
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public CarTypeModel GetCarType(string guid)
        {
            CarTypeModel model = null;
            redisoperate.model = new CarTypeModel() { Guid = guid };
            model = redisoperate.GetFromRedis();

            //从数据库读
            if (model == null)
            {
                model = databaseoperate.GetFromDataBase(guid);
                //缓存到redis
                if (model != null)
                {
                    redisoperate.model = model;
                    redisoperate.SaveToRedis();
                }
            }

            return model;
        }

        /// <summary>
        /// 某车场的所有车类
        /// </summary>
        /// <param name="parkcode"></param>
        /// <returns></returns>
        public List<CarTypeModel> AllCarType(string parkcode,string projectGuid)
        {
            //批量数据都从数据库获取
            string commandText = (string.Format("select * from t_cartype where parkCode='{0}'  and projectGuid='{1}'  order by carType ", parkcode, projectGuid));
            List<CarTypeModel> model = databaseoperate.GetMostFromDataBase(commandText) as List<CarTypeModel>;
            return model;
        }

        /// <summary>
        /// 设置默认车类（临时车用）
        /// </summary>
        /// <param name="guid">车类guid</param>
        /// <returns></returns>
        public bool SetDefaultCarType(string guid)
        {
            CarTypeModel model = GetCarType(guid);
            //设置默认值
            bool flag = extendatabaseoperate.ToggleValue(model);
            if (!flag)
                return false;
            //更新当前车场redis中所有的车类数据
            List<CarTypeModel> allmodel = AllCarType(model.ParkCode,model.ProjectGuid);
            foreach (var item in allmodel)
            {
                if (item.Guid == guid)
                {
                    item.DefaultType = true;
                }
                else
                {
                    item.DefaultType = false;
                }
                //批量保存到redis
                redisoperate.model = item;
                redisoperate.SaveToRedis();
                //此时不需要判断redis是否成功，因为修改时redis一定会返回false
                //if (!flag)
                //{
                //    break;
                //}
            }
            return flag;
        }

    }
}
