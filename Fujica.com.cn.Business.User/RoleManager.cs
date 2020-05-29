/***************************************************************************************
 * *
 * *        File Name        : RoleManager.cs
 * *        Creator          : llp
 * *        Create Time      : 2019-09-21 
 * *        Remark           : 角色管理器 业务逻辑层
 * *
 * *  Copyright (c) 深圳市富士智能系统有限公司.  All rights reserved. 
 * ***************************************************************************************/
using Fujica.com.cn.Business.Base;
using Fujica.com.cn.Context.Model;
using Fujica.com.cn.DataService.DataBase;
using Fujica.com.cn.DataService.RedisCache;
using Fujica.com.cn.Logger;
using Fujica.com.cn.Tools;
using System;
using System.Collections.Generic;

namespace Fujica.com.cn.Business.User
{
    /// <summary>
    /// 角色管理器.
    /// </summary> 
    /// <remarks>
    /// 2019.09.21: 新增备注信息. llp <br/> 
    /// </remarks>  
    public class RoleManager : IBaseBusiness
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
        private IBaseRedisOperate<RolePermissionModel> redisoperate = null;

        /// <summary>
        /// 数据库操作类
        /// </summary>
        private IBaseDataBaseOperate<RolePermissionModel> databaseoperate = null;

        public string LastErrorDescribe
        {
            get;
            set;
        }

        public RoleManager(ILogger _logger, ISerializer _serializer,
            IBaseRedisOperate<RolePermissionModel> _redisoperate,
            IBaseDataBaseOperate<RolePermissionModel> _databaseoperate)
        {
            m_logger = _logger;
            m_serializer = _serializer;
            redisoperate = _redisoperate;
            databaseoperate = _databaseoperate;
        }

        /// <summary>
        /// 添加角色
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public bool AddRole(RolePermissionModel model)
        {
            redisoperate.model = model;
            bool flag = databaseoperate.SaveToDataBase(model);
            if (!flag) return false; //数据库不成功就不要往下执行了
            return redisoperate.SaveToRedis();
        }

        /// <summary>
        /// 修改角色
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public bool ModifyRole(RolePermissionModel model)
        {
            redisoperate.model = model;
            bool flag = databaseoperate.SaveToDataBase(model);
            if (!flag) return false; //数据库不成功就不要往下执行了
            //此时不需要判断redis是否成功，因为修改时redis一定会返回false
            redisoperate.SaveToRedis();
            return true;
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public bool DeleteRole(RolePermissionModel model)
        {
            redisoperate.model = model;
            RolePermissionModel contentModel = GetRole(model.Guid);
            if (model.ProjectGuid != contentModel.ProjectGuid)
            {
                LastErrorDescribe = "项目编码不一致";//后续添加到错误枚举
                return false;
            }
            if (model.IsAdmin)
            {
                LastErrorDescribe = "禁止删除超级管理员";//后续添加到错误枚举
                return false;
            }
            bool flag = databaseoperate.DeleteInDataBase(model);
            if (!flag) return false; //数据库不成功就不要往下执行了
            return redisoperate.DeleteInRedis();
        }

        /// <summary>
        /// 获取某角色
        /// </summary>
        /// <param name="projectGuid"></param>
        /// <returns></returns>
        public RolePermissionModel GetRole(string guid)
        {
            RolePermissionModel model = null;
            redisoperate.model = new RolePermissionModel() { Guid = guid };
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
                }else
                {
                    LastErrorDescribe = "未找到该角色";
                }
            }

            return model;
        }

        /// <summary>
        /// 获取所有角色列表
        /// </summary>
        /// <param name="projectGuid"></param>
        /// <returns></returns>
        public List<RolePermissionModel> GetRoleList(string projectGuid)
        {
            //批量数据都从数据库获取
            List<RolePermissionModel> model = databaseoperate.GetMostFromDataBase(projectGuid) as List<RolePermissionModel>;
            return model;
        }

        /// <summary>
        /// 给角色增加新的授权车场
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="parkingCode"></param>
        /// <returns></returns>
        public bool ModifyRoleAddParkingCode(string guid,string parkingCode)
        {
            RolePermissionModel roleModel = GetRole(guid);
            if (roleModel == null) return false;
            //角色的车场权限编号，不进行重复增加
            if (roleModel.ParkingCodeList.IndexOf(parkingCode) > 0)
                return true;
            roleModel.ParkingCodeList += "," + parkingCode;
            return ModifyRole(roleModel);
        }

        /// <summary>
        /// 删除当前角色授权车场
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="parkingCode"></param>
        /// <returns></returns>
        public bool ModifyRoleDeleteParkingCode(string guid, string parkingCode)
        {
            RolePermissionModel roleModel = GetRole(guid);
            if (roleModel == null) return false;
            if (roleModel.ParkingCodeList.IndexOf(parkingCode) > 0)
            {
                roleModel.ParkingCodeList = roleModel.ParkingCodeList.Replace(parkingCode, "");
                string[] codeList = roleModel.ParkingCodeList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                string codeStr = "";
                foreach (var item in codeList)
                {
                    codeStr += item + ",";
                }
                codeStr = codeStr.TrimEnd(',');
                roleModel.ParkingCodeList = codeStr;
                return ModifyRole(roleModel);
            }
            return true;
        }

    }
}
