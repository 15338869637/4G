using System;
using System.Collections.Generic;
using Fujica.com.cn.Context.Model;
using Fujica.com.cn.Tools;
using Fujica.com.cn.Logger;
using Fujica.com.cn.BaseConnect;
using System.Data.Common;
using System.Data;
/***************************************************************************************
 * *
 * *        File Name        : ParkLotPersistent.cs
 * *        Creator          : llp
 * *        Create Time      : 2019-10-18 
 * *        Remark           : 车场数据库交互管理
 * *
 * *  Copyright (c) 深圳市富士智能系统有限公司.  All rights reserved. 
 * ***************************************************************************************/
namespace Fujica.com.cn.DataService.DataBase
{
    /// <summary>
    /// 车场表持久化层
    /// </summary>
    /// <remarks>
    /// 2019.10.18: 修改.新增注释信息,方法命名规则修改 LLP <br/> 
    /// </remarks>
    public class ParkLotPersistent : IBaseDataBaseOperate<ParkLotModel>
    {
        /// <summary>
        /// 日志记录器
        /// </summary>
        internal readonly ILogger m_logger;

        /// <summary>
        /// 序列化器
        /// </summary>
        internal readonly ISerializer m_serializer = null;

        public ParkLotPersistent(ILogger _logger, ISerializer _serializer)
        {
            m_logger = _logger;
            m_serializer = _serializer;
        }

        public bool DeleteInDataBase(ParkLotModel model)
        {
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            int ret = dbhelper.ExecuteNonQuery(string.Format("delete from t_parklot where parkCode='{0}'", model.ParkCode));
            if (ret > 0) return true;
            return false;
        }

        public bool DeleteMostInDataBase(IList<ParkLotModel> modlelist)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public ParkLotModel GetFromDataBase(string parkCode)
        {
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            string jsonstr = (string)dbhelper.ExecuteScalar(string.Format("select entityContent from t_parklot where parkCode='{0}'", parkCode));
            if (string.IsNullOrWhiteSpace(jsonstr)) return null;
            ParkLotModel model = m_serializer.Deserialize<ParkLotModel>(jsonstr);
            return model;
        }

        public IList<ParkLotModel> GetFromDataBaseByPage<T>(T model)
        {
            throw new NotImplementedException();
        }

        public IList<ParkLotModel> GetMostFromDataBase(string projectGuid)
        {
            List<ParkLotModel> model = new List<ParkLotModel>();
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            DataTable table = dbhelper.ExecuteDataTable(string.Format("select entityContent from t_parklot where projectGuid='{0}'", projectGuid));
            foreach (DataRow item in table.Rows)
            {
                string jsonstr = (string)item[0];
                ParkLotModel content = m_serializer.Deserialize<ParkLotModel>(jsonstr);
                model.Add(content);
            }
            return model;
        }

        public bool SaveToDataBase(ParkLotModel model)
        {
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            string commandtext = @"replace into t_parklot(projectGuid,parkCode,parkName,entityContent,existence) 
                                   values(@projectGuid,@parkCode,@parkName,@entityContent,@existence)";

            DbParameter projectGuid = dbhelper.factory.CreateParameter();
            projectGuid.ParameterName = "@projectGuid";
            projectGuid.Value = model.ProjectGuid;

            DbParameter parkCode = dbhelper.factory.CreateParameter();
            parkCode.ParameterName = "@parkCode";
            parkCode.Value = model.ParkCode;

            DbParameter parkName = dbhelper.factory.CreateParameter();
            parkName.ParameterName = "@parkName";
            parkName.Value = model.ParkName;

            DbParameter entityContent = dbhelper.factory.CreateParameter();
            entityContent.ParameterName = "@entityContent";
            entityContent.Value = m_serializer.Serialize(model);

            DbParameter existence = dbhelper.factory.CreateParameter();
            existence.ParameterName = "@existence";
            existence.Value = model.Existence ? 1 : 0;

            DbParameter[] parameter = new DbParameter[] { projectGuid, parkCode, parkName, entityContent, existence };
            return dbhelper.ExecuteNonQuery(commandtext, parameter) > 0 ? true : false;
        }
    }
}
