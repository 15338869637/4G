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
 * *        File Name        : OpenGateReasonPersistent.cs
 * *        Creator          : llp
 * *        Create Time      : 2019-10-18 
 * *        Remark           : 开闸原因数据库交互管理
 * *
 * *  Copyright (c) 深圳市富士智能系统有限公司.  All rights reserved. 
 * ***************************************************************************************/
namespace Fujica.com.cn.DataService.DataBase
{
    /// <remarks>
    /// 2019.10.18: 修改.新增注释信息,方法命名规则修改 LLP <br/> 
    /// </remarks>
    public class OpenGateReasonPersistent : IBaseDataBaseOperate<OpenGateReasonModel>
    {
        /// <summary>
        /// 日志记录器
        /// </summary>
        internal readonly ILogger m_logger;

        /// <summary>
        /// 序列化器
        /// </summary>
        internal readonly ISerializer m_serializer = null;

        public OpenGateReasonPersistent(ILogger _logger, ISerializer _serializer)
        {
            m_logger = _logger;
            m_serializer = _serializer;
        }

        public bool DeleteInDataBase(OpenGateReasonModel model)
        {
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            int ret = dbhelper.ExecuteNonQuery(string.Format("delete from t_opengatereason where guid='{0}'", model.Guid));
            if (ret > 0) return true;
            return false;
        }

        public bool DeleteMostInDataBase(IList<OpenGateReasonModel> modlelist)
        {
            return false;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public OpenGateReasonModel GetFromDataBase(string guid)
        {
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            DataTable table = dbhelper.ExecuteDataTable(string.Format("select * from t_opengatereason where guid='{0}'", guid));
            if (table.Rows.Count > 0)
            {
                OpenGateReasonModel model = new OpenGateReasonModel();
                model.ProjectGuid = (string)table.Rows[0]["projectGuid"];
                model.Guid = (string)table.Rows[0]["guid"];
                model.OpenType = (int)table.Rows[0]["openType"];
                model.ReasonRemark = (string)table.Rows[0]["reasonRemark"];
                return model;
            }
            return null;
        }

        public IList<OpenGateReasonModel> GetFromDataBaseByPage<T>(T model)
        {
            throw new NotImplementedException();
        }

        public IList<OpenGateReasonModel> GetMostFromDataBase(string projectGuid)
        {
            List<OpenGateReasonModel> model = new List<OpenGateReasonModel>();
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            DataTable table = dbhelper.ExecuteDataTable(string.Format("select * from t_opengatereason where projectGuid='{0}'", projectGuid));
            foreach (DataRow item in table.Rows)
            {
                model.Add(new OpenGateReasonModel()
                {
                    ProjectGuid = (string)item["projectGuid"],
                    Guid = (string)item["guid"],
                    OpenType = (int)item["openType"],
                    ReasonRemark = (string)item["reasonRemark"]
                });
            }
            return model;
        }

        public bool SaveToDataBase(OpenGateReasonModel model)
        {
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            string commandtext = @"replace into t_opengatereason(projectGuid,guid,openType,reasonRemark) 
                                   values(@projectGuid,@guid,@openType,@reasonRemark)";

            DbParameter projectGuid = dbhelper.factory.CreateParameter();
            projectGuid.ParameterName = "@projectGuid";
            projectGuid.Value = model.ProjectGuid;

            DbParameter guid = dbhelper.factory.CreateParameter();
            guid.ParameterName = "@guid";
            guid.Value = model.Guid;

            DbParameter openType = dbhelper.factory.CreateParameter();
            openType.ParameterName = "@openType";
            openType.Value = model.OpenType;

            DbParameter reasonRemark = dbhelper.factory.CreateParameter();
            reasonRemark.ParameterName = "@reasonRemark";
            reasonRemark.Value = model.ReasonRemark;

            DbParameter[] parameter = new DbParameter[] { projectGuid, guid, openType, reasonRemark };
            return dbhelper.ExecuteNonQuery(commandtext, parameter) > 0 ? true : false;
        }
    }
}
