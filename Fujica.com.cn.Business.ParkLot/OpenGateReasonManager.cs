/***************************************************************************************
 * *
 * *        File Name        : OpenGateReasonManager.cs
 * *        Creator          : llp
 * *        Create Time      : 2019-09-17 
 * *        Remark           : 开闸管理器 逻辑类 
 * *
 * *  Copyright (c) 深圳市富士智能系统有限公司.  All rights reserved. 
 * ***************************************************************************************/
using Fujica.com.cn.Business.Base;
using Fujica.com.cn.Business.Model;
using Fujica.com.cn.Bussiness.Entity;
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
    /// 用户对象业务逻辑层.
    /// </summary>
    /// <remarks>
    /// 2019.09.17: 新增版本备注信息 llp <br/> 
    /// 2019.09.17: 修改枚举引用 llp<br/>
    /// </remarks> 
    public class OpenGateReasonManager : IBaseBusiness
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
        /// 开闸信息操作上下文
        /// </summary>
        private IOpenGateReasonContext _iOpenGateReasonContext = null;

        /// <summary>
        /// 车进出信息操作上下文
        /// </summary>
        private IVehicleInOutContext _iVehicleInOutContext = null;

        /// <summary>
        /// MQ消息发送器
        /// </summary>
        private readonly RabbitMQSender m_rabbitMQ;
        /// <summary>
        /// 卡务管理
        /// </summary>
        private CardServiceManager _cardServiceManager = null;
        public string LastErrorDescribe
        {
            get;
            set;
        }

        public OpenGateReasonManager(ILogger logger, ISerializer serializer,
            IOpenGateReasonContext iOpenGateReasonContext,
            IVehicleInOutContext iVehicleInOutContext,
            RabbitMQSender rabbitMQ, CardServiceManager cardServiceManager)
        {
            m_logger = logger;
            m_serializer = serializer;
            _iOpenGateReasonContext = iOpenGateReasonContext;
            m_rabbitMQ = rabbitMQ;
            _cardServiceManager = cardServiceManager;
            _iVehicleInOutContext = iVehicleInOutContext;
        }

        /// <summary>
        /// 保存开闸原因
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool SaveOpenReason(OpenGateReasonModel model)
        {
            return _iOpenGateReasonContext.SaveOpenReason(model);
        }

        /// <summary>
        /// 读取开闸原因
        /// </summary>
        /// <param name="drivewayGuid"></param>
        /// <returns></returns>
        public OpenGateReasonModel GetOpenReason(string guid)
        {
            return _iOpenGateReasonContext.GetOpenReason(guid);
        }

        /// <summary>
        /// 删除开闸原因
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public bool DeleteOpenReason(string guid)
        {
            return _iOpenGateReasonContext.DeleteOpenReason(guid);
        }

        /// <summary>
        /// 读取开闸原因列表
        /// </summary>
        /// <param name="parkCode"></param>
        /// <returns></returns>
        public List<OpenGateReasonModel> GetOpenReasonList(string projectGuid)
        {
            //批量数据都从数据库获取
            List<OpenGateReasonModel> list = _iOpenGateReasonContext.GetOpenReasonList(projectGuid);
            return list;
        }


        /// <summary>
        /// 收费开闸
        /// </summary>
        /// <param name="parkingCode">车编</param>
        /// <param name="carNumber">车牌号</param>
        /// <param name="tolloperator">操作员</param>
        /// <param name="deviceIdentify">设备标识</param> 
        /// <returns></returns>
        public bool ChargeOpenGate(FreeOpenGateModel model)
        {
            bool falg = false;
            try
            {
                //判断 是否有入场数据
                VehicleInOutModel entryModel = _iVehicleInOutContext.GetEntryRecord(model.CarNo, model.ParkingCode);
                if (entryModel != null)
                {
                    //通过主平台api接口读取停车费用信息
                    RequestFujicaStandard requestFujica = new RequestFujicaStandard();
                    //请求方法
                    string servername = "Park/GetTempCarPaymentInfoByCarNo";
                    //请求参数
                    Dictionary<string, object> dicParam = new Dictionary<string, object>();
                    dicParam["ParkingCode"] = model.ParkingCode;
                    dicParam["CarNo"] = model.CarNo;
                    falg = requestFujica.RequestInterfaceV2(servername, dicParam);
                    if (falg)
                    {
                        string fujicaResult = requestFujica.FujicaResult;
                        if (!string.IsNullOrEmpty(fujicaResult))
                        { 
                            Dictionary<string, object> tempResultDic = m_serializer.Deserialize<Dictionary<string, object>>(fujicaResult);
                            if (tempResultDic["Result"] != null)
                            {
                                fujicaResult = Convert.ToString(tempResultDic["Result"]); 
                                Dictionary<string, object> dicFujicaResult = null;
                                dicFujicaResult = m_serializer.Deserialize<Dictionary<string, object>>(fujicaResult); 
                                #region  线下临时卡缴费 添加
                                Dictionary<string, object> dicParam2 = new Dictionary<string, object>();
                                string servername2 = "CalculationCost/OfflinePayV2"; //添加线下临时卡缴费
                                dicParam2["CarType"] = dicFujicaResult["CarType"].ToString();
                                dicParam2["AdmissionDate"] = Convert.ToDateTime(dicFujicaResult["BeginTime"].ToString());
                                // 计费开始时间 （算费接口返回的 最后一次缴费时间或入场时间）
                                dicParam2["BillingStartTime"] = Convert.ToDateTime(Convert.ToDateTime(dicFujicaResult["FinalPaymentTime"].ToString()).Year > 1970 ? dicFujicaResult["FinalPaymentTime"].ToString() : dicFujicaResult["BeginTime"].ToString());
                                dicParam2["BillingDeadline"] = Convert.ToDateTime(dicFujicaResult["ChargeableTime"].ToString());
                                dicParam2["TollOperator"] = model.TolloPerator;
                                dicParam2["PayStyle"] = PaymentTypeEnum.SP4G;//支付类型：G4车场
                                dicParam2["OrderType"] = OrderTypeEnum.ReadyMoney;//订单类型：现金支付 
                                dicParam2["ActualAmount"] = Convert.ToDecimal(dicFujicaResult["ActualAmount"].ToString()); //缴费金额 
                                dicParam2["CarNo"] = model.CarNo;
                                dicParam2["ParkingCode"] = model.ParkingCode;
                                dicParam2["LineRecordCode"] = dicFujicaResult["LineRecordCode"].ToString();
                                dicParam2["Amount"] = Convert.ToDecimal(dicFujicaResult["ParkingFee"].ToString());
                                dicParam2["AmountTime"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                dicParam2["CouponList"] = m_serializer.Deserialize<List<CouponCalculateView>>(dicFujicaResult["CouponList"].ToString());
                                dicParam2["DealNo"] = "4G"; //值给4G,和小乔确认过 
                                dicParam2["Remarks"] = model.Remark;
                                falg = requestFujica.RequestInterfaceV2(servername2, dicParam2);
                                #endregion
                                falg = OpenGateToCamera(new OpenGateModel() { DeviceIdentify = model.DeviceIdentify, OpenType = OpenTypeEnum.Charge,Operator = model.TolloPerator, Remark = model.Remark }, model.ParkingCode);
                                 
                            }
                        }
                    }
                    return falg;
                }
                return falg;
            }

            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.Bussiness, "", model.ParkingCode, model.CarNo, "Fujica.com.cn.Business.ParkLot.CarInOutManager.ChargeOpenGate", "收费开闸时发生异常", ex.ToString());
                return false;
            }
        }


        /// <summary>
        /// 手动开闸--出口未识别到车牌(添加到异常记录报表) 
        ///         --出口识别到车牌（如果是临时车则添加免费临停车缴费记录）
        ///         --入口开闸 未识别到车牌 (添加到异常记录报表)
        /// </summary>
        /// <param name="model">异常开闸记录</param> 
        /// <returns></returns>
        public bool OpenGate(OpenGateRecordModel model)
        {
            bool result = false;
            try
            {
                if (model.EntranceType == 1) //出口 
                {
                    /// 404-禁止无牌车   407-无压地感车辆  加到 "异常开闸记录" 
                    if ((model.ErrorCode != "404") || (model.ErrorCode != "407")) //识别到车牌
                    {
                        if (model.CarType == "0" && model.ErrorCode!="13") //如果是临时车(并且有入场记录)则添加临停车缴费记录
                        {
                            FreeOpenGateModel freemodel = new FreeOpenGateModel()
                            {
                                ParkingCode = model.ParkingCode,
                                DeviceIdentify = model.DeviceIdentify,
                                TolloPerator = model.OpenGateOperator,
                                CarNo = model.CarNo,
                                Remark = model.Remark
                            };
                            result = ChargeOpenGate(freemodel);
                        } 
                        else
                        {
                            result = AddOpenGateRecord(model);
                            
                           // if(result) //先执行扣费
                            //{
                                //if (model.CarType == "2" && model.ErrorCode != "13") //如果是储值车(并且有入场记录) 
                                //{
                                //    ValueCardFeeModel valueCardFeeModel = new ValueCardFeeModel()
                                //    {
                                //        ParkingCode=model.ParkingCode,
                                //        CarNo=model.CarNo
                                //    };
                                //    _cardServiceManager.GetValueCardFeeInfoByCarNo(valueCardFeeModel);
                                //}
                          //  }
                            //在发送出场命令
                            result = OpenGateToCamera(new OpenGateModel() { DeviceIdentify = model.DeviceIdentify, OpenType = OpenTypeEnum.Manual, Operator = model.OpenGateOperator, Remark = model.Remark }, model.ParkingCode);

                        }
                    }
                    else //出口未识别到车牌 
                    {
                        // 添加到异常记录报表
                        result = AddOpenGateRecord(model);
                        if (result)
                            result = OpenGateToCamera(new OpenGateModel() { DeviceIdentify = model.DeviceIdentify, OpenType = OpenTypeEnum.Manual, Operator = model.OpenGateOperator, Remark = model.Remark }, model.ParkingCode);
                    }
                }
                else
                {
                    if ((model.ErrorCode == "404") || (model.ErrorCode == "407")) //入口 未识别到车牌 (添加到异常记录报表)
                    {
                        // 添加到异常记录报表
                        result = AddOpenGateRecord(model); 
                    }
                    result = OpenGateToCamera(new OpenGateModel() { DeviceIdentify = model.DeviceIdentify, OpenType = OpenTypeEnum.Manual, Operator = model.OpenGateOperator, Remark = model.Remark }, model.ParkingCode);
                }
                return result;
            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.Bussiness, "", model.ParkingCode, model.CarNo, "Fujica.com.cn.Business.ParkLot.CarInOutManager.OpenGate", "手动开闸时发生异常", ex.ToString());
                return false;
            }
        }



        /// <summary>
        /// 免费放行(人工免费=应缴费金额）
        /// </summary>
        /// <param name="parkingCode">车编</param>
        /// <param name="carNumber">车牌号</param>
        /// <param name="tolloperator">操作员</param>
        /// <param name="deviceIdentify">设备标识</param> 
        /// <returns></returns>
        public bool FreeOpenGate(FreeOpenGateModel model)
        {
            bool falg = false; 
            try
            {
                //判断 是否有入场数据
                VehicleInOutModel entryModel = _iVehicleInOutContext.GetEntryRecord(model.CarNo, model.ParkingCode);
                if (entryModel != null)
                {
                    //通过主平台api接口读取停车费用信息
                    RequestFujicaStandard requestFujica = new RequestFujicaStandard();
                    //请求方法
                    string servername = "Park/GetTempCarPaymentInfoByCarNo";
                    //请求参数
                    Dictionary<string, object> dicParam = new Dictionary<string, object>();
                    dicParam["ParkingCode"] = model.ParkingCode;
                    dicParam["CarNo"] = model.CarNo;
                    falg = requestFujica.RequestInterfaceV2(servername, dicParam);
                    if (falg)
                    {
                        string fujicaResult = requestFujica.FujicaResult;
                        if (!string.IsNullOrEmpty(fujicaResult))
                        {

                            Dictionary<string, object> tempResultDic = m_serializer.Deserialize<Dictionary<string, object>>(fujicaResult);
                            if (tempResultDic["Result"] != null)
                            {
                                fujicaResult = Convert.ToString(tempResultDic["Result"]);

                                Dictionary<string, object> dicFujicaResult = null;
                                dicFujicaResult = m_serializer.Deserialize<Dictionary<string, object>>(fujicaResult);
                                #region  线下临时卡缴费 添加 
                                Dictionary<string, object> dicParam2 = new Dictionary<string, object>();
                                string servername2 = "CalculationCost/OfflinePayV2"; //添加线下临时卡缴费
                                dicParam2["CarType"] = dicFujicaResult["CarType"].ToString();
                                dicParam2["AdmissionDate"] = Convert.ToDateTime(dicFujicaResult["BeginTime"].ToString());
                                // 计费开始时间 （算费接口返回的 最后一次缴费时间(如果缴费时间>1970说明有过缴费，否则就是入场时间） 
                                dicParam2["BillingStartTime"] = Convert.ToDateTime(Convert.ToDateTime(dicFujicaResult["FinalPaymentTime"].ToString()).Year > 1970 ? dicFujicaResult["FinalPaymentTime"].ToString() : dicFujicaResult["BeginTime"].ToString());
                                dicParam2["BillingDeadline"] = Convert.ToDateTime(dicFujicaResult["ChargeableTime"].ToString());
                                dicParam2["TollOperator"] = model.TolloPerator;
                                dicParam2["PayStyle"] = PaymentTypeEnum.SP4G;//支付类型：G4车场
                                dicParam2["OrderType"] = OrderTypeEnum.ReadyMoney;//订单类型：现金支付  
                                dicParam2["FreeAdmission"] = Convert.ToDecimal(dicFujicaResult["ActualAmount"].ToString()); //人工免费
                                dicParam2["CarNo"] = model.CarNo;
                                dicParam2["ParkingCode"] = model.ParkingCode;
                                dicParam2["LineRecordCode"] = dicFujicaResult["LineRecordCode"].ToString();
                                dicParam2["Amount"] = Convert.ToDecimal(dicFujicaResult["ParkingFee"].ToString());
                                dicParam2["AmountTime"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                dicParam2["CouponList"] = m_serializer.Deserialize<List<CouponCalculateView>>(dicFujicaResult["CouponList"].ToString()); 
                                dicParam2["DealNo"] = "4G"; //值给4G,和小乔确认过  
                                dicParam2["Remarks"] = model.Remark;
                                falg = requestFujica.RequestInterfaceV2(servername2, dicParam2);
                                #endregion 
                                falg = OpenGateToCamera(new OpenGateModel() { DeviceIdentify = model.DeviceIdentify, OpenType = OpenTypeEnum.Free,Operator=model.TolloPerator, Remark = model.Remark }, model.ParkingCode);
                             }
                        } 
                    }
                    return falg;
                }
                else
                {
                    return falg;
                }
            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.Bussiness, "", model.ParkingCode, model.CarNo, "Fujica.com.cn.Business.ParkLot.CarInOutManager.FreeOpenGate", "免费开闸时发生异常", ex.ToString());
                return false;
            }


        }

        /// <summary>
        /// 添加到异常记录报表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddOpenGateRecord(OpenGateRecordModel model)
        {
            bool result = false;
            try
            { 
                Dictionary<string, object> dicParam = new Dictionary<string, object>();
                RequestFujicaStandard requestFujica = new RequestFujicaStandard();
                string servername = "Park/UnderLineAddOpenGateRecord";
                dicParam["ParkingCode"] = model.ParkingCode;
                dicParam["EntranceType"] = model.EntranceType;
                dicParam["EquipmentCode"] = model.DeviceIdentify;
                dicParam["ThroughName"] = model.ThroughName;
                dicParam["DiscernCamera"] = model.DiscernCamera;
                dicParam["ThroughType"] = model.ThroughType;
                dicParam["OpenGateTime"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                dicParam["OpenGateReason"] = model.OpenGateReason;
                dicParam["OpenGateOperator"] = model.OpenGateOperator;
                dicParam["Remarks"] = model.Remark;
                dicParam["ImageUrl"] = model.ImageUrl; 
                result = requestFujica.RequestInterfaceV2(servername, dicParam);
                return result;
            } 
             
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.Bussiness, "", model.ParkingCode, "", "Fujica.com.cn.Business.ParkLot.CarInOutManager.AddOpenGateRecord", "添加到异常记录报表时发生异常", ex.ToString());
                return false;
            }
        }
        /// <summary>
        /// 发送开闸命令 
        /// </summary>
        /// <param name="model">开闸模型实体参数</param>
        /// <returns></returns>
        public bool OpenGateToCamera(OpenGateModel model, string parkingCode)
        {
            CommandEntity<OpenGateModel> entity = new CommandEntity<OpenGateModel>()
            {
                command = BussineCommand.OpenGate,
                idMsg = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                message = new OpenGateModel()
                {
                    DeviceIdentify = model.DeviceIdentify,
                    OpenType = model.OpenType,
                    Operator=model.Operator,
                    Remark=model.Remark
                }
            }; 
            return m_rabbitMQ.SendMessageForRabbitMQ(string.Format("发送{0}开闸命令", model.OpenType.GetDesc()), m_serializer.Serialize(entity), entity.idMsg, parkingCode);
        }
        
    }
}
