using Fujica.com.cn.BaseConnect;
using Fujica.com.cn.Context.Model;
using Fujica.com.cn.Logger;
using Fujica.com.cn.Tools;
using System;
using System.Collections.Generic;
using System.Data.Common;
/***************************************************************************************
 * *
 * *        File Name        : FunctionPointPersistent.cs
 * *        Creator          : llp
 * *        Create Time      : 2019-10-18 
 * *        Remark           : 功能点数据库交互管理
 * *
 * *  Copyright (c) 深圳市富士智能系统有限公司.  All rights reserved. 
 * ***************************************************************************************/
namespace Fujica.com.cn.DataService.DataBase
{
    /// <remarks>
    /// 2019.10.18: 修改.新增注释信息,方法命名规则修改 LLP <br/> 
    /// </remarks>
    public class FunctionPointPersistent : IBaseDataBaseOperate<FunctionPointModel>
    {
        /// <summary>
        /// 日志记录器
        /// </summary>
        internal readonly ILogger m_logger;

        /// <summary>
        /// 序列化器
        /// </summary>
        internal readonly ISerializer m_serializer = null;

        public FunctionPointPersistent(ILogger _logger, ISerializer _serializer)
        {
            m_logger = _logger;
            m_serializer = _serializer;
        }


        public bool DeleteInDataBase(FunctionPointModel model)
        {
            //不允许删除
            //FollowMysqlHelper dbhelper = new FollowMysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            //int ret = dbhelper.ExecuteNonQuery(string.Format("delete from t_functionpoint where parkCode='{0}'", model.parkCode));
            //if (ret > 0) return true;
            return false;
        }

        public bool DeleteMostInDataBase(IList<FunctionPointModel> modlelist)
        {
            //不允许删除
            return false;
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        public FunctionPointModel GetFromDataBase(string parkCode)
        {
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            string jsonstr = (string)dbhelper.ExecuteScalar(string.Format("select entityContent from t_functionpoint where parkCode='{0}'", parkCode));
            if (string.IsNullOrWhiteSpace(jsonstr)) return null;
            FunctionPointModel model = m_serializer.Deserialize<FunctionPointModel>(jsonstr);
            return model;
        }

        public IList<FunctionPointModel> GetFromDataBaseByPage<T>(T model)
        {
            throw new NotImplementedException();
        }

        public IList<FunctionPointModel> GetMostFromDataBase(string commandText)
        {
            return null;
        }

        public bool SaveToDataBase(FunctionPointModel model)
        {
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            string commandtext = @"replace into t_functionpoint(projectGuid,parkCode,entityContent)
                                    values(@projectGuid,@parkCode,@entityContent)";

            DbParameter projectGuid = dbhelper.factory.CreateParameter();
            projectGuid.ParameterName = "@projectGuid";
            projectGuid.Value = model.ProjectGuid;

            DbParameter parkCode = dbhelper.factory.CreateParameter();
            parkCode.ParameterName = "@parkCode";
            parkCode.Value = model.ParkCode;

            DbParameter entityContent = dbhelper.factory.CreateParameter();
            entityContent.ParameterName = "@entityContent";
            entityContent.Value = m_serializer.Serialize(model);

            DbParameter[] parameter = new DbParameter[] { projectGuid, parkCode, entityContent };
            return dbhelper.ExecuteNonQuery(commandtext, parameter) > 0 ? true : false;
        }
    }
}
