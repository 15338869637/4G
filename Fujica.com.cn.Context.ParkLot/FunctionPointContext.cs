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
    /// 功能点管理器
    /// </summary>
    public class FunctionPointContext : IBasicContext, IFunctionPointContext
    { 

        /// <summary>
        /// 数据库操作类
        /// </summary>
        private IBaseDataBaseOperate<FunctionPointModel> databaseoperate = null;


        /// <summary>
        /// redis操作类
        /// </summary>
        private IBaseRedisOperate<FunctionPointModel> redisoperate = null;


        public FunctionPointContext(IBaseRedisOperate<FunctionPointModel> _redisoperate, IBaseDataBaseOperate<FunctionPointModel> _databaseoperate)
        {
            redisoperate = _redisoperate;
            databaseoperate = _databaseoperate;
        }
        /// <summary>
        /// 设置功能点
        /// </summary>
        /// <returns></returns>
        public bool SetFunctionPoint(FunctionPointModel model)
        {
            redisoperate.model = model;
            bool flag = databaseoperate.SaveToDataBase(model);
            if (!flag) return false;
            //此时不需要判断redis是否成功，因为修改时redis一定会返回false
            redisoperate.SaveToRedis();
            return flag;
        }

        /// <summary>
        /// 获取功能点
        /// </summary>
        /// <param name="parkingcode"></param>
        /// <returns></returns>
        public FunctionPointModel GetFunctionPoint(string projectguid, string parkcode)
        {
            FunctionPointModel model = null;
            redisoperate.model = new FunctionPointModel() { ParkCode = parkcode };
            model = redisoperate.GetFromRedis();

            //从数据库读
            if (model == null)
            {
                model = databaseoperate.GetFromDataBase(parkcode);
                if (model == null)
                {
                    //都没有的时候设置一个默认的
                    model = new FunctionPointModel()
                    {
                        ProjectGuid = projectguid,
                        ParkCode = parkcode,
                        PostponeMode = 0,
                        PastDueMode = 0,
                        MinDays = 0,
                        MinBalance = 0
                    }; 
                } 
            }

            return model;
        }


    }
}
