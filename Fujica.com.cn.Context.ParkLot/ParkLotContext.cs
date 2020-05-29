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
    public class ParkLotContext : IBasicContext, IParkLotContext
    {
        /// <summary>
        /// redis操作类
        /// </summary>
        private IBaseRedisOperate<ParkLotModel> redisoperate = null;

        /// <summary>
        /// 数据库操作类
        /// </summary>
        private IBaseDataBaseOperate<ParkLotModel> databaseoperate = null;
        /// <summary>
        ///补录数据 数据库操作类
        /// </summary>
        private IBaseDataBaseOperate<AddRecordModel> recorddatabaseoperate = null;
        /// <summary>
        ///车牌修正记录 数据库操作类
        /// </summary>
        private IBaseDataBaseOperate<CorrectCarnoModel> carnoRecorddatabaseoperate = null;
        public ParkLotContext(IBaseRedisOperate<ParkLotModel> _redisoperate, IBaseDataBaseOperate<ParkLotModel> _databaseoperate,
            IBaseDataBaseOperate<AddRecordModel> _recorddatabaseoperate,
            IBaseDataBaseOperate<CorrectCarnoModel> _carnoRecorddatabaseoperate)
        {
            redisoperate = _redisoperate;
            databaseoperate = _databaseoperate;
            recorddatabaseoperate = _recorddatabaseoperate;
            carnoRecorddatabaseoperate = _carnoRecorddatabaseoperate;
        }

        /// <summary>
        /// 添加车场
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddParkLot(ParkLotModel model)
        {
            redisoperate.model = model;
            bool flag = databaseoperate.SaveToDataBase(model);
            if (flag)
                flag = redisoperate.SaveToRedis();
            return flag;
        }


        /// <summary>
        /// 修改车场
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool ModifyParkLot(ParkLotModel model)
        {
            redisoperate.model = model;
            bool flag = databaseoperate.SaveToDataBase(model);
            if (flag)
                 redisoperate.SaveToRedis(); //此时不需要判断redis是否成功，因为修改时redis一定会返回false
             return flag;   
        }

        /// <summary>
        /// 注销车场
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool CancelParkLot(ParkLotModel model)
        {
            redisoperate.model = model;
            bool flag = databaseoperate.DeleteInDataBase(model);
            if (flag)
                flag = redisoperate.DeleteInRedis();  //此时不需要判断redis是否成功，因为修改时redis一定会返回false
             return flag;
        }

        /// <summary>
        /// 根据停车场编码获取停车场
        /// </summary>
        /// <param name="parkingCode">停车场编码</param>
        /// <returns>停车场实体</returns>
        public ParkLotModel GetParkLot(string parkingCode)
        {
            ParkLotModel model = null;
            redisoperate.model = new ParkLotModel() { ParkCode = parkingCode };
            model = redisoperate.GetFromRedis();

            //从数据库读
            if (model == null)
            {
                model = databaseoperate.GetFromDataBase(parkingCode);
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
        /// 根据项目编码获取所有停车场
        /// </summary>
        /// <param name="projectGuid">项目编码</param>
        /// <returns>所有停车场集合</returns>
        public List<ParkLotModel> GetParklotAll(string projectGuid)
        {
            //批量数据都从数据库获取
            List<ParkLotModel> list = databaseoperate.GetMostFromDataBase(projectGuid) as List<ParkLotModel>;
            return list;
        }
 

        /// <summary>
        /// 补录数据添加数据库
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddRecordToDatabase(AddRecordModel model)
        {
            return recorddatabaseoperate.SaveToDataBase(model);

        }

        /// <summary>
        /// 车牌修正 添加数据库
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddCarnoRecorddatabaseoperate(CorrectCarnoModel model)
        {
            return carnoRecorddatabaseoperate.SaveToDataBase(model);

        }

       

    }
}
