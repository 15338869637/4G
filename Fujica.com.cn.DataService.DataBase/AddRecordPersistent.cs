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
 * *        File Name        : AddRecordPersistent.cs
 * *        Creator          : llp
 * *        Create Time      : 2019-10-18 
 * *        Remark           : 入场补录数据库交互管理
 * *
 * *  Copyright (c) 深圳市富士智能系统有限公司.  All rights reserved. 
 * ***************************************************************************************/
namespace Fujica.com.cn.DataService.DataBase
{
    /// <summary>
    /// 入场补录记录
    /// </summary>
    /// <remarks>
    /// 2019.10.18: 修改.新增注释信息,方法命名规则修改 LLP <br/> 
    /// </remarks>
    public class AddRecordPersistent : IBaseDataBaseOperate<AddRecordModel>
    {
        /// <summary>
        /// 日志记录器
        /// </summary>
        internal readonly ILogger m_logger;

        /// <summary>
        /// 序列化器
        /// </summary>
        internal readonly ISerializer m_serializer = null;

        public AddRecordPersistent(ILogger _logger, ISerializer _serializer)
        {
            m_logger = _logger;
            m_serializer = _serializer;
        }
        public IList<AddRecordModel> GetFromDataBaseByPage<T>(T model)
        {
            RecordInSearch searchModel = model as RecordInSearch;

            List<AddRecordModel> list = null;
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            DataTable table = new DataTable();
            //入场补录列表
            string cmdText = " SELECT * FROM t_admissionrecord INNER JOIN (SELECT id FROM t_admissionrecord  {0}"
                           + $" ORDER BY id DESC LIMIT {(searchModel.PageIndex - 1) * searchModel.PageSize},{searchModel.PageSize}) AS page USING(id); ";
            //查询条件cmd 
            string whereText = $" WHERE projectGuid = '{searchModel.ProjectGuid}' and parkCode = '{searchModel.ParkingCode}' ";

            if (!string.IsNullOrEmpty(searchModel.CarNo))
                whereText += $" and (carNo = '{searchModel.CarNo}' or carNo like '%{searchModel.CarNo}%') ";
            if (!string.IsNullOrEmpty(searchModel.CarTypeGuid))
                whereText += $" and carTypeGuid = '{searchModel.CarTypeGuid}' ";
            if (!string.IsNullOrEmpty(searchModel.Operator))
                whereText += $" and operator = '{searchModel.Operator}' ";
            if (searchModel.StrTime != null && searchModel.StrTime != DateTime.MinValue)
                whereText += $" and recordTime >= '{searchModel.StrTime}' ";
            if (searchModel.EndTime != null && searchModel.EndTime != DateTime.MinValue)
                whereText += $" and recordTime <= '{searchModel.StrTime}' "; 
            cmdText = string.Format(cmdText, whereText);

            //总条数cmd
            string cmdTextCount = "SELECT COUNT(*) FROM t_admissionrecord " + whereText;

            long count = (long)dbhelper.ExecuteScalar(cmdTextCount);
            if (count <= 0) return list;
            searchModel.TotalCount = count;

            table = dbhelper.ExecuteDataTable(cmdText); 
            if (table == null || table.Rows.Count <= 0)
                return list; 
            list = new List<AddRecordModel>();
            foreach (DataRow item in table.Rows)
            {
                AddRecordModel csm = new AddRecordModel()
                {
                    ProjectGuid = item["projectGuid"].ToString(),
                    ParkingCode = item["parkCode"].ToString(),
                    CarNo = item["carNo"].ToString(),
                    CarTypeGuid = item["carTypeGuid"].ToString(),
                    Entrance = item["entrance"].ToString(),
                    InTime = Convert.ToDateTime(item["entranceTime"].ToString()),
                    RecordTime = Convert.ToDateTime(item["recordTime"].ToString()),
                    Operator = item["operator"].ToString()
                };
                 
                list.Add(csm);
            }
            table.Clear();
            return list;
        } 
         
       
        public bool DeleteInDataBase(AddRecordModel model)
        {
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            int ret = dbhelper.ExecuteNonQuery(string.Format("delete from t_admissionrecord where projectGuid='{0}'", model.ProjectGuid));
            if (ret > 0) return true;
            return false;
        }

