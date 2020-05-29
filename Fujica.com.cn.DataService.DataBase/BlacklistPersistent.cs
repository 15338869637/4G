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
 * *        File Name        : BlacklistPersistent.cs
 * *        Creator          : llp
 * *        Create Time      : 2019-10-18 
 * *        Remark           : 黑名单数据库交互管理
 * *
 * *  Copyright (c) 深圳市富士智能系统有限公司.  All rights reserved. 
 * ***************************************************************************************/
namespace Fujica.com.cn.DataService.DataBase
{
    /// <summary>
    /// 黑名单表持久化
    /// </summary>
    /// <remarks>
    /// 2019.10.18: 修改.新增注释信息,方法命名规则修改 LLP <br/> 
    /// </remarks>
    public class BlacklistPersistent : IBaseDataBaseOperate<BlacklistModel>
    {

        /// <summary>
        /// 日志记录器
        /// </summary>
        internal readonly ILogger m_logger;

        /// <summary>
        /// 序列化器
        /// </summary>
        internal readonly ISerializer m_serializer = null;

        public BlacklistPersistent(ILogger _logger, ISerializer _serializer)
        {
            m_logger = _logger;
            m_serializer = _serializer;
        }

        public bool DeleteInDataBase(BlacklistModel model)
        {
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            foreach (var item in model.List)
            {
                dbhelper.ExecuteNonQuery(string.Format("delete from t_blacklist where parkCode='{0}' and carNo='{1}'", model.ParkCode, item.CarNo));
            }
            return true;
        }

        public bool DeleteMostInDataBase(IList<BlacklistModel> modlelist)
        {
            return false;
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        public BlacklistModel GetFromDataBase(string parkCode)
        {
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            DataTable table = dbhelper.ExecuteDataTable(string.Format("select projectGuid,entityContent from t_blacklist where parkCode='{0}'", parkCode));
            if (table.Rows.Count <= 0) return null;
            BlacklistModel model = new BlacklistModel();
            model.List = new List<BlacklistSingleModel>();
            model.ParkCode = parkCode;
            foreach (DataRow item in table.Rows)
            {
                model.ProjectGuid = (string)item[0];
                string jsonstr = (string)item[1];
                BlacklistSingleModel content = m_serializer.Deserialize<BlacklistSingleModel>(jsonstr);
                model.List.Add(content);
            }
            return model;
        }

        public IList<BlacklistModel> GetFromDataBaseByPage<T>(T model)
        {
            throw new NotImplementedException();
        }

        public IList<BlacklistModel> GetMostFromDataBase(string commandText)
        {
            return null;
        }

        public bool SaveToDataBase(BlacklistModel model)
        {
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            foreach (var item in model.List)
            {
                string commandtext = @"replace into t_blacklist(projectGuid,parkCode,carNo,entityContent) 
                                   values(@projectGuid,@parkCode,@carNo,@entityContent)";

                DbParameter projectGuid = dbhelper.factory.CreateParameter();
                projectGuid.ParameterName = "@projectGuid";
                projectGuid.Value = model.ProjectGuid;

                DbParameter parkCode = dbhelper.factory.CreateParameter();
                parkCode.ParameterName = "@parkCode";
                parkCode.Value = model.ParkCode;

                DbParameter carNo = dbhelper.factory.CreateParameter();
                carNo.ParameterName = "@carNo";
                carNo.Value = item.CarNo;

                DbParameter entityContent = dbhelper.factory.CreateParameter();
                entityContent.ParameterName = "@entityContent";
                entityContent.Value = m_serializer.Serialize(item);

                DbParameter[] parameter = new DbParameter[] { projectGuid, parkCode, carNo, entityContent };
                dbhelper.ExecuteNonQuery(commandtext, parameter);
            }

            return true;
        }
    }
}
