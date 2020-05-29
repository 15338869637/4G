using Fujica.com.cn.Context.IContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fujica.com.cn.Context.Model;
using Fujica.com.cn.DataService.RedisCache;
using Fujica.com.cn.DataService.DataBase;
using Fujica.com.cn.Tools;

namespace Fujica.com.cn.Context.ParkLot
{
    /// <summary>
    /// 卡务管理
    /// </summary>
    public class CardServiceContext : IBasicContext, ICardServiceContext
    {
        /// <summary>
        /// redis操作类
        /// </summary>
        private IBaseRedisOperate<CardServiceModel> _redisoperate = null;
     

        /// <summary>
        /// 月租车数据库操作类
        /// </summary>
        private IBaseDataBaseOperate<CardServiceModel> _monthCardDatabaseoperate = null;

        /// <summary>
        /// 储值车数据库操作类
        /// </summary>
        private IBaseDataBaseOperate<CardServiceModel> _valueCardDatabaseoperate = null;

        /// <summary>
        /// 临时车数据库操作类
        /// </summary>
        private IBaseDataBaseOperate<CardServiceModel> _tempCardDatabaseoperate = null;

        IExtenDataBaseOperate<CardServiceModel> _MonthCardExtendatabaseoperate = null;
        IExtenDataBaseOperate<CardServiceModel> _ValueCardExtendatabaseoperate = null;
        IExtenDataBaseOperate<CardServiceModel> _TempCardExtendatabaseoperate = null;


        public CardServiceContext(IBaseRedisOperate<CardServiceModel> redisoperate, IServiceGetter getter)
        {
            _redisoperate = redisoperate;
            _monthCardDatabaseoperate = getter.GetByName<IBaseDataBaseOperate<CardServiceModel>>("monthCard");
            _valueCardDatabaseoperate = getter.GetByName<IBaseDataBaseOperate<CardServiceModel>>("valueCard");
            _tempCardDatabaseoperate = getter.GetByName<IBaseDataBaseOperate<CardServiceModel>>("tempCard");
            _MonthCardExtendatabaseoperate = _monthCardDatabaseoperate as IExtenDataBaseOperate<CardServiceModel>;
            _ValueCardExtendatabaseoperate = _valueCardDatabaseoperate as IExtenDataBaseOperate<CardServiceModel>;
            _TempCardExtendatabaseoperate = _tempCardDatabaseoperate as IExtenDataBaseOperate<CardServiceModel>;   
        }

        /// <summary>
        /// 开卡
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddCard(CardServiceModel model, CarTypeEnum carType)
        {
            bool flag = false;
            switch (carType)
            {
                case CarTypeEnum.TempCar:
                    flag = _tempCardDatabaseoperate.SaveToDataBase(model);
                    break;
                case CarTypeEnum.MonthCar:
                    flag = _monthCardDatabaseoperate.SaveToDataBase(model);
                    break;
                case CarTypeEnum.ValueCar:
                    flag = _valueCardDatabaseoperate.SaveToDataBase(model);
                    break;
                case CarTypeEnum.VIPCar:
                    flag = _monthCardDatabaseoperate.SaveToDataBase(model);
                    break;
            }
            return flag;

            //if (!flag) return false; //数据库不成功就不要往下执行了
            //_redisoperate.model = model;
            //return _redisoperate.SaveToRedis();
        }

        /// <summary>
        /// 修改卡信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool ModifyCard(CardServiceModel model, CarTypeEnum carType)
        {
            bool flag = false;
            switch (carType)
            {
                case CarTypeEnum.TempCar:
                    flag = _tempCardDatabaseoperate.SaveToDataBase(model);
                    break;
                case CarTypeEnum.MonthCar:
                    flag = _monthCardDatabaseoperate.SaveToDataBase(model);
                    break;
                case CarTypeEnum.ValueCar:
                    flag = _valueCardDatabaseoperate.SaveToDataBase(model);
                    break;
                case CarTypeEnum.VIPCar:
                    flag = _monthCardDatabaseoperate.SaveToDataBase(model);
                    break;
            }
            //if (flag)
            //{
            //    _redisoperate.model = model;
            //    _redisoperate.SaveToRedis();
            //}
            return flag;
        }

