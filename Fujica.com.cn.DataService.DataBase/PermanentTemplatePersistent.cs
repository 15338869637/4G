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
 * *        File Name        : PermanentTemplatePersistent.cs
 * *        Creator          : llp
 * *        Create Time      : 2019-10-18 
 * *        Remark           : 临时车计费模板数据库交互管理
 * *
 * *  Copyright (c) 深圳市富士智能系统有限公司.  All rights reserved. 
 * ***************************************************************************************/
namespace Fujica.com.cn.DataService.DataBase
{
    /// <remarks>
    /// 2019.10.18: 修改.新增注释信息,方法命名规则修改 LLP <br/> 
    /// </remarks>
    public class PermanentTemplatePersistent : IBaseDataBaseOperate<PermanentTemplateModel>
    {
        /// <summary>
        /// 日志记录器
        /// </summary>
        internal readonly ILogger m_logger;

        /// <summary>
        /// 序列化器
        /// </summary>
        internal readonly ISerializer m_serializer = null;

        public PermanentTemplatePersistent(ILogger _logger, ISerializer _serializer)
        {
            m_logger = _logger;
            m_serializer = _serializer;
        }

        public bool DeleteInDataBase(PermanentTemplateModel model)
        {
            try
            {
                MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
                dbhelper.ExecuteNonQuery(string.Format("delete from t_permanenttemplate where carTypeGuid='{0}'", model.CarTypeGuid));
                //当前方法只要程序未报错，则直接返回成功。影响行数不作为成功失败的判断值
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool DeleteMostInDataBase(IList<PermanentTemplateModel> modlelist)
        {
            return false;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public PermanentTemplateModel GetFromDataBase(string carTypeGuid)
        {
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            DataTable table = dbhelper.ExecuteDataTable(string.Format("select * from t_permanenttemplate where carTypeGuid='{0}'", carTypeGuid));
            if (table.Rows.Count > 0)
            {
                PermanentTemplateModel model = new PermanentTemplateModel();
                model.ProjectGuid = (string)table.Rows[0]["projectGuid"];
                model.ParkCode = (string)table.Rows[0]["parkCode"];
                model.CarTypeGuid = (string)table.Rows[0]["carTypeGuid"];
                model.Months = (uint)(int)table.Rows[0]["months"];
                model.Amount = decimal.Parse(table.Rows[0]["amount"].ToString());
                model.OperateTime = (string)table.Rows[0]["operateTime"];
                model.OperateUser = (string)table.Rows[0]["operateUser"];
                return model;
            }
            return null;
        }

        public IList<PermanentTemplateModel> GetFromDataBaseByPage<T>(T model)
        {
            throw new NotImplementedException();
        }

        public IList<PermanentTemplateModel> GetMostFromDataBase(string parkCode)
        {
            List<PermanentTemplateModel> model = new List<PermanentTemplateModel>();
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            DataTable table = dbhelper.ExecuteDataTable(string.Format("select * from t_permanenttemplate where parkCode='{0}'", parkCode));
            foreach (DataRow item in table.Rows)
            {
                model.Add(new PermanentTemplateModel()
                {
                    ProjectGuid = (string)item["projectGuid"],
                    ParkCode = (string)item["parkCode"],
                    CarTypeGuid = (string)item["cartypeGuid"],
                    Months = (uint)(int)item["months"],
                    Amount = (decimal)item["amount"],
                    OperateTime = (string)item["operateTime"],
                    OperateUser = (string)item["operateUser"]
                });
            }
            return model;
        }

        public bool SaveToDataBase(PermanentTemplateModel model)
        {
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            string commandtext = @"replace into t_permanenttemplate(projectGuid,parkCode,cartypeGuid,months,amount,operateTime,operateUser)
                                    values(@projectGuid,@parkCode,@cartypeGuid,@months,@amount,@operateTime,@operateUser)";

            DbParameter projectGuid = dbhelper.factory.CreateParameter();
            projectGuid.ParameterName = "@projectGuid";
            projectGuid.Value = model.ProjectGuid;

            DbParameter parkCode = dbhelper.factory.CreateParameter();
            parkCode.ParameterName = "@parkCode";
            parkCode.Value = model.ParkCode;

            DbParameter cartypeGuid = dbhelper.factory.CreateParameter();
            cartypeGuid.ParameterName = "@cartypeGuid";
            cartypeGuid.Value = model.CarTypeGuid;

            DbParameter months = dbhelper.factory.CreateParameter();
            months.ParameterName = "@months";
            months.Value = model.Months;

            DbParameter amount = dbhelper.factory.CreateParameter();
            amount.ParameterName = "@amount";
            amount.Value = model.Amount;

            DbParameter operateTime = dbhelper.factory.CreateParameter();
            operateTime.ParameterName = "@operateTime";
            operateTime.Value = model.OperateTime;

            DbParameter operateUser = dbhelper.factory.CreateParameter();
            operateUser.ParameterName = "@operateUser";
            operateUser.Value = model.OperateUser;

            DbParameter[] parameter = new DbParameter[] { projectGuid, parkCode, cartypeGuid, months, amount, operateTime, operateUser };
            return dbhelper.ExecuteNonQuery(commandtext, parameter) > 0 ? true : false;
        }
    }
}