        public bool DeleteMostInDataBase(IList<AddRecordModel> modlelist)
        {
            return false;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
         

        public AddRecordModel GetFromDataBase(string guid)
        {
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            DataTable table = dbhelper.ExecuteDataTable(string.Format("select * from t_admissionrecord where projectGuid='{0}'", guid));
            if (table.Rows.Count > 0)
            {
                AddRecordModel model = new AddRecordModel()
                {
                    ProjectGuid = table.Rows[0]["projectGuid"].ToString(),
                    ParkingCode = table.Rows[0]["parkCode"].ToString(),
                    CarNo = table.Rows[0]["carNo"].ToString(),
                    CarTypeGuid = table.Rows[0]["carTypeGuid"].ToString(),
                    InTime =Convert.ToDateTime(table.Rows[0]["entranceTime"].ToString()),
                    RecordTime = Convert.ToDateTime(table.Rows[0]["recordTime"].ToString()),
                    Entrance = table.Rows[0]["entrance"].ToString(),  
                    Operator= table.Rows[0]["operator"].ToString(),

                };
                return model;
            }
            return null;
        }

        public IList<AddRecordModel> GetMostFromDataBase(string commandText)
        {
            List<AddRecordModel> model = new List<AddRecordModel>();
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            DataTable table = dbhelper.ExecuteDataTable("select * from t_admissionrecord");
            foreach (DataRow item in table.Rows)
            {
                model.Add(new AddRecordModel()
                {
                    ProjectGuid = item["projectGuid"].ToString(), 
                    ParkingCode = item["parkCode"].ToString(),
                    CarNo = item["carNo"].ToString(),
                    CarTypeGuid = item["carTypeGuid"].ToString(),
                    Entrance = item["entrance"].ToString(),
                    InTime = Convert.ToDateTime(item["entranceTime"].ToString()),
                    RecordTime = Convert.ToDateTime(item["recordTime"].ToString()),
                    Operator = item["operator"].ToString()
                });
            }
            return model;
        }

        public bool SaveToDataBase(AddRecordModel model)
        {
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            string commandtext = @"replace into t_admissionrecord(identifying,projectGuid,parkCode,carNo,carTypeGuid,entrance,entranceTime,recordTime,operator) 
                                   values(@identifying,@projectGuid,@parkCode,@carNo,@carTypeGuid,@entrance,@entranceTime,@recordTime,@operator)";


            DbParameter identifying = dbhelper.factory.CreateParameter();
            identifying.ParameterName = "@identifying";
            identifying.Value = model.ParkingCode + Convert.ToBase64String(Encoding.UTF8.GetBytes(model.CarNo));//唯一标识 车场编码+车牌base64

            DbParameter projectGuid = dbhelper.factory.CreateParameter();
            projectGuid.ParameterName = "@projectGuid";
            projectGuid.Value = model.ProjectGuid;

            DbParameter parkCode = dbhelper.factory.CreateParameter();
            parkCode.ParameterName = "@parkCode";
            parkCode.Value = model.ParkingCode;

            DbParameter carNo = dbhelper.factory.CreateParameter();
            carNo.ParameterName = "@carNo";
            carNo.Value = model.CarNo; 
             

            DbParameter carTypeGuid = dbhelper.factory.CreateParameter();
            carTypeGuid.ParameterName = "@carTypeGuid";
            carTypeGuid.Value = model.CarTypeGuid;

            DbParameter entrance = dbhelper.factory.CreateParameter();
            entrance.ParameterName = "@entrance";
            entrance.Value = model.Entrance;

            DbParameter entranceTime = dbhelper.factory.CreateParameter();
            entranceTime.ParameterName = "@entranceTime";
            entranceTime.Value = model.InTime;

            DbParameter recordTime = dbhelper.factory.CreateParameter();
            recordTime.ParameterName = "@recordTime";
            recordTime.Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            DbParameter operatorr = dbhelper.factory.CreateParameter();
            operatorr.ParameterName = "@operator";
            operatorr.Value = model.Operator;

            DbParameter[] parameter = new DbParameter[] { identifying, projectGuid, parkCode, carNo, carTypeGuid,entrance, entranceTime, recordTime, operatorr };

            return dbhelper.ExecuteNonQuery(commandtext, parameter) > 0 ? true : false;
        }
    }
}
