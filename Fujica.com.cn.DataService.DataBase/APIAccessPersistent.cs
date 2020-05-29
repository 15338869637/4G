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
 * *        File Name        : APIAccessPersistent.cs
 * *        Creator          : llp
 * *        Create Time      : 2019-10-18 
 * *        Remark           : API接数据库交互管理
 * *
 * *  Copyright (c) 深圳市富士智能系统有限公司.  All rights reserved. 
 * ***************************************************************************************/
namespace Fujica.com.cn.DataService.DataBase
{
    /// <summary>
    /// API接入控制持久化
    /// </summary>
    /// <remarks>
    /// 2019.10.18: 修改.新增注释信息,方法命名规则修改 LLP <br/> 
    /// </remarks>
    public class APIAccessPersistent : IBaseDataBaseOperate<APIAccessModel>
    {
        /// <summary>
        /// 日志记录器
        /// </summary>
        internal readonly ILogger m_logger;

        /// <summary>
        /// 序列化器
        /// </summary>
        internal readonly ISerializer m_serializer = null;

        public APIAccessPersistent(ILogger _logger, ISerializer _serializer)
        {
            m_logger = _logger;
            m_serializer = _serializer;
        }

        public bool DeleteInDataBase(APIAccessModel model)
        {
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            int ret = dbhelper.ExecuteNonQuery(string.Format("delete from t_apiaccess where appid='{0}'", model.AppID));
            if (ret > 0) return true;
            return false;
        }

        public bool DeleteMostInDataBase(IList<APIAccessModel> modlelist)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public APIAccessModel GetFromDataBase(string appid)
        {      
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            DataTable table = dbhelper.ExecuteDataTable(string.Format("select * from t_apiaccess where appid='{0}'", appid));
            if (table.Rows.Count > 0)
            {
                APIAccessModel model = new APIAccessModel();
                model.AppID = (string)table.Rows[0]["appid"];
                model.Secret = (string)table.Rows[0]["secret"];
                model.PublicKey = (string)table.Rows[0]["publickey"];
                model.PrivateKey = (string)table.Rows[0]["privatekey"];
                model.PrivateKeyPKCS8 = (string)table.Rows[0]["privatekeypkcs8"];
                model.NeedVerify = (int)table.Rows[0]["needverify"];
                model.Enable = (int)table.Rows[0]["enable"];
                return model;
            }
            return null;
        }

        public IList<APIAccessModel> GetFromDataBaseByPage<T>(T model)
        {
            throw new NotImplementedException();
        }

        public IList<APIAccessModel> GetMostFromDataBase(string commandText)
        {
            List<APIAccessModel> model = new List<APIAccessModel>();
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            DataTable table = dbhelper.ExecuteDataTable("select * from t_apiaccess");
            foreach (DataRow item in table.Rows)
            {
                model.Add(new APIAccessModel()
                {
                    AppID = (string)item["appid"],
                    Secret = (string)item["secret"],
                    PublicKey = (string)item["publickey"],
                    PrivateKey = (string)item["privatekey"],
                    PrivateKeyPKCS8 = (string)item["privatekeypkcs8"],
                    NeedVerify = (int)item["needverify"],
                    Enable = (int)item["enable"]
                });
            }
            return model;
        }

        public bool SaveToDataBase(APIAccessModel model)
        {
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            string commandtext = @"replace into t_apiaccess(appid,secret,publickey,privatekey,privatekeypkcs8,needverify,enable)
                                    values(@appid,@secret,@publickey,@privatekey,@privatekeypkcs8,@needverify,@enable)";

            DbParameter appid = dbhelper.factory.CreateParameter();
            appid.ParameterName = "@appid";
            appid.Value = model.AppID;

            DbParameter secret = dbhelper.factory.CreateParameter();
            secret.ParameterName = "@secret";
            secret.Value = model.Secret;

            DbParameter publickey = dbhelper.factory.CreateParameter();
            publickey.ParameterName = "@publickey";
            publickey.Value = model.PublicKey;

            DbParameter privatekey = dbhelper.factory.CreateParameter();
            privatekey.ParameterName = "@privatekey";
            privatekey.Value = model.PrivateKey;

            DbParameter privatekeypkcs8 = dbhelper.factory.CreateParameter();
            privatekeypkcs8.ParameterName = "@privatekeypkcs8";
            privatekeypkcs8.Value = model.PrivateKeyPKCS8;

            DbParameter needverify = dbhelper.factory.CreateParameter();
            needverify.ParameterName = "@needverify";
            needverify.Value = model.NeedVerify;

            DbParameter enable = dbhelper.factory.CreateParameter();
            enable.ParameterName = "@enable";
            enable.Value = model.Enable;

            DbParameter[] parameter = new DbParameter[] { appid, secret, publickey, privatekey, privatekeypkcs8, needverify, enable };
            return dbhelper.ExecuteNonQuery(commandtext, parameter) > 0 ? true : false;
        }
    }
}
