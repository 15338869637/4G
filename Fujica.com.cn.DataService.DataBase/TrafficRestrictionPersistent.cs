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
 * *        File Name        : TrafficRestrictionPersistent.cs
 * *        Creator          : llp
 * *        Create Time      : 2019-10-18 
 * *        Remark           : 通行限制数据库交互管理
 * *
 * *  Copyright (c) 深圳市富士智能系统有限公司.  All rights reserved. 
 * ***************************************************************************************/
namespace Fujica.com.cn.DataService.DataBase
{
    /// <remarks>
    /// 2019.10.18: 修改.新增注释信息,方法命名规则修改 LLP <br/> 
    /// </remarks>
    public class TrafficRestrictionPersistent : IBaseDataBaseOperate<TrafficRestrictionModel>
    {
        /// <summary>
        /// 日志记录器
        /// </summary>
        internal readonly ILogger m_logger;

        /// <summary>
        /// 序列化器
        /// </summary>
        internal readonly ISerializer m_serializer = null;

        public TrafficRestrictionPersistent(ILogger _logger, ISerializer _serializer)
        {
            m_logger = _logger;
            m_serializer = _serializer;
        }

        public bool DeleteInDataBase(TrafficRestrictionModel model)
        {
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            int ret = dbhelper.ExecuteNonQuery(string.Format("delete from t_trafficrestriction where guid='{0}'", model.Guid));
            if (ret > 0) return true;
            return false;
        }

        public bool DeleteMostInDataBase(IList<TrafficRestrictionModel> modlelist)
        {
            return false;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public TrafficRestrictionModel GetFromDataBase(string guid)
        {
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            string jsonstr = (string)dbhelper.ExecuteScalar(string.Format("select entityContent from t_trafficrestriction where guid='{0}'", guid));
            if (string.IsNullOrWhiteSpace(jsonstr)) return null;
            TrafficRestrictionModel model = m_serializer.Deserialize<TrafficRestrictionModel>(jsonstr);
            return model;
        }

        public IList<TrafficRestrictionModel> GetFromDataBaseByPage<T>(T model)
        {
            throw new NotImplementedException();
        }

        public IList<TrafficRestrictionModel> GetMostFromDataBase(string projectGuid)
        {
            List<TrafficRestrictionModel> model = new List<TrafficRestrictionModel>();
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            DataTable table = dbhelper.ExecuteDataTable(string.Format("select * from t_trafficrestriction where projectGuid='{0}'", projectGuid));
            foreach (DataRow item in table.Rows)
            {
                model.Add(m_serializer.Deserialize<TrafficRestrictionModel>((string)item["entityContent"]));
            }
            return model;
        }

        public bool SaveToDataBase(TrafficRestrictionModel model)
        {
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            string commandtext = @"replace into t_trafficrestriction(projectGuid,parkCode,guid,entityContent) 
                                   values(@projectGuid,@parkCode,@guid,@entityContent)";

            DbParameter projectGuid = dbhelper.factory.CreateParameter();
            projectGuid.ParameterName = "@projectGuid";
            projectGuid.Value = model.ProjectGuid;

            DbParameter parkCode = dbhelper.factory.CreateParameter();
            parkCode.ParameterName = "@parkCode";
            parkCode.Value = model.ParkCode;

            DbParameter guid = dbhelper.factory.CreateParameter();
            guid.ParameterName = "@guid";
            guid.Value = model.Guid;

            DbParameter entityContent = dbhelper.factory.CreateParameter();
            entityContent.ParameterName = "@entityContent";
            entityContent.Value = m_serializer.Serialize(model);


            DbParameter[] parameter = new DbParameter[] { projectGuid, parkCode, guid, entityContent };
            return dbhelper.ExecuteNonQuery(commandtext, parameter) > 0 ? true : false;
        }
    }
}
