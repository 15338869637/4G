/***************************************************************************************
 * *
 * *        File Name        : CardServiceManager.cs
 * *        Creator          : llp
 * *        Create Time      : 2019-09-21 
 * *        Remark           : 卡务管理 业务逻辑层
 * *
 * *  Copyright (c) 深圳市富士智能系统有限公司.  All rights reserved. 
 * ***************************************************************************************/
using Fujica.com.cn.Business.Base;
using Fujica.com.cn.Business.Model;
using Fujica.com.cn.Communication.RabbitMQ;
using Fujica.com.cn.Context.IContext;
using Fujica.com.cn.Context.Model;
using Fujica.com.cn.Logger;
using Fujica.com.cn.Tools;
using FujicaService.Infrastructure.Core.Enums;
using System;
using System.Collections.Generic;

namespace Fujica.com.cn.Business.ParkLot
{
    /// <summary>
    /// 车辆入场数据管理.
    /// </summary> 
    /// <remarks>
    /// 2019.09.21: 日志参数完善. llp <br/> 
    /// 2019.09.26: 修改AddNewCardByMonth方法：场内临时车转卡类业务调整. Ase <br/> 
    /// </remarks> 
    public class CardServiceManager : IBaseBusiness
    {
        /// <summary>
        /// 日志记录器
        /// </summary>
        internal readonly ILogger m_logger;
        /// <summary>
        /// 序列化器
        /// </summary>
        internal readonly ISerializer m_serializer = null;
        /// <summary>
        /// MQ消息发送器
        /// </summary>
        private readonly RabbitMQSender m_rabbitMQ;

        /// <summary>
        /// 卡务信息操作上下文
        /// </summary>
        private ICardServiceContext _iCardServiceContext = null;
        /// <summary>
        /// 车类信息上下文
        /// </summary>
        private ICarTypeContext _iCarTypeContext;
        /// <summary>
        /// 固定车模板信息操作上下文
        /// </summary>
        private IPermanentTemplateContext _iPermanentTemplateContext = null;
        /// <summary>
        /// 车道信息操作上下文
        /// </summary>
        private IDrivewayContext _iDrivewayContext = null;
        /// <summary>
        /// 车进出信息操作上下文
        /// </summary>
        private IVehicleInOutContext _iVehicleInOutContext = null;
        public string LastErrorDescribe
        {
            get;
            set;
        }
        public CardServiceManager(ILogger logger, ISerializer serializer, RabbitMQSender rabbitMQ,
            ICardServiceContext iCardServiceContext,
            ICarTypeContext iCarTypeContext,
            IPermanentTemplateContext iPermanentTemplateContext,
            IDrivewayContext iDrivewayContext,
            IVehicleInOutContext iVehicleInOutContext)
        {
            m_logger = logger;
            m_serializer = serializer;
            m_rabbitMQ = rabbitMQ;
            _iCardServiceContext = iCardServiceContext;
            _iCarTypeContext = iCarTypeContext;
            _iPermanentTemplateContext = iPermanentTemplateContext;
            _iDrivewayContext = iDrivewayContext;
            _iVehicleInOutContext = iVehicleInOutContext;
        }

