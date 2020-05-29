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
    public class CardServiceRedisCache : IBaseRedisOperate<CardServiceModel>
    {
        /// <summary>
        /// 日志记录器
        /// </summary>
        internal readonly ILogger m_logger;

        /// <summary>
        /// 序列化器
        /// </summary>
        internal readonly ISerializer m_serializer = null;

        public CardServiceRedisCache(ILogger _logger, ISerializer _serializer)
        {
            m_logger = _logger;
            m_serializer = _serializer;
        }

        public CardServiceModel model
        {
            get;
            set;
        }

        public bool DeleteInRedis()
        {
            try
            {

                IDatabase db = RedisHelper.GetDatabase(0);
                string hashkey = model.ParkCode + Convert.ToBase64String(Encoding.UTF8.GetBytes(model.CarNo));
                return db.HashDelete("PermanentCarList", hashkey);
            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.DataService, "", "", "", "Fujica.com.cn.DataService.RedisCache.CardServiceRedisCache.DeleteInRedis", "删除固定车数据异常", ex.ToString());
                return false;
            }
        }

        public IList<CardServiceModel> GetAllFromRedis()
        {
            throw new NotImplementedException();
        }

        public CardServiceModel GetFromRedis()
        {
            return null;
        }

        public bool SaveToRedis()
        {
            try
            {
                IDatabase db = RedisHelper.GetDatabase(0);
                CarTypeModel cartype = m_serializer.Deserialize<CarTypeModel>(db.HashGet("CarTypeList", model.CarTypeGuid));
                //暂不做固定车延期模板的验证（规则的匹配）
                //PermanentTemplateModel template= m_serializer.Deserialize<PermanentTemplateModel>(db.HashGet("PermanentTemplateList", model.CarTypeGuid));
                if (cartype.CarType == CarTypeEnum.MonthCar|| cartype.CarType == CarTypeEnum.VIPCar)
                {
                    //月卡车
                    string hashkey = model.ParkCode + Convert.ToBase64String(Encoding.UTF8.GetBytes(model.CarNo));
                    MonthCarModel monthcarmodel = new MonthCarModel()
                    {
                        ProjectGuid=model.ProjectGuid,
                        ParkCode=model.ParkCode,
                        CarOwnerName=model.CarOwnerName,
                        Mobile=model.Mobile,
                        CarNo=model.CarNo,
                        CarTypeGuid=model.CarTypeGuid,
                        //Cost= template.Amount,
                        //Units= template.Months,
                        Enable=model.Enable,
                        Locked=model.Locked,
                        StartDate=model.StartDate,
                        EndDate=model.EndDate,
                        DrivewayGuidList=model.DrivewayGuidList
                    };
                    return db.HashSet("PermanentCarList", hashkey, m_serializer.Serialize(monthcarmodel));
                }
                else if (cartype.CarType == CarTypeEnum.ValueCar)
                {
                    //储值车
                    string hashkey = model.ParkCode + Convert.ToBase64String(Encoding.UTF8.GetBytes(model.CarNo));
                    ValueCarModel monthcarmodel = new ValueCarModel()
                    {
                        ProjectGuid = model.ProjectGuid,
                        ParkCode = model.ParkCode,
                        CarOwnerName = model.CarOwnerName,
                        Mobile = model.Mobile,
                        CarNo = model.CarNo,
                        CarTypeGuid = model.CarTypeGuid,
                        Enable = model.Enable,
                        StartDate = model.StartDate,
                        Locked = model.Locked,
                        Balance=model.Balance,
                        DrivewayGuidList = model.DrivewayGuidList
                    };
                    return db.HashSet("PermanentCarList", hashkey, m_serializer.Serialize(monthcarmodel));
                }
                return false;
            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.DataService, "", "", "", "Fujica.com.cn.DataService.RedisCache.CardServiceRedisCache.SaveToRedis", "保存固定车数据异常", ex.ToString());
                return false;
            }
        }
    }
}