        /// <summary>
        /// 锁定 禁止出场、入场
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool LockedCard(CardServiceModel model, CarTypeEnum carType)
        {
            model.Locked = true;

            bool flag = false;
            switch (carType)
            {
                case CarTypeEnum.TempCar:
                    flag = _TempCardExtendatabaseoperate.ToggleValue(model);
                    break;
                case CarTypeEnum.MonthCar:
                    flag = _MonthCardExtendatabaseoperate.ToggleValue(model);
                    break;
                case CarTypeEnum.ValueCar:
                    flag = _ValueCardExtendatabaseoperate.ToggleValue(model);
                    break;
                case CarTypeEnum.VIPCar:
                    flag = _MonthCardExtendatabaseoperate.ToggleValue(model);
                    break;
            }
            //if (flag)
            //{
            //    _redisoperate.model = model;
            //    _redisoperate.SaveToRedis();
            //}
            return flag;
        }

        /// <summary>
        /// 解锁 可以出场、入场
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UnLockedCard(CardServiceModel model, CarTypeEnum carType)
        {
            model.Locked = false;

            bool flag = false;
            switch (carType)
            {
                case CarTypeEnum.TempCar:
                    flag = _TempCardExtendatabaseoperate.ToggleValue(model);
                    break;
                case CarTypeEnum.MonthCar:
                    flag = _MonthCardExtendatabaseoperate.ToggleValue(model);
                    break;
                case CarTypeEnum.ValueCar:
                    flag = _ValueCardExtendatabaseoperate.ToggleValue(model);
                    break;
                case CarTypeEnum.VIPCar:
                    flag = _MonthCardExtendatabaseoperate.ToggleValue(model);
                    break;
            }
            //if (flag)
            //{
            //    _redisoperate.model = model;
            //    _redisoperate.SaveToRedis();
            //}
            return flag;

        }

        /// <summary>
        /// 注销 从数据库删除
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool DeleteCard(CardServiceModel model, CarTypeEnum carType)
        {
            //_redisoperate.model = model;
            bool flag = false;
            switch (carType)
            {
                case CarTypeEnum.TempCar:
                    flag = _tempCardDatabaseoperate.DeleteInDataBase(model);
                    break;
                case CarTypeEnum.MonthCar:
                    flag = _monthCardDatabaseoperate.DeleteInDataBase(model);
                    break;
                case CarTypeEnum.ValueCar:
                    flag = _valueCardDatabaseoperate.DeleteInDataBase(model);
                    break;
                case CarTypeEnum.VIPCar:
                    flag = _monthCardDatabaseoperate.DeleteInDataBase(model);
                    break;
            }
            //if (flag)
            //{
            //    flag = _redisoperate.DeleteInRedis(); //数据库不成功就不要往下执行了
            //}
            return flag;
        }

