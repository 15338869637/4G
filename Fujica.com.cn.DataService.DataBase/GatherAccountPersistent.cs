using Fujica.com.cn.BaseConnect;
using Fujica.com.cn.Context.Model;
using Fujica.com.cn.Logger;
using Fujica.com.cn.Tools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using static Fujica.com.cn.Context.Model.GatherAccountModel;

namespace Fujica.com.cn.DataService.DataBase
{
    public class GatherAccountPersistent : IBaseDataBaseOperate<GatherAccountModel>
    {
        /// <summary>
        /// 日志记录器
        /// </summary>
        internal readonly ILogger m_logger;

        /// <summary>
        /// 序列化器
        /// </summary>
        internal readonly ISerializer m_serializer = null;

        public GatherAccountPersistent(ILogger _logger, ISerializer _serializer)
        {
            m_logger = _logger;
            m_serializer = _serializer;
        }

        public bool DeleteInDataBase(GatherAccountModel model)
        {
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            int ret = dbhelper.ExecuteNonQuery(string.Format("delete from t_gatheraccount where guid='{0}'", model.Guid));
            if (ret > 0) return true;
            return false;
        }

        public bool DeleteMostInDataBase(IList<GatherAccountModel> modlelist)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public GatherAccountModel GetFromDataBase(string guid)
        {
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            DataTable table = dbhelper.ExecuteDataTable(string.Format("select * from t_gatheraccount where guid='{0}'", guid));
            if (table.Rows.Count > 0)
            {
                GatherAccountModel model = new GatherAccountModel();
                model.Guid = guid;
                model.ProjectGuid = (string)table.Rows[0]["projectGuid"];
                model.AccountName = (string)table.Rows[0]["accountName"];
                model.AlipayAccount = m_serializer.Deserialize<AliPayAccountModel>((string)table.Rows[0]["aliEntityContent"]);
                model.WechatAccount = m_serializer.Deserialize<WeChatAccountModel>((string)table.Rows[0]["wxEntityContent"]);
                return model;
            }
            return null;
        }

        public IList<GatherAccountModel> GetFromDataBaseByPage<T>(T model)
        {
            throw new NotImplementedException();
        }

        public IList<GatherAccountModel> GetMostFromDataBase(string projectGuid)
        {
            List<GatherAccountModel> model = new List<GatherAccountModel>();
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            DataTable table = dbhelper.ExecuteDataTable(string.Format("select * from t_gatheraccount where projectGuid='{0}'", projectGuid));
            foreach (DataRow item in table.Rows)
            {
                model.Add(new GatherAccountModel()
                {
                    Guid = (string)item["guid"],
                    ProjectGuid = (string)item["projectGuid"],
                    AccountName = (string)item["accountName"],
                    AlipayAccount = m_serializer.Deserialize<AliPayAccountModel>((string)item["aliEntityContent"]),
                    WechatAccount = m_serializer.Deserialize<WeChatAccountModel>((string)item["wxEntityContent"]),
                });
            }
            return model;
        }

        public bool SaveToDataBase(GatherAccountModel model)
        {
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            string commandtext = @"replace into t_gatheraccount(projectGuid,guid,accountName,aliEntityContent,wxEntityContent) 
                                   values(@projectGuid,@guid,@accountName,@aliEntityContent,@wxEntityContent)";

            DbParameter projectGuid = dbhelper.factory.CreateParameter();
            projectGuid.ParameterName = "@projectGuid";
            projectGuid.Value = model.ProjectGuid;

            DbParameter guid = dbhelper.factory.CreateParameter();
            guid.ParameterName = "@guid";
            guid.Value = model.Guid;

            DbParameter accountName = dbhelper.factory.CreateParameter();
            accountName.ParameterName = "@accountName";
            accountName.Value = m_serializer.Serialize(model);

            DbParameter aliEntityContent = dbhelper.factory.CreateParameter();
            aliEntityContent.ParameterName = "@aliEntityContent";
            aliEntityContent.Value = m_serializer.Serialize(model.AlipayAccount ?? new AliPayAccountModel());

            DbParameter wxEntityContent = dbhelper.factory.CreateParameter();
            wxEntityContent.ParameterName = "@wxEntityContent";
            wxEntityContent.Value = m_serializer.Serialize(model.WechatAccount ?? new WeChatAccountModel());

            DbParameter[] parameter = new DbParameter[] { projectGuid, guid, accountName, aliEntityContent, wxEntityContent };
            return dbhelper.ExecuteNonQuery(commandtext, parameter) > 0 ? true : false;
        }
    }
}