        /// <summary>
        /// 开卡
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddNewCard(CardServiceModel model)
        {
            try
            {
                //需要验证当前车牌是否存在(查询范围月卡、贵宾卡、储值卡、临时卡集合)
                CardServiceModel contentModel = ExistCard(model.CarNo, model.ParkCode);
                if (contentModel != null)
                {
                    LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_EXISTS_CARD.GetDesc();
                    return false;
                }
                //验证车类是否存在
                CarTypeModel cartypeModel = _iCarTypeContext.GetCarType(model.CarTypeGuid);
                if (cartypeModel == null)
                {
                    LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_NOTEXISTS_CARTYPE.GetDesc();
                    return false;
                }

                bool flag = false;
                switch (cartypeModel.CarType)
                {
                    case CarTypeEnum.TempCar:
                        flag = AddNewCardByTemp(model, cartypeModel);
                        break;
                    case CarTypeEnum.MonthCar:
                        flag = AddNewCardByMonth(model, cartypeModel);
                        break;
                    case CarTypeEnum.ValueCar:
                        flag = AddNewCardByValue(model, cartypeModel);
                        break;
                    case CarTypeEnum.VIPCar:
                        flag = AddNewCardByMonth(model, cartypeModel);
                        break;
                }
                return flag;
            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.Bussiness, "", "", "", "Fujica.com.cn.Business.ParkLot.CardServiceManager.AddNewCard", string.Format("开卡发生异常,入参:{0}", m_serializer.Serialize(model)), ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// 新增月租车信息
        /// </summary>
        /// <param name="model">月租车实体</param>
        /// <param name="cartypeModel">前端传过来的车类实体</param>
        /// <returns></returns>
        private bool AddNewCardByMonth(CardServiceModel model, CarTypeModel cartypeModel)
        {
            //-------临时替代业务流程 begin
            //判断车辆是否在场并且当前停车记录是临时车，则进行转卡操作。同步到Fujica Api
            VehicleInOutModel entryModel = _iVehicleInOutContext.GetEntryRecord(model.CarNo, model.ParkCode);
            if (entryModel != null)
            {
                if (entryModel.EventTime.Date != DateTime.Now.Date)
                {
                    LastErrorDescribe = "该车场目前只支持不在场车辆或当天入场车辆开月卡。";
                    return false;
                }
            }
            //-------临时替代业务流程 end

            if (model.StartDate < DateTime.Now.Date)
            {
                //起始日期小于当前日期
                LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_SAVE_CARD_STARTDATE.GetDesc();
                return false;
            }
            if (model.EndDate.AddDays(1).AddSeconds(-1) < DateTime.Now.Date)
            {
                //截至日期小于当前日期
                LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_SAVE_CARD_ENDDATE.GetDesc();
                return false;
            }
            model.Balance = 0;
            model.PauseDate = default(DateTime); //报停日期 置默认值
            model.Enable = true;
            //保存至数据库
            bool flag = _iCardServiceContext.AddCard(model, cartypeModel.CarType);
            if (!flag)
            {
                LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_SAVE_CARD.GetDesc();
                return false;
            }
            //数据同步到主平台Fujica Api
            flag = AddMonthCardDataToFujica(model, 1, cartypeModel.Idx);
            if (!flag)
            {
                LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_SAVE_CARD_TOFUJICA.GetDesc();
                return flag;
            }

            //场内车辆转卡操作
            ////判断车辆是否在场并且当前停车记录是临时车，则进行转卡操作。同步到Fujica Api
            //VehicleInOutModel entryModel = _iVehicleInOutContext.GetEntryRecord(model.CarNo, model.ParkCode);
            //是否在场车辆
            if (entryModel != null)
            {
                //本次停车卡类
                CarTypeModel entryCarTypeModel = _iCarTypeContext.GetCarType(entryModel.CarTypeGuid);
                //本次停车是否临时车卡类
                if (entryCarTypeModel != null && entryCarTypeModel.CarType == CarTypeEnum.TempCar)
                {
                    //-------因线上主平台fujica业务原因暂时取消该业务流程，后续再放回来 begin

                    ////变更时间
                    //DateTime changeTime = DateTime.Now;
                    ////本次临时车停车入场时间是否今天
                    ////如果入场时间为今天，变更时间 字段则设为入场时间
                    //if (entryModel.EventTime.Date == DateTime.Now.Date)
                    //{
                    //    changeTime = entryModel.EventTime;
                    //}
                    //else
                    //{
                    //    //如果入场时间为今天以前，变更时间 字段则设为昨天23:59:59
                    //    changeTime = DateTime.Now.Date.AddMinutes(-1);
                    //}

                    //flag = CarChangeDataToFujica(model, cartypeModel.Idx, cartypeModel.CarType, "场内临时车转月租车卡类", changeTime);
                    //if (!flag)
                    //{
                    //    LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_SAVE_CARD_TOFUJICA.GetDesc();
                    //    return flag;
                    //}

                    //-------因线上主平台fujica业务原因暂时取消该业务流程，后续再放回来 end

                    //-------临时替代业务流程 begin

                    //场内临时车转月卡类
                    CorrectCarnoModel correctModel = new CorrectCarnoModel();
                    correctModel.ParkingCode = model.ParkCode;
                    correctModel.OldCarno = entryModel.CarNo;
                    correctModel.NewCarno = entryModel.CarNo;
                    correctModel.CarTypeGuid = entryModel.CarTypeGuid;
                    correctModel.NewCarTypeGuid = model.CarTypeGuid;
                    correctModel.CarType = cartypeModel.CarType;
                    correctModel.Remark = $"场内转卡：临时车转月租车卡类";
                    correctModel.OldGuid = entryModel.Guid;
                    correctModel.DeviceIdentify = entryModel.DriveWayMAC;
                    correctModel.ImgUrl = entryModel.ImgUrl;
                    correctModel.InTime = entryModel.EventTime;
                    correctModel.Operator = model.RechargeOperator;
                    CorrectToEntryCamera(correctModel);

                    //-------临时替代业务流程 end
                }
            }

            //MQ发送数据给相机
            flag = SendMonthCarToCameras(model);
            if (!flag)
            {
                LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_MQ_SEND_ERROR.GetDesc();
                return flag;
            }

            return flag;
        }

        /// <summary>
        /// 新增储值车信息
        /// </summary>
        /// <param name="model">月租车实体</param>
        /// <param name="cartypeModel">前端传过来的车类实体</param>
        /// <returns></returns>
        private bool AddNewCardByValue(CardServiceModel model, CarTypeModel cartypeModel)
        {
            model.Balance = model.PayAmount; //开卡时，余额等于支付金额，仅当储值车时有效
            model.PauseDate = default(DateTime); //报停日期 置默认值
            model.Enable = true;

            //保存至数据库
            bool flag = _iCardServiceContext.AddCard(model, cartypeModel.CarType);
            if (!flag)
            {
                LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_SAVE_CARD.GetDesc();
                return false;
            }
            //储值卡数据同步到主平台Fujica Api
            flag = AddValueCardToFujica(model, 1, cartypeModel.Idx);
            if (!flag)
            {
                LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_SAVE_CARD_TOFUJICA.GetDesc();
                return flag;
            }

            //场内车辆转卡操作
            //判断车辆是否在场并且当前停车记录是临时车，则进行转卡操作。同步到Fujica Api
            VehicleInOutModel entryModel = _iVehicleInOutContext.GetEntryRecord(model.CarNo, model.ParkCode);
            //是否在场车辆
            if (entryModel != null)
            {
                //本次停车卡类
                CarTypeModel entryCarTypeModel = _iCarTypeContext.GetCarType(entryModel.CarTypeGuid);
                //本次停车是否临时车卡类
                if (entryCarTypeModel != null && entryCarTypeModel.CarType == CarTypeEnum.TempCar)
                {
                    //场内临时车转储值卡
                    AddRecordModel recordModel = new AddRecordModel();
                    recordModel.DeviceIdentify = entryModel.DriveWayMAC;
                    recordModel.Guid = entryModel.Guid;
                    recordModel.ParkingCode = model.ParkCode;
                    recordModel.CarNo = model.CarNo;
                    recordModel.CarTypeGuid = model.CarTypeGuid;
                    recordModel.ImgUrl = entryModel.ImgUrl;
                    recordModel.InTime = entryModel.EventTime;
                    recordModel.Remark = "场内临时车转储值车卡类";
                    recordModel.CarType = cartypeModel.CarType;

                    //重新以储值卡车类发送入场信息
                    bool addResult = SendEntryRecordToCamera(recordModel);
                    if (!flag)
                    {
                        LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_MQ_SEND_ERROR.GetDesc();
                        return flag;
                    }
                }
            }

            //MQ发送数据给相机
            flag = SendValueCarToCameras(model);
            if (!flag)
            {
                LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_MQ_SEND_ERROR.GetDesc();
                return flag;
            }

            return flag;
        }

        /// <summary>
        /// 新增临时车信息
        /// </summary>
        /// <param name="model">月租车实体</param>
        /// <param name="cartypeModel">前端传过来的车类实体</param>
        /// <returns></returns>
        private bool AddNewCardByTemp(CardServiceModel model, CarTypeModel cartypeModel)
        {
            if (model.StartDate < DateTime.Now.Date)
            {
                //起始日期小于当前日期
                LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_SAVE_CARD_STARTDATE.GetDesc();
                return false;
            }
            if (model.EndDate.AddDays(1).AddSeconds(-1) < DateTime.Now.Date)
            {
                //截至日期小于当前日期
                LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_SAVE_CARD_ENDDATE.GetDesc();
                return false;
            }
            model.Balance = 0;
            model.PauseDate = default(DateTime); //报停日期 置默认值
            model.Enable = true;

            //保存至数据库
            bool flag = _iCardServiceContext.AddCard(model, cartypeModel.CarType);
            if (!flag)
            {
                LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_SAVE_CARD.GetDesc();
                return false;
            }

            //MQ发送数据给相机
            flag = SendTempCarToCameras(model);
            if (!flag)
            {
                LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_MQ_SEND_ERROR.GetDesc();
                return flag;
            }

            return flag;
        }

        /// <summary>
        /// 修改卡信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool ModifyCard(CardServiceModel model)
        {
            try
            {
                CardServiceModel contentModel = ExistCard(model.CarNo, model.ParkCode);
                if (contentModel == null)
                {
                    LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_NOTEXISTS_CARD.GetDesc();
                    return false;
                }
                CarTypeModel cartypeModel = _iCarTypeContext.GetCarType(model.CarTypeGuid);
                if (cartypeModel == null)
                {
                    LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_NOTEXISTS_CARTYPE.GetDesc();
                    return false;
                }

                contentModel.CarOwnerName = model.CarOwnerName;
                contentModel.Mobile = model.Mobile;
                contentModel.DrivewayGuidList = model.DrivewayGuidList;
                contentModel.CarTypeGuid = model.CarTypeGuid;
                contentModel.RechargeOperator = model.RechargeOperator;
                
                //保存至数据库
                bool flag = _iCardServiceContext.ModifyCard(contentModel, cartypeModel.CarType);
                if (!flag)
                {
                    LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_SAVE_CARD.GetDesc();
                    return false;
                }

                //发送数据到主平台Fujica Api
                switch (cartypeModel.CarType)
                {
                    case CarTypeEnum.TempCar:
                        break;
                    case CarTypeEnum.MonthCar:
                        //月卡数据同步到主平台Fujica Api
                        flag = UpdateMonthCardDataToFujica(contentModel,cartypeModel.Idx);
                        break;
                    case CarTypeEnum.ValueCar:
                        //储值卡数据同步到主平台Fujica Api
                        flag = UpdateValueCardToFujica(contentModel, cartypeModel.Idx);
                        break;
                    case CarTypeEnum.VIPCar:
                        //贵宾卡数据同步到主平台Fujica Api
                        flag = UpdateMonthCardDataToFujica(contentModel, cartypeModel.Idx);
                        break;
                    default:
                        break;
                }
                if (!flag)
                {
                    LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_SAVE_CARD_TOFUJICA.GetDesc();
                    return flag;
                }

                //发送数据给相机MQ
                switch (cartypeModel.CarType)
                {
                    case CarTypeEnum.TempCar:
                        flag = SendTempCarToCameras(contentModel);
                        break;
                    case CarTypeEnum.MonthCar:
                        flag = SendMonthCarToCameras(contentModel);
                        break;
                    case CarTypeEnum.ValueCar:
                        flag = SendValueCarToCameras(contentModel);
                        break;
                    case CarTypeEnum.VIPCar:
                        flag = SendMonthCarToCameras(contentModel);
                        break;
                    default:
                        break;
                }
                if (!flag)
                {
                    LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_MQ_SEND_ERROR.GetDesc();
                    return flag;
                }
                return flag;
            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.Bussiness, "", "", model.CarNo, "Fujica.com.cn.Business.ParkLot.CardServiceManager.ModifyCard", string.Format("修改卡发生异常,入参:{0}", m_serializer.Serialize(model)), ex.ToString());
            }
            return false;
        }

        /// <summary>
        /// 延期/充值
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool RenewCard(CardServiceModel model)
        {
            try
            {
                CardServiceModel contentModel = ExistCard(model.CarNo, model.ParkCode);
                if (contentModel == null)
                {
                    LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_NOTEXISTS_CARD.GetDesc();
                    return false;
                }

                CarTypeModel cartypeModel = _iCarTypeContext.GetCarType(model.CarTypeGuid);
                if (cartypeModel == null)
                {
                    LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_NOTEXISTS_CARTYPE.GetDesc();
                    return false;
                }

                contentModel.PayAmount = model.PayAmount;
                contentModel.PayStyle = model.PayStyle;
                contentModel.RechargeOperator = model.RechargeOperator;
                contentModel.PrimaryEndDate = model.PrimaryEndDate;
                if (cartypeModel.CarType == CarTypeEnum.MonthCar || cartypeModel.CarType == CarTypeEnum.VIPCar)
                {
                    //_model.startDate = model.startDate; //勿要重写 开始时间，数据库存的开始时间是开卡时间，千万不要覆盖
                    contentModel.EndDate = model.EndDate; //月卡车变动截止日期
                }
                else if (cartypeModel.CarType == CarTypeEnum.ValueCar)
                {
                    contentModel.Balance += contentModel.PayAmount; //储值车变动余额
                }
                else if (cartypeModel.CarType == CarTypeEnum.TempCar)
                {
                    contentModel.EndDate = model.EndDate;  
                }
                //保存至数据库
                bool flag = _iCardServiceContext.ModifyCard(contentModel, cartypeModel.CarType);
                if (!flag)
                {
                    LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_SAVE_CARD.GetDesc();
                    return false;
                }

                //月卡、贵宾卡数据同步到主平台Fujica Api
                if (cartypeModel.CarType == CarTypeEnum.MonthCar || cartypeModel.CarType == CarTypeEnum.VIPCar)
                {
                    flag = AddMonthCardDataToFujica(contentModel,2, cartypeModel.Idx);
                }
                else if (cartypeModel.CarType == CarTypeEnum.ValueCar)
                {
                    //储值卡数据同步到主平台Fujica Api
                    flag = AddValueCardToFujica(contentModel, 2, cartypeModel.Idx);
                }
                if (!flag)
                {
                    LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_SAVE_CARD_TOFUJICA.GetDesc();
                    return flag;
                }

                //发送数据给相机MQ
                switch (cartypeModel.CarType)
                {
                    case CarTypeEnum.TempCar:
                        flag = SendTempCarToCameras(contentModel);
                        break;
                    case CarTypeEnum.MonthCar:
                        flag = SendMonthCarToCameras(contentModel);
                        break;
                    case CarTypeEnum.ValueCar:
                        flag = SendValueCarToCameras(contentModel);
                        break;
                    case CarTypeEnum.VIPCar:
                        flag = SendMonthCarToCameras(contentModel);
                        break;
                    default:
                        break;
                }
                if (!flag)
                {
                    LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_MQ_SEND_ERROR.GetDesc();
                    return flag;
                }
                return flag;
            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.Bussiness, "", "", model.CarNo, "Fujica.com.cn.Business.ParkLot.CardServiceManager.RenewCard", string.Format("延期/续费发生异常,入参:{0}", m_serializer.Serialize(model)), ex.ToString());
            }
            return false;
        }

