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
 * *        File Name        : BillingTemplatePersistent.cs
 * *        Creator          : llp
 * *        Create Time      : 2019-10-18 
 * *        Remark           : 计费模板据库交互管理
 * *
 * *  Copyright (c) 深圳市富士智能系统有限公司.  All rights reserved. 
 * ***************************************************************************************/
namespace Fujica.com.cn.DataService.DataBase
{
    public class BillingTemplatePersistent : IBaseDataBaseOperate<BillingTemplateModel>
    {
        /// <summary>
        /// 日志记录器
        /// </summary>
        /// <remarks>
        /// 2019.10.18: 修改.新增注释信息,方法命名规则修改 LLP <br/> 
        /// </remarks>
        internal readonly ILogger m_logger;

        /// <summary>
        /// 序列化器
        /// </summary>
        internal readonly ISerializer m_serializer = null;

        public BillingTemplatePersistent(ILogger _logger, ISerializer _serializer)
        {
            m_logger = _logger;
            m_serializer = _serializer;
        }

        public bool DeleteInDataBase(BillingTemplateModel model)
        {
            try
            {
                MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
                dbhelper.ExecuteNonQuery(string.Format("delete from t_billingtemplate where carTypeGuid='{0}'", model.CarTypeGuid));
                //当前方法只要程序未报错，则直接返回成功。影响行数不作为成功失败的判断值
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool DeleteMostInDataBase(IList<BillingTemplateModel> modlelist)
        {
            return false;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public BillingTemplateModel GetFromDataBase(string carTypeGuid)
        {
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            DataTable table = dbhelper.ExecuteDataTable(string.Format("select * from t_billingtemplate where carTypeGuid='{0}'", carTypeGuid));
            if (table.Rows.Count > 0)
            {
                BillingTemplateModel model = new BillingTemplateModel();
                model.ProjectGuid = (string)table.Rows[0]["projectGuid"];
                model.ParkCode = (string)table.Rows[0]["parkCode"];
                model.CarTypeGuid = (string)table.Rows[0]["cartypeGuid"];
                model.ChargeMode = (int)table.Rows[0]["chargeMode"];
                model.TemplateJson = (string)table.Rows[0]["templateJson"];
                return model;
            }
            return null;
        }

        public IList<BillingTemplateModel> GetFromDataBaseByPage<T>(T model)
        {
            throw new NotImplementedException();
        }

        public IList<BillingTemplateModel> GetMostFromDataBase(string parkCode)
        {
            return null;
        }

        public bool SaveToDataBase(BillingTemplateModel model)
        {
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            string commandtext = @"replace into t_billingtemplate(projectGuid,parkCode,cartypeGuid,chargeMode,templateJson)
                                    values(@projectGuid,@parkCode,@cartypeGuid,@chargeMode,@templateJson)";

            DbParameter projectGuid = dbhelper.factory.CreateParameter();
            projectGuid.ParameterName = "@projectGuid";
            projectGuid.Value = model.ProjectGuid;

            DbParameter parkCode = dbhelper.factory.CreateParameter();
            parkCode.ParameterName = "@parkCode";
            parkCode.Value = model.ParkCode;

            DbParameter cartypeGuid = dbhelper.factory.CreateParameter();
            cartypeGuid.ParameterName = "@cartypeGuid";
            cartypeGuid.Value = model.CarTypeGuid;

            DbParameter chargeMode = dbhelper.factory.CreateParameter();
            chargeMode.ParameterName = "@chargeMode";
            chargeMode.Value = model.ChargeMode;

            DbParameter templateJson = dbhelper.factory.CreateParameter();
            templateJson.ParameterName = "@templateJson";
            templateJson.Value = model.TemplateJson;

            DbParameter[] parameter = new DbParameter[] { projectGuid, parkCode, cartypeGuid, chargeMode, templateJson };
            return dbhelper.ExecuteNonQuery(commandtext, parameter) > 0 ? true : false;
        }
    }
}
