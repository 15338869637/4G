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
 * *        File Name        : 
 * *        Creator          : llp
 * *        Create Time      : 2019-10-18 
 * *        Remark           : 卡类数据库交互管理
 * *
 * *  Copyright (c) 深圳市富士智能系统有限公司.  All rights reserved. 
 * ***************************************************************************************/
namespace Fujica.com.cn.DataService.DataBase
{

    /// <summary>
    /// 月卡数据库管理
    /// </summary>
    /// <remarks>
    /// 2019.10.18: 修改.新增注释信息,方法命名规则修改 LLP <br/> 
    /// 2019.10.25: 修改.月卡、临时卡、储值卡分页搜索增加卡类查询条件 Ase <br/> 
    /// </remarks>
    public class MonthCardServicePersistent : IBaseDataBaseOperate<CardServiceModel>, IExtenDataBaseOperate<CardServiceModel>
    {
        /// <summary>
        /// 日志记录器
        /// </summary>
        internal readonly ILogger m_logger;

        /// <summary>
        /// 序列化器
        /// </summary>
        internal readonly ISerializer m_serializer = null;

        public MonthCardServicePersistent(ILogger _logger, ISerializer _serializer)
        {
            m_logger = _logger;
            m_serializer = _serializer;
        }

        public bool DeleteInDataBase(CardServiceModel model)
        {
            try
            {
                MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
                //月卡车
                int ret = dbhelper.ExecuteNonQuery(string.Format("delete from t_monthcard where identifying='{0}'",
                    model.ParkCode + Convert.ToBase64String(Encoding.UTF8.GetBytes(model.CarNo))));
                if (ret > 0) return true;
            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.DataService, "", "", "", "Fujica.com.cn.DataService.MonthCardServicePersistent.DeleteInDataBase", string.Format("删除卡信息时发生异常，入参:{0}", m_serializer.Serialize(model)), ex.ToString());
            }
            return false;
        }

        public bool DeleteMostInDataBase(IList<CardServiceModel> modlelist)
        {
            return false;
        }

        public void Dispose()
        {
            return;
        }