        /// <summary>
        /// 锁定 禁止出场、入场
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool LockedCard(string carNo, string parkCode)
        {
            CardServiceModel contentModel = ExistCard(carNo, parkCode);
            if (contentModel == null)
            {
                LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_NOTEXISTS_CARD.GetDesc();
                return false;
            }
            CarTypeModel cartypeModel = _iCarTypeContext.GetCarType(contentModel.CarTypeGuid);
            if (cartypeModel == null)
            {
                LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_NOTEXISTS_CARTYPE.GetDesc();
                return false;
            }

            //保存至数据库
            bool flag = _iCardServiceContext.LockedCard(contentModel, cartypeModel.CarType);
            if (!flag)
            {
                LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_SAVE_CARD.GetDesc();
                return false;
            }

            //发送数据给相机MQ
            switch (cartypeModel.CarType)
            {
                case CarTypeEnum.TempCar:
                    flag = SendTempCarToCameras(contentModel);
                    break;
                case CarTypeEnum.MonthCar:
                    flag = SendMonthCarToCameras(contentModel);
                    break;
                case CarTypeEnum.ValueCar:
                    flag = SendValueCarToCameras(contentModel);
                    break;
                case CarTypeEnum.VIPCar:
                    flag = SendMonthCarToCameras(contentModel);
                    break;
                default:
                    break;
            }
            if (!flag)
            {
                LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_MQ_SEND_ERROR.GetDesc();
                return flag;
            }
            return flag;
        }

        /// <summary>
        /// 解锁 可以出场、入场
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UnLockedCard(string carNo, string parkCode)
        {
            CardServiceModel contentModel = ExistCard(carNo, parkCode);
            if (contentModel == null)
            {
                LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_NOTEXISTS_CARD.GetDesc();
                return false;
            }
            CarTypeModel cartypeModel = _iCarTypeContext.GetCarType(contentModel.CarTypeGuid);
            if (cartypeModel == null)
            {
                LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_NOTEXISTS_CARTYPE.GetDesc();
                return false;
            }

            //保存至数据库
            bool flag = _iCardServiceContext.UnLockedCard(contentModel, cartypeModel.CarType);
            if (!flag)
            {
                LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_SAVE_CARD.GetDesc();
                return false;
            }

            //发送数据给相机MQ
            switch (cartypeModel.CarType)
            {
                case CarTypeEnum.TempCar:
                    flag = SendTempCarToCameras(contentModel);
                    break;
                case CarTypeEnum.MonthCar:
                    flag = SendMonthCarToCameras(contentModel);
                    break;
                case CarTypeEnum.ValueCar:
                    flag = SendValueCarToCameras(contentModel);
                    break;
                case CarTypeEnum.VIPCar:
                    flag = SendMonthCarToCameras(contentModel);
                    break;
                default:
                    break;
            }
            if (!flag)
            {
                LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_MQ_SEND_ERROR.GetDesc();
                return flag;
            }
            return flag;
        }

