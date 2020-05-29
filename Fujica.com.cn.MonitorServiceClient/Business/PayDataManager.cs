/***************************************************************************************
 * *
 * *        File Name        : PayDataManager.cs
 * *        Creator          : llp
 * *        Create Time      : 2019-09-21 
 * *        Remark           : 支付数据管理
 * *
 * *  Copyright (c) 深圳市富士智能系统有限公司.  All rights reserved. 
 * ***************************************************************************************/
using Fujica.com.cn.BaseConnect;
using Fujica.com.cn.Business.Base;
using Fujica.com.cn.Business.Model;
using Fujica.com.cn.Communication.RabbitMQ;
using Fujica.com.cn.Context.Model;
using Fujica.com.cn.Logger;
using Fujica.com.cn.MonitorServiceClient.Model;
using Fujica.com.cn.Tools;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace Fujica.com.cn.MonitorServiceClient.Business
{
    /// <summary>
    /// 支付数据管理（月卡延期、储值卡充值）.
    /// </summary>
    /// <remarks>
    /// 2019.09.21: 日志参数完善. llp <br/>  
    /// 2019.10.24: 增加订单号重复判断. Ase <br/> 
    public class PayDataManager : BaseBusiness
    {
        private static string mq_ListKey = "MQ_PayCard";
        public static ResponseCommon DataHandle(ILogger m_ilogger, ISerializer m_serializer)
        {
            ResponseCommon response = new ResponseCommon()
            {
                IsSuccess = false,
                MsgType = MsgType.PayCard
            };

            IDatabase db;
            db = RedisHelper.GetDatabase(4);
            string redisContent = db.ListLeftPop(mq_ListKey);
            if (string.IsNullOrEmpty(redisContent))
            {
                response.MessageContent = "redis数据库读取值为空";
                return response;
            }
            response.RedisContent = redisContent;
            m_ilogger.LogInfo(LoggerLogicEnum.Tools, "", "", "", "Fujica.com.cn.MonitorServiceClient.PayDataManager", "支付数据接收成功.原始数据：" + redisContent);

            IssuedRecord recordModel = m_serializer.Deserialize<IssuedRecord>(redisContent);
            if (recordModel == null)
            {
                response.MessageContent = "redis数据库读取值转换成实体失败：";
                return response;
            }
            if (string.IsNullOrEmpty(recordModel.ParkingCode)
                || string.IsNullOrEmpty(recordModel.CarNo)
                || recordModel.CardType <= 0
                || string.IsNullOrEmpty(recordModel.TradeNo)
                //|| string.IsNullOrEmpty(inmodel.Remark)
                )
            {
                response.MessageContent = "redis数据转换成实体后必要参数缺失";
                return response;
            }
            if (recordModel.CardType == 1)
            {
                if (recordModel.RenewDay <= 0 && recordModel.RenewDayType <= 0)
                {
                    response.MessageContent = "redis数据转换成实体后必要参数缺失";
                    return response;
                }
            }
            else if (recordModel.CardType == 2 && recordModel.Price <= 0)
            {
                response.MessageContent = "redis数据转换成实体后必要参数缺失";
                return response;
            }

            db = RedisHelper.GetDatabase(0);
            //判断当前订单号，是否已执行过
            string mqPayDataListKey = "PayDataList:" + recordModel.ParkingCode + ":" + DateTime.Now.ToString("yyyyMMdd");
            if (db.HashExists(mqPayDataListKey, recordModel.TradeNo))
            {
                response.MessageContent = "当前订单记录重复执行";
                m_ilogger.LogInfo(LoggerLogicEnum.Tools, recordModel.TradeNo, recordModel.ParkingCode, recordModel.CardNo, "Fujica.com.cn.MonitorServiceClient.PayDataManager", $"TradeNo:{recordModel.TradeNo} 当前订单记录重复执行");
                return response;
            }
            else
            {
                db.HashSet(mqPayDataListKey, recordModel.TradeNo, redisContent);
                db.KeyExpire(mqPayDataListKey, DateTime.Now.AddDays(1).Date);
            }

            string hashkey = recordModel.ParkingCode + Convert.ToBase64String(Encoding.UTF8.GetBytes(recordModel.CarNo));            
            CardServiceModel cardModel = m_serializer.Deserialize<CardServiceModel>(db.HashGet("PermanentCarList", hashkey));
            if (cardModel == null)
            {
                cardModel = GetCard(hashkey, m_ilogger, m_serializer);
                if (cardModel == null)
                {
                    response.MessageContent = "根据车场编号和车牌号，读取卡务模型为空";
                    m_ilogger.LogError(LoggerLogicEnum.Tools, recordModel.TradeNo, recordModel.ParkingCode, recordModel.CardNo, "Fujica.com.cn.MonitorServiceClient.PayDataManager", "根据车场编号和车牌号，读取卡务模型为空");
                    return response;
                }
            }

            CarTypeModel cartypemodel = m_serializer.Deserialize<CarTypeModel>(db.HashGet("CarTypeList", cardModel.CarTypeGuid));
            if (cartypemodel == null)
            {
                response.MessageContent = "根据车类Guid，读取车类模型为空";
                m_ilogger.LogError(LoggerLogicEnum.Tools, recordModel.TradeNo, recordModel.ParkingCode, recordModel.CardNo, "Fujica.com.cn.MonitorServiceClient.PayDataManager", "根据车类Guid，读取车类模型为空");
                return response;
            }

            try
            {
                if (recordModel.CardType == 1)
                {
                    //月卡延期（截止时间、延期金额、支付方式）
                    //暂时只控制截至日期，暂无日志记录，所以不需要存储延期金额和支付方式
                    cardModel.EndDate = FormatEndDate(cardModel.EndDate, recordModel.RenewDay, recordModel.RenewDayType);

                    bool flag = false;
                    //修改数据库
                    flag = SaveMonthCarToDB(cardModel, m_ilogger, m_serializer);
                    if (!flag)
                    {
                        response.MessageContent = "月卡保存数据库失败";
                        m_ilogger.LogError(LoggerLogicEnum.Tools, recordModel.TradeNo, recordModel.ParkingCode, recordModel.CardNo, "Fujica.com.cn.MonitorServiceClient.PayDataManager", "月卡保存数据库失败");
                        return response;
                    }
                    //修改redis
                    flag = SaveMonthCarToRedis(cardModel, m_ilogger, m_serializer);
                    if (!flag)
                    {
                        response.MessageContent = "月卡保存数据库成功，保存redis失败";
                        m_ilogger.LogError(LoggerLogicEnum.Tools, recordModel.TradeNo, recordModel.ParkingCode, recordModel.CardNo, "Fujica.com.cn.MonitorServiceClient.PayDataManager", "月卡保存数据库成功，保存redis失败");
                        return response;
                    }
                    //发送给相机
                    SendMonthCarToCameras(cardModel, m_ilogger, m_serializer);
                    //发送给Fujica
                    flag = SendMonthCarToFujica(cardModel, cartypemodel.Idx);
                    if (!flag)
                    {
                        response.MessageContent = "月卡保存数据库、redis成功，发送Fujica失败";
                        m_ilogger.LogError(LoggerLogicEnum.Tools, recordModel.TradeNo, recordModel.ParkingCode, recordModel.CardNo, "Fujica.com.cn.MonitorServiceClient.PayDataManager", "月卡保存数据库、redis成功，发送Fujica失败");
                        return response;
                    }
                    m_ilogger.LogInfo(LoggerLogicEnum.Tools, recordModel.TradeNo, recordModel.ParkingCode, recordModel.CardNo, "Fujica.com.cn.MonitorServiceClient.PayDataManager", "月卡保存数据库、redis、发送Fujica成功");
                }
                else if (recordModel.CardType == 2)
                {
                    //储值卡充值（充值金额、充值方式）
                    //暂时只控制余额，暂无日志记录，所以不需要操作充值金额和充值方式
                    cardModel.Balance += recordModel.Price;

                    bool flag = false;
                    //修改数据库、redis
                    flag = SaveValueCarToDB(cardModel, m_ilogger, m_serializer);
                    if (!flag)
                    {
                        response.MessageContent = "储值卡保存数据库失败";
                        m_ilogger.LogError(LoggerLogicEnum.Tools, recordModel.TradeNo, recordModel.ParkingCode, recordModel.CardNo, "Fujica.com.cn.MonitorServiceClient.PayDataManager", "储值卡保存数据库失败");
                        return response;
                    }

                    //修改redis
                    flag = SaveValueCarToRedis(cardModel, m_ilogger, m_serializer);
                    if (!flag)
                    {
                        response.MessageContent = "储值卡保存数据库成功，保存redis失败";
                        m_ilogger.LogError(LoggerLogicEnum.Tools, recordModel.TradeNo, recordModel.ParkingCode, recordModel.CardNo, "Fujica.com.cn.MonitorServiceClient.PayDataManager", "储值卡保存数据库成功，保存redis失败");
                        return response;
                    }
                    //发送给相机
                    SendValueCarToCameras(cardModel, m_ilogger, m_serializer);
                    //发送给Fujica
                    flag = SendValueCarToFujica(cardModel, cartypemodel.Idx);
                    if (!flag)
                    {
                        response.MessageContent = "储值卡保存数据库、redis成功，发送Fujica失败";
                        m_ilogger.LogError(LoggerLogicEnum.Tools, recordModel.TradeNo, recordModel.ParkingCode, recordModel.CardNo, "Fujica.com.cn.MonitorServiceClient.PayDataManager", "储值卡保存数据库、redis成功，发送Fujica失败");
                        return response;
                    }
                    m_ilogger.LogInfo(LoggerLogicEnum.Tools, recordModel.TradeNo, recordModel.ParkingCode, recordModel.CardNo, "Fujica.com.cn.MonitorServiceClient.PayDataManager", "储值卡保存数据库、redis、发送Fujica成功");
                }

                response.IsSuccess = true;
                response.MessageContent = "支付数据操作成功";     
                
            }
            catch (Exception ex)
            {
                m_ilogger.LogFatal(LoggerLogicEnum.Tools, recordModel.TradeNo, cardModel.ParkCode, recordModel.CardNo, "Fujica.com.cn.MonitorServiceClient.PayDataManager", cardModel.CarNo + "支付数据发生异常", ex.ToString());

                response.MessageContent = cardModel.CarNo + "支付数据发生异常：" + ex.ToString();
                return response;
            }

            return response;
        }

        /// <summary>
        /// 发送月卡数据到相机
        /// </summary>
        /// <returns></returns>
        private static bool SendMonthCarToCameras(CardServiceModel model, ILogger m_ilogger, ISerializer m_serializer)
        {
            MonthCardModel sendmodel = new MonthCardModel()
            {
                CarOwnerName = model.CarOwnerName,
                CarNo = model.CarNo,
                Delete = model.Enable ? false : true,
                CarTypeGuid = model.CarTypeGuid,
                Locked = model.Locked,
                StartDate = model.StartDate.Date.AddHours(0).AddMinutes(0).AddSeconds(0).ToString("yyyy-MM-dd HH:mm:ss"),
                EndDate = model.EndDate.Date.AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss") //结束时间截止到23:59:59
            };

            CommandEntity<MonthCardModel> entity = new CommandEntity<MonthCardModel>()
            {
                command = BussineCommand.MonthCar,
                idMsg = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                message = sendmodel
            };

            RabbitMQSender rabbitMq = new RabbitMQSender(m_ilogger, m_serializer);
            return rabbitMq.SendMessageForRabbitMQ("发送月卡命令", m_serializer.Serialize(entity), entity.idMsg, model.ParkCode);
        }

        /// <summary>
        /// 发送储值卡数据到相机
        /// </summary>
        /// <returns></returns>
        private static bool SendValueCarToCameras(CardServiceModel model, ILogger m_ilogger, ISerializer m_serializer)
        {
            ValueCardModel sendmodel = new ValueCardModel()
            {
                CarOwnerName = model.CarOwnerName,
                CarNo = model.CarNo,
                Delete = model.Enable ? false : true,
                CarTypeGuid = model.CarTypeGuid,
                Locked = false,
                Balance = model.Balance
            };

            CommandEntity<ValueCardModel> entity = new CommandEntity<ValueCardModel>()
            {
                command = BussineCommand.ValueCar,
                idMsg = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                message = sendmodel
            };

            RabbitMQSender rabbitMq = new RabbitMQSender(m_ilogger, m_serializer);
            return rabbitMq.SendMessageForRabbitMQ("发送储值卡命令", m_serializer.Serialize(entity), entity.idMsg, model.ParkCode);
        }

        /// <summary>
        /// 保存月卡、贵宾卡数据到数据库
        /// </summary>
        /// <param name="model"></param>
        /// <param name="m_ilogger"></param>
        /// <param name="m_serializer"></param>
        /// <returns></returns>
        public static bool SaveMonthCarToDB(CardServiceModel model, ILogger m_ilogger, ISerializer m_serializer)
        {
            try
            {
                int cartype = (int)dbhelper.ExecuteScalar(string.Format("select carType from t_cartype where guid='{0}'", model.CarTypeGuid));
                if (cartype == 1 || cartype == 3)
                {
                    //月卡车、贵宾车
                    string commandtext = @"replace into t_monthcard(projectGuid,identifying,parkCode,carNo,carOwnerName,mobile,carTypeGuid,remark,enable,locked,startDate,endDate,pauseDate,continueDate,drivewayListContent)
                                    values(@projectGuid,@identifying,@parkCode,@carNo,@carOwnerName,@mobile,@carTypeGuid,@remark,@enable,@locked,@startDate,@endDate,@pauseDate,@continueDate,@drivewayListContent)";

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

                    DbParameter[] parameter = new DbParameter[] { projectGuid, identifying, parkCode, carNo, carOwnerName, mobile, carTypeGuid, remark, enable, locked, startDate, endDate, pauseDate, continueDate, drivewayListContent };
                    return dbhelper.ExecuteNonQuery(commandtext, parameter) > 0 ? true : false;
                }
            }
            catch (Exception ex)
            {
                m_ilogger.LogFatal(LoggerLogicEnum.Tools,"", model.ParkCode, model.CarNo, "Fujica.com.cn.MonitorServiceClient.PayDataManager.SaveMonthCarToDB", string.Format("保存月卡信息时发生异常，入参:{0}", m_serializer.Serialize(model)), ex.ToString());
            }
            return false;
        }

        /// <summary>
        /// 保存储值卡数据到数据库
        /// </summary>
        /// <param name="model"></param>
        /// <param name="m_ilogger"></param>
        /// <param name="m_serializer"></param>
        /// <returns></returns>
        public static bool SaveValueCarToDB(CardServiceModel model, ILogger m_ilogger, ISerializer m_serializer)
        {
            try
            {
                int cartype = (int)dbhelper.ExecuteScalar(string.Format("select carType from t_cartype where guid='{0}'", model.CarTypeGuid));
                if (cartype == 2)
                {
                    //储值车
                    string commandtext = @"replace into t_valuecard(projectGuid,identifying,parkCode,carNo,carOwnerName,mobile,carTypeGuid,remark,enable,locked,startDate,balance,drivewayListContent)
                                    values(@projectGuid,@identifying,@parkCode,@carNo,@carOwnerName,@mobile,@carTypeGuid,@remark,@enable,@locked,@startDate,@balance,@drivewayListContent)";

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

                    DbParameter[] parameter = new DbParameter[] { projectGuid, identifying, parkCode, carNo, carOwnerName, mobile, carTypeGuid, remark, enable, locked, startDate, balance, drivewayListContent };
                    return dbhelper.ExecuteNonQuery(commandtext, parameter) > 0 ? true : false;
                }
            }
            catch (Exception ex)
            {
                m_ilogger.LogFatal(LoggerLogicEnum.Tools, "", model.ParkCode, model.CarNo, "Fujica.com.cn.MonitorServiceClient.PayDataManager.SaveValueCarToDB", string.Format("保存储值卡信息时发生异常，入参:{0}", m_serializer.Serialize(model)), ex.ToString());
            }
            return false;
        }

        /// <summary>
        /// 保存月卡、贵宾卡数据到redis
        /// </summary>
        /// <param name="model"></param>
        /// <param name="m_ilogger"></param>
        /// <param name="m_serializer"></param>
        /// <returns></returns>
        public static bool SaveMonthCarToRedis(CardServiceModel model, ILogger m_ilogger, ISerializer m_serializer)
        {
            try
            {
                IDatabase db = RedisHelper.GetDatabase(0);
                CarTypeModel cartype = m_serializer.Deserialize<CarTypeModel>(db.HashGet("CarTypeList", model.CarTypeGuid));
                if (cartype.CarType == CarTypeEnum.MonthCar || cartype.CarType == CarTypeEnum.VIPCar)
                {
                    //月卡车
                    string hashkey = model.ParkCode + Convert.ToBase64String(Encoding.UTF8.GetBytes(model.CarNo));
                    MonthCarModel monthcarmodel = new MonthCarModel()
                    {
                        ProjectGuid = model.ProjectGuid,
                        ParkCode = model.ParkCode,
                        CarOwnerName = model.CarOwnerName,
                        Mobile = model.Mobile,
                        CarNo = model.CarNo,
                        CarTypeGuid = model.CarTypeGuid,
                        //Cost= template.Amount,
                        //Units= template.Months,
                        Enable = model.Enable,
                        Locked = model.Locked,
                        StartDate = model.StartDate,
                        EndDate = model.EndDate,
                        DrivewayGuidList = model.DrivewayGuidList
                    };
                    db.HashSet("PermanentCarList", hashkey, m_serializer.Serialize(monthcarmodel));
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                m_ilogger.LogFatal(LoggerLogicEnum.Tools, "", model.ParkCode, model.CarNo, "Fujica.com.cn.MonitorServiceClient.PayDataManager.SaveMonthCarToRedis", "保存月卡车数据异常", ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// 保存储值卡数据到redis
        /// </summary>
        /// <param name="model"></param>
        /// <param name="m_ilogger"></param>
        /// <param name="m_serializer"></param>
        /// <returns></returns>
        public static bool SaveValueCarToRedis(CardServiceModel model, ILogger m_ilogger, ISerializer m_serializer)
        {
            try
            {
                IDatabase db = RedisHelper.GetDatabase(0);
                CarTypeModel cartype = m_serializer.Deserialize<CarTypeModel>(db.HashGet("CarTypeList", model.CarTypeGuid));
                if (cartype.CarType == CarTypeEnum.ValueCar)
                {
                    //储值车
                    string hashkey = model.ParkCode + Convert.ToBase64String(Encoding.UTF8.GetBytes(model.CarNo));
                    ValueCarModel monthcarmodel = new ValueCarModel()
                    {
                        ProjectGuid = model.ProjectGuid,
                        ParkCode = model.ParkCode,
                        CarOwnerName = model.CarOwnerName,
                        Mobile = model.Mobile,
                        CarNo = model.CarNo,
                        CarTypeGuid = model.CarTypeGuid,
                        Enable = model.Enable,
                        StartDate = model.StartDate,
                        Locked = model.Locked,
                        Balance = model.Balance,
                        DrivewayGuidList = model.DrivewayGuidList
                    };
                    return db.HashSet("PermanentCarList", hashkey, m_serializer.Serialize(monthcarmodel));
                }
                return false;
            }
            catch (Exception ex)
            {
                m_ilogger.LogFatal(LoggerLogicEnum.Tools, "", model.ParkCode, model.CarNo, "Fujica.com.cn.MonitorServiceClient.PayDataManager.SaveValueCarToRedis", "保存储值车数据异常", ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// 查询数据库卡务信息
        /// </summary>
        /// <param name="identifying"></param>
        /// <param name="m_ilogger"></param>
        /// <param name="m_serializer"></param>
        /// <returns></returns>
        public static CardServiceModel GetCard(string identifying, ILogger m_ilogger, ISerializer m_serializer)
        {
            try
            {
                //优先查月卡表
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
                else
                {
                    //没有则查储值卡表
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
            }
            catch (Exception ex)
            {
                m_ilogger.LogFatal(LoggerLogicEnum.Tools, "", "", "", "Fujica.com.cn.MonitorServiceClient.PayDataManager.GetCard", string.Format("获取卡信息时发生异常，入参:{0}", identifying), ex.ToString());
            }
            return null;
        }

        /// <summary>
        /// 发送月卡数据到Fujica
        /// </summary>
        /// <returns></returns>
        private static bool SendMonthCarToFujica(CardServiceModel model,string idx)
        {
            string beginDt = model.StartDate.Date.AddHours(0).AddMinutes(0).AddSeconds(0).ToString("yyyy-MM-dd HH:mm:ss");//开始时间从0分秒
            string endDt = model.EndDate.Date.AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss");  //结束时间截止到23:59:59
            RequestFujicaStandard requestFujica = new RequestFujicaStandard();
            //请求方法
            string servername = "/BasicResource/ApiUpdateMonthCardInfo";

            //请求参数
            Dictionary<string, object> dicParam = new Dictionary<string, object>();
            dicParam["ParkingCode"] = model.ParkCode;
            dicParam["CarType"] = idx;
            dicParam["CarNo"] = model.CarNo;
            dicParam["CardNo"] = "";
            dicParam["Phone"] = model.Mobile;
            dicParam["CardStatus"] = 1;

            dicParam["StartDate"] = beginDt;
            dicParam["EndDate"] = endDt;

            //返回fujica api计费模板添加、修改请求结果
            return requestFujica.RequestInterfaceV2(servername, dicParam);
        }

        /// <summary>
        /// 发送储值卡数据到Fujica
        /// </summary>
        /// <returns></returns>
        private static bool SendValueCarToFujica(CardServiceModel model,string idx)
        {
            RequestFujicaStandard requestFujica = new RequestFujicaStandard();
            //请求方法(新增、修改、删除全用同一个接口)
            string servername = "/BasicResource/ApiUpdateValueCardInfo";
            //请求参数
            Dictionary<string, object> dicParam = new Dictionary<string, object>();
            dicParam["ParkingCode"] = model.ParkCode;
            dicParam["CarType"] = idx;
            dicParam["CarNo"] = model.CarNo;
            dicParam["CardNo"] = "";
            dicParam["Phone"] = model.Mobile;
            dicParam["CardStatus"] = model.Enable ? 1 : 2;
            dicParam["Balance"] = model.Balance;

            //返回fujica api计费模板添加、修改请求结果
            return requestFujica.RequestInterfaceV2(servername, dicParam);
        }

        /// <summary>
        /// 格式化续费值，返回结束日期
        /// </summary>
        /// <param name="renewDay">续费日期数目</param>
        /// <param name="renewDayType">续费日期类型 1日  2月 3年</param>
        /// <returns></returns>
        public static DateTime FormatEndDate(DateTime endDt, int renewDay, int renewDayType)
        {
            DateTime dt = endDt;

            if (renewDayType == 1)
            {
                dt.AddDays(renewDay);
            }
            else
            {
                if (renewDayType == 3)
                {
                    //年的统一按月处理
                    renewDay = renewDay * 12;
                }
                var yy = endDt.Year;
                var mm = endDt.Month;
                var dd = endDt.Day;
                dt = dt.AddMonths(renewDay);
                if (new DateTime(yy, mm, 1).AddDays(1 - 1).Date.AddMonths(1).AddDays(-1).Day == dd)
                {
                    dt = new DateTime(dt.Year, dt.Month, dt.AddDays(1 - dt.Day).Date.AddMonths(1).AddDays(-1).Day);
                }
            }
            return dt;
        }

    }
}
