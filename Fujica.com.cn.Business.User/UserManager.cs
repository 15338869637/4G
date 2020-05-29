/***************************************************************************************
 * *
 * *        File Name        : UserManager.cs
 * *        Creator          : llp
 * *        Create Time      : 2019-09-21 
 * *        Remark           : 系统管理器 业务逻辑层
 * *
 * *  Copyright (c) 深圳市富士智能系统有限公司.  All rights reserved. 
 * ***************************************************************************************/
using Fujica.com.cn.Business.Base;
using Fujica.com.cn.Context.Model;
using Fujica.com.cn.DataService.DataBase;
using Fujica.com.cn.DataService.RedisCache;
using Fujica.com.cn.Logger;
using Fujica.com.cn.Tools;
using System.Collections.Generic;

namespace Fujica.com.cn.Business.User
{
    /// <summary>
    /// 系统管理器.
    /// </summary> 
    /// <remarks>
    /// 2019.09.21: 新增备注信息. llp <br/>  
    /// 2019.10.23：新增接口GetUserList根据项目编码和停车场编码获取所有用户列表. Ase <br/>
    public class UserManager : IBaseBusiness
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
        /// 角色管理器
        /// </summary>
        private RoleManager _roleManager = null;

        /// <summary>
        /// 用户redis操作类
        /// </summary>
        private IBaseRedisOperate<UserAccountModel> redisoperate = null;

        /// <summary>
        /// 用户数据库操作类
        /// </summary>
        private IBaseDataBaseOperate<UserAccountModel> databaseoperate = null;
        public string LastErrorDescribe
        {
            get;
            set;
        }

        public UserManager(ILogger _logger, ISerializer _serializer,
            IBaseRedisOperate<UserAccountModel> _redisoperate,
            IBaseDataBaseOperate<UserAccountModel> _databaseoperate,
            RoleManager roleManager)
        {
            m_logger = _logger;
            m_serializer = _serializer;
            redisoperate = _redisoperate;
            databaseoperate = _databaseoperate;
            _roleManager = roleManager;
        }

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddUser(UserAccountModel model)
        {
            List<UserAccountModel> alluser = GetUserList(model.ProjectGuid);
            if (!alluser.Exists(o => o.Mobile == model.Mobile))
            {
                redisoperate.model = model;
                bool flag = databaseoperate.SaveToDataBase(model);
                if (!flag) return false; //数据库不成功就不要往下执行了
                return redisoperate.SaveToRedis();
            }else
            {

                LastErrorDescribe = "添加失败!当前用户已存在";
                return false;
            }
        }

        /// <summary>
        /// 修改用户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool ModifyUser(UserAccountModel model)
        {
            List<UserAccountModel> alluser = GetUserList(model.ProjectGuid);
            if (alluser.Exists(o => o.Guid == model.Guid))
            {
                if (alluser.Exists(o => o.Mobile == model.Mobile))
                {
                    redisoperate.model = model;
                    bool flag = databaseoperate.SaveToDataBase(model);
                    if (!flag) return false; //数据库不成功就不要往下执行了
                                             //此时不需要判断redis是否成功，因为修改时redis一定会返回false
                    redisoperate.SaveToRedis();
                    return true;
                }
                else
                {
                    LastErrorDescribe = "修改失败!修改的用户不存在";
                    return false;
                } 
            }
            else
            {
                LastErrorDescribe = "修改的用户不存在";
                return false;
            }
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public bool DeleteUser(UserAccountModel model)
        {
            redisoperate.model = model;
            bool flag = databaseoperate.DeleteInDataBase(model);
            if (!flag) return false; //数据库不成功就不要往下执行了
            return redisoperate.DeleteInRedis();
        }

        /// <summary>
        /// 获取某用户
        /// </summary>
        /// <param name="projectGuid"></param>
        /// <returns></returns>
        public UserAccountModel GetUser(string guid)
        {
            UserAccountModel model = null;
            redisoperate.model = new UserAccountModel() { Guid = guid };
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
        /// 获取所有用户列表
        /// </summary>
        /// <param name="projectGuid"></param>
        /// <returns></returns>
        public List<UserAccountModel> GetUserList(string projectGuid)
        {
            //批量数据都从数据库获取
            List<UserAccountModel> model = databaseoperate.GetMostFromDataBase(projectGuid) as List<UserAccountModel>;
            return model;
        }

        /// <summary>
        /// 根据项目编码和停车场编码获取所有用户列表
        /// </summary>
        /// <param name="projectGuid">项目编码</param>
        /// <param name="parkingCode">车场编码</param>
        /// <returns></returns>
        public List<UserAccountModel> GetUserList(string projectGuid,string parkingCode)
        {
            //该项目下所有用户列表
            List<UserAccountModel> userList = databaseoperate.GetMostFromDataBase(projectGuid) as List<UserAccountModel>;
            //该项目下所有角色列表
            List<RolePermissionModel> roleList = _roleManager.GetRoleList(projectGuid);

            //创建一个新的用户列表
            List<UserAccountModel> newUserList = new List<UserAccountModel>();

            if (userList != null && roleList != null)
                foreach (var userItem in userList)
                {
                    //查找当前用户所属的角色
                    RolePermissionModel tempRoleModel = roleList.Find(m => m.Guid == userItem.RoleGuid);
                    if (tempRoleModel != null)
                    {
                        if (!string.IsNullOrEmpty(tempRoleModel.ParkingCodeList))
                        {
                            //当前角色是否拥有当前停车场的权限
                            foreach (var parkingCodeItem in tempRoleModel.ParkingCodeList.Split(new string[] { "," }, System.StringSplitOptions.RemoveEmptyEntries))
                            {
                                if (parkingCodeItem == parkingCode)
                                {
                                    //拥有当前车场的权限，则将当前用户添加到新用户列表集合中
                                    newUserList.Add(userItem);
                                }
                            } 
                        }
                    }
            }
            return newUserList;
        }
    }
}
