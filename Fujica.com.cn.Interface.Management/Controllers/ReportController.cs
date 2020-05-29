using Fujica.com.cn.Business.ParkLot; 
using Fujica.com.cn.Context.Model; 
using Fujica.com.cn.Interface.Management.Models.OutPut;
using Fujica.com.cn.Logger;
using Fujica.com.cn.Security.AdmissionControl;
using Fujica.com.cn.Tools;
using FujicaService.Module.Entitys.Parking;
using FujicaService.Module.Entitys.ReportForms;
using System;
using System.Collections.Generic; 
using System.Web.Http;
using System.Web.Http.Description;

namespace Fujica.com.cn.Interface.Management.Controllers
{
    /// <summary>
    /// 报表管理
    /// </summary>
    public class ReportController : BaseController
    {
        /// <summary>
        /// 报表管理
        /// </summary>
        private ReportManager _reportManager = null;  

        /// <summary>
        /// 唯一构造函数
        /// </summary>
        /// <param name="logger">日志接口器</param>
        /// <param name="serializer">序列化接口器</param>
        /// <param name="apiaccesscontrol">api接入控制器</param>
        /// <param name="reportManager">报表管理</param> 
        public ReportController(ILogger logger, ISerializer serializer, APIAccessControl apiaccesscontrol,
            ReportManager reportManager) : 
            base(logger, serializer, apiaccesscontrol)
        {
            _reportManager = reportManager; 
        }  

        /// <summary>
        /// 查询停车记录(报表)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ResponseType(typeof(GetRecordListResponse<ParkingRecordResponse>))]
        public IHttpActionResult SearchParkingRecord(ParkingSearchRequest model)
        {
            GetRecordListResponse<ParkingRecordResponse> response = new GetRecordListResponse<ParkingRecordResponse>()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };

            if (string.IsNullOrWhiteSpace(model.ParkingCode))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "车场编码不能为空,请检查";
                return Ok(response);
            }
            GetRecordListResponse<ParkingRecordResponse> opengatereasonlistmodel = _reportManager.SearchParkingRecord(model);

