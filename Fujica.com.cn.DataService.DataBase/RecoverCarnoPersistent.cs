using Fujica.com.cn.BaseConnect;
using Fujica.com.cn.Context.Model;
using Fujica.com.cn.Logger;
using Fujica.com.cn.Tools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
/***************************************************************************************
 * *
 * *        File Name        : RecoverCarnoPersistent.cs
 * *        Creator          : llp
 * *        Create Time      : 2019-10-18 
 * *        Remark           : 车牌修正记录数据库交互管理
 * *
 * *  Copyright (c) 深圳市富士智能系统有限公司.  All rights reserved. 
 * ***************************************************************************************/
namespace Fujica.com.cn.DataService.DataBase
{
    /// <summary>
    /// 车牌修正记录
    /// </summary>
    /// <remarks>
    /// 2019.10.18: 修改.新增注释信息,方法命名规则修改 LLP <br/> 
    /// </remarks>
    public class RecoverCarnoPersistent : IBaseDataBaseOperate<CorrectCarnoModel>
    {
        /// <summary>
        /// 日志记录器
        /// </summary>
        internal readonly ILogger m_logger;

        /// <summary>
        /// 序列化器
        /// </summary>
        internal readonly ISerializer m_serializer = null;

        public RecoverCarnoPersistent(ILogger _logger, ISerializer _serializer)
        {
            m_logger = _logger;
            m_serializer = _serializer;
        }

        public IList<CorrectCarnoModel> GetFromDataBaseByPage<T>(T model)

        {
            CorrectCarnoSearch searchModel = model as CorrectCarnoSearch; 
            List<CorrectCarnoModel> list = null;
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            DataTable table = new DataTable();
            //车牌修正列表
            string cmdText = " SELECT * FROM t_correctcarno INNER JOIN (SELECT id FROM t_correctcarno  {0}"
                           + $" ORDER BY id DESC LIMIT {(searchModel.PageIndex - 1) * searchModel.PageSize},{searchModel.PageSize}) AS page USING(id); ";
            //查询条件cmd 
            string whereText = $" WHERE projectGuid = '{searchModel.ProjectGuid}' and parkCode = '{searchModel.ParkingCode}' ";

            if (!string.IsNullOrEmpty(searchModel.OldCarno))
                whereText += $" and ( oldcarNo = '{searchModel.OldCarno}' or oldcarNo like '%{searchModel.OldCarno}%')";
            if (!string.IsNullOrEmpty(searchModel.NewCarno))
                whereText += $" and (newCarno = '{searchModel.NewCarno}' or newCarno like '%{searchModel.NewCarno}%')";
            if (!string.IsNullOrEmpty(searchModel.Operator))
                whereText += $" and operator = '{searchModel.Operator}' ";
            if (searchModel.StrTime != null && searchModel.StrTime != DateTime.MinValue)
                whereText += $" and evnetTime >= '{searchModel.StrTime}' ";
            if (searchModel.EndTime != null && searchModel.EndTime != DateTime.MinValue)
                whereText += $" and evnetTime <= '{searchModel.StrTime}' "; 
           cmdText = string.Format(cmdText, whereText);

            //总条数cmd
            string cmdTextCount = "SELECT COUNT(*) FROM t_correctcarno " + whereText;

            long count = (long)dbhelper.ExecuteScalar(cmdTextCount);
            if (count <= 0) return list;
            searchModel.TotalCount = count;

            table = dbhelper.ExecuteDataTable(cmdText);
            if (table == null || table.Rows.Count <= 0)
                return list;
            list = new List<CorrectCarnoModel>();
            foreach (DataRow item in table.Rows)
            {
                CorrectCarnoModel csm = new CorrectCarnoModel()
                {
                    ProjectGuid = item["projectGuid"].ToString(),
                    ParkingCode = item["parkCode"].ToString(),
                    OperationType = (int)item["operationtype"],
                    OldCarno = item["oldcarNo"].ToString(),
                    NewCarno = item["newCarno"].ToString(),
                    InTime = Convert.ToDateTime(item["evnetTime"]),
                    ThroughName = item["throughName"].ToString(),
                    Discerncamera = item["discerncamera"].ToString(),
                    ImgUrl = item["imgUrl"].ToString(),
                    Operator = item["operator"].ToString()
                };

                list.Add(csm);
            }
            table.Clear();
            return list;
        }
        public bool DeleteInDataBase(CorrectCarnoModel model)
        {
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            int ret = dbhelper.ExecuteNonQuery(string.Format("delete from t_correctcarno where projectGuid='{0}'", model.ProjectGuid));
            if (ret > 0) return true;
            return false;
        }

