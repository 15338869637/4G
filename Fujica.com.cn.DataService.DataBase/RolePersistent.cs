using Fujica.com.cn.BaseConnect;
using Fujica.com.cn.Context.Model;
using Fujica.com.cn.Logger;
using Fujica.com.cn.Tools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
/***************************************************************************************
 * *
 * *        File Name        : RolePersistent.cs
 * *        Creator          : llp
 * *        Create Time      : 2019-10-18 
 * *        Remark           : 角色数据库交互管理
 * *
 * *  Copyright (c) 深圳市富士智能系统有限公司.  All rights reserved. 
 * ***************************************************************************************/
namespace Fujica.com.cn.DataService.DataBase
{
    /// <remarks>
    /// 2019.10.18: 修改.新增注释信息,方法命名规则修改 LLP <br/> 
    /// 2019-10-23：修改.GetMostFromDataBase方法数据库参数赋值错误 Ase<br/>
    /// </remarks>
    public class RolePersistent : IBaseDataBaseOperate<RolePermissionModel>
    {
        /// <summary>
        /// 日志记录器
        /// </summary>
        internal readonly ILogger m_logger;

        /// <summary>
        /// 序列化器
        /// </summary>
        internal readonly ISerializer m_serializer = null;

        public RolePersistent(ILogger _logger, ISerializer _serializer)
        {
            m_logger = _logger;
            m_serializer = _serializer;
        }

        public bool DeleteInDataBase(RolePermissionModel model)
        {
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            int ret = dbhelper.ExecuteNonQuery(string.Format("delete from t_rolepermission where guid='{0}'", model.Guid));
            if (ret > 0) return true;
            return false;
        }

        public bool DeleteMostInDataBase(IList<RolePermissionModel> modlelist)
        {
            return false;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public RolePermissionModel GetFromDataBase(string guid)
        {
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            DataTable table = dbhelper.ExecuteDataTable(string.Format("select * from t_rolepermission where guid='{0}'", guid));
            if (table.Rows.Count > 0)
            {
                RolePermissionModel model = new RolePermissionModel()
                {
                    ProjectGuid = table.Rows[0]["projectGuid"].ToString(),
                    Guid = guid,
                    RoleName = table.Rows[0]["roleName"].ToString(),
                    ContentDetial = table.Rows[0]["contentDetial"].ToString(),
                    ParkingCodeList = table.Rows[0]["parkingCodeList"].ToString(),
                    IsAdmin = table.Rows[0]["isAdmin"].ToString() == "1" ? true : false
                };
                return model;
            }
            return null;
        }

        public IList<RolePermissionModel> GetFromDataBaseByPage<T>(T model)
        {
            throw new NotImplementedException();
        }

        public IList<RolePermissionModel> GetMostFromDataBase(string projectGuid)
        {
            List<RolePermissionModel> model = new List<RolePermissionModel>();
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            DataTable table = dbhelper.ExecuteDataTable(string.Format("select * from t_rolepermission where projectGuid='{0}'", projectGuid));
            foreach (DataRow item in table.Rows)
            {
                model.Add(new RolePermissionModel() {
                    ProjectGuid = projectGuid,
                    Guid = item["guid"].ToString(),
                    RoleName = item["roleName"].ToString(),
                    ContentDetial = item["contentDetial"].ToString(),
                    ParkingCodeList = item["parkingCodeList"].ToString(),
                    IsAdmin = item["isAdmin"].ToString() == "1" ? true : false
                });
            }
            return model;
        }

        public bool SaveToDataBase(RolePermissionModel model)
        {
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
                string commandtext = @"replace into t_rolepermission(projectGuid,guid,roleName,contentDetial,parkingCodeList,isAdmin) 
                                   values(@projectGuid,@guid,@roleName,@contentDetial,@parkingCodeList,@isAdmin)";

            DbParameter projectGuid = dbhelper.factory.CreateParameter();
            projectGuid.ParameterName = "@projectGuid";
            projectGuid.Value = model.ProjectGuid;

            DbParameter guid = dbhelper.factory.CreateParameter();
            guid.ParameterName = "@guid";
            guid.Value = model.Guid;

            DbParameter roleName = dbhelper.factory.CreateParameter();
            roleName.ParameterName = "@roleName";
            roleName.Value = model.RoleName;

            DbParameter contentDetial = dbhelper.factory.CreateParameter();
            contentDetial.ParameterName = "@contentDetial";
            contentDetial.Value = model.ContentDetial;

            DbParameter parkingCodeList = dbhelper.factory.CreateParameter();
            parkingCodeList.ParameterName = "@parkingCodeList";
            parkingCodeList.Value = model.ParkingCodeList;

            DbParameter isAdmin = dbhelper.factory.CreateParameter();
            isAdmin.ParameterName = "@isAdmin";
            isAdmin.Value = model.IsAdmin ? 1 : 0;

            DbParameter[] parameter = new DbParameter[] { projectGuid, guid, roleName, contentDetial, parkingCodeList , isAdmin };

            return dbhelper.ExecuteNonQuery(commandtext, parameter) > 0 ? true : false;
        }
    }
}