            if (opengatereasonlistmodel != null)
            {
                response = opengatereasonlistmodel;
            }
            else
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiPersonnelErrorCode.API_DATA_NULL_ERROR;
                response.MessageContent = ApiPersonnelErrorCode.API_DATA_NULL_ERROR.ToString();
            }
            return Ok(response);
        }


        /// <summary>
        /// 查询缴费记录(报表)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ResponseType(typeof(GetRecordListResponse<PaymentRecordResponse>))]
        public IHttpActionResult SearchPaymentRecord(PaymentSearchRequest model)
        {
            GetRecordListResponse<PaymentRecordResponse> response = new GetRecordListResponse<PaymentRecordResponse>()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };

            if (string.IsNullOrWhiteSpace(model.ParkingCode))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "车场编码不能为空,请检查";
                return Ok(response);
            }
            GetRecordListResponse<PaymentRecordResponse> opengatereasonlistmodel = _reportManager.SearchPaymentRecord(model);

            if (opengatereasonlistmodel != null)
            {
                response = opengatereasonlistmodel;
            }
            else
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiPersonnelErrorCode.API_DATA_NULL_ERROR;
                response.MessageContent = ApiPersonnelErrorCode.API_DATA_NULL_ERROR.ToString();
            }
            return Ok(response);
        }

        /// <summary>
        /// 查询充值记录(报表)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ResponseType(typeof(GetRecordListResponse<RechargeRecordResponse>))]
        public IHttpActionResult SearchRechargeRecord(RechargeSearchRequest model)
        {
            GetRecordListResponse<RechargeRecordResponse> response = new GetRecordListResponse<RechargeRecordResponse>()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };

            if (string.IsNullOrWhiteSpace(model.ParkingCode))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "车场编码不能为空,请检查";
                return Ok(response);
            }
            GetRecordListResponse<RechargeRecordResponse> opengatereasonlistmodel = _reportManager.SearchRechargeRecord(model);

            if (opengatereasonlistmodel != null)
            {
                response = opengatereasonlistmodel;
            }
            else
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiPersonnelErrorCode.API_DATA_NULL_ERROR;
                response.MessageContent = ApiPersonnelErrorCode.API_DATA_NULL_ERROR.ToString();
            }
            return Ok(response);
        }


        /// <summary>
        /// 查询储值卡扣费记录(报表)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ResponseType(typeof(GetRecordListResponse<ConsumeRecordResponse>))]
        public IHttpActionResult SearchConsumeRecord(ConsumeSearchRequest model)
        {
            GetRecordListResponse<ConsumeRecordResponse> response = new GetRecordListResponse<ConsumeRecordResponse>()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };

            if (string.IsNullOrWhiteSpace(model.ParkingCode))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "车场编码不能为空,请检查";
                return Ok(response);
            }
            GetRecordListResponse<ConsumeRecordResponse> opengatereasonlistmodel = _reportManager.SearchConsumeRecord(model);

            if (opengatereasonlistmodel != null)
            {
                response = opengatereasonlistmodel;
            }
            else
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiPersonnelErrorCode.API_DATA_NULL_ERROR;
                response.MessageContent = ApiPersonnelErrorCode.API_DATA_NULL_ERROR.ToString();
            }
            return Ok(response);
        }

        /// <summary>
        /// 查询异常开闸信息(报表)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ResponseType(typeof(GetRecordListResponse<OpenGateRecordResponse>))]
        public IHttpActionResult SearchOpenGateRecord(OpenGateSearchRequest model)
        {
            GetRecordListResponse<OpenGateRecordResponse> response = new GetRecordListResponse<OpenGateRecordResponse>()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };

            if (string.IsNullOrWhiteSpace(model.ParkingCode))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "车场编码不能为空,请检查";
                return Ok(response);
            }
            GetRecordListResponse<OpenGateRecordResponse> opengatereasonlistmodel = _reportManager.GetOpenGateRecordList(model);

            if (opengatereasonlistmodel != null)
            {
                response = opengatereasonlistmodel;
            }
            else
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiPersonnelErrorCode.API_DATA_NULL_ERROR;
                response.MessageContent = ApiPersonnelErrorCode.API_DATA_NULL_ERROR.ToString();
            }
            return Ok(response);
        }


        /// <summary>
        ///  查询车辆在场记录信息(报表)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ResponseType(typeof(GetRecordListResponse<PresentRecordResponse>))]
        public IHttpActionResult SearchPresentRecord(PresentSearchRequest model)
        {
            GetRecordListResponse<PresentRecordResponse> response = new GetRecordListResponse<PresentRecordResponse>()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };

            if (string.IsNullOrWhiteSpace(model.ParkingCode))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "车场编码不能为空,请检查";
                return Ok(response);
            }
            GetRecordListResponse<PresentRecordResponse> opengatereasonlistmodel = _reportManager.SearchPresentRecord(model);

            if (opengatereasonlistmodel != null)
            {
                response = opengatereasonlistmodel;
            }
            else
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiPersonnelErrorCode.API_DATA_NULL_ERROR;
                response.MessageContent = ApiPersonnelErrorCode.API_DATA_NULL_ERROR.ToString();
            }
            return Ok(response);
        }


        /// <summary>
        /// 补录报表数据(报表)
        /// </summary>
        /// <param name="requestModel">请求参数实体</param>
        /// <returns></returns>
        [HttpPost]
        [ResponseType(typeof(ResponseBasePaper<AddRecordModel>))]
        public IHttpActionResult SearchRecordInRecord(RecordInRequest requestModel)
        { 
            ResponseBasePaper<AddRecordModel> response = new ResponseBasePaper<AddRecordModel>()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString(),
                Result = new List<AddRecordModel>()
            };
            if (string.IsNullOrWhiteSpace(requestModel.ProjectGuid) ||
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
                RecordInSearch searchModel = new RecordInSearch();
                searchModel.PageIndex = requestModel.PageIndex;
                searchModel.PageSize = requestModel.PageSize;
                searchModel.ProjectGuid = requestModel.ProjectGuid;
                searchModel.ParkingCode = requestModel.ParkingCode; 
                searchModel.CarTypeGuid = requestModel.CarTypeGuid;
                searchModel.CarNo = requestModel.CarNo; 
                searchModel.Operator = requestModel.Operator;
                searchModel.StrTime = requestModel.StrTime;
                searchModel.EndTime = requestModel.EndTime;

                List<AddRecordModel> list = _reportManager.AllAddRecord(searchModel);
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
        ///  车牌修正报表数据(报表)
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ResponseType(typeof(ResponseBasePaper<CorrectCarnoModel>))]
        public IHttpActionResult SearchCorrectCarnoRecord(CorrectCarnoRequest requestModel)
        {
            ResponseBasePaper<CorrectCarnoModel> response = new ResponseBasePaper<CorrectCarnoModel>()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString(),
                 Result = new List<CorrectCarnoModel>()
            };
            if (string.IsNullOrWhiteSpace(requestModel.ProjectGuid) ||
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
                CorrectCarnoSearch searchModel = new CorrectCarnoSearch();
                searchModel.PageIndex = requestModel.PageIndex;
                searchModel.PageSize = requestModel.PageSize;
                searchModel.ProjectGuid = requestModel.ProjectGuid;
                searchModel.ParkingCode = requestModel.ParkingCode; 
                searchModel.OperationType = requestModel.OperationType;
                searchModel.OldCarno = requestModel.OldCarno;
                searchModel.NewCarno = requestModel.NewCarno;
                searchModel.Operator = requestModel.Operator;
                searchModel.StrTime = requestModel.StrTime;
                searchModel.EndTime = requestModel.EndTime; 
                 
                List<CorrectCarnoModel> list = _reportManager.AllCorrectCarno(searchModel);
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
