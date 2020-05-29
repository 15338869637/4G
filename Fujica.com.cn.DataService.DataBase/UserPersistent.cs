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
 * *        File Name        : UserPersistent.cs
 * *        Creator          : llp
 * *        Create Time      : 2019-10-18 
 * *        Remark           : 用户数据库交互管理
 * *
 * *  Copyright (c) 深圳市富士智能系统有限公司.  All rights reserved. 
 * ***************************************************************************************/
namespace Fujica.com.cn.DataService.DataBase
{
    /// <remarks>
    /// 2019.10.18: 修改.新增注释信息,方法命名规则修改 LLP <br/> 
    /// </remarks>
    public class UserPersistent : IBaseDataBaseOperate<UserAccountModel>
    {
        /// <summary>
        /// 日志记录器
        /// </summary>
        internal readonly ILogger m_logger;

        /// <summary>
        /// 序列化器
        /// </summary>
        internal readonly ISerializer m_serializer = null;

        public UserPersistent(ILogger _logger, ISerializer _serializer)
        {
            m_logger = _logger;
            m_serializer = _serializer;
        }

        public bool DeleteInDataBase(UserAccountModel model)
        {
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            int ret = dbhelper.ExecuteNonQuery(string.Format("delete from t_operator where guid='{0}'", model.Guid));
            if (ret > 0) return true;
            return false;
        }

        public bool DeleteMostInDataBase(IList<UserAccountModel> modlelist)
        {
            return false;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public UserAccountModel GetFromDataBase(string guid)
        {
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            DataTable table = dbhelper.ExecuteDataTable(string.Format("select * from t_operator where guid='{0}'", guid));
            if (table.Rows.Count > 0)
            {
                UserAccountModel model = new UserAccountModel()
                {
                    ProjectGuid = table.Rows[0]["projectGuid"].ToString(),
                    Guid = guid,
                    UserName= table.Rows[0]["userName"].ToString(),
                    UserPswd = table.Rows[0]["userPswd"].ToString(),
                    Mobile = table.Rows[0]["mobile"].ToString(),
                    Privilege = table.Rows[0]["privilege"].ToString(),
                    RoleGuid=table.Rows[0]["roleGuid"].ToString()
                };
                return model;
            }
            return null;
        }

        public IList<UserAccountModel> GetFromDataBaseByPage<T>(T model)
        {
            throw new NotImplementedException();
        }

        public IList<UserAccountModel> GetMostFromDataBase(string projectGuid)
        {
            List<UserAccountModel> model = new List<UserAccountModel>();
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            DataTable table = dbhelper.ExecuteDataTable(string.Format("select * from t_operator where projectGuid='{0}'", projectGuid));
            foreach (DataRow item in table.Rows)
            {
                model.Add(new UserAccountModel()
                {
                    ProjectGuid = projectGuid,
                    Guid = item["guid"].ToString(),
                    UserName = item["userName"].ToString(),
                    UserPswd = item["userPswd"].ToString(),
                    Mobile = item["mobile"].ToString(),
                    Privilege = item["privilege"].ToString(),
                    RoleGuid=item["roleGuid"].ToString()
                });
            }
            return model;
        }

        public bool SaveToDataBase(UserAccountModel model)
        {
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            string commandtext = @"replace into t_operator(projectGuid,guid,userName,userPswd,mobile,privilege,roleGuid) 
                                   values(@projectGuid,@guid,@userName,@userPswd,@mobile,@privilege,@roleGuid)";

            DbParameter projectGuid = dbhelper.factory.CreateParameter();
            projectGuid.ParameterName = "@projectGuid";
            projectGuid.Value = model.ProjectGuid;

            DbParameter guid = dbhelper.factory.CreateParameter();
            guid.ParameterName = "@guid";
            guid.Value = model.Guid;

            DbParameter userName = dbhelper.factory.CreateParameter();
            userName.ParameterName = "@userName";
            userName.Value = model.UserName;

            DbParameter userPswd = dbhelper.factory.CreateParameter();
            userPswd.ParameterName = "@userPswd";
            userPswd.Value = model.UserPswd;

            DbParameter mobile = dbhelper.factory.CreateParameter();
            mobile.ParameterName = "@mobile";
            mobile.Value = model.Mobile;

            DbParameter privilege = dbhelper.factory.CreateParameter();
            privilege.ParameterName = "@privilege";
            privilege.Value = model.Privilege;

            DbParameter roleGuid = dbhelper.factory.CreateParameter();
            roleGuid.ParameterName = "@roleGuid";
            roleGuid.Value = model.RoleGuid;

            DbParameter[] parameter = new DbParameter[] { projectGuid, guid, userName, userPswd, mobile, privilege, roleGuid };

            return dbhelper.ExecuteNonQuery(commandtext, parameter) > 0 ? true : false;
        }
    }
}