        /// <summary>
        /// 注销 从数据库删除:（储值卡.月卡注销时，先出场，在 入临时车）
        /// </summary>
        /// <param name="balance">卡上原金额</param>
        /// <param name="refundAmount">退款金额</param>
        /// <param name="rechargeOperator">操作人</param>
        /// <returns></returns> 
        public bool DeleteCard(CardServiceModel model)
        {
            CardServiceModel contentModel = ExistCard(model.CarNo, model.ParkCode);
            if (contentModel == null)
            {
                LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_NOTEXISTS_CARD.GetDesc();
                return false;
            }
            CarTypeModel cartypeModel = _iCarTypeContext.GetCarType(contentModel.CarTypeGuid);
            if (cartypeModel == null)
            {
                LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_NOTEXISTS_CARTYPE.GetDesc();
                return false;
            }

            //标记为失效（删除）
            contentModel.Enable = false;
            contentModel.RechargeOperator = model.RechargeOperator;
            contentModel.PayAmount = model.PayAmount;
            contentModel.Balance = contentModel.Balance - model.PayAmount; //储值车变动余额 
            contentModel.Remark = model.Remark;
            contentModel.PrimaryEndDate = model.StartDate;
            contentModel.EndDate = contentModel.EndDate;

            //保存至数据库
            bool flag = _iCardServiceContext.DeleteCard(contentModel, cartypeModel.CarType);
            if (!flag)
            {
                LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_SAVE_CARD.GetDesc();
                return false;
            }


            //月卡、贵宾卡数据同步到主平台Fujica Api
            if (cartypeModel.CarType == CarTypeEnum.MonthCar || cartypeModel.CarType == CarTypeEnum.VIPCar)
            {
                flag = BackMonthCardDataToFujica(contentModel,cartypeModel.Idx);
            }
            else if(cartypeModel.CarType == CarTypeEnum.ValueCar)
            {
                //储值卡数据同步到主平台Fujica Api
                flag =BackValueCardToFujica(contentModel,cartypeModel.Idx);
            }
            if (!flag)
            {
                LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_SAVE_CARD_TOFUJICA.GetDesc();
                return flag;
            }

            //发送数据给相机MQ
            switch (cartypeModel.CarType)
            {
                case CarTypeEnum.TempCar:
                    flag = SendTempCarToCameras(contentModel);
                    break;
                case CarTypeEnum.MonthCar:
                    flag = SendMonthCarToCameras(contentModel);
                    break;
                case CarTypeEnum.ValueCar:
                    flag = SendValueCarToCameras(contentModel);
                    break;
                case CarTypeEnum.VIPCar:
                    flag = SendMonthCarToCameras(contentModel);
                    break;
                default:
                    break;
            }
            if (!flag)
            {
                LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_MQ_SEND_ERROR.GetDesc();
                return flag;
            }

            //月卡、储值卡、贵宾卡当前车辆在场内，卡注销： 先按原卡类型 出场，在按照临时卡入场
            if (cartypeModel.CarType != CarTypeEnum.TempCar)
            {
                //读取车辆的在场记录
                VehicleInOutModel entermodel = _iVehicleInOutContext.GetEntryRecord(model.CarNo, model.ParkCode);
                if (entermodel != null)
                {
                    //封装 修正车牌模型实体
                    CorrectCarnoModel correctModel = new CorrectCarnoModel();
                    correctModel.ParkingCode = contentModel.ParkCode;
                    correctModel.OldCarno = contentModel.CarNo;
                    correctModel.NewCarno = contentModel.CarNo;
                    correctModel.CarTypeGuid = contentModel.CarTypeGuid;
                    correctModel.CarType = cartypeModel.CarType;
                    correctModel.Remark = $"场内注销：{correctModel.OldCarno}转为临时卡";
                    correctModel.Operator = model.RechargeOperator;

                    if (cartypeModel.CarType == CarTypeEnum.MonthCar || cartypeModel.CarType == CarTypeEnum.VIPCar)
                    {
                        //获取本次停车卡类
                        CarTypeModel entryCarTypeModel = _iCarTypeContext.GetCarType(entermodel.CarTypeGuid);
                        if (entryCarTypeModel != null)
                        {
                            //本次停车是月卡或者贵宾卡，才执行下面的操作(考虑到临时车入场》开卡后并未出场一直以临时车状态存在，则执行退卡注销操作)
                            if (entryCarTypeModel.CarType == CarTypeEnum.MonthCar || entryCarTypeModel.CarType == CarTypeEnum.VIPCar)
                            {
                                correctModel.OldGuid = entermodel.Guid;
                                correctModel.DeviceIdentify = entermodel.DriveWayMAC;
                                correctModel.ImgUrl = entermodel.ImgUrl;
                                correctModel.InTime = DateTime.Now;
                                CorrectToEntryCamera(correctModel);   //将修正车牌数据通过mq交给相机去处理业务流程(新车牌入场、旧车牌出场)

                            }
                        }
                    }
                    else if (cartypeModel.CarType == CarTypeEnum.ValueCar)
                    {
                        correctModel.OldGuid = entermodel.Guid;
                        correctModel.DeviceIdentify = entermodel.DriveWayMAC;
                        correctModel.ImgUrl = entermodel.ImgUrl;
                        correctModel.InTime = entermodel.EventTime;//改为储值车之前的 入场时间 
                        CorrectToEntryCamera(correctModel);   //将修正车牌数据通过mq交给相机去处理业务流程(新车牌入场、旧车牌出场)
                    }
                }
            }
            return flag;
        }