        /// <summary>
        /// 某车场的所有卡 调用时根据业务筛选月卡，储值卡
        /// </summary>
        /// <param name="parkcode"></param>
        /// <returns></returns>
        public List<CardServiceModel> AllTypeCard(string parkcode, CarTypeEnum carType)
        {
            //批量数据都从数据库获取 redis并不缓存此实体
            try
            {
                List<CardServiceModel> list = null;
                switch (carType)
                {
                    case CarTypeEnum.TempCar:
                        list = _tempCardDatabaseoperate.GetMostFromDataBase(parkcode) as List<CardServiceModel>;
                        break;
                    case CarTypeEnum.MonthCar:
                        list = _monthCardDatabaseoperate.GetMostFromDataBase(parkcode) as List<CardServiceModel>;
                        break;
                    case CarTypeEnum.ValueCar:
                        list = _valueCardDatabaseoperate.GetMostFromDataBase(parkcode) as List<CardServiceModel>;
                        break;
                    case CarTypeEnum.VIPCar:
                        list = _monthCardDatabaseoperate.GetMostFromDataBase(parkcode) as List<CardServiceModel>;
                        break;
                }
                if (list != null)
                    list = list.OrderByDescending(a => a.StartDate).ToList();//降序
                return list;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        /// <summary>
        /// 获取分页的数据
        /// </summary>
        /// <param name="model">查询条件实体</param>
        /// <param name="carType">查询的卡类</param>
        /// <returns></returns>
        public List<CardServiceModel> GetCardPage(CardServiceSearchModel model, CarTypeEnum carType)
        {
            try
            {
                List<CardServiceModel> list = null;
                switch (carType)
                {
                    case CarTypeEnum.TempCar:
                        list = _tempCardDatabaseoperate.GetFromDataBaseByPage(model) as List<CardServiceModel>;
                        break;
                    case CarTypeEnum.MonthCar:
                        list = _monthCardDatabaseoperate.GetFromDataBaseByPage(model) as List<CardServiceModel>;
                        break;
                    case CarTypeEnum.ValueCar:
                        list = _valueCardDatabaseoperate.GetFromDataBaseByPage(model) as List<CardServiceModel>;
                        break;
                    case CarTypeEnum.VIPCar:
                        list = _monthCardDatabaseoperate.GetFromDataBaseByPage(model) as List<CardServiceModel>;
                        break;
                }
                if (list != null)
                    list = list.OrderByDescending(a => a.StartDate).ToList();//降序
                return list;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取某卡
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public CardServiceModel GetCard(string carNo, string parkCode, CarTypeEnum carType)
        {
            try
            {
                CardServiceModel model = null;
                string key = parkCode + Convert.ToBase64String(Encoding.UTF8.GetBytes(carNo));
                switch (carType)
                {
                    case CarTypeEnum.TempCar:
                        model = _tempCardDatabaseoperate.GetFromDataBase(key);
                        break;
                    case CarTypeEnum.MonthCar:
                        model = _monthCardDatabaseoperate.GetFromDataBase(key);
                        break;
                    case CarTypeEnum.ValueCar:
                        model = _valueCardDatabaseoperate.GetFromDataBase(key);
                        break;
                    case CarTypeEnum.VIPCar:
                        model = _monthCardDatabaseoperate.GetFromDataBase(key);
                        break;
                }
                return model;
            }
            catch (Exception ex)
            {
                return null;
            }
        } 
 

    }

    ///// <summary>
    ///// 月卡管理
    ///// </summary>
    //public class MonthCardServiceContext : IBasicContext, ICardServiceContext
    //{
    //    /// <summary>
    //    /// redis操作类
    //    /// </summary>
    //    private IBaseRedisOperate<CardServiceModel> _redisoperate = null;

    //    /// <summary>
    //    /// 月租车数据库操作类
    //    /// </summary>
    //    private IBaseDataBaseOperate<CardServiceModel> _monthCardDatabaseoperate = null;

    //    public MonthCardServiceContext(IBaseRedisOperate<CardServiceModel> redisoperate, IServiceGetter getter)
    //    {
    //        _redisoperate = redisoperate;
    //        _monthCardDatabaseoperate = getter.GetByName<IBaseDataBaseOperate<CardServiceModel>>("monthCard");

    //    }

    //    /// <summary>
    //    /// 开卡
    //    /// </summary>
    //    /// <param name="model"></param>
    //    /// <returns></returns>
    //    public bool AddCard(CardServiceModel model)
    //    {
    //        bool flag = _monthCardDatabaseoperate.SaveToDataBase(model);
    //        if (!flag) return false; //数据库不成功就不要往下执行了
    //        _redisoperate.model = model;
    //        return _redisoperate.SaveToRedis();
    //    }

    //    /// <summary>
    //    /// 修改卡信息
    //    /// </summary>
    //    /// <param name="model"></param>
    //    /// <returns></returns>
    //    public bool ModifyCard(CardServiceModel model)
    //    {
    //        bool flag = _monthCardDatabaseoperate.SaveToDataBase(model);
    //        if (flag)
    //        {
    //            _redisoperate.model = model;
    //            _redisoperate.SaveToRedis();
    //        }
    //        return flag;
    //    }

    //    /// <summary>
    //    /// 锁定 禁止出场、入场
    //    /// </summary>
    //    /// <param name="model"></param>
    //    /// <returns></returns>
    //    public bool LockedCard(CardServiceModel model)
    //    {
    //        model.Locked = true;
    //        IExtenDataBaseOperate<CardServiceModel> extendatabaseoperate = _monthCardDatabaseoperate as IExtenDataBaseOperate<CardServiceModel>;
    //        bool flag = extendatabaseoperate.ToggleValue(model);
    //        if (flag)
    //        {
    //            _redisoperate.model = model;
    //            _redisoperate.SaveToRedis();
    //        }
    //        return flag;
    //    }

    //    /// <summary>
    //    /// 解锁 可以出场、入场
    //    /// </summary>
    //    /// <param name="model"></param>
    //    /// <returns></returns>
    //    public bool UnLockedCard(CardServiceModel model)
    //    {
    //        model.Locked = false;
    //        IExtenDataBaseOperate<CardServiceModel> extendatabaseoperate = _monthCardDatabaseoperate as IExtenDataBaseOperate<CardServiceModel>;
    //        bool flag = extendatabaseoperate.ToggleValue(model);
    //        if (flag)
    //        {
    //            _redisoperate.model = model;
    //            _redisoperate.SaveToRedis();
    //        }
    //        return flag;

    //    }

    //    /// <summary>
    //    /// 注销 从数据库删除
    //    /// </summary>
    //    /// <param name="model"></param>
    //    /// <returns></returns>
    //    public bool DeleteCard(CardServiceModel model)
    //    {
    //        _redisoperate.model = model;
    //        bool flag = _monthCardDatabaseoperate.DeleteInDataBase(model);
    //        if (flag)
    //        {
    //            flag = _redisoperate.DeleteInRedis(); //数据库不成功就不要往下执行了
    //        }
    //        return flag;
    //    }

    //    /// <summary>
    //    /// 某车场的所有卡 调用时根据业务筛选月卡，储值卡
    //    /// </summary>
    //    /// <param name="parkcode"></param>
    //    /// <returns></returns>
    //    public List<CardServiceModel> AllTypeCard(string parkcode)
    //    {
    //        //批量数据都从数据库获取 redis并不缓存此实体
    //        try
    //        {
    //            List<CardServiceModel> model = _monthCardDatabaseoperate.GetMostFromDataBase(parkcode) as List<CardServiceModel>;
    //            model = model.OrderByDescending(a => a.StartDate).ToList();//降序
    //            return model;
    //        }
    //        catch (Exception ex)
    //        {
    //            return null;
    //        }
    //    }

    //    /// <summary>
    //    /// 获取某卡
    //    /// </summary>
    //    /// <param name="guid"></param>
    //    /// <returns></returns>
    //    public CardServiceModel GetCard(string carNo, string parkCode)
    //    {
    //        try
    //        {  //只从数据库读，redis并不缓存此实体
    //            return _monthCardDatabaseoperate.GetFromDataBase(parkCode + Convert.ToBase64String(Encoding.UTF8.GetBytes(carNo)));
    //        }
    //        catch (Exception ex)
    //        {
    //            return null;
    //        }
    //    }

    //}

    ///// <summary>
    ///// 储值卡管理
    ///// </summary>
    //public class ValueCardServiceContext : IBasicContext, ICardServiceContext
    //{
    //    /// <summary>
    //    /// redis操作类
    //    /// </summary>
    //    private IBaseRedisOperate<CardServiceModel> _redisoperate = null;

    //    /// <summary>
    //    /// 储值车数据库操作类
    //    /// </summary>
    //    private IBaseDataBaseOperate<CardServiceModel> _valueCardDatabaseoperate = null;

    //    public ValueCardServiceContext(IBaseRedisOperate<CardServiceModel> redisoperate, IServiceGetter getter)
    //    {
    //        _redisoperate = redisoperate;
    //        _valueCardDatabaseoperate = getter.GetByName<IBaseDataBaseOperate<CardServiceModel>>("valueCard");

    //    }

    //    /// <summary>
    //    /// 开卡
    //    /// </summary>
    //    /// <param name="model"></param>
    //    /// <returns></returns>
    //    public bool AddCard(CardServiceModel model)
    //    {
    //        bool flag = _valueCardDatabaseoperate.SaveToDataBase(model);
    //        if (!flag) return false; //数据库不成功就不要往下执行了
    //        _redisoperate.model = model;
    //        return _redisoperate.SaveToRedis();
    //    }

    //    /// <summary>
    //    /// 修改卡信息
    //    /// </summary>
    //    /// <param name="model"></param>
    //    /// <returns></returns>
    //    public bool ModifyCard(CardServiceModel model)
    //    {
    //        bool flag = _valueCardDatabaseoperate.SaveToDataBase(model);
    //        if (flag)
    //        {
    //            _redisoperate.model = model;
    //            _redisoperate.SaveToRedis();
    //        }
    //        return flag;
    //    }

    //    /// <summary>
    //    /// 锁定 禁止出场、入场
    //    /// </summary>
    //    /// <param name="model"></param>
    //    /// <returns></returns>
    //    public bool LockedCard(CardServiceModel model)
    //    {
    //        model.Locked = true;
    //        IExtenDataBaseOperate<CardServiceModel> extendatabaseoperate = _valueCardDatabaseoperate as IExtenDataBaseOperate<CardServiceModel>;
    //        bool flag = extendatabaseoperate.ToggleValue(model);
    //        if (flag)
    //        {
    //            _redisoperate.model = model;
    //            _redisoperate.SaveToRedis();
    //        }
    //        return flag;
    //    }

    //    /// <summary>
    //    /// 解锁 可以出场、入场
    //    /// </summary>
    //    /// <param name="model"></param>
    //    /// <returns></returns>
    //    public bool UnLockedCard(CardServiceModel model)
    //    {
    //        model.Locked = false;
    //        IExtenDataBaseOperate<CardServiceModel> extendatabaseoperate = _valueCardDatabaseoperate as IExtenDataBaseOperate<CardServiceModel>;
    //        bool flag = extendatabaseoperate.ToggleValue(model);
    //        if (flag)
    //        {
    //            _redisoperate.model = model;
    //            _redisoperate.SaveToRedis();
    //        }
    //        return flag;

    //    }

    //    /// <summary>
    //    /// 注销 从数据库删除
    //    /// </summary>
    //    /// <param name="model"></param>
    //    /// <returns></returns>
    //    public bool DeleteCard(CardServiceModel model)
    //    {
    //        _redisoperate.model = model;
    //        bool flag = _valueCardDatabaseoperate.DeleteInDataBase(model);
    //        if (flag)
    //        {
    //            flag = _redisoperate.DeleteInRedis(); //数据库不成功就不要往下执行了
    //        }
    //        return flag;
    //    }

    //    /// <summary>
    //    /// 某车场的所有卡 调用时根据业务筛选月卡，储值卡
    //    /// </summary>
    //    /// <param name="parkcode"></param>
    //    /// <returns></returns>
    //    public List<CardServiceModel> AllTypeCard(string parkcode)
    //    {
    //        //批量数据都从数据库获取 redis并不缓存此实体
    //        try
    //        {
    //            List<CardServiceModel> model = _valueCardDatabaseoperate.GetMostFromDataBase(parkcode) as List<CardServiceModel>;
    //            model = model.OrderByDescending(a => a.StartDate).ToList();//降序
    //            return model;
    //        }
    //        catch (Exception ex)
    //        {
    //            return null;
    //        }
    //    }

    //    /// <summary>
    //    /// 获取某卡
    //    /// </summary>
    //    /// <param name="guid"></param>
    //    /// <returns></returns>
    //    public CardServiceModel GetCard(string carNo, string parkCode)
    //    {
    //        try
    //        {  //只从数据库读，redis并不缓存此实体
    //            return _valueCardDatabaseoperate.GetFromDataBase(parkCode + Convert.ToBase64String(Encoding.UTF8.GetBytes(carNo)));
    //        }
    //        catch (Exception ex)
    //        {
    //            return null;
    //        }
    //    }

    //}

    ///// <summary>
    ///// 临时卡管理
    ///// </summary>
    //public class TempCardServiceContext : IBasicContext, ICardServiceContext
    //{
    //    /// <summary>
    //    /// redis操作类
    //    /// </summary>
    //    private IBaseRedisOperate<CardServiceModel> _redisoperate = null;

    //    /// <summary>
    //    /// 临时车数据库操作类
    //    /// </summary>
    //    private IBaseDataBaseOperate<CardServiceModel> _tempCardDatabaseoperate = null;


    //    public TempCardServiceContext(IBaseRedisOperate<CardServiceModel> redisoperate, IServiceGetter getter)
    //    {
    //        _redisoperate = redisoperate;
    //        _tempCardDatabaseoperate = getter.GetByName<IBaseDataBaseOperate<CardServiceModel>>("tempCard");

    //    }

    //    /// <summary>
    //    /// 开卡
    //    /// </summary>
    //    /// <param name="model"></param>
    //    /// <returns></returns>
    //    public bool AddCard(CardServiceModel model)
    //    {
    //        bool flag = _tempCardDatabaseoperate.SaveToDataBase(model);
    //        if (!flag) return false; //数据库不成功就不要往下执行了
    //        _redisoperate.model = model;
    //        return _redisoperate.SaveToRedis();
    //    }

    //    /// <summary>
    //    /// 修改卡信息
    //    /// </summary>
    //    /// <param name="model"></param>
    //    /// <returns></returns>
    //    public bool ModifyCard(CardServiceModel model)
    //    {
    //        bool flag = _tempCardDatabaseoperate.SaveToDataBase(model);
    //        if (flag)
    //        {
    //            _redisoperate.model = model;
    //            _redisoperate.SaveToRedis();
    //        }
    //        return flag;
    //    }

    //    /// <summary>
    //    /// 锁定 禁止出场、入场
    //    /// </summary>
    //    /// <param name="model"></param>
    //    /// <returns></returns>
    //    public bool LockedCard(CardServiceModel model)
    //    {
    //        model.Locked = true;
    //        IExtenDataBaseOperate<CardServiceModel> extendatabaseoperate = _tempCardDatabaseoperate as IExtenDataBaseOperate<CardServiceModel>;
    //        bool flag = extendatabaseoperate.ToggleValue(model);
    //        if (flag)
    //        {
    //            _redisoperate.model = model;
    //            _redisoperate.SaveToRedis();
    //        }
    //        return flag;
    //    }

    //    /// <summary>
    //    /// 解锁 可以出场、入场
    //    /// </summary>
    //    /// <param name="model"></param>
    //    /// <returns></returns>
    //    public bool UnLockedCard(CardServiceModel model)
    //    {
    //        model.Locked = false;
    //        IExtenDataBaseOperate<CardServiceModel> extendatabaseoperate = _tempCardDatabaseoperate as IExtenDataBaseOperate<CardServiceModel>;
    //        bool flag = extendatabaseoperate.ToggleValue(model);
    //        if (flag)
    //        {
    //            _redisoperate.model = model;
    //            _redisoperate.SaveToRedis();
    //        }
    //        return flag;

    //    }

    //    /// <summary>
    //    /// 注销 从数据库删除
    //    /// </summary>
    //    /// <param name="model"></param>
    //    /// <returns></returns>
    //    public bool DeleteCard(CardServiceModel model)
    //    {
    //        _redisoperate.model = model;
    //        bool flag = _tempCardDatabaseoperate.DeleteInDataBase(model);
    //        if (flag)
    //        {
    //            flag = _redisoperate.DeleteInRedis(); //数据库不成功就不要往下执行了
    //        }
    //        return flag;
    //    }

    //    /// <summary>
    //    /// 某车场的所有卡 调用时根据业务筛选月卡，储值卡
    //    /// </summary>
    //    /// <param name="parkcode"></param>
    //    /// <returns></returns>
    //    public List<CardServiceModel> AllTypeCard(string parkcode)
    //    {
    //        //批量数据都从数据库获取 redis并不缓存此实体
    //        try
    //        {
    //            List<CardServiceModel> model = _tempCardDatabaseoperate.GetMostFromDataBase(parkcode) as List<CardServiceModel>;
    //            model = model.OrderByDescending(a => a.StartDate).ToList();//降序
    //            return model;
    //        }
    //        catch (Exception ex)
    //        {
    //            return null;
    //        }
    //    }

    //    /// <summary>
    //    /// 获取某卡
    //    /// </summary>
    //    /// <param name="guid"></param>
    //    /// <returns></returns>
    //    public CardServiceModel GetCard(string carNo, string parkCode)
    //    {
    //        try
    //        {  //只从数据库读，redis并不缓存此实体
    //            return _tempCardDatabaseoperate.GetFromDataBase(parkCode + Convert.ToBase64String(Encoding.UTF8.GetBytes(carNo)));
    //        }
    //        catch (Exception ex)
    //        {
    //            return null;
    //        }
    //    }

    //}
}
