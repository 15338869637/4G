using Fujica.com.cn.BaseConnect;
using Fujica.com.cn.Context.Model;
using Fujica.com.cn.Logger;
using Fujica.com.cn.Tools;
using System;
using System.Collections.Generic;
using System.Data.Common;
/***************************************************************************************
 * *
 * *        File Name        : VoiceCommandPersistent.cs
 * *        Creator          : llp
 * *        Create Time      : 2019-10-18 
 * *        Remark           : 语言数据库交互管理
 * *
 * *  Copyright (c) 深圳市富士智能系统有限公司.  All rights reserved. 
 * ***************************************************************************************/
namespace Fujica.com.cn.DataService.DataBase
{
    /// <remarks>
    /// 2019.10.18: 修改.新增注释信息,方法命名规则修改 LLP <br/> 
    /// </remarks>
    public class VoiceCommandPersistent : IBaseDataBaseOperate<VoiceCommandModel>
    {
        /// <summary>
        /// 日志记录器
        /// </summary>
        internal readonly ILogger m_logger;

        /// <summary>
        /// 序列化器
        /// </summary>
        internal readonly ISerializer m_serializer = null;

        public VoiceCommandPersistent(ILogger _logger, ISerializer _serializer)
        {
            m_logger = _logger;
            m_serializer = _serializer;
        }

        public bool DeleteInDataBase(VoiceCommandModel model)
        {
            return false;
        }

        public bool DeleteMostInDataBase(IList<VoiceCommandModel> modlelist)
        {
            return false;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public VoiceCommandModel GetFromDataBase(string drivewayGuid)
        {
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            string jsonstr = (string)dbhelper.ExecuteScalar(string.Format("select entityContent from t_voicecommand where drivewayGuid='{0}'", drivewayGuid));
            if (string.IsNullOrWhiteSpace(jsonstr)) return null;
            VoiceCommandModel model = m_serializer.Deserialize<VoiceCommandModel>(jsonstr);
            return model;
        }

        public IList<VoiceCommandModel> GetFromDataBaseByPage<T>(T model)
        {
            throw new NotImplementedException();
        }

        public IList<VoiceCommandModel> GetMostFromDataBase(string commandText = "")
        {
            return new List<VoiceCommandModel>();
        }

        public bool SaveToDataBase(VoiceCommandModel model)
        {
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            string commandtext = @"replace into t_voicecommand(projectGuid,parkCode,drivewayGuid,entityContent) 
                                   values(@projectGuid,@parkCode,@drivewayGuid,@entityContent)";

            DbParameter projectGuid = dbhelper.factory.CreateParameter();
            projectGuid.ParameterName = "@projectGuid";
            projectGuid.Value = model.ProjectGuid;

            DbParameter parkCode = dbhelper.factory.CreateParameter();
            parkCode.ParameterName = "@parkCode";
            parkCode.Value = model.ParkCode;

            DbParameter drivewayGuid = dbhelper.factory.CreateParameter();
            drivewayGuid.ParameterName = "@drivewayGuid";
            drivewayGuid.Value = model.DrivewayGuid;

            DbParameter entityContent = dbhelper.factory.CreateParameter();
            entityContent.ParameterName = "@entityContent";
            entityContent.Value = m_serializer.Serialize(model);


            DbParameter[] parameter = new DbParameter[] { projectGuid, parkCode, drivewayGuid, entityContent };
            return dbhelper.ExecuteNonQuery(commandtext, parameter) > 0 ? true : false;
        }
    }
}