        public bool DeleteMostInDataBase(IList<CorrectCarnoModel> modlelist)
        {
            return false;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
         

        public CorrectCarnoModel GetFromDataBase(string projectGuid)
        {
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            DataTable table = dbhelper.ExecuteDataTable(string.Format("select * from t_correctcarno where projectGuid='{0}'", projectGuid));
            if (table.Rows.Count > 0)
            {
                CorrectCarnoModel model = new CorrectCarnoModel()
                {
                    ProjectGuid = table.Rows[0]["projectGuid"].ToString(),
                    ParkingCode = table.Rows[0]["parkCode"].ToString(),
                    OperationType = (int)table.Rows[0]["operationtype"], 
                    OldCarno = table.Rows[0]["newCarno"].ToString(),
                    NewCarno = table.Rows[0]["throughName"].ToString(),
                    InTime = Convert.ToDateTime(table.Rows[0]["evnetTime"]),  
                    ThroughName = table.Rows[0]["throughName"].ToString(),
                    DeviceIdentify = table.Rows[0]["discerncamera"].ToString(),
                    ImgUrl = table.Rows[0]["imgUrl"].ToString(),
                    Operator = table.Rows[0]["operator"].ToString()
                };
                return model;
            }
            return null;
        }

        public IList<CorrectCarnoModel> GetMostFromDataBase(string commandText)
        {
            List<CorrectCarnoModel> model = new List<CorrectCarnoModel>();
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            DataTable table = dbhelper.ExecuteDataTable("select * from t_correctcarno");
            foreach (DataRow item in table.Rows)
            {
                model.Add(new CorrectCarnoModel()
                {
                    ProjectGuid = item["projectGuid"].ToString(), 
                    ParkingCode = item["parkCode"].ToString(),
                    OperationType = (int)item["operationtype"],
                    OldCarno = item["oldCarno"].ToString(),
                    NewCarno = item["newCarno"].ToString(),
                    InTime = Convert.ToDateTime(item["evnetTime"].ToString()),
                    ThroughName = item["throughName"].ToString(),
                    DeviceIdentify = item["discerncamera"].ToString(),
                    ImgUrl = item["imgUrl"].ToString(),
                    Operator = item["operator"].ToString()
                });
            }
            return model;
        }

        public bool SaveToDataBase(CorrectCarnoModel model)
        {
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            string commandtext = @"replace into t_correctcarno(identifying,projectGuid,parkCode,operationtype,oldCarno,newCarno,evnetTime,throughName,deviceIdentify,discerncamera,imgurl,operator) 
                                   values(@identifying,@projectGuid,@parkCode,@operationtype,@oldCarno,@newCarno,@evnetTime,@throughName,@deviceIdentify,@discerncamera,@imgUrl,@operator)";


            DbParameter identifying = dbhelper.factory.CreateParameter();
            identifying.ParameterName = "@identifying";
            identifying.Value = model.ParkingCode + Convert.ToBase64String(Encoding.UTF8.GetBytes(model.NewCarno));//唯一标识 车场编码+车牌base64 

            DbParameter projectGuid = dbhelper.factory.CreateParameter();
            projectGuid.ParameterName = "@projectGuid";
            projectGuid.Value = model.ProjectGuid;

            DbParameter parkCode = dbhelper.factory.CreateParameter();
            parkCode.ParameterName = "@parkCode";
            parkCode.Value = model.ParkingCode;

            DbParameter operationtype = dbhelper.factory.CreateParameter();
            operationtype.ParameterName = "@operationtype";
            operationtype.Value = model.OperationType;

            DbParameter oldCarno = dbhelper.factory.CreateParameter();
            oldCarno.ParameterName = "@oldCarno";
            oldCarno.Value = model.OldCarno; 
             
             
            DbParameter newCarno = dbhelper.factory.CreateParameter();
            newCarno.ParameterName = "@newCarno";
            newCarno.Value = model.NewCarno;

            DbParameter evnetTime = dbhelper.factory.CreateParameter();
            evnetTime.ParameterName = "@evnetTime";
            evnetTime.Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            DbParameter throughName = dbhelper.factory.CreateParameter();
            throughName.ParameterName = "@throughName";
            throughName.Value = model.ThroughName;

            DbParameter discerncamera = dbhelper.factory.CreateParameter();
            discerncamera.ParameterName = "@discerncamera";
            discerncamera.Value = model.Discerncamera;

            DbParameter deviceIdentify = dbhelper.factory.CreateParameter();
            deviceIdentify.ParameterName = "@deviceIdentify";
            deviceIdentify.Value = model.DeviceIdentify;

            DbParameter imgUrl = dbhelper.factory.CreateParameter();
            imgUrl.ParameterName = "@imgUrl";
            imgUrl.Value = model.ImgUrl;
             
            DbParameter operatorr = dbhelper.factory.CreateParameter();
            operatorr.ParameterName = "@operator";
            operatorr.Value = model.Operator;

            DbParameter[] parameter = new DbParameter[] { identifying, projectGuid, parkCode, operationtype, oldCarno,newCarno, evnetTime, throughName, deviceIdentify,discerncamera, imgUrl,operatorr };

            return dbhelper.ExecuteNonQuery(commandtext, parameter) > 0 ? true : false;
        }
    }
}
