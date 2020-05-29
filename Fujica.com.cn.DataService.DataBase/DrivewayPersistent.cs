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
 * *        File Name        : DrivewayPersistent.cs
 * *        Creator          : llp
 * *        Create Time      : 2019-10-18 
 * *        Remark           : 车道数据库交互管理
 * *
 * *  Copyright (c) 深圳市富士智能系统有限公司.  All rights reserved. 
 * ***************************************************************************************/
namespace Fujica.com.cn.DataService.DataBase
{
    /// <remarks>
    /// 2019.10.18: 修改.新增注释信息,方法命名规则修改 LLP <br/> 
    /// </remarks>
    public class DrivewayPersistent : IBaseDataBaseOperate<DrivewayModel>, IExtenDataBaseOperate<DrivewayModel>
    {
        /// <summary>
        /// 日志记录器
        /// </summary>
      
        internal readonly ILogger m_logger;

        /// <summary>
        /// 序列化器
        /// </summary>
        internal readonly ISerializer m_serializer = null;

        public DrivewayPersistent(ILogger _logger, ISerializer _serializer)
        {
            m_logger = _logger;
            m_serializer = _serializer;
        }

        public bool DeleteInDataBase(DrivewayModel model)
        {
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            int ret = dbhelper.ExecuteNonQuery(string.Format("delete from t_driveway where guid='{0}'", model.Guid));
            if (ret > 0) return true;
            return false;
        }

        public bool DeleteMostInDataBase(IList<DrivewayModel> modlelist)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public DrivewayModel GetFromDataBase(string guid)
        {
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            DataTable table = dbhelper.ExecuteDataTable(string.Format("select * from t_driveway where guid='{0}'", guid));
            if (table.Rows.Count > 0)
            {
                DrivewayModel model = new DrivewayModel();
                model.ProjectGuid = (string)table.Rows[0]["projectGuid"];
                model.ParkCode = (string)table.Rows[0]["parkCode"];
                model.Guid = (string)table.Rows[0]["guid"];
                model.DrivewayName = (string)table.Rows[0]["drivewayName"];
                model.Type = (DrivewayType)(int)table.Rows[0]["type"];
                model.DeviceName = (string)table.Rows[0]["deviceName"];
                model.DeviceMacAddress = (string)table.Rows[0]["deviceMacAddress"];
                model.DeviceEntranceURI = (string)table.Rows[0]["deviceEntranceURI"];
                model.DeviceAccount = (string)table.Rows[0]["deviceAccount"];
                model.DeviceVerification = (string)table.Rows[0]["deviceVerification"];
                return model;
            }
            return null;
        }

        public IList<DrivewayModel> GetMostFromDataBase(string parkcode)
        {
            List<DrivewayModel> model = new List<DrivewayModel>();
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            DataTable table = dbhelper.ExecuteDataTable(string.Format("select * from t_driveway where parkCode='{0}'",parkcode));
            foreach (DataRow item in table.Rows)
            {
                model.Add(new DrivewayModel()
                {
                    ProjectGuid = (string)item["projectGuid"],
                    ParkCode = (string)item["parkCode"],
                    Guid = (string)item["guid"],
                    DrivewayName = (string)item["drivewayName"],
                    Type = (DrivewayType)(int)item["type"],
                    DeviceName = (string)item["deviceName"],
                    DeviceMacAddress = (string)item["deviceMacAddress"],
                    DeviceEntranceURI = (string)item["deviceEntranceURI"],
                    DeviceAccount = (string)item["deviceAccount"],
                    DeviceVerification = (string)item["deviceVerification"],
                });
            }
            return model;
        }

        public IList<DrivewayModel> GetFromDataBaseByPage<T>(T model)
        {
            throw new NotImplementedException();
        }

        public bool SaveToDataBase(DrivewayModel model)
        {
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            string commandtext = @"replace into t_driveway(projectGuid,parkCode,guid,drivewayName,type,deviceName,deviceMacAddress,deviceEntranceURI,deviceAccount,deviceVerification)
                                    values(@projectGuid,@parkCode,@guid,@drivewayName,@type,@deviceName,@deviceMacAddress,@deviceEntranceURI,@deviceAccount,@deviceVerification)";

            DbParameter projectGuid = dbhelper.factory.CreateParameter();
            projectGuid.ParameterName = "@projectGuid";
            projectGuid.Value = model.ProjectGuid;

            DbParameter parkCode = dbhelper.factory.CreateParameter();
            parkCode.ParameterName = "@parkCode";
            parkCode.Value = model.ParkCode;

            DbParameter guid = dbhelper.factory.CreateParameter();
            guid.ParameterName = "@guid";
            guid.Value = model.Guid;

            DbParameter drivewayName = dbhelper.factory.CreateParameter();
            drivewayName.ParameterName = "@drivewayName";
            drivewayName.Value = model.DrivewayName;

            DbParameter type = dbhelper.factory.CreateParameter();
            type.ParameterName = "@type";
            type.Value = model.Type;

            DbParameter deviceName = dbhelper.factory.CreateParameter();
            deviceName.ParameterName = "@deviceName";
            deviceName.Value = model.DeviceName;

            DbParameter deviceMacAddress = dbhelper.factory.CreateParameter();
            deviceMacAddress.ParameterName = "@deviceMacAddress";
            deviceMacAddress.Value = model.DeviceMacAddress;

            DbParameter deviceEntranceURI = dbhelper.factory.CreateParameter();
            deviceEntranceURI.ParameterName = "@deviceEntranceURI";
            deviceEntranceURI.Value = model.DeviceEntranceURI;

            DbParameter deviceAccount = dbhelper.factory.CreateParameter();
            deviceAccount.ParameterName = "@deviceAccount";
            deviceAccount.Value = model.DeviceAccount;

            DbParameter deviceVerification = dbhelper.factory.CreateParameter();
            deviceVerification.ParameterName = "@deviceVerification";
            deviceVerification.Value = model.DeviceVerification;

            DbParameter[] parameter = new DbParameter[] { projectGuid, parkCode, guid, drivewayName, type, deviceName, deviceMacAddress, deviceEntranceURI, deviceAccount, deviceVerification };
            return dbhelper.ExecuteNonQuery(commandtext, parameter) > 0 ? true : false;
        }

        public bool ToggleValue(DrivewayModel model)
        {
            return false;
        }

        public string GetGuidByKey(string key)
        {
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            object guid = dbhelper.ExecuteScalar(string.Format("select guid from t_driveway where deviceMacAddress='{0}'", key));
            return guid == null ? "" : guid.ToString();
        }

        List<DrivewayModel> IExtenDataBaseOperate<DrivewayModel>.GetListByKeys(string key)
        {
            List<DrivewayModel> model = new List<DrivewayModel>();
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            DataTable table = dbhelper.ExecuteDataTable(string.Format("select * from t_driveway where guid in({0})", key));
            foreach (DataRow item in table.Rows)
            {
                model.Add(new DrivewayModel()
                {
                    ProjectGuid = (string)item["projectGuid"],
                    ParkCode = (string)item["parkCode"],
                    Guid = (string)item["guid"],
                    DrivewayName = (string)item["drivewayName"],
                    Type = (DrivewayType)(int)item["type"],
                    DeviceName = (string)item["deviceName"],
                    DeviceMacAddress = (string)item["deviceMacAddress"],
                    DeviceEntranceURI = (string)item["deviceEntranceURI"],
                    DeviceAccount = (string)item["deviceAccount"],
                    DeviceVerification = (string)item["deviceVerification"],
                });
            }
            return model;
        }

    }
}
