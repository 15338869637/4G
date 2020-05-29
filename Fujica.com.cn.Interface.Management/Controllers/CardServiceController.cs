using Fujica.com.cn.Business.ParkLot;
using Fujica.com.cn.Context.Model;
using Fujica.com.cn.Interface.Management.Models.InPut;
using Fujica.com.cn.Interface.Management.Models.OutPut;
using Fujica.com.cn.Logger;
using Fujica.com.cn.Security.AdmissionControl;
using Fujica.com.cn.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;

/***************************************************************************************
 * *
 * *        File Name        : CardServiceController.cs
 * *        Creator          : 吕林平
 * *        Create Time      : 2019-09-03
 * *        Functional Description  : 卡务业务模块
 * *        Remark           :  
 * *
 * *  Copyright (c) 深圳市富士智能系统有限公司.  All rights reserved. 
 * ***************************************************************************************/
namespace Fujica.com.cn.Interface.Management.Controllers
{
    /// <summary>
    /// 卡务管理模块
    /// </summary>
    /// <remarks>
    /// 2019.10.25: 修改.月卡、临时卡、储值卡分页搜索增加卡类查询条件 Ase <br/> 
    /// </remarks>
    public class CardServiceController : BaseController
    {
        private CardServiceManager _cardServiceManager = null;

        /// <summary>
        /// 唯一构造函数
        /// </summary>
        /// <param name="logger">日志接口器</param>
        /// <param name="serializer">序列化接口器</param>
        /// <param name="apiaccesscontrol">api接入控制器</param>
        /// <param name="cardServiceManager">卡务管理器</param>
        public CardServiceController(ILogger logger, ISerializer serializer, APIAccessControl apiaccesscontrol, 
            CardServiceManager cardServiceManager) : base(logger, serializer, apiaccesscontrol)
        {
            _cardServiceManager = cardServiceManager;
        }

        /// <summary>
        /// 开卡  
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ResponseType(typeof(ResponseBaseCommon))]
        public IHttpActionResult ApplyNewCard(ApplyCardServiceRequest model)
        {
            ResponseBaseCommon response = new ResponseBaseCommon()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };

