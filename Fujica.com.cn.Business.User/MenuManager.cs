/***************************************************************************************
 * *
 * *        File Name        : MenuManager.cs
 * *        Creator          : llp
 * *        Create Time      : 2019-09-21 
 * *        Remark           : 菜单管理器 业务逻辑层
 * *
 * *  Copyright (c) 深圳市富士智能系统有限公司.  All rights reserved. 
 * ***************************************************************************************/
using Fujica.com.cn.Business.Base;
using Fujica.com.cn.Context.Model;
using Fujica.com.cn.DataService.DataBase;
using Fujica.com.cn.DataService.RedisCache;
using Fujica.com.cn.Logger;
using Fujica.com.cn.Tools;

namespace Fujica.com.cn.Business.User
{
    /// <summary>
    /// 菜单管理器.
    /// </summary> 
    /// <remarks>
    /// 2019.09.21: 日志参数完善. llp <br/> 
    /// </remarks>  
    public class MenuManager : IBaseBusiness
    {
        /// <summary>
        /// 日志记录器
        /// </summary>
        internal readonly ILogger m_logger;

        /// <summary>
        /// 序列化器
        /// </summary>
        internal readonly ISerializer m_serializer = null;

        /// <summary>
        /// redis操作类
        /// </summary>
        private IBaseRedisOperate<MenuModel> redisoperate = null;

        /// <summary>
        /// 数据库操作类
        /// </summary>
        private IBaseDataBaseOperate<MenuModel> databaseoperate = null;

        public string LastErrorDescribe
        {
            get;
            set;
        }

        public MenuManager(ILogger _logger, ISerializer _serializer,
            IBaseRedisOperate<MenuModel> _redisoperate,
            IBaseDataBaseOperate<MenuModel> _databaseoperate)
        {
            m_logger = _logger;
            m_serializer = _serializer;
            redisoperate = _redisoperate;
            databaseoperate = _databaseoperate;
        }

        /// <summary>
        /// 添加菜单
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public bool AddMenu(MenuModel model)
        {
            redisoperate.model = model;
            bool flag = databaseoperate.SaveToDataBase(model);
            if (!flag) return false; //数据库不成功就不要往下执行了
            return redisoperate.SaveToRedis();
        }

        /// <summary>
        /// 修改菜单
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public bool ModifyMenu(MenuModel model)
        {
            redisoperate.model = model;
            bool flag = databaseoperate.SaveToDataBase(model);
            if (!flag) return false; //数据库不成功就不要往下执行了
            //此时不需要判断redis是否成功，因为修改时redis一定会返回false
            redisoperate.SaveToRedis();
            return true;
        }

        /// <summary>
        /// 删除菜单
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public bool DeleteMenu(MenuModel model)
        {
            redisoperate.model = model;
            bool flag = databaseoperate.DeleteInDataBase(model);
            if (!flag) return false; //数据库不成功就不要往下执行了
            return redisoperate.DeleteInRedis();
        }

        /// <summary>
        /// 获取菜单
        /// </summary>
        /// <param name="projectGuid"></param>
        /// <returns></returns>
        public MenuModel GetMenu(string projectGuid)
        {
            MenuModel model = null;
            redisoperate.model = new MenuModel() { ProjectGuid = projectGuid };
            model = redisoperate.GetFromRedis();

            //从数据库读
            if (model == null)
            {
                model = databaseoperate.GetFromDataBase(projectGuid);
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