        /// <summary>
        /// 某车场的所有月卡
        /// </summary>
        /// <param name="parkcode"></param>
        /// <returns></returns>
        public List<CardServiceModel> AllMonthCard(string parkcode)
        {
            try
            {
                List<CardServiceModel> list = _iCardServiceContext.AllTypeCard(parkcode, CarTypeEnum.MonthCar);
                return list;
            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.Bussiness, "", parkcode, "", "Fujica.com.cn.Business.ParkLot.CardServiceManager.AllMonthCard", string.Format("获取所有月卡发生异常，入参:{0}", parkcode), ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// 某车场的所有储值卡
        /// </summary>
        /// <param name="pakcode"></param>
        /// <returns></returns>
        public List<CardServiceModel> AllValueCard(string parkcode)
        {
            try
            {
                List<CardServiceModel> list = _iCardServiceContext.AllTypeCard(parkcode, CarTypeEnum.ValueCar);
                return list;
            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.Bussiness, "", parkcode, "", "Fujica.com.cn.Business.ParkLot.CardServiceManager.AllMonthCard", string.Format("获取所有月卡发生异常，入参:{0}", parkcode), ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// 某车场的所有临时车
        /// </summary>
        /// <param name="parkcode"></param>
        /// <returns></returns>
        public List<CardServiceModel> AllTempCard(string parkcode)
        {
            try
            {
                List<CardServiceModel> list = _iCardServiceContext.AllTypeCard(parkcode, CarTypeEnum.TempCar);
                return list;
            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.Bussiness, "", parkcode, "", "Fujica.com.cn.Business.ParkLot.CardServiceManager.AllTempCard", string.Format("获取所有临时卡发生异常，入参:{0}", parkcode), ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// 获取临时车分页的数据
        /// </summary>
        /// <param name="model">查询条件实体</param>
        /// <returns></returns>
        public List<CardServiceModel> GetTempCardPage(CardServiceSearchModel model)
        {
            try
            {
                List<CardServiceModel> list = _iCardServiceContext.GetCardPage(model, CarTypeEnum.TempCar);
                return list;
            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.Bussiness,"", model.ParkingCode,model.CarNo, "Fujica.com.cn.Business.ParkLot.CardServiceManager.GetTempCardPage", "获取临时卡分页发生异常", ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// 获取月租车分页的数据
        /// </summary>
        /// <param name="model">查询条件实体</param>
        /// <returns></returns>
        public List<CardServiceModel> GetMonthCardPage(CardServiceSearchModel model)
        {
            try
            {
                List<CardServiceModel> list = _iCardServiceContext.GetCardPage(model, CarTypeEnum.MonthCar);
                return list;
            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.Bussiness, "", "", model.CarNo, "Fujica.com.cn.Business.ParkLot.CardServiceManager.GetMonthCardPage", "获取月租车分页发生异常", ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// 获取储值车分页的数据
        /// </summary>
        /// <param name="model">查询条件实体</param>
        /// <returns></returns>
        public List<CardServiceModel> GetValueCardPage(CardServiceSearchModel model)
        {
            try
            {
                List<CardServiceModel> list = _iCardServiceContext.GetCardPage(model, CarTypeEnum.ValueCar);
                return list;
            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.Bussiness, "", model.ParkingCode, model.CarNo, "Fujica.com.cn.Business.ParkLot.CardServiceManager.GetValueCardPage", "获取储值车分页发生异常", ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// 获取某卡
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        //public CardServiceModel GetCard(string carNo, string parkCode)
        //{
        //    return _iCardServiceContext.GetCard(carNo, parkCode, CarTypeEnum.ValueCar);
        //}

        /// <summary>
        /// 验证当前车牌是否存在(查询范围月卡、贵宾卡、储值卡、临时卡集合)
        /// </summary>
        /// <param name="carNo"></param>
        /// <param name="parkingCode"></param>
        /// <returns></returns>
        public CardServiceModel ExistCard(string carNo, string parkingCode)
        {
            CardServiceModel model = null;
            model = _iCardServiceContext.GetCard(carNo, parkingCode, CarTypeEnum.MonthCar);
            if (model != null) return model;
            model = _iCardServiceContext.GetCard(carNo, parkingCode, CarTypeEnum.ValueCar);
            if (model != null) return model;
            model = _iCardServiceContext.GetCard(carNo, parkingCode, CarTypeEnum.TempCar);
            return model;
        }

        /// <summary>
        /// 发送临时卡数据到相机
        /// </summary>
        /// <returns></returns>
        private bool SendTempCarToCameras(CardServiceModel model)
        {
            try
            {
                bool result = false;
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
                    command = BussineCommand.TempCar,
                    idMsg = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                    message = sendmodel
                };

                result = m_rabbitMQ.SendMessageForRabbitMQ("发送临时卡命令", m_serializer.Serialize(entity), entity.idMsg, model.ParkCode);
                if (!result)
                    LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_MQ_SEND_ERROR.GetDesc();
                return result;
            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.Bussiness, "", model.ParkCode,model.CarNo, "Fujica.com.cn.Business.ParkLot.CardServiceManager.SendTempCarToCameras", "下发临时卡时发生异常", ex.ToString());
                LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_MQ_SEND_ERROR.GetDesc();
                return false;
            }
        }
        /// <summary>
        /// 发送月卡数据到相机
        /// </summary>
        /// <returns></returns>
        private bool SendMonthCarToCameras(CardServiceModel model)
        {
            try
            {
                bool result = false;
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

                result = m_rabbitMQ.SendMessageForRabbitMQ("发送月卡命令", m_serializer.Serialize(entity), entity.idMsg, model.ParkCode);
                if (!result)
                    LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_MQ_SEND_ERROR.GetDesc();
                return result;
            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.Bussiness, "", model.ParkCode, model.CarNo, "Fujica.com.cn.Business.ParkLot.CardServiceManager.SendMonthCarToCameras", "下发月卡时发生异常", ex.ToString());
                LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_MQ_SEND_ERROR.GetDesc();
                return false;
            }
        }

        /// <summary>
        /// 发送储值卡数据到相机
        /// </summary>
        /// <returns></returns>
        public bool SendValueCarToCameras(CardServiceModel model)
        {
            try
            {
                bool result = false;
                ValueCardModel sendmodel = new ValueCardModel()
                {
                    CarOwnerName = model.CarOwnerName,
                    CarNo = model.CarNo,
                    Delete = model.Enable ? false : true,
                    CarTypeGuid = model.CarTypeGuid,
                    Locked = model.Locked,
                    Balance = model.Balance
                };

                CommandEntity<ValueCardModel> entity = new CommandEntity<ValueCardModel>()
                {
                    command = BussineCommand.ValueCar,
                    idMsg = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                    message = sendmodel
                };

                //广播到该车场的所有设备
                result = m_rabbitMQ.SendMessageForRabbitMQ("发送储值卡卡命令", m_serializer.Serialize(entity), entity.idMsg, model.ParkCode);
                if (!result)
                    LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_MQ_SEND_ERROR.GetDesc();
                return result;
            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.Bussiness, "", model.ParkCode, model.CarNo, "Fujica.com.cn.Business.ParkLot.CardServiceManager.SendValueCarToCameras", "下发储值卡时发生异常", ex.ToString());
                LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_MQ_SEND_ERROR.GetDesc();
                return false;
            }
        }

        /// <summary>
        /// 发送贵宾卡数据到相机
        /// </summary>
        /// <returns></returns>
        private bool SendVipCarToCameras(CardServiceModel model)
        {
            //MonthCardModel sendmodel = new MonthCardModel()
            //{
            //    parkCode = model.parkCode,
            //    carOwnerName = model.carOwnerName,
            //    carNo = model.carNo,
            //    delete = false,
            //    carTypeGuid = model.carTypeGuid,
            //    locked = false,
            //    startDate = model.startDate,
            //    endDate = model.endDate
            //};

            //CommandEntity<MonthCardModel> entity = new CommandEntity<MonthCardModel>()
            //{
            //    command = BussineCommand.MonthCar,
            //    idMsg = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
            //    message = sendmodel
            //};
            //List<DrivewayModel> driveways = drivewaymanager.AllDriveway(model.parkCode);
            ////广播到该车场的所有设备
            //foreach (var item in driveways)
            //{
            //    return m_rabbitMQ.SendMessageForRabbitMQ("发送月卡命令", m_serializer.Serialize(entity), item.deviceMacAddress, model.parkCode);
            //}
            return true;
        }

        /// <summary>
        /// 发送所有月卡数据到相机
        /// </summary>
        /// <param name="parkingCode">停车场编码</param>
        /// <returns>返回成功数量</returns>
        public int SendMonthCarAllToCameras(string parkingCode)
        {
            int successCount = 0;
            try
            {

                List<CardServiceModel> list = AllMonthCard(parkingCode);
                foreach (var item in list)
                {
                    bool itemResult = SendMonthCarToCameras(item);
                    if (itemResult) successCount++;
                }
            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.Bussiness, "", parkingCode, "", "Fujica.com.cn.Business.ParkLot.CardServiceManager.SendMonthCarAllToCameras", "下发所有月卡时发生异常", ex.ToString());
            }
            return successCount;
        }

        /// <summary>
        /// 发送所有储值卡数据到相机
        /// </summary>
        /// <param name="parkingCode">停车场编码</param>
        /// <returns>返回成功数量</returns>
        public int SendValueCarAllToCameras(string parkingCode)
        {
            int successCount = 0;
            try
            {

                List<CardServiceModel> list = AllValueCard(parkingCode);
                foreach (var item in list)
                {
                    bool itemResult = SendValueCarToCameras(item);
                    if (itemResult) successCount++;
                }
            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.Bussiness, "", parkingCode, "", "Fujica.com.cn.Business.ParkLot.CardServiceManager.SendValueCarAllToCameras", "下发所有储值卡时发生异常", ex.ToString());
            }
            return successCount;
        }

        /// <summary>
        /// 发送所有临时卡数据到相机
        /// </summary>
        /// <param name="parkingCode">停车场编码</param>
        /// <returns>返回成功数量</returns>
        public int SendTempCarAllToCameras(string parkingCode)
        {
            int successCount = 0;
            try
            {
                List<CardServiceModel> list = AllTempCard(parkingCode);
                foreach (var item in list)
                {
                    bool itemResult = SendTempCarToCameras(item);
                    if (itemResult) successCount++;
                }
            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.Bussiness, "", parkingCode, "", "Fujica.com.cn.Business.ParkLot.CardServiceManager.SendMonthCarAllToCameras", "下发所有月卡时发生异常", ex.ToString());
            }
            return successCount;
        }

        /// <summary>
        /// 补发入场记录到入口相机
        /// </summary>
        /// <param name="model">补录车牌实体参数</param>
        /// <returns></returns>
        public bool SendEntryRecordToCamera(AddRecordModel model)
        {
            CommandEntity<AddRecordModel> entity = new CommandEntity<AddRecordModel>()
            {
                command = BussineCommand.AddRecord,
                idMsg = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                message = model
            };
            //将补发入场的数据通过mq交给相机去处理业务
            return m_rabbitMQ.SendMessageForRabbitMQ("发送补录入场命令", m_serializer.Serialize(entity), entity.idMsg, model.ParkingCode);

        }

        /// <summary>
        /// 场内储值卡、月卡注销时，场内转卡类 发送车牌修正命令
        /// </summary>
        /// <param name="model">修正车牌实体参数</param>
        /// <returns></returns>
        public bool CorrectToEntryCamera(CorrectCarnoModel model)
        {
            CommandEntity<CorrectCarnoModel> entity = new CommandEntity<CorrectCarnoModel>()
            {
                command = BussineCommand.Correct,
                idMsg = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                message = model
            };
            //将修正车牌数据通过mq交给相机去处理业务流程(新车牌入场、旧车牌出场)
            return m_rabbitMQ.SendMessageForRabbitMQ("卡务:发送修正车牌命令", m_serializer.Serialize(entity), entity.idMsg, model.ParkingCode);
        } 

        #region 月卡数据同步到主平台 增删改方法
        /// <summary>
        /// 月卡数据同步到主平台(1-新增) (2-/延期/续费)
        /// </summary>
        /// <param name="model"></param> 
        /// <param name="idx"></param>
        /// <returns></returns>
        private bool AddMonthCardDataToFujica(CardServiceModel model, int actionType, string idx)
        {
            Dictionary<string, object> dicParam = new Dictionary<string, object>(); 
            string beginDt = model.StartDate.Date.ToString("yyyy-MM-dd HH:mm:ss");//开始时间从0分秒
            string endDt = model.EndDate.Date.AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss");  //结束时间截止到23:59:59
            model.EndDate =Convert.ToDateTime(model.EndDate.Date.ToString("yyyy-MM-dd HH:mm:ss"));
            model.PrimaryEndDate = Convert.ToDateTime(model.PrimaryEndDate.Date.ToString("yyyy-MM-dd HH:mm:ss"));
            RequestFujicaStandard requestFujica = new RequestFujicaStandard();
            //请求方法
            string servername = "Park/UnderLineMonthCard";
            switch (actionType)
            {
                case 1:// 新增
                    dicParam["OperatType"] = OperatTypeEnum.Activate; //操作类型 -新增  
                    break;
                case 2:// 卡延期/续费
                    dicParam["OperatType"] = OperatTypeEnum.Recharge; //操作类型-充值    
                    break;
            }
            //请求参数  
            dicParam["CarType"] = idx;
            dicParam["PhoneNumber"] = model.Mobile;
            dicParam["OwnerName"] = model.CarOwnerName;
            dicParam["StartDate"] = Convert.ToDateTime(beginDt);
            dicParam["PrimaryEndDate"] = model.PrimaryEndDate; //计算前结束日期 
            dicParam["EndDate"] = Convert.ToDateTime(endDt); //计算后结束日期
            dicParam["RenewalType"] = RenewalTypeEnum.Day; //目前默认都是按天延期  
            dicParam["RenewalValue"] = (model.EndDate - model.PrimaryEndDate).Days;
            dicParam["ParkingCode"] = model.ParkCode;
            dicParam["CarNo"] = model.CarNo;
            dicParam["RechargeAmount"] =model.PayAmount;
            dicParam["PaymentType"] = PaymentTypeEnum.SP4G;//支付类型：4G支付
            dicParam["OrderType"] = OrderTypeEnum.ReadyMoney;//订单类型：现金支付
            dicParam["TransactionTime"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            dicParam["Remarks"] = model.Remark;
            dicParam["OrderStauts"] = OrderStautsEnum.OfflineConfirm; //线下确认
            dicParam["RechargeOperator"] = model.RechargeOperator;
           // dicParam["CardStatus"] = isDelete ? 2 : 1; 
            return requestFujica.RequestInterfaceV2(servername, dicParam);
        }

        /// <summary>
        /// 月卡数据同步到主平台(修改)
        /// </summary>
        /// <param name="model"></param> 
        /// <param name="idx"></param>
        /// <returns></returns>
        private bool UpdateMonthCardDataToFujica(CardServiceModel model, string idx)
        {
            Dictionary<string, object> dicParam = new Dictionary<string, object>();  
            RequestFujicaStandard requestFujica = new RequestFujicaStandard(); 
            string servername = "BasicResource/ApiUpdateMonthCardInfo"; 
            dicParam["CarNo"] = model.CarNo;
            dicParam["ParkingCode"] = model.ParkCode;
            dicParam["CarType"] = idx;
            dicParam["Phone"] = model.Mobile;
            dicParam["OwnerName"] = model.CarOwnerName; 
            dicParam["CardStatus"] = model.Enable == true ? 1 : 2; 
            dicParam["StartDate"] =model.StartDate; 
            dicParam["EndDate"] = model.EndDate;    
            return requestFujica.RequestInterfaceV2(servername, dicParam);
        }

        /// <summary>
        /// 月卡数据同步到主平台(删除退卡)
        /// </summary>
        /// <param name="model"></param> 
        /// <param name="idx"></param>
        /// <returns></returns>
        private bool BackMonthCardDataToFujica(CardServiceModel model, string idx)
        {
            Dictionary<string, object> dicParam = new Dictionary<string, object>(); 
             RequestFujicaStandard requestFujica = new RequestFujicaStandard(); 
            string servername = "Park/UnderLineBackMonthCard"; 
            dicParam["OperatType"] = OperatTypeEnum.Withdraw; //操作类型 -退卡
            dicParam["CarType"] = idx;  
            dicParam["PrimaryEndDate"] =model.EndDate; //计算前结束日期 (退卡只给当前卡的截止日期)
            dicParam["EndDate"] =model.EndDate; //计算后结束日期 
            dicParam["ParkingCode"] = model.ParkCode;
            dicParam["CarNo"] = model.CarNo;
            dicParam["RechargeAmount"] =model.PayAmount;
            dicParam["PaymentType"] = PaymentTypeEnum.SP4G;//支付类型：4G支付
            dicParam["OrderType"] = OrderTypeEnum.ReadyMoney;//订单类型：现金支付
            dicParam["TransactionTime"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            dicParam["Remarks"] = model.Remark;
            dicParam["OrderStauts"] = OrderStautsEnum.OfflineConfirm; //线下确认
            dicParam["RechargeOperator"] = model.RechargeOperator;   
            return requestFujica.RequestInterfaceV2(servername, dicParam);
        }

        #endregion 
         

        #region  储值卡数据同步至主平台 增删改方法
        /// <summary>
        /// 储值卡数据同步到主平台(新增和充值)
        /// </summary>
        /// <param name="model"></param>
        /// <param name="actionType"></param>
        /// <param name="idx">卡类标识 3A 3C</param>
        /// <returns></returns>
        public bool AddValueCardToFujica(CardServiceModel model, int actionType, string idx)
        {
            //请求参数
            Dictionary<string, object> dicParam = new Dictionary<string, object>();
            RequestFujicaStandard requestFujica = new RequestFujicaStandard(); 
            string servername = "/Park/UnderLineValueCard";
            switch (actionType)
            {
                case 1://新增api 
                    servername += "";
                    dicParam["OperatType"] = OperatTypeEnum.Activate; //操作类型 -新增 
                    dicParam["PrimaryBalance"] = 0; //储值卡计算前余额
                    break;
                case 2://充值   
                    dicParam["OperatType"] = OperatTypeEnum.Recharge; //操作类型-充值 
                    dicParam["PrimaryBalance"] = model.Balance - model.PayAmount; //储值卡计算前余额
                    break;
            }
            dicParam["PhoneNumber"] = model.Mobile;
            dicParam["OwnerName"] = model.CarOwnerName; //车主名
            dicParam["CarType"] = idx;
            dicParam["CurrentBalance"] = model.Balance; //储值卡计算后余额 
            dicParam["ParkingCode"] = model.ParkCode; 
            dicParam["CarNo"] = model.CarNo;
            dicParam["CardNo"] = ""; 
            dicParam["RechargeAmount"] = model.PayAmount;
            dicParam["PaymentType"] = PaymentTypeEnum.SP4G;//支付类型：4G支付
            dicParam["OrderType"] = OrderTypeEnum.ReadyMoney;//订单类型：现金支付
            dicParam["TransactionTime"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            dicParam["Remarks"] = model.Remark;
            dicParam["OrderStauts"] = OrderStautsEnum.OfflineConfirm; //线下确认
            dicParam["RechargeOperator"] = model.RechargeOperator;  
            return requestFujica.RequestInterfaceV2(servername, dicParam);
        }

        /// <summary>
        /// 储值卡数据同步到主平台(退卡删除)
        /// </summary>
        /// <param name="model"></param> 
        /// <param name="idx">卡类标识 3A 3C</param>
        /// <returns></returns>
        public bool BackValueCardToFujica(CardServiceModel model, string idx)
        {
            //请求参数
            Dictionary<string, object> dicParam = new Dictionary<string, object>();
            RequestFujicaStandard requestFujica = new RequestFujicaStandard(); 
            string servername = "/Park/UnderLineBackValueCard"; 
            dicParam["CarType"] = idx;
            dicParam["CurrentBalance"] = model.Balance; //储值卡计算后余额 
            dicParam["PrimaryBalance"] = model.Balance + model.PayAmount; //储值卡计算前余额
            dicParam["ParkingCode"] = model.ParkCode;
            dicParam["CarNo"] = model.CarNo;
            dicParam["CardNo"] = "";
            dicParam["RechargeAmount"] = model.PayAmount;
            dicParam["PaymentType"] = PaymentTypeEnum.SP4G;//支付类型：4G支付
            dicParam["OrderType"] = OrderTypeEnum.ReadyMoney;//订单类型：现金支付
            dicParam["TransactionTime"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            dicParam["Remarks"] = model.Remark;
            dicParam["OrderStauts"] = OrderStautsEnum.OfflineConfirm; //线下确认
            dicParam["RechargeOperator"] = model.RechargeOperator; 
            return requestFujica.RequestInterfaceV2(servername, dicParam);
        }

        /// <summary>
        /// 储值卡数据同步到主平台(扣费出场)
        /// </summary>
        /// <param name="model"></param> 
        /// <param name="idx">卡类标识 3A 3C</param>
        ///  <param name="lineRecordCode">线下停车记录编码</param>
        /// <returns></returns>
        public bool PayV2ValueCardToFujica(CardServiceModel model, string idx, string lineRecordCode)
        {
            //请求参数
            Dictionary<string, object> dicParam = new Dictionary<string, object>();
            RequestFujicaStandard requestFujica = new RequestFujicaStandard(); 
            string servername = "/CalculationCost/OfflineValueCardPayV2"; 
            dicParam["ParkingCode"] = model.ParkCode;
            dicParam["CarNo"] = model.CarNo;
            TimeSpan timeSpan = DateTime.Now - model.AdmissionDate;  //两个时间相减 。默认得到的是两个的时间差  
            double getMinute = timeSpan.TotalMinutes; //将这个天数转换成分钟, 返回值是double类型的  
            dicParam["LongStop"] = (int)Math.Round(getMinute, 0);//（保留0位小数就是取整，四舍五入） 
            dicParam["CardNo"] = "";
            dicParam["LineRecordCode"] = lineRecordCode; //线下停车记录编码
            dicParam["CarType"] = idx;  
            dicParam["AdmissionDate"] = model.AdmissionDate;
            dicParam["BillingStartTime"] = model.AdmissionDate;  //计费开始时间 即为入场时间，已问过小乔； //model.BillingStartTime;
            dicParam["BillingDeadline"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); 
            dicParam["DeductionAmount"] = model.PayAmount;
            dicParam["Remark"] = model.Remark;
            dicParam["Balance"] = model.Balance;
            dicParam["ConsumeOperator"] = model.RechargeOperator; 
            return requestFujica.RequestInterfaceV2(servername, dicParam);
        }

        /// <summary>
        /// 储值卡数据同步到主平台(修改)
        /// </summary>
        /// <param name="model"></param> 
        /// <param name="idx">卡类标识 3A 3C</param> 
        /// <returns></returns>
        public bool UpdateValueCardToFujica(CardServiceModel model, string idx)
        {
            //请求参数
            Dictionary<string, object> dicParam = new Dictionary<string, object>();
            RequestFujicaStandard requestFujica = new RequestFujicaStandard(); 
            string servername = "/BasicResource/ApiUpdateValueCardInfo";
            dicParam["CarNo"] = model.CarNo;
            dicParam["ParkingCode"] = model.ParkCode;
            dicParam["CarType"] = idx;
            dicParam["Phone"] = model.Mobile;
            dicParam["OwnerName"] = model.CarOwnerName; 
            dicParam["CardStatus"] = model.Enable == true ? 1 : 2;
            dicParam["Balance"] = model.Balance; 
            return requestFujica.RequestInterfaceV2(servername, dicParam);
        }
        #endregion


        /// <summary>
        /// 在场车辆场内转卡
        /// </summary>
        /// <param name="model">卡务实体模型</param>
        /// <param name="idx">变更后的 卡类标识 </param>
        /// <param name="cardTypeEnum">变更后的 卡类型</param>
        /// <param name="describe">备注</param>
        /// <param name="changeTime">变更时间</param>
        /// <returns></returns>
        private bool CarChangeDataToFujica(CardServiceModel model, string idx, CarTypeEnum cardTypeEnum, string describe,DateTime changeTime)
        {
            //将cardType转换成fujica Api的对应值
            //互联网车场卡类型：0=时租车 1=月租车 2=储值车 3=贵宾车
            //fujica卡类型： 1月卡  2储值卡 3 临时卡
            int cardType = 0;
            switch (cardTypeEnum)
            {
                case CarTypeEnum.TempCar:
                    cardType = 3;
                    break;
                case CarTypeEnum.MonthCar:
                    cardType = 1;
                    break;
                case CarTypeEnum.ValueCar:
                    cardType = 2;
                    break;
                case CarTypeEnum.VIPCar:
                    cardType = 1;
                    break;
            }

            RequestFujicaStandard requestFujica = new RequestFujicaStandard();
            //请求方法
            string servername = "/Park/ApiInParkingCarChange";

            //请求参数
            Dictionary<string, object> dicParam = new Dictionary<string, object>();
            dicParam["ChangeType"] = 1;
            dicParam["CarNo"] = model.CarNo;
            dicParam["CardNo"] = "";
            dicParam["ParkingCode"] = model.ParkCode;
            dicParam["SmallParkingCode"] = 0;
            dicParam["Describe"] = describe;
            dicParam["ParkingCard"] = "";
            dicParam["ChangeTime"] = changeTime;
            dicParam["CarType"] = idx;
            dicParam["CardType"] = cardType;
            dicParam["CustomDate"] = DateTime.Now;

            //返回fujica api请求结果
            return requestFujica.RequestInterfaceV2(servername, dicParam);
        }

        /// <summary>
        ///  根据车场编号和车牌号码 获取储值车缴费信息(请求主平台接口)
        /// </summary>  
        /// <returns></returns>
        public ValueCardFeeModel GetValueCardFeeInfoByCarNo(ValueCardFeeModel model )
        {
            ValueCardFeeModel responseModel = null; 
            //通过主平台api接口读取储值卡类 停车费用信息
            RequestFujicaStandard requestFujica = new RequestFujicaStandard();
            //请求方法
            string servername = "/Park/GetValueCardInfoByCarNo";
            //请求参数
            Dictionary<string, object> dicParam = new Dictionary<string, object>();
            dicParam["ParkingCode"] = model.ParkingCode;
            dicParam["CarNo"] = model.CarNo;
            if (requestFujica.RequestInterfaceV2(servername, dicParam))
            {
                string fujicaResult = requestFujica.FujicaResult;
                if (!string.IsNullOrEmpty(fujicaResult))
                {
                    //解析返回参数json字符串，拿到停车金额信息
                    Dictionary<string, object> tempResultDic = m_serializer.Deserialize<Dictionary<string, object>>(fujicaResult);

                    if (tempResultDic["Result"] != null)
                    {
                        fujicaResult = Convert.ToString(tempResultDic["Result"]);
                        Dictionary<string, object> dicFujicaResult = null;
                        dicFujicaResult = m_serializer.Deserialize<Dictionary<string, object>>(fujicaResult);
                        decimal parkingFee = 0;//停车费
                        decimal balance = 0;//账户余额 
                        string lineRecordCode = null;//线下停车记录编码 
                        CardServiceModel cardServiceModel = _iCardServiceContext.GetCard(model.CarNo, model.ParkingCode, CarTypeEnum.ValueCar);// 获取某卡的余额  
                        if (cardServiceModel != null) balance = cardServiceModel.Balance; //账户余额
                        parkingFee = Convert.ToDecimal(dicFujicaResult["ParkingFee"]); //从主平台获取停车费
                        lineRecordCode = dicFujicaResult["LineRecordCode"].ToString(); 
                        responseModel = new ValueCardFeeModel();
                        responseModel.CarNo = model.CarNo;
                        responseModel.ParkingCode = model.ParkingCode;
                        responseModel.BeginTime = Convert.ToDateTime(dicFujicaResult["BeginTime"]).ToString("yyyy-MM-dd HH:mm:ss");
                        responseModel.ParkingFee = parkingFee;
                        responseModel.Balance = balance;
                        responseModel.LineRecordCode = lineRecordCode;
                        if (balance <parkingFee) //余额不足
                        {
                            responseModel.IsPaid = false;
                            responseModel.ResponseType = ValueCardFeeType.BalanceNoEnough;
                            //97+车编
                            responseModel.QRCode = "http://mops-test.fujica.com.cn/v2/Login/Index?key=FUJICA97" + model.ParkingCode + "&redictType=16";
                           // responseModel.QRCode = "http://mops-test.fujica.com.cn/v2/Login/Index?key=FUJICA97" + model.ParkingCode + "&redictType=16"; //项目更新发布，这里一定要改回来

                        }
                        else
                        {
                            responseModel.IsPaid = true;
                            responseModel.ResponseType = ValueCardFeeType.BalanceSuccess;
                        }
                    }
                }

            }
            return responseModel;
        }

        /// <summary>
        /// 储值车执行扣费
        /// </summary>
        /// <param name="model"></param>
        /// <param name="lineRecordCode">线下停车记录编码</param>
        /// <returns></returns>
        public bool DoValueCardFee(CardServiceModel model,string lineRecordCode)
        {
            //修改储值车 余额
            CardServiceModel content = _iCardServiceContext.GetCard(model.CarNo, model.ParkCode, CarTypeEnum.ValueCar);
            CarTypeModel cartype = _iCarTypeContext.GetCarType(content.CarTypeGuid);
            content.PayAmount = model.PayAmount; //支付金额
            content.Balance -= content.PayAmount; //储值车变动余额 
            content.AdmissionDate = model.AdmissionDate ;
            content.BillingStartTime = model.BillingStartTime;//ChargeableTime 计费时间  
            bool flag = _iCardServiceContext.ModifyCard(content, CarTypeEnum.ValueCar);// 修改储值车 余额 
            if (flag)
            {
                flag= PayV2ValueCardToFujica(content, cartype.Idx, lineRecordCode); //储值卡数据同步到主平台Fujica Api  
                flag= SendValueCarToCameras(content); //发送储值卡数据到相机
            }
            return flag;
        }

        /// <summary>
        /// 根据车场编号和车牌号码获取当前车辆的停车费用信息
        /// </summary>
        /// <param name="parkingCode">停车场编码</param>
        /// <param name="carNumber">车牌号码</param>
        /// <returns></returns>
        public TempCarParkingModel GetTempCarParkingInfo(string parkingCode, string carNumber)
        {
            TempCarParkingModel responseModel = null;
            //通过主平台api接口读取停车费用信息
            RequestFujicaStandard requestFujica = new RequestFujicaStandard();
            //请求方法
            string servername = "/Park/GetTempCarPaymentInfoByCarNo";
            //请求参数
            Dictionary<string, object> dicParam = new Dictionary<string, object>();
            dicParam["ParkingCode"] = parkingCode;
            dicParam["CarNo"] = carNumber;
            bool falg = requestFujica.RequestInterfaceV2(servername, dicParam);
            if (falg)
            {
                string fujicaResult = requestFujica.FujicaResult;
                if (!string.IsNullOrEmpty(fujicaResult))
                {
                    //解析返回参数json字符串，拿到停车金额信息
                    Dictionary<string, object> tempResultDic = m_serializer.Deserialize<Dictionary<string, object>>(fujicaResult);
                    if (tempResultDic["Result"] != null)
                    {
                        fujicaResult = Convert.ToString(tempResultDic["Result"]);

                        Dictionary<string, object> dicFujicaResult = null;
                        dicFujicaResult = m_serializer.Deserialize<Dictionary<string, object>>(fujicaResult);

                        responseModel = new TempCarParkingModel();
                        responseModel.CarNumber = dicFujicaResult["CarNo"].ToString();
                        responseModel.ParkingFee = Convert.ToDecimal(dicFujicaResult["ParkingFee"]);
                        responseModel.ActualAmount = Convert.ToDecimal(dicFujicaResult["ActualAmount"]);
                        //  responseModel.CouponList = dicFujicaResult["CouponList"].ToString();  
                        List<MCoupon> ls = m_serializer.Deserialize<List<MCoupon>>(dicFujicaResult["CouponList"].ToString());
                        responseModel.CouponList = ls;
                        //已缴费用需要通过redis读取
                        responseModel.PaymentAmount = 0;
                    }
                }
            }

            return responseModel;
        }


    }
}
 