            if (string.IsNullOrWhiteSpace(model.ParkingCode) ||
                string.IsNullOrWhiteSpace(model.ProjectGuid) ||
                string.IsNullOrWhiteSpace(model.CarOwnerName) ||
                string.IsNullOrWhiteSpace(model.Mobile) ||
                string.IsNullOrWhiteSpace(model.CarNo) ||
                string.IsNullOrWhiteSpace(model.DrivewayGuidList) ||
                string.IsNullOrWhiteSpace(model.CarTypeGuid) ||
                model.PayAmount < 0 ||
                string.IsNullOrWhiteSpace(model.PayStyle))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "必要参数缺失,请检查";
                Logger.LogWarn(LoggerLogicEnum.Interface, "", "", "",
                    "Fujica.com.cn.Interface.Management.Controllers.CardServiceController.ApplyNewCard",
                    string.Format("开卡必要参数不全，入参:{0}", Serializer.Serialize(model)));
                return Ok(response);
            }

            if (model.StartDate.Date != DateTime.Now.Date) model.StartDate = DateTime.Now; //开始日期必须是当天

            string[] drivewayguidarray = drivewayguidarray = model.DrivewayGuidList.Split(',');

            CardServiceModel content = new CardServiceModel()
            {
                ProjectGuid = model.ProjectGuid,
                ParkCode = model.ParkingCode,
                CarOwnerName = model.CarOwnerName,
                CarNo = model.CarNo,
                CarTypeGuid = model.CarTypeGuid,
                DrivewayGuidList = drivewayguidarray.ToList().FindAll(o => o != ""),
                Mobile = model.Mobile,
                PayAmount = model.PayAmount,
                PayStyle = model.PayStyle,
                Remark = model.Remark,
                Balance = model.PayAmount, //新开卡时余额就等于支付金额(仅当储值卡时后端逻辑才会读取此值)
                StartDate = model.StartDate,
                PrimaryEndDate=model.StartDate,
                EndDate = model.EndDate,
                RechargeOperator = model.RechargeOperator,
                PauseDate = default(DateTime),
                Locked=false,
                Enable = true
            };

            //需要加上报表存储逻辑

            bool flag = _cardServiceManager.AddNewCard(content);
            if (!flag)
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiCardServiceErrorCode.API_DATA_SAVE_ERROR;
                response.MessageContent = _cardServiceManager.LastErrorDescribe;
            }
            return Ok(response);
        }

        /// <summary>
        /// 月卡修改
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ResponseType(typeof(ResponseBaseCommon))]
        public IHttpActionResult ModifyMonthCard(ModifyCardServiceRequest model)
        {
            ResponseBaseCommon response = new ResponseBaseCommon()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };

            if (string.IsNullOrWhiteSpace(model.ParkingCode) ||
                string.IsNullOrWhiteSpace(model.ProjectGuid) ||
                string.IsNullOrWhiteSpace(model.CarOwnerName) ||
                string.IsNullOrWhiteSpace(model.Mobile) ||
                string.IsNullOrWhiteSpace(model.CarNo) ||
                string.IsNullOrWhiteSpace(model.DrivewayGuidList) ||
                string.IsNullOrWhiteSpace(model.CarTypeGuid))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "必要参数缺失,请检查";
                Logger.LogWarn(LoggerLogicEnum.Interface, "", "", "",
                    "Fujica.com.cn.Interface.Management.Controllers.CardServiceController.ModifyMonthCard",
                    string.Format("月卡修改必要参数不全，入参:{0}", Serializer.Serialize(model)));
                return Ok(response);
            }

            string[] drivewayguidarray = drivewayguidarray = model.DrivewayGuidList.Split(',');

            CardServiceModel content = new CardServiceModel()
            {
                ProjectGuid = model.ProjectGuid,
                ParkCode = model.ParkingCode,
                CarOwnerName = model.CarOwnerName,
                CarNo = model.CarNo,
                CarTypeGuid = model.CarTypeGuid,
                DrivewayGuidList = drivewayguidarray.ToList().FindAll(o => o != ""),
                Mobile = model.Mobile,
                RechargeOperator = model.RechargeOperator
            };

            bool flag = _cardServiceManager.ModifyCard(content);
            if (!flag)
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiCardServiceErrorCode.API_DATA_SAVE_ERROR;
                response.MessageContent = _cardServiceManager.LastErrorDescribe;
            }
            return Ok(response);
        }

        /// <summary>
        /// 储值卡修改
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ResponseType(typeof(ResponseBaseCommon))]
        public IHttpActionResult ModifyValueCard(ModifyCardServiceRequest model)
        {
            ResponseBaseCommon response = new ResponseBaseCommon()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };

            if (string.IsNullOrWhiteSpace(model.ParkingCode) ||
                string.IsNullOrWhiteSpace(model.ProjectGuid) ||
                string.IsNullOrWhiteSpace(model.CarOwnerName) ||
                string.IsNullOrWhiteSpace(model.Mobile) ||
                string.IsNullOrWhiteSpace(model.CarNo) ||
                string.IsNullOrWhiteSpace(model.DrivewayGuidList) ||
                string.IsNullOrWhiteSpace(model.CarTypeGuid))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "必要参数缺失,请检查";
                Logger.LogWarn(LoggerLogicEnum.Interface, "", "", "",
                    "Fujica.com.cn.Interface.Management.Controllers.CardServiceController.ModifyValueCard",
                    string.Format("储值卡修改必要参数不全，入参:{0}", Serializer.Serialize(model)));
                return Ok(response);
            }

            string[] drivewayguidarray = drivewayguidarray = model.DrivewayGuidList.Split(',');

            CardServiceModel content = new CardServiceModel()
            {
                ProjectGuid = model.ProjectGuid,
                ParkCode = model.ParkingCode,
                CarOwnerName = model.CarOwnerName,
                CarNo = model.CarNo,
                CarTypeGuid = model.CarTypeGuid,
                DrivewayGuidList = drivewayguidarray.ToList().FindAll(o => o != ""),
                Mobile = model.Mobile,
                RechargeOperator=model.RechargeOperator
            };

            bool flag = _cardServiceManager.ModifyCard(content);
            if (!flag)
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiCardServiceErrorCode.API_DATA_SAVE_ERROR;
                response.MessageContent = _cardServiceManager.LastErrorDescribe;
            }
            return Ok(response);
        }

        /// <summary>
        /// 月卡续费 注意此时的延期开始时间不是开卡时间
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ResponseType(typeof(ResponseBaseCommon))]
        public IHttpActionResult MonthCardRenew(MonthCardServiceRequest model)
        {
            ResponseBaseCommon response = new ResponseBaseCommon()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };

            if (string.IsNullOrWhiteSpace(model.ParkingCode) ||
                string.IsNullOrWhiteSpace(model.ProjectGuid) ||
                string.IsNullOrWhiteSpace(model.PayStyle) ||
                string.IsNullOrWhiteSpace(model.CarTypeGuid) ||
                model.PayAmount < 0 ||
                string.IsNullOrWhiteSpace(model.CarNo)||
                model.StartDate==default(DateTime)||
                model.EndDate == default(DateTime))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "必要参数缺失,请检查";
                Logger.LogWarn(LoggerLogicEnum.Interface, "", "", "",
                    "Fujica.com.cn.Interface.Management.Controllers.CardServiceController.MonthCardRenew",
                    string.Format("月卡延期必要参数不全，入参:{0}", Serializer.Serialize(model)));
                return Ok(response);
            }

            if (model.StartDate.Date != DateTime.Now.Date) model.StartDate = DateTime.Now; //延期开始日期必须是当天

            CardServiceModel content = new CardServiceModel()
            {
                ProjectGuid = model.ProjectGuid,
                ParkCode = model.ParkingCode,
                CarNo = model.CarNo,
                StartDate=model.StartDate,
                EndDate=model.EndDate,
                PayAmount=model.PayAmount,
                PayStyle=model.PayStyle,
                CarTypeGuid=model.CarTypeGuid,
                PrimaryEndDate=model.PrimaryEndDate,
                RechargeOperator=model.RechargeOperator
            };

            //需要加上报表存储逻辑

            bool flag = _cardServiceManager.RenewCard(content);
            if (!flag)
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiCardServiceErrorCode.API_DATA_SAVE_ERROR;
                response.MessageContent = _cardServiceManager.LastErrorDescribe;
            }
            return Ok(response);
        }

        /// <summary>
        /// 储值车续费
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ResponseType(typeof(ResponseBaseCommon))]
        public IHttpActionResult ValueCardRenew(ValueCardServiceRequest model)
        {
            ResponseBaseCommon response = new ResponseBaseCommon()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };

            if (string.IsNullOrWhiteSpace(model.ParkingCode) ||
                string.IsNullOrWhiteSpace(model.ProjectGuid) ||
                string.IsNullOrWhiteSpace(model.PayStyle) ||
                string.IsNullOrWhiteSpace(model.CarTypeGuid) ||
                model.PayAmount < 0 ||
                string.IsNullOrWhiteSpace(model.CarNo))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "必要参数缺失,请检查";
                Logger.LogWarn(LoggerLogicEnum.Interface, "", "", "",
                    "Fujica.com.cn.Interface.Management.Controllers.CardServiceController.ValueCardRenew",
                    string.Format("储值车续费必要参数不全，入参:{0}", Serializer.Serialize(model)));
                return Ok(response);
            }

            CardServiceModel content = new CardServiceModel()
            {
                ProjectGuid = model.ProjectGuid,
                ParkCode = model.ParkingCode,
                CarNo = model.CarNo,
                PayAmount = model.PayAmount,
                PayStyle = model.PayStyle,
                CarTypeGuid = model.CarTypeGuid,
                RechargeOperator = model.RechargeOperator
            };

            //需要加上报表存储逻辑

            bool flag = _cardServiceManager.RenewCard(content);
            if (!flag)
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiCardServiceErrorCode.API_DATA_SAVE_ERROR;
                response.MessageContent = _cardServiceManager.LastErrorDescribe;
            }
            return Ok(response);
        }

        /// <summary>
        /// 锁定车辆
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ResponseType(typeof(ResponseBaseCommon))]
        public IHttpActionResult CarLocked(CardServiceBaseRequest model)
        {
            ResponseBaseCommon response = new ResponseBaseCommon()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };

            if (string.IsNullOrWhiteSpace(model.ParkingCode) ||
                string.IsNullOrWhiteSpace(model.ProjectGuid) ||
                string.IsNullOrWhiteSpace(model.CarNo))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "必要参数缺失,请检查";
                Logger.LogWarn(LoggerLogicEnum.Interface, "", "", "",
                    "Fujica.com.cn.Interface.Management.Controllers.CardServiceController.CarLocked",
                    string.Format("车辆锁定必要参数不全，入参:{0}", Serializer.Serialize(model)));
                return Ok(response);
            }

            bool flag = _cardServiceManager.LockedCard(model.CarNo,model.ParkingCode);
            if (!flag)
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiCardServiceErrorCode.API_DATA_SAVE_ERROR;
                response.MessageContent = _cardServiceManager.LastErrorDescribe;
            }
            return Ok(response);
        }

        /// <summary>
        /// 解锁车辆
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ResponseType(typeof(ResponseBaseCommon))]
        public IHttpActionResult CarUnLocked(CardServiceBaseRequest model)
        {
            ResponseBaseCommon response = new ResponseBaseCommon()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };

            if (string.IsNullOrWhiteSpace(model.ParkingCode) ||
                string.IsNullOrWhiteSpace(model.ProjectGuid) ||
                string.IsNullOrWhiteSpace(model.CarNo))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "必要参数缺失,请检查";
                Logger.LogWarn(LoggerLogicEnum.Interface, "", "", "",
                    "Fujica.com.cn.Interface.Management.Controllers.CardServiceController.CarLocked",
                    string.Format("车辆解锁必要参数不全，入参:{0}", Serializer.Serialize(model)));
                return Ok(response);
            }

            bool flag = _cardServiceManager.UnLockedCard(model.CarNo, model.ParkingCode);
            if (!flag)
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiCardServiceErrorCode.API_DATA_SAVE_ERROR;
                response.MessageContent = _cardServiceManager.LastErrorDescribe;
            }
            return Ok(response);
        }

        /// <summary>
        /// 注销车辆
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ResponseType(typeof(ResponseBaseCommon))]
        public IHttpActionResult CancelCar(CancelCardServiceRequest model)
        {
            ResponseBaseCommon response = new ResponseBaseCommon()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };

            if (string.IsNullOrWhiteSpace(model.ParkingCode) ||
                string.IsNullOrWhiteSpace(model.ProjectGuid) ||
                string.IsNullOrWhiteSpace(model.CarNo)||
                 model.RefundAmount < 0)
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "必要参数缺失,请检查";
                Logger.LogWarn(LoggerLogicEnum.Interface, "", "", "",
                    "Fujica.com.cn.Interface.Management.Controllers.CardServiceController.CarLocked",
                    string.Format("车辆注销时必要参数不全，入参:{0}", Serializer.Serialize(model)));
                return Ok(response);
            }

            CardServiceModel content = new CardServiceModel()
            {
                ProjectGuid = model.ProjectGuid,
                ParkCode = model.ParkingCode,
                CarNo = model.CarNo,
                PayAmount = model.RefundAmount,
                Remark = model.Remark,
                RechargeOperator = model.RechargeOperator 
            };

            //需要加上报表存储逻辑  
            bool flag = _cardServiceManager.DeleteCard(content); 
            if (!flag)
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiCardServiceErrorCode.API_DATA_SAVE_ERROR;
                response.MessageContent = _cardServiceManager.LastErrorDescribe;
            }
            return Ok(response);
        }

        /// <summary>
        /// 获取月卡列表
        /// </summary>
        /// <param name="requestModel">请求参数实体</param>
        /// <returns></returns>
        [HttpPost]
        [ResponseType(typeof(ResponseBasePaper<CardServiceModel>))]
        public IHttpActionResult GetMonthCardList(MonthCardSearchRequest requestModel)
        {
            ResponseBasePaper<CardServiceModel> response = new ResponseBasePaper<CardServiceModel>()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString(),
                Result = new List<CardServiceModel>()
            };
            if (requestModel == null || string.IsNullOrWhiteSpace(requestModel.ProjectGuid) ||
                string.IsNullOrWhiteSpace(requestModel.ParkingCode) ||
                requestModel.PageIndex <= 0 || requestModel.PageSize <= 0)
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "必要参数格式不对,请检查";
                return Ok(response);
            }

            try
            {
                CardServiceSearchModel searchModel = new CardServiceSearchModel();
                searchModel.PageIndex = requestModel.PageIndex;
                searchModel.PageSize = requestModel.PageSize;
                searchModel.ProjectGuid = requestModel.ProjectGuid;
                searchModel.ParkingCode = requestModel.ParkingCode;

                searchModel.CarNo = requestModel.CarNo; ;
                searchModel.CarOwnerName = requestModel.CarOwnerName;
                searchModel.Mobile = requestModel.Mobile;
                searchModel.StartDate = requestModel.ApplyDate;
                searchModel.CarTypeGuid = requestModel.CarTypeGuid;
                int statusType = requestModel.StatusType;
                if (statusType == 1)
                {
                    searchModel.Enable = true;
                    searchModel.Locked = false;
                }
                else if (statusType == 3)
                {
                    searchModel.Locked = true;
                }


                List<CardServiceModel> list = _cardServiceManager.GetMonthCardPage(searchModel);
                response.Result = list;
                response.TotalCount = searchModel.TotalCount;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_FAIL;
                response.MessageContent = "请检查参数格式是否符合";
            }
            return Ok(response);
        }

        /// <summary>
        /// 获取储值卡列表
        /// </summary>
        /// <param name="requestModel">请求参数实体</param>
        /// <returns></returns>
        [HttpPost]
        [ResponseType(typeof(ResponseBasePaper<CardServiceModel>))]
        public IHttpActionResult GetValueCardList(ValueCardSearchRequest requestModel)
        {
            ResponseBasePaper<CardServiceModel> response = new ResponseBasePaper<CardServiceModel>()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString(),
                Result = new List<CardServiceModel>()
            };
            if (requestModel == null || string.IsNullOrWhiteSpace(requestModel.ProjectGuid) ||
                string.IsNullOrWhiteSpace(requestModel.ParkingCode) ||
                requestModel.PageIndex <= 0 || requestModel.PageSize <= 0)
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "必要参数格式不对,请检查";
                return Ok(response);
            }

            try
            {
                CardServiceSearchModel searchModel = new CardServiceSearchModel();
                searchModel.PageIndex = requestModel.PageIndex;
                searchModel.PageSize = requestModel.PageSize;
                searchModel.ProjectGuid = requestModel.ProjectGuid;
                searchModel.ParkingCode = requestModel.ParkingCode;

                searchModel.CarNo = requestModel.CarNo; ;
                searchModel.CarOwnerName = requestModel.CarOwnerName;
                searchModel.Mobile = requestModel.Mobile;
                searchModel.StartDate = requestModel.ApplyDate;
                searchModel.CarTypeGuid = requestModel.CarTypeGuid;
                int statusType = requestModel.StatusType;
                if (statusType == 1)
                {
                    searchModel.Enable = true;
                    searchModel.Locked = false;
                }
                else if (statusType == 3)
                {
                    searchModel.Locked = true;
                }


                List<CardServiceModel> list = _cardServiceManager.GetValueCardPage(searchModel);
                response.Result = list;
                response.TotalCount = searchModel.TotalCount;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_FAIL;
                response.MessageContent = "请检查参数格式是否符合";
            }
            return Ok(response);
        }


        /// <summary>
        /// 获取临时卡列表
        /// </summary>
        /// <param name="requestModel">请求参数实体</param>
        /// <returns></returns>
        [HttpPost]
        [ResponseType(typeof(ResponseBasePaper<CardServiceModel>))]
        public IHttpActionResult GetTempCardList(TempCardSearchRequest requestModel)
        {            
            ResponseBasePaper<CardServiceModel> response = new ResponseBasePaper<CardServiceModel>()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString(),
                Result = new List<CardServiceModel>()
            };
            if (requestModel == null || string.IsNullOrWhiteSpace(requestModel.ProjectGuid) ||
                string.IsNullOrWhiteSpace(requestModel.ParkingCode) ||
                requestModel.PageIndex <= 0 || requestModel.PageSize <= 0)
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "必要参数格式不对,请检查";
                return Ok(response);
            }

            try
            {
                CardServiceSearchModel searchModel = new CardServiceSearchModel();
                searchModel.PageIndex = requestModel.PageIndex;
                searchModel.PageSize = requestModel.PageSize;
                searchModel.ProjectGuid = requestModel.ProjectGuid;
                searchModel.ParkingCode = requestModel.ParkingCode;

                searchModel.CarNo = requestModel.CarNo;;
                searchModel.CarOwnerName = requestModel.CarOwnerName;
                searchModel.Mobile = requestModel.Mobile;
                searchModel.StartDate = requestModel.ApplyDate;
                searchModel.CarTypeGuid = requestModel.CarTypeGuid;
                int statusType = requestModel.StatusType;
                if (statusType == 1)
                {
                    searchModel.Enable = true;
                    searchModel.Locked = false;
                }
                else if (statusType == 3)
                {
                    searchModel.Locked = true;
                }


                List<CardServiceModel> list = _cardServiceManager.GetTempCardPage(searchModel);
                response.Result = list;
                response.TotalCount = searchModel.TotalCount;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_FAIL;
                response.MessageContent = "请检查参数格式是否符合";
            }
            return Ok(response);
        }
    }
}