        public CardServiceModel GetFromDataBase(string identifying)
        {
            try
            {
                MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
                //查月卡表
                DataTable table = new DataTable();
                table = dbhelper.ExecuteDataTable(string.Format("select * from t_monthcard where identifying='{0}'", identifying));
                if (table.Rows.Count > 0)
                {
                    CardServiceModel model = new CardServiceModel();
                    model.ProjectGuid = (string)table.Rows[0]["projectGuid"];
                    model.ParkCode = (string)table.Rows[0]["parkCode"];
                    model.CarNo = (string)table.Rows[0]["carNo"];
                    model.CarOwnerName = (string)table.Rows[0]["carOwnerName"];
                    model.Mobile = (string)table.Rows[0]["mobile"];
                    model.DrivewayGuidList = m_serializer.Deserialize<List<string>>(table.Rows[0]["drivewayListContent"].ToString());
                    model.CarTypeGuid = (string)table.Rows[0]["carTypeGuid"];
                    model.Remark = table.Rows[0]["remark"].ToString();
                    model.Enable = (table.Rows[0]["enable"].ToString() == "0" ? false : true);
                    model.Locked = (table.Rows[0]["locked"].ToString() == "0" ? false : true);
                    if (!string.IsNullOrWhiteSpace(table.Rows[0]["startDate"].ToString()))
                        model.StartDate = DateTime.Parse(table.Rows[0]["startDate"].ToString());
                    if (!string.IsNullOrWhiteSpace(table.Rows[0]["endDate"].ToString()))
                        model.EndDate = DateTime.Parse(table.Rows[0]["endDate"].ToString());
                    if (!string.IsNullOrWhiteSpace(table.Rows[0]["pauseDate"].ToString()))
                        model.PauseDate = DateTime.Parse(table.Rows[0]["pauseDate"].ToString());
                    if (!string.IsNullOrWhiteSpace(table.Rows[0]["continueDate"].ToString()))
                        model.ContinueDate = DateTime.Parse(table.Rows[0]["continueDate"].ToString());
                    model.Balance = 0;
                    return model;
                }
            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.DataService, "", "", "", "Fujica.com.cn.DataService.MonthCardServicePersistent.GetFromDataBase", string.Format("获取卡信息时发生异常，入参:{0}", identifying), ex.ToString());
            }
            return null;
        }

        public IList<CardServiceModel> GetMostFromDataBase(string parkcode)
        {
            List<CardServiceModel> model = new List<CardServiceModel>();
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            DataTable table = new DataTable();
            //月卡表
            table = dbhelper.ExecuteDataTable(string.Format("select * from t_monthcard where parkCode='{0}'", parkcode));
            foreach (DataRow item in table.Rows)
            {
                CardServiceModel csm = new CardServiceModel()
                {
                    ProjectGuid = (string)item["projectGuid"],
                    ParkCode = (string)item["parkCode"],
                    CarNo = (string)item["carNo"],
                    CarOwnerName = (string)item["carOwnerName"],
                    Mobile = (string)item["mobile"],
                    DrivewayGuidList = m_serializer.Deserialize<List<string>>(item["drivewayListContent"].ToString()),
                    CarTypeGuid = (string)item["carTypeGuid"],
                    Remark = item["remark"].ToString(),
                    Enable = (item["enable"].ToString() == "0" ? false : true),
                    Locked = (item["locked"].ToString() == "0" ? false : true),
                    Balance = 0
                };
                if (!string.IsNullOrWhiteSpace(item["startDate"].ToString()))
                    csm.StartDate = DateTime.Parse(item["startDate"].ToString());
                if (!string.IsNullOrWhiteSpace(item["endDate"].ToString()))
                    csm.EndDate = DateTime.Parse(item["endDate"].ToString());
                if (!string.IsNullOrWhiteSpace(item["pauseDate"].ToString()))
                    csm.PauseDate = DateTime.Parse(item["pauseDate"].ToString());
                if (!string.IsNullOrWhiteSpace(item["continueDate"].ToString()))
                    csm.ContinueDate = DateTime.Parse(item["continueDate"].ToString());
                model.Add(csm);
            }
            table.Clear();
            return model;
        }

        public IList<CardServiceModel> GetFromDataBaseByPage<T>(T model)
        {
            CardServiceSearchModel searchModel = model as CardServiceSearchModel;

            List<CardServiceModel> list = null;
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            DataTable table = new DataTable();
            //临时卡表查询cmd
            string cmdText = " SELECT * FROM t_monthcard INNER JOIN (SELECT id FROM t_monthcard  {0}"
                           + $" ORDER BY id DESC LIMIT {(searchModel.PageIndex - 1) * searchModel.PageSize},{searchModel.PageSize}) AS page USING(id); ";
            //查询条件cmd 
            string whereText = $" WHERE projectGuid = '{searchModel.ProjectGuid}' and parkCode = '{searchModel.ParkingCode}' ";

            if (!string.IsNullOrEmpty(searchModel.CarNo))
                whereText += $" and instr(carNo,'{searchModel.CarNo}')> 0 ";
            if (!string.IsNullOrEmpty(searchModel.Mobile))
                whereText += $" and mobile = '{searchModel.Mobile}' ";
            if (!string.IsNullOrEmpty(searchModel.CarOwnerName))
                whereText += $" and carOwnerName = '{searchModel.CarOwnerName}' ";
            if (searchModel.StartDate != null && searchModel.StartDate != DateTime.MinValue)
                whereText += $" and DATE_FORMAT(startDate,'%Y-%m-%d') = '{searchModel.StartDate.Value.ToString("yyyy-MM-dd")}' ";
            if (searchModel.Locked != null)
                whereText += $" and locked = {(searchModel.Locked.Value ? 1 : 0)} ";
            if (!string.IsNullOrEmpty(searchModel.CarTypeGuid))
                whereText += $" and carTypeGuid = '{searchModel.CarTypeGuid}' ";

            cmdText = string.Format(cmdText, whereText);

            //总条数cmd
            string cmdTextCount = "SELECT COUNT(*) FROM t_monthcard " + whereText;

            long count = (long)dbhelper.ExecuteScalar(cmdTextCount);
            if (count <= 0) return list;
            searchModel.TotalCount = count;

            table = dbhelper.ExecuteDataTable(cmdText);

            if (table == null || table.Rows.Count <= 0)
                return list;

            list = new List<CardServiceModel>();
            foreach (DataRow item in table.Rows)
            {
                CardServiceModel csm = new CardServiceModel()
                {
                    ProjectGuid = (string)item["projectGuid"],
                    ParkCode = (string)item["parkCode"],
                    CarNo = (string)item["carNo"],
                    CarOwnerName = (string)item["carOwnerName"],
                    Mobile = (string)item["mobile"],
                    DrivewayGuidList = m_serializer.Deserialize<List<string>>(item["drivewayListContent"].ToString()),
                    CarTypeGuid = (string)item["carTypeGuid"],
                    Remark = item["remark"].ToString(),
                    Enable = (item["enable"].ToString() == "0" ? false : true),
                    Locked = (item["locked"].ToString() == "0" ? false : true),
                    Balance = 0,
                    RechargeOperator=item["rechargeOperator"].ToString()
                };
                if (!string.IsNullOrWhiteSpace(item["startDate"].ToString()))
                    csm.StartDate = DateTime.Parse(item["startDate"].ToString());
                if (!string.IsNullOrWhiteSpace(item["endDate"].ToString()))
                    csm.EndDate = DateTime.Parse(item["endDate"].ToString());
                if (!string.IsNullOrWhiteSpace(item["pauseDate"].ToString()))
                    csm.PauseDate = DateTime.Parse(item["pauseDate"].ToString());
                if (!string.IsNullOrWhiteSpace(item["continueDate"].ToString()))
                    csm.ContinueDate = DateTime.Parse(item["continueDate"].ToString());
                list.Add(csm);
            }
            table.Clear();
            return list;
        }

        public bool SaveToDataBase(CardServiceModel model)
        {
            try
            {
                MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
                //月卡车
                string commandtext = @"replace into t_monthcard(projectGuid,identifying,parkCode,carNo,carOwnerName,mobile,carTypeGuid,remark,enable,locked,startDate,endDate,pauseDate,continueDate,drivewayListContent,rechargeOperator)
                                values(@projectGuid,@identifying,@parkCode,@carNo,@carOwnerName,@mobile,@carTypeGuid,@remark,@enable,@locked,@startDate,@endDate,@pauseDate,@continueDate,@drivewayListContent,@rechargeOperator)";

                DbParameter projectGuid = dbhelper.factory.CreateParameter();
                projectGuid.ParameterName = "@projectGuid";
                projectGuid.Value = model.ProjectGuid;

                DbParameter identifying = dbhelper.factory.CreateParameter();
                identifying.ParameterName = "@identifying";
                identifying.Value = model.ParkCode + Convert.ToBase64String(Encoding.UTF8.GetBytes(model.CarNo));

                DbParameter parkCode = dbhelper.factory.CreateParameter();
                parkCode.ParameterName = "@parkCode";
                parkCode.Value = model.ParkCode;

                DbParameter carNo = dbhelper.factory.CreateParameter();
                carNo.ParameterName = "@carNo";
                carNo.Value = model.CarNo;

                DbParameter carOwnerName = dbhelper.factory.CreateParameter();
                carOwnerName.ParameterName = "@carOwnerName";
                carOwnerName.Value = model.CarOwnerName;

                DbParameter mobile = dbhelper.factory.CreateParameter();
                mobile.ParameterName = "@mobile";
                mobile.Value = model.Mobile;

                DbParameter carTypeGuid = dbhelper.factory.CreateParameter();
                carTypeGuid.ParameterName = "@carTypeGuid";
                carTypeGuid.Value = model.CarTypeGuid;

                DbParameter remark = dbhelper.factory.CreateParameter();
                remark.ParameterName = "@remark";
                remark.Value = model.Remark;

                DbParameter enable = dbhelper.factory.CreateParameter();
                enable.ParameterName = "@enable";
                enable.Value = model.Enable;

                DbParameter locked = dbhelper.factory.CreateParameter();
                locked.ParameterName = "@locked";
                locked.Value = model.Locked;

                DbParameter startDate = dbhelper.factory.CreateParameter();
                startDate.ParameterName = "@startDate";
                startDate.Value = model.StartDate.ToString("yyyy-MM-dd HH:mm:ss");

                DbParameter endDate = dbhelper.factory.CreateParameter();
                endDate.ParameterName = "@endDate";
                endDate.Value = model.EndDate.ToString("yyyy-MM-dd HH:mm:ss");

                DbParameter pauseDate = dbhelper.factory.CreateParameter();
                pauseDate.ParameterName = "@pauseDate";
                pauseDate.Value = model.PauseDate.ToString("yyyy-MM-dd HH:mm:ss");

                DbParameter continueDate = dbhelper.factory.CreateParameter();
                continueDate.ParameterName = "@continueDate";
                continueDate.Value = model.ContinueDate.ToString("yyyy-MM-dd HH:mm:ss");

                DbParameter drivewayListContent = dbhelper.factory.CreateParameter();
                drivewayListContent.ParameterName = "@drivewayListContent";
                drivewayListContent.Value = m_serializer.Serialize(model.DrivewayGuidList ?? new List<string>());


                DbParameter rechargeOperator = dbhelper.factory.CreateParameter();
                rechargeOperator.ParameterName = "@rechargeOperator";
                rechargeOperator.Value = model.RechargeOperator;

                DbParameter[] parameter = new DbParameter[] { projectGuid, identifying, parkCode, carNo, carOwnerName, mobile, carTypeGuid, remark, enable, locked, startDate, endDate, pauseDate, continueDate, drivewayListContent, rechargeOperator };
                return dbhelper.ExecuteNonQuery(commandtext, parameter) > 0 ? true : false;
            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.DataService, "", "", "", "Fujica.com.cn.DataService.MonthCardServicePersistent.SaveToDataBase", string.Format("保存卡信息时发生异常，入参:{0}", m_serializer.Serialize(model)), ex.ToString());
            }
            return false;
        }

        public string GetGuidByKey(string key)
        {
            return "";
        }

        public bool ToggleValue(CardServiceModel model)
        {
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            //月卡车
            int ret = dbhelper.ExecuteNonQuery(string.Format("update t_monthcard set locked={0} where identifying='{1}'", model.Locked ? 1 : 0, model.ParkCode + Convert.ToBase64String(Encoding.UTF8.GetBytes(model.CarNo)))); //
            if (ret > 0) return true;
            return false;
        }

        public List<CardServiceModel> GetListByKeys(string key)
        {
            throw new NotImplementedException();
        }

    }

    /// <summary>
    /// 储值卡数据库管理
    /// </summary>
    public class ValueCardServicePersistent : IBaseDataBaseOperate<CardServiceModel>, IExtenDataBaseOperate<CardServiceModel>
    {
        /// <summary>
        /// 日志记录器
        /// </summary>
        internal readonly ILogger m_logger;

        /// <summary>
        /// 序列化器
        /// </summary>
        internal readonly ISerializer m_serializer = null;

        public ValueCardServicePersistent(ILogger _logger, ISerializer _serializer)
        {
            m_logger = _logger;
            m_serializer = _serializer;
        }

        public bool DeleteInDataBase(CardServiceModel model)
        {
            try
            {
                MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
                //储值车
                int ret = dbhelper.ExecuteNonQuery(string.Format("delete from t_valuecard where identifying='{0}'",
                    model.ParkCode + Convert.ToBase64String(Encoding.UTF8.GetBytes(model.CarNo))));
                if (ret > 0) return true;
            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.DataService, "", "", "", "Fujica.com.cn.DataService.ValueCardServicePersistent.DeleteInDataBase", string.Format("删除卡信息时发生异常，入参:{0}", m_serializer.Serialize(model)), ex.ToString());
            }
            return false;
        }

        public bool DeleteMostInDataBase(IList<CardServiceModel> modlelist)
        {
            return false;
        }

        public void Dispose()
        {
            return;
        }

        public CardServiceModel GetFromDataBase(string identifying)
        {
            try
            {
                MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
                DataTable table = new DataTable();
                table = dbhelper.ExecuteDataTable(string.Format("select * from t_valuecard where identifying='{0}'", identifying));
                if (table.Rows.Count > 0)
                {
                    CardServiceModel model = new CardServiceModel();
                    model.ProjectGuid = (string)table.Rows[0]["projectGuid"];
                    model.ParkCode = (string)table.Rows[0]["parkCode"];
                    model.CarNo = (string)table.Rows[0]["carNo"];
                    model.CarOwnerName = (string)table.Rows[0]["carOwnerName"];
                    model.Mobile = (string)table.Rows[0]["mobile"];
                    model.DrivewayGuidList = m_serializer.Deserialize<List<string>>(table.Rows[0]["drivewayListContent"].ToString());
                    model.CarTypeGuid = (string)table.Rows[0]["carTypeGuid"];
                    model.Remark = table.Rows[0]["remark"].ToString();
                    model.Enable = (table.Rows[0]["enable"].ToString() == "0" ? false : true);
                    model.Locked = (table.Rows[0]["locked"].ToString() == "0" ? false : true);
                    model.Balance = (decimal)table.Rows[0]["balance"];
                    if (!string.IsNullOrWhiteSpace(table.Rows[0]["startDate"].ToString()))
                        model.StartDate = DateTime.Parse(table.Rows[0]["startDate"].ToString());
                    return model;
                }
            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.DataService, "", "", "", "Fujica.com.cn.DataService.ValueCardServicePersistent.GetFromDataBase", string.Format("获取卡信息时发生异常，入参:{0}", identifying), ex.ToString());
            }
            return null;
        }

        public IList<CardServiceModel> GetMostFromDataBase(string parkcode)
        {
            List<CardServiceModel> model = new List<CardServiceModel>();
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            DataTable table = new DataTable();
            
            //储值卡表
            table = dbhelper.ExecuteDataTable(string.Format("select * from t_valuecard where parkCode='{0}'", parkcode));
            foreach (DataRow item in table.Rows)
            {
                CardServiceModel csv = new CardServiceModel()
                {
                    ProjectGuid = (string)item["projectGuid"],
                    ParkCode = (string)item["parkCode"],
                    CarNo = (string)item["carNo"],
                    CarOwnerName = (string)item["carOwnerName"],
                    Mobile = (string)item["mobile"],
                    DrivewayGuidList = m_serializer.Deserialize<List<string>>(item["drivewayListContent"].ToString()),
                    CarTypeGuid = (string)item["carTypeGuid"],
                    Remark = item["remark"].ToString(),
                    Enable = (item["enable"].ToString() == "0" ? false : true),
                    Locked = (item["locked"].ToString() == "0" ? false : true),
                    Balance = (decimal)item["balance"]
                };
                if (!string.IsNullOrWhiteSpace(item["startDate"].ToString()))
                    csv.StartDate = DateTime.Parse(item["startDate"].ToString());
                model.Add(csv);
            }
            table.Clear();
            //贵宾卡表
            return model;
        }

        public IList<CardServiceModel> GetFromDataBaseByPage<T>(T model)
        {
            CardServiceSearchModel searchModel = model as CardServiceSearchModel;

            List<CardServiceModel> list = null;
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            DataTable table = new DataTable();
            //临时卡表查询cmd
            string cmdText = " SELECT * FROM t_valuecard INNER JOIN (SELECT id FROM t_valuecard  {0}"
                           + $" ORDER BY id DESC LIMIT {(searchModel.PageIndex - 1) * searchModel.PageSize},{searchModel.PageSize}) AS page USING(id); ";
            //查询条件cmd 
            string whereText = $" WHERE projectGuid = '{searchModel.ProjectGuid}' and parkCode = '{searchModel.ParkingCode}' ";

            if (!string.IsNullOrEmpty(searchModel.CarNo))
                whereText += $" and instr(carNo,'{searchModel.CarNo}')> 0 ";
            if (!string.IsNullOrEmpty(searchModel.Mobile))
                whereText += $" and mobile = '{searchModel.Mobile}' ";
            if (!string.IsNullOrEmpty(searchModel.CarOwnerName))
                whereText += $" and carOwnerName = '{searchModel.CarOwnerName}' ";
            if (searchModel.StartDate != null && searchModel.StartDate != DateTime.MinValue)
                whereText += $" and DATE_FORMAT(startDate,'%Y-%m-%d') = '{searchModel.StartDate.Value.ToString("yyyy-MM-dd")}' ";
            if (searchModel.Locked != null)
                whereText += $" and locked = {(searchModel.Locked.Value ? 1 : 0)} ";
            if (!string.IsNullOrEmpty(searchModel.CarTypeGuid))
                whereText += $" and carTypeGuid = '{searchModel.CarTypeGuid}' ";

            cmdText = string.Format(cmdText, whereText);

            //总条数cmd
            string cmdTextCount = "SELECT COUNT(*) FROM t_valuecard " + whereText;

            long count = (long)dbhelper.ExecuteScalar(cmdTextCount);
            if (count <= 0) return list;
            searchModel.TotalCount = count;

            table = dbhelper.ExecuteDataTable(cmdText);

            if (table == null || table.Rows.Count <= 0)
                return list;

            list = new List<CardServiceModel>();
            foreach (DataRow item in table.Rows)
            {
                CardServiceModel csm = new CardServiceModel()
                {
                    ProjectGuid = (string)item["projectGuid"],
                    ParkCode = (string)item["parkCode"],
                    CarNo = (string)item["carNo"],
                    CarOwnerName = (string)item["carOwnerName"],
                    Mobile = (string)item["mobile"],
                    DrivewayGuidList = m_serializer.Deserialize<List<string>>(item["drivewayListContent"].ToString()),
                    CarTypeGuid = (string)item["carTypeGuid"],
                    Remark = item["remark"].ToString(),
                    Enable = (item["enable"].ToString() == "0" ? false : true),
                    Locked = (item["locked"].ToString() == "0" ? false : true),
                    Balance = (decimal)item["balance"],
                    RechargeOperator = item["rechargeOperator"].ToString()
                };
                if (!string.IsNullOrWhiteSpace(item["startDate"].ToString()))
                    csm.StartDate = DateTime.Parse(item["startDate"].ToString());
                list.Add(csm);
            }
            table.Clear();
            return list;
        }

        public bool SaveToDataBase(CardServiceModel model)
        {
            try
            {
                MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
                
                //储值车
                string commandtext = @"replace into t_valuecard(projectGuid,identifying,parkCode,carNo,carOwnerName,mobile,carTypeGuid,remark,enable,locked,startDate,balance,drivewayListContent,rechargeOperator)
                                values(@projectGuid,@identifying,@parkCode,@carNo,@carOwnerName,@mobile,@carTypeGuid,@remark,@enable,@locked,@startDate,@balance,@drivewayListContent,@rechargeOperator)";

                DbParameter projectGuid = dbhelper.factory.CreateParameter();
                projectGuid.ParameterName = "@projectGuid";
                projectGuid.Value = model.ProjectGuid;

                DbParameter identifying = dbhelper.factory.CreateParameter();
                identifying.ParameterName = "@identifying";
                identifying.Value = model.ParkCode + Convert.ToBase64String(Encoding.UTF8.GetBytes(model.CarNo));

                DbParameter parkCode = dbhelper.factory.CreateParameter();
                parkCode.ParameterName = "@parkCode";
                parkCode.Value = model.ParkCode;

                DbParameter carNo = dbhelper.factory.CreateParameter();
                carNo.ParameterName = "@carNo";
                carNo.Value = model.CarNo;

                DbParameter carOwnerName = dbhelper.factory.CreateParameter();
                carOwnerName.ParameterName = "@carOwnerName";
                carOwnerName.Value = model.CarOwnerName;

                DbParameter mobile = dbhelper.factory.CreateParameter();
                mobile.ParameterName = "@mobile";
                mobile.Value = model.Mobile;

                DbParameter carTypeGuid = dbhelper.factory.CreateParameter();
                carTypeGuid.ParameterName = "@carTypeGuid";
                carTypeGuid.Value = model.CarTypeGuid;

                DbParameter remark = dbhelper.factory.CreateParameter();
                remark.ParameterName = "@remark";
                remark.Value = model.Remark;

                DbParameter enable = dbhelper.factory.CreateParameter();
                enable.ParameterName = "@enable";
                enable.Value = model.Enable;

                DbParameter locked = dbhelper.factory.CreateParameter();
                locked.ParameterName = "@locked";
                locked.Value = model.Locked;

                DbParameter startDate = dbhelper.factory.CreateParameter();
                startDate.ParameterName = "@startDate";
                startDate.Value = model.StartDate.ToString("yyyy-MM-dd HH:mm:ss");

                DbParameter balance = dbhelper.factory.CreateParameter();
                balance.ParameterName = "@balance";
                balance.Value = model.Balance;

                DbParameter drivewayListContent = dbhelper.factory.CreateParameter();
                drivewayListContent.ParameterName = "@drivewayListContent";
                drivewayListContent.Value = m_serializer.Serialize(model.DrivewayGuidList ?? new List<string>());

                DbParameter rechargeOperator = dbhelper.factory.CreateParameter();
                rechargeOperator.ParameterName = "@rechargeOperator";
                rechargeOperator.Value = model.RechargeOperator;

                DbParameter[] parameter = new DbParameter[] { projectGuid, identifying, parkCode, carNo, carOwnerName, mobile, carTypeGuid, remark, enable, locked, startDate, balance, drivewayListContent, rechargeOperator };
                return dbhelper.ExecuteNonQuery(commandtext, parameter) > 0 ? true : false;
            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.DataService, "", "", "", "Fujica.com.cn.DataService.ValueCardServicePersistent.SaveToDataBase", string.Format("保存卡信息时发生异常，入参:{0}", m_serializer.Serialize(model)), ex.ToString());
            }
            return false;
        }

        public string GetGuidByKey(string key)
        {
            return "";
        }

        public bool ToggleValue(CardServiceModel model)
        {
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            
            //储值卡
            int ret = dbhelper.ExecuteNonQuery(string.Format("update t_valuecard set locked={0} where identifying='{1}'", model.Locked ? 1 : 0, model.ParkCode + Convert.ToBase64String(Encoding.UTF8.GetBytes(model.CarNo)))); //
            if (ret > 0) return true;
            return false;
        }

        public List<CardServiceModel> GetListByKeys(string key)
        {
            throw new NotImplementedException();
        }

    }

    /// <summary>
    /// 临时卡数据库管理
    /// </summary>
    public class TempCardServicePersistent : IBaseDataBaseOperate<CardServiceModel>, IExtenDataBaseOperate<CardServiceModel>
    {
        /// <summary>
        /// 日志记录器
        /// </summary>
        internal readonly ILogger m_logger;

        /// <summary>
        /// 序列化器
        /// </summary>
        internal readonly ISerializer m_serializer = null;

        public TempCardServicePersistent(ILogger _logger, ISerializer _serializer)
        {
            m_logger = _logger;
            m_serializer = _serializer;
        }

        public bool DeleteInDataBase(CardServiceModel model)
        {
            try
            {
                MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
                //月卡车
                int ret = dbhelper.ExecuteNonQuery(string.Format("delete from t_tempcard where identifying='{0}'",
                    model.ParkCode + Convert.ToBase64String(Encoding.UTF8.GetBytes(model.CarNo))));
                if (ret > 0) return true;
            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.DataService, "", "", "", "Fujica.com.cn.DataService.TempCardServicePersistent.DeleteInDataBase", string.Format("删除卡信息时发生异常，入参:{0}", m_serializer.Serialize(model)), ex.ToString());
            }
            return false;
        }

        public bool DeleteMostInDataBase(IList<CardServiceModel> modlelist)
        {
            return false;
        }

        public void Dispose()
        {
            return;
        }

        public CardServiceModel GetFromDataBase(string identifying)
        {
            try
            {
                MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
                //查月卡表
                DataTable table = new DataTable();
                table = dbhelper.ExecuteDataTable(string.Format("select * from t_tempcard where identifying='{0}'", identifying));
                if (table.Rows.Count > 0)
                {
                    CardServiceModel model = new CardServiceModel();
                    model.ProjectGuid = (string)table.Rows[0]["projectGuid"];
                    model.ParkCode = (string)table.Rows[0]["parkCode"];
                    model.CarNo = (string)table.Rows[0]["carNo"];
                    model.CarOwnerName = (string)table.Rows[0]["carOwnerName"];
                    model.Mobile = (string)table.Rows[0]["mobile"];
                    model.DrivewayGuidList = m_serializer.Deserialize<List<string>>(table.Rows[0]["drivewayListContent"].ToString());
                    model.CarTypeGuid = (string)table.Rows[0]["carTypeGuid"];
                    model.Remark = table.Rows[0]["remark"].ToString();
                    model.Enable = (table.Rows[0]["enable"].ToString() == "0" ? false : true);
                    model.Locked = (table.Rows[0]["locked"].ToString() == "0" ? false : true);
                    if (!string.IsNullOrWhiteSpace(table.Rows[0]["startDate"].ToString()))
                        model.StartDate = DateTime.Parse(table.Rows[0]["startDate"].ToString());
                    if (!string.IsNullOrWhiteSpace(table.Rows[0]["endDate"].ToString()))
                        model.EndDate = DateTime.Parse(table.Rows[0]["endDate"].ToString());
                    if (!string.IsNullOrWhiteSpace(table.Rows[0]["pauseDate"].ToString()))
                        model.PauseDate = DateTime.Parse(table.Rows[0]["pauseDate"].ToString());
                    if (!string.IsNullOrWhiteSpace(table.Rows[0]["continueDate"].ToString()))
                        model.ContinueDate = DateTime.Parse(table.Rows[0]["continueDate"].ToString());
                    model.Balance = 0;
                    return model;
                }
            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.DataService, "", "", "", "Fujica.com.cn.DataService.TempCardServicePersistent.GetFromDataBase", string.Format("获取卡信息时发生异常，入参:{0}", identifying), ex.ToString());
            }
            return null;
        }

        public IList<CardServiceModel> GetMostFromDataBase(string parkcode)
        {
            List<CardServiceModel> model = new List<CardServiceModel>();
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            DataTable table = new DataTable();
            //临时卡表
            table = dbhelper.ExecuteDataTable(string.Format("select * from t_tempcard where parkCode='{0}'", parkcode));
            foreach (DataRow item in table.Rows)
            {
                CardServiceModel csm = new CardServiceModel()
                {
                    ProjectGuid = (string)item["projectGuid"],
                    ParkCode = (string)item["parkCode"],
                    CarNo = (string)item["carNo"],
                    CarOwnerName = (string)item["carOwnerName"],
                    Mobile = (string)item["mobile"],
                    DrivewayGuidList = m_serializer.Deserialize<List<string>>(item["drivewayListContent"].ToString()),
                    CarTypeGuid = (string)item["carTypeGuid"],
                    Remark = item["remark"].ToString(),
                    Enable = (item["enable"].ToString() == "0" ? false : true),
                    Locked = (item["locked"].ToString() == "0" ? false : true),
                    Balance = 0
                };
                if (!string.IsNullOrWhiteSpace(item["startDate"].ToString()))
                    csm.StartDate = DateTime.Parse(item["startDate"].ToString());
                if (!string.IsNullOrWhiteSpace(item["endDate"].ToString()))
                    csm.EndDate = DateTime.Parse(item["endDate"].ToString());
                if (!string.IsNullOrWhiteSpace(item["pauseDate"].ToString()))
                    csm.PauseDate = DateTime.Parse(item["pauseDate"].ToString());
                if (!string.IsNullOrWhiteSpace(item["continueDate"].ToString()))
                    csm.ContinueDate = DateTime.Parse(item["continueDate"].ToString());
                model.Add(csm);
            }
            table.Clear();
            return model;
        }

        public IList<CardServiceModel> GetFromDataBaseByPage<T>(T model)
        {
            CardServiceSearchModel searchModel = model as CardServiceSearchModel;

            List<CardServiceModel> list = null;
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            DataTable table = new DataTable();
            //临时卡表查询cmd
            string cmdText = " SELECT * FROM t_tempcard INNER JOIN (SELECT id FROM t_tempcard  {0}"
                           + $" ORDER BY id DESC LIMIT {(searchModel.PageIndex - 1) * searchModel.PageSize},{searchModel.PageSize}) AS page USING(id); ";
            //查询条件cmd 
            string whereText = $" WHERE projectGuid = '{searchModel.ProjectGuid}' and parkCode = '{searchModel.ParkingCode}' ";

            if (!string.IsNullOrEmpty(searchModel.CarNo))
                whereText += $" and instr(carNo,'{searchModel.CarNo}')> 0 ";
            if (!string.IsNullOrEmpty(searchModel.Mobile))
                whereText += $" and mobile = '{searchModel.Mobile}' ";
            if (!string.IsNullOrEmpty(searchModel.CarOwnerName))
                whereText += $" and carOwnerName = '{searchModel.CarOwnerName}' ";
            if (searchModel.StartDate != null && searchModel.StartDate != DateTime.MinValue)
                whereText += $" and DATE_FORMAT(startDate,'%Y-%m-%d') = '{searchModel.StartDate.Value.ToString("yyyy-MM-dd")}' ";
            if (searchModel.Locked != null)
                whereText += $" and locked = {(searchModel.Locked.Value ? 1 : 0)} ";
            if (!string.IsNullOrEmpty(searchModel.CarTypeGuid))
                whereText += $" and carTypeGuid = '{searchModel.CarTypeGuid}' ";

            cmdText = string.Format(cmdText, whereText);

            //总条数cmd
            string cmdTextCount = "SELECT COUNT(*) FROM t_tempcard " + whereText;

            long count = (long)dbhelper.ExecuteScalar(cmdTextCount);
            if (count <= 0) return list;
            searchModel.TotalCount = count;

            table = dbhelper.ExecuteDataTable(cmdText);

            if (table == null || table.Rows.Count <= 0)
                return list;

            list = new List<CardServiceModel>();
            foreach (DataRow item in table.Rows)
            {
                CardServiceModel csm = new CardServiceModel()
                {
                    ProjectGuid = (string)item["projectGuid"],
                    ParkCode = (string)item["parkCode"],
                    CarNo = (string)item["carNo"],
                    CarOwnerName = (string)item["carOwnerName"],
                    Mobile = (string)item["mobile"],
                    DrivewayGuidList = m_serializer.Deserialize<List<string>>(item["drivewayListContent"].ToString()),
                    CarTypeGuid = (string)item["carTypeGuid"],
                    Remark = item["remark"].ToString(),
                    Enable = (item["enable"].ToString() == "0" ? false : true),
                    Locked = (item["locked"].ToString() == "0" ? false : true),
                    Balance = 0,
                    RechargeOperator = item["rechargeOperator"].ToString()
                };
                if (!string.IsNullOrWhiteSpace(item["startDate"].ToString()))
                    csm.StartDate = DateTime.Parse(item["startDate"].ToString());
                if (!string.IsNullOrWhiteSpace(item["endDate"].ToString()))
                    csm.EndDate = DateTime.Parse(item["endDate"].ToString());
                if (!string.IsNullOrWhiteSpace(item["pauseDate"].ToString()))
                    csm.PauseDate = DateTime.Parse(item["pauseDate"].ToString());
                if (!string.IsNullOrWhiteSpace(item["continueDate"].ToString()))
                    csm.ContinueDate = DateTime.Parse(item["continueDate"].ToString());
                list.Add(csm);
            }
            table.Clear();
            return list;
        }

        public bool SaveToDataBase(CardServiceModel model)
        {
            try
            {
                MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
                //临时车
                string commandtext = @"replace into t_tempcard(projectGuid,identifying,parkCode,carNo,carOwnerName,mobile,carTypeGuid,remark,enable,locked,startDate,endDate,pauseDate,continueDate,drivewayListContent,rechargeOperator)
                                values(@projectGuid,@identifying,@parkCode,@carNo,@carOwnerName,@mobile,@carTypeGuid,@remark,@enable,@locked,@startDate,@endDate,@pauseDate,@continueDate,@drivewayListContent,@rechargeOperator)";

                DbParameter projectGuid = dbhelper.factory.CreateParameter();
                projectGuid.ParameterName = "@projectGuid";
                projectGuid.Value = model.ProjectGuid;

                DbParameter identifying = dbhelper.factory.CreateParameter();
                identifying.ParameterName = "@identifying";
                identifying.Value = model.ParkCode + Convert.ToBase64String(Encoding.UTF8.GetBytes(model.CarNo));

                DbParameter parkCode = dbhelper.factory.CreateParameter();
                parkCode.ParameterName = "@parkCode";
                parkCode.Value = model.ParkCode;

                DbParameter carNo = dbhelper.factory.CreateParameter();
                carNo.ParameterName = "@carNo";
                carNo.Value = model.CarNo;

                DbParameter carOwnerName = dbhelper.factory.CreateParameter();
                carOwnerName.ParameterName = "@carOwnerName";
                carOwnerName.Value = model.CarOwnerName;

                DbParameter mobile = dbhelper.factory.CreateParameter();
                mobile.ParameterName = "@mobile";
                mobile.Value = model.Mobile;

                DbParameter carTypeGuid = dbhelper.factory.CreateParameter();
                carTypeGuid.ParameterName = "@carTypeGuid";
                carTypeGuid.Value = model.CarTypeGuid;

                DbParameter remark = dbhelper.factory.CreateParameter();
                remark.ParameterName = "@remark";
                remark.Value = model.Remark;

                DbParameter enable = dbhelper.factory.CreateParameter();
                enable.ParameterName = "@enable";
                enable.Value = model.Enable;

                DbParameter locked = dbhelper.factory.CreateParameter();
                locked.ParameterName = "@locked";
                locked.Value = model.Locked;

                DbParameter startDate = dbhelper.factory.CreateParameter();
                startDate.ParameterName = "@startDate";
                startDate.Value = model.StartDate.ToString("yyyy-MM-dd HH:mm:ss");

                DbParameter endDate = dbhelper.factory.CreateParameter();
                endDate.ParameterName = "@endDate";
                endDate.Value = model.EndDate.ToString("yyyy-MM-dd HH:mm:ss");

                DbParameter pauseDate = dbhelper.factory.CreateParameter();
                pauseDate.ParameterName = "@pauseDate";
                pauseDate.Value = model.PauseDate.ToString("yyyy-MM-dd HH:mm:ss");

                DbParameter continueDate = dbhelper.factory.CreateParameter();
                continueDate.ParameterName = "@continueDate";
                continueDate.Value = model.ContinueDate.ToString("yyyy-MM-dd HH:mm:ss");

                DbParameter drivewayListContent = dbhelper.factory.CreateParameter();
                drivewayListContent.ParameterName = "@drivewayListContent";
                drivewayListContent.Value = m_serializer.Serialize(model.DrivewayGuidList ?? new List<string>());


                DbParameter rechargeOperator = dbhelper.factory.CreateParameter();
                rechargeOperator.ParameterName = "@rechargeOperator";
                rechargeOperator.Value = model.RechargeOperator;

                DbParameter[] parameter = new DbParameter[] { projectGuid, identifying, parkCode, carNo, carOwnerName, mobile, carTypeGuid, remark, enable, locked, startDate, endDate, pauseDate, continueDate, drivewayListContent, rechargeOperator };
                return dbhelper.ExecuteNonQuery(commandtext, parameter) > 0 ? true : false;
            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.DataService, "", "", "", "Fujica.com.cn.DataService.TempCardServicePersistent.SaveToDataBase", string.Format("保存卡信息时发生异常，入参:{0}", m_serializer.Serialize(model)), ex.ToString());
            }
            return false;
        }

        public string GetGuidByKey(string key)
        {
            return "";
        }

        public bool ToggleValue(CardServiceModel model)
        {
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            //月卡车
            int ret = dbhelper.ExecuteNonQuery(string.Format("update t_tempcard set locked={0} where identifying='{1}'", model.Locked ? 1 : 0, model.ParkCode + Convert.ToBase64String(Encoding.UTF8.GetBytes(model.CarNo)))); //
            if (ret > 0) return true;
            return false;
        }

        public List<CardServiceModel> GetListByKeys(string key)
        {
            throw new NotImplementedException();
        }

    }
}
