/***************************************************************************************
 * *
 * *        File Name        : ParkLotManager.cs
 * *        Creator          : llp
 * *        Create Time      : 2019-09-21 
 * *        Remark           : 功能点管理器 业务逻辑层
 * *
 * *  Copyright (c) 深圳市富士智能系统有限公司.  All rights reserved. 
 * ***************************************************************************************/
using Fujica.com.cn.Business.Base;
using Fujica.com.cn.Business.Model;
using Fujica.com.cn.Context.Model;
using Fujica.com.cn.Logger;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fujica.com.cn.Business.ParkLot
{
    /// <summary>
    /// 功能点管理器.
    /// </summary> 
    /// <remarks>
    /// 2019.09.21: 日志参数完善. llp <br/> 
    /// 2019.09.23: 修改方法SendBarredEntryCarTypeOnParkingFull的返回参数，数据为空也返回true. Ase <br/>
    /// 2019.09.23: 修改方法SendMonthCarExpireMode的返回参数，数据为空也返回true. Ase <br/>
    /// 2019.09.23: 修改方法SendMonthCarExpireRemindDay的返回参数，数据为空也返回true. Ase <br/>
    /// </remarks>  
    partial class ParkLotManager
    {
        ///// <summary>
        ///// 日志记录器
        ///// </summary>
        //internal readonly ILogger m_logger;

        ///// <summary>
        ///// 序列化器
        ///// </summary>
        //internal readonly ISerializer m_serializer = null;

        ///// <summary>
        ///// redis操作类
        ///// </summary>
        //private IBaseRedisOperate<FunctionPointModel> redisoperate = null;

        ///// <summary>
        ///// 数据库操作类
        ///// </summary>
        //private IBaseDataBaseOperate<FunctionPointModel> databaseoperate = null;

        //private readonly RabbitMQSender m_rabbitMQ;
        
        ///// <summary>
        ///// 车类管理器
        ///// </summary>
        //private CarTypeManager cartypemanager;

        ///// <summary>
        ///// 计费模板管理器
        ///// </summary>
        //private BillingTemplateManager billingtemplatemanager = null;

        //public string LastErrorDescribe
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }

        //    set
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //public FunctionPointManager(ILogger _logger, ISerializer _serializer, 
        //    IBaseRedisOperate<FunctionPointModel> _redisoperate, 
        //    IBaseDataBaseOperate<FunctionPointModel> _databaseoperate,
        //    RabbitMQSender _rabbitMQ,
        //    CarTypeManager _cartypemanager, BillingTemplateManager _billingtemplatemanager)
        //{
        //    m_logger = _logger;
        //    m_serializer = _serializer;
        //    redisoperate = _redisoperate;
        //    databaseoperate = _databaseoperate;
        //    m_rabbitMQ = _rabbitMQ;
        //    cartypemanager = _cartypemanager;
        //    billingtemplatemanager = _billingtemplatemanager;
        //}

        /// <summary>
        /// 设置功能点
        /// 设置固定车延期方式 0=顺延 1=跳延
        /// 设置固定车过期处理方式 -1=禁止入场 0=看做临时车 大于等于N(N>=1) 过期N天后禁止入场
        /// 设置满位禁止入场车类
        /// 设置参与剩余车位统计的车类
        /// 设置不同颜色车牌的收费车类
        /// 设置月租车延期提醒
        /// 设置储值车充值提醒
        /// </summary>
        /// <returns></returns>
        public bool SetFunctionPoint(FunctionPointModel model)
        {
            bool flag = _iFunctionPointContext.SetFunctionPoint(model);
            if (!flag)
            {
                LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_SAVE_ERROR.GetDesc(); 
                return false;
            }

            //返回值不参考mq的结果
            SendTempCarTypeOfPlateColor(model.ProjectGuid, model.ParkCode);
            SendBarredEntryCarTypeOnParkingFull(model.ProjectGuid, model.ParkCode);
            SendMonthCarExpireRemindDay(model.ProjectGuid, model.ParkCode);
            SendMonthCarExpireMode(model.ProjectGuid, model.ParkCode);
            SendManualOpenGateGuid(model.ProjectGuid, model.ParkCode);
            return true;
        }

        /// <summary>
        /// 获取功能点
        /// </summary>
        /// <param name="parkingcode"></param>
        /// <returns></returns>
        public FunctionPointModel GetFunctionPoint(string projectguid,string parkcode)
        {
            FunctionPointModel model = _iFunctionPointContext.GetFunctionPoint(projectguid, parkcode);
            
            return model;
        }

        /// <summary>
        /// 下发临时车不同车牌颜色对应车类计费信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool SendTempCarTypeOfPlateColor(string projectGuid,string parkCode)
        {
            try
            {
                FunctionPointModel model = GetFunctionPoint(projectGuid,parkCode);
                CarTypeModel cartypemode = _iCarTypeContext.AllCarType(parkCode,projectGuid).Where(o => o.DefaultType == true).FirstOrDefault(); //找出默认的车类的
                if (cartypemode == null)
                {
                    LastErrorDescribe = "该停车场未设置默认车类";
                    return false;
                }
                List<TempCarTypeModel> sendmodel = new List<TempCarTypeModel>() { };

                sendmodel.Add(TempCarTypeDataInit(1, string.IsNullOrWhiteSpace(model.BluePlateCarTypeGuid) ? cartypemode.Guid : model.BluePlateCarTypeGuid));
                sendmodel.Add(TempCarTypeDataInit(2, string.IsNullOrWhiteSpace(model.YellowPlateCarTypeGuid) ? cartypemode.Guid : model.YellowPlateCarTypeGuid));
                sendmodel.Add(TempCarTypeDataInit(3, string.IsNullOrWhiteSpace(model.WhitePlateCarTypeGuid) ? cartypemode.Guid : model.WhitePlateCarTypeGuid));
                sendmodel.Add(TempCarTypeDataInit(4, string.IsNullOrWhiteSpace(model.GreenPlateCarTypeGuid) ? cartypemode.Guid : model.GreenPlateCarTypeGuid));
                sendmodel.Add(TempCarTypeDataInit(5, string.IsNullOrWhiteSpace(model.BlackPlateCarTypeGuid) ? cartypemode.Guid : model.BlackPlateCarTypeGuid));


                CommandEntity<List<TempCarTypeModel>> entity = new CommandEntity<List<TempCarTypeModel>>()
                {
                    command = BussineCommand.CarType,
                    idMsg = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                    message = sendmodel
                };
                        
                return m_rabbitMQ.SendMessageForRabbitMQ("发送临时车车类指令", m_serializer.Serialize(entity), entity.idMsg, model.ParkCode);
            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.Bussiness, "", parkCode, "", "Fujica.com.cn.Business.ParkLot.FunctionPointManager.SendTempCarTypeOfPlateColor", "下发临时车车类时发生异常", ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// 下发满位禁入车类
        /// </summary>
        /// <param name="parkCode"></param>
        /// <returns></returns>
        public bool SendBarredEntryCarTypeOnParkingFull(string projectGuid, string parkCode)
        {
            try
            {
                FunctionPointModel model = GetFunctionPoint(projectGuid,parkCode);
                if (model == null || model.BarredEntryCarTypeOnParkingFull == null) return true;
                BarredEntryCarTypeModel sendmodel = new BarredEntryCarTypeModel();
                sendmodel.CarTypeList = new List<string>();
                foreach (var item in model.BarredEntryCarTypeOnParkingFull)
                {
                    sendmodel.CarTypeList.Add(item);
                }
                CommandEntity<BarredEntryCarTypeModel> entity = new CommandEntity<BarredEntryCarTypeModel>()
                {
                    command = BussineCommand.BarredEntryFull,
                    idMsg = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                    message = sendmodel
                };

                return m_rabbitMQ.SendMessageForRabbitMQ("发送满位禁入车类指令", m_serializer.Serialize(entity), entity.idMsg, model.ParkCode);
            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.Bussiness, "", parkCode, "", "Fujica.com.cn.Business.ParkLot.FunctionPointManager.SendBarredEntryCarTypeOnParkingFull", "下发满位禁入车类时发生异常", ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// 下发月租车延期提醒天数
        /// </summary>
        /// <param name="projectGuid"></param>
        /// <param name="parkCode"></param>
        /// <returns></returns>
        public bool SendMonthCarExpireRemindDay(string projectGuid, string parkCode)
        {
            try
            {
                FunctionPointModel model = GetFunctionPoint(projectGuid, parkCode);
                if (model == null) return true;
                CommandEntity<int> entity = new CommandEntity<int>()
                {
                    command = BussineCommand.MonthCarExpireRemindDay,
                    idMsg = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                    message = model.MinDays
                };

                return m_rabbitMQ.SendMessageForRabbitMQ("发送月租车延期提醒天数", m_serializer.Serialize(entity), entity.idMsg, model.ParkCode);
            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.Bussiness, "", parkCode, "", "Fujica.com.cn.Business.ParkLot.FunctionPointManager.SendMonthCarExpireRemindDay", "下发月租车延期提醒天数时发生异常", ex.ToString());
                return false;
            }
        }
        
        /// <summary>
        /// 下发月租车过期处理方式
        /// </summary>
        /// <param name="projectGuid"></param>
        /// <param name="parkCode"></param>
        /// <returns></returns>
        public bool SendMonthCarExpireMode(string projectGuid, string parkCode)
        {
            try
            {
                FunctionPointModel model = GetFunctionPoint(projectGuid, parkCode);
                if (model == null) return true;
                CommandEntity<int> entity = new CommandEntity<int>()
                {
                    command = BussineCommand.MonthCarExpireMode,
                    idMsg = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                    message = model.PastDueMode
                };

                return m_rabbitMQ.SendMessageForRabbitMQ("发送月租车过期处理方式", m_serializer.Serialize(entity), entity.idMsg, model.ParkCode);
            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.Bussiness, "", parkCode, "", "Fujica.com.cn.Business.ParkLot.FunctionPointManager.SendMonthCarExpireMode", "下发月租车过期处理方式时发生异常", ex.ToString());
                return false;
            }
        }


        /// <summary>
        /// 下发手动开闸车类guid集合
        /// </summary>
        /// <param name="projectGuid"></param>
        /// <param name="parkCode"></param>
        /// <returns></returns>
        public bool SendManualOpenGateGuid(string projectGuid, string parkCode)
        {
            try
            {
                FunctionPointModel model = GetFunctionPoint(projectGuid, parkCode);
                if (model == null || model.ManualOpenGateGuid == null) return false;
                ManualOpenGateModel sendmodel = new ManualOpenGateModel();
                sendmodel.CarTypeList = new List<string>();
                foreach (var item in model.ManualOpenGateGuid)
                {
                    sendmodel.CarTypeList.Add(item);
                }
                CommandEntity<ManualOpenGateModel> entity = new CommandEntity<ManualOpenGateModel>()
                {
                    command = BussineCommand.ManualOpenGate,
                    idMsg = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                    message = sendmodel
                };

                return m_rabbitMQ.SendMessageForRabbitMQ("发送手动开闸车类guid集合", m_serializer.Serialize(entity), entity.idMsg, model.ParkCode);
            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.Bussiness, "", parkCode, "", "Fujica.com.cn.Business.ParkLot.FunctionPointManager.SendManualOpenGateGuid", "下发手动开闸车类guid集合时发生异常", ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// 不同颜色临时车对应车类数据实体初始化
        /// </summary>
        /// <param name="carColor">车颜色类别 1:蓝牌车 2:黄牌车 3:白牌车 4:绿牌车 5:黑牌车</param>
        /// <param name="carTypeGuid">车类guid</param>
        /// <returns></returns>
        private TempCarTypeModel TempCarTypeDataInit(int carColor, string carTypeGuid)
        {
            //根据车类id查到对应的计费模板
            BillingTemplateChargeModel billingTemplateModel = GetBillingTemplateChargeModel(carTypeGuid);

            TempCarTypeModel model = new TempCarTypeModel();
            model.CarColor = carColor;
            model.CarTypeGuid = carTypeGuid;

            if (billingTemplateModel == null) return model;
            
            model.BeginTime1 = billingTemplateModel.BeginTime1;
            model.BeginTime2 = billingTemplateModel.BeginTime2;
            model.BeginTime3 = billingTemplateModel.BeginTime3;
            model.EndTime1 = billingTemplateModel.EndTime1;
            model.EndTime2 = billingTemplateModel.EndTime2;
            model.EndTime3 = billingTemplateModel.EndTime3;
            model.FreeMinutes1 = billingTemplateModel.FreeMinutes1;
            model.FreeMinutes2 = billingTemplateModel.FreeMinutes2;
            model.FreeMinutes3 = billingTemplateModel.FreeMinutes3;
            model.LeaveTimeout1 = billingTemplateModel.LeaveTimeout1;
            model.LeaveTimeout2 = billingTemplateModel.LeaveTimeout2;
            model.LeaveTimeout2 = billingTemplateModel.LeaveTimeout3;
            
            return model;
        }



    }
}
