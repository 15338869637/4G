/***************************************************************************************
 * *
 * *        File Name        : ReportManager.cs
 * *        Creator          : llp
 * *        Create Time      : 2019-09-17 
 * *        Remark           : 报表管理 逻辑类 
 * *
 * *  Copyright (c) 深圳市富士智能系统有限公司.  All rights reserved. 
 * ***************************************************************************************/
using Fujica.com.cn.Business.Base;
using Fujica.com.cn.Context.IContext;
using Fujica.com.cn.Context.Model;
using Fujica.com.cn.Logger;
using Fujica.com.cn.Tools;
using FujicaService.Module.Entitys.Parking;
using FujicaService.Module.Entitys.ReportForms;
using System;
using System.Collections.Generic;

namespace Fujica.com.cn.Business.ParkLot
{
    /// <summary>
    /// 报表管理.
    /// </summary>
    /// <remarks>
    /// 2019.09.21: 日志参数完善. llp <br/> 
    /// </remarks>  
    public class ReportManager : IBaseBusiness
    {
        public string LastErrorDescribe
        {
            get;
            set;
        }
        /// <summary>
        /// 日志记录器
        /// </summary>
        internal readonly ILogger m_logger;
        /// <summary>
        /// 序列化器
        /// </summary>
        internal readonly ISerializer m_serializer = null;
        /// <summary>
        ///报表管理操作上下文
        /// </summary>
        private IReportContext _iReportContext = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="serializer"></param>
        /// <param name="_ReportContext"></param>
        public ReportManager(ILogger logger, ISerializer serializer,
             IReportContext iReportContext)
        {
            m_logger = logger;
            m_serializer = serializer;
            _iReportContext = iReportContext;
        }


        /// <summary>
        ///   查询停车记录(报表)
        /// </summary>
        /// <param name="parkCode"></param>
        /// <returns></returns>
        public GetRecordListResponse<ParkingRecordResponse> SearchParkingRecord(ParkingSearchRequest model)
        {
            GetRecordListResponse<ParkingRecordResponse> responseModel = null;
            RequestFujicaReportStatistical requestFujica = new RequestFujicaReportStatistical();
            //请求方法
            string servername = "SearchRecord/SearchParkingRecord";
            //请求参数
            Dictionary<string, object> dicParam = new Dictionary<string, object>();
            dicParam["Id"] = model.Id;
            dicParam["ParkingCode"] = model.ParkingCode;
            if (model.LicensePlate != null) dicParam["LicensePlate"] = model.LicensePlate;
            if (model.Entrance != null) dicParam["Entrance"] = model.Entrance;
            if (model.Export != null) dicParam["Export"] = model.Export;
            if (model.CarType!= null) dicParam["CarType"] = model.CarType;
            if (model.CardType != null) dicParam["CardType"] = model.CardType;
            if (model.InThroughType != null) dicParam["InThroughType"] = model.InThroughType;
            if (model.OutThroughType != null) dicParam["OutThroughType"] = model.OutThroughType;
            if (model.OutParkingOperator != null) dicParam["OutParkingOperator"] = model.OutParkingOperator;
            if (model.SpecialCar != null) dicParam["SpecialCar"] = model.SpecialCar;
            if (model.AdmissionStartDate != null) dicParam["AdmissionStartDate"] = model.AdmissionStartDate;
            if (model.AppearanceStartDate != null) dicParam["AppearanceStartDate"] = model.AppearanceStartDate;
            if (model.AdmissionEndDate != null) dicParam["AdmissionEndDate"] = model.AdmissionEndDate;
            if (model.AppearanceEndDate != null) dicParam["AppearanceEndDate"] = model.AppearanceEndDate;
            if (model.NextToken != null) dicParam["NextToken"] = model.NextToken; //就是第一次查询,需要制定当前页  页大小 ,需要下一页的时候,直接给这个nexttoken就可以了
            dicParam["PageIndex"] = model.PageIndex;
            dicParam["PageSize"] = model.PageSize;
            dicParam["Sort"] = model.Sort;
            dicParam["FieldSort"] = model.FieldSort;
            bool falg = requestFujica.RequestFujicaReport(servername, dicParam);
            if (falg)
            {
                string fujicaResult = requestFujica.FujicaResult;
                if (!string.IsNullOrEmpty(fujicaResult))
                {
                    //解析返回参数json字符串 
                    GetRecordListResponse<ParkingRecordResponse> response = m_serializer.Deserialize<GetRecordListResponse<ParkingRecordResponse>>(fujicaResult);
                    responseModel = response;
                }
            } 
            return responseModel;
        }
        /// <summary>
        /// 查询储值卡扣费记录(报表)
        /// </summary>
        /// <param name="parkCode"></param>
        /// <returns></returns>
        public GetRecordListResponse<ConsumeRecordResponse> SearchConsumeRecord(ConsumeSearchRequest model)
        {
            GetRecordListResponse<ConsumeRecordResponse> responseModel = null;
            RequestFujicaReportStatistical requestFujica = new RequestFujicaReportStatistical();
            //请求方法
            string servername = "SearchRecord/SearchConsumeRecord";
            //请求参数
            Dictionary<string, object> dicParam = new Dictionary<string, object>();
            dicParam["Id"] = model.Id;
            dicParam["ParkingRecordCode"] = model.ParkingRecordCode;
            dicParam["ParkingCode"] = model.ParkingCode;
            dicParam["LicensePlate"] = model.LicensePlate;
            dicParam["CarType"] = model.CarType;
            dicParam["DealNo"] = model.DealNo;
            dicParam["ConsumeOperator"] = model.ConsumeOperator;
            dicParam["BillingStartTime"] = model.BillingStartTime;
            dicParam["BillingDeadline"] = model.BillingDeadline;
            dicParam["AdmissionStartDate"] = model.AdmissionStartDate;
            dicParam["AdmissionEndDate"] = model.AdmissionEndDate;
            dicParam["NextToken"] = model.NextToken; //就是第一次查询,需要制定当前页  页大小 ,需要下一页的时候,直接给这个nexttoken就可以了
            dicParam["PageIndex"] = model.PageIndex;
            dicParam["PageSize"] = model.PageSize;
            dicParam["Sort"] = model.Sort;
            dicParam["FieldSort"] = model.FieldSort;
            bool falg = requestFujica.RequestFujicaReport(servername, dicParam);
            if (falg)
            {
                string fujicaResult = requestFujica.FujicaResult;
                if (!string.IsNullOrEmpty(fujicaResult))
                {
                    //解析返回参数json字符串 
                    GetRecordListResponse<ConsumeRecordResponse> response = m_serializer.Deserialize<GetRecordListResponse<ConsumeRecordResponse>>(fujicaResult);
                    //List<MCoupon> ls = m_serializer.Deserialize<List<MCoupon>>(dicFujicaResult["CouponList"].ToString());
                    //responseModel.CouponList = ls; 
                    responseModel = response;
                }
            }
            return responseModel;
        }

        /// <summary>
        /// 查询缴费记录(报表)
        /// </summary>
        /// <param name="parkCode"></param>
        /// <returns></returns>
        public GetRecordListResponse<PaymentRecordResponse> SearchPaymentRecord(PaymentSearchRequest model)
        {
            GetRecordListResponse<PaymentRecordResponse> responseModel = null;
            RequestFujicaReportStatistical requestFujica = new RequestFujicaReportStatistical();
            //请求方法
            string servername = "SearchRecord/SearchPaymentRecord";
            //请求参数
            Dictionary<string, object> dicParam = new Dictionary<string, object>();
            dicParam["Id"] = model.Id;
            dicParam["ParkingRecordCode"] = model.ParkingRecordCode;
            dicParam["ParkingCode"] = model.ParkingCode;
            dicParam["LicensePlate"] = model.LicensePlate;
            dicParam["CarType"] = model.CarType;
            dicParam["DealNo"] = model.DealNo;
            dicParam["UserCode"] = model.UserCode;
            dicParam["OpenId"] = model.OpenId;
            dicParam["PhoneNumber"] = model.PhoneNumber;
            dicParam["Subject"] = model.Subject;
            dicParam["TollOperator"] = model.TollOperator;
            dicParam["InvoiceCode"] = model.InvoiceCode;
            dicParam["InvoiceState"] = model.InvoiceState;
            dicParam["PaymentType"] = model.PaymentType;
            dicParam["OrderType"] = model.OrderType;
            dicParam["AdmissionStartDate"] = model.AdmissionStartDate;
            dicParam["AdmissionEndDate"] = model.AdmissionEndDate;
            dicParam["BillingStartTime"] = model.BillingStartTime;
            dicParam["BillingDeadline"] = model.BillingDeadline;
            dicParam["PaymentStartDate"] = model.PaymentStartDate;
            dicParam["PaymentEndDate"] = model.PaymentEndDate;
            dicParam["InvoiceStartTime"] = model.InvoiceStartTime;
            dicParam["InvoiceEndTime"] = model.InvoiceEndTime;
            dicParam["NextToken"] = model.NextToken; //就是第一次查询,需要制定当前页  页大小 ,需要下一页的时候,直接给这个nexttoken就可以了
            dicParam["PageIndex"] = model.PageIndex;
            dicParam["PageSize"] = model.PageSize;
            dicParam["Sort"] = model.Sort;
            dicParam["FieldSort"] = model.FieldSort;
            bool falg = requestFujica.RequestFujicaReport(servername, dicParam);
            if (falg)
            {
                string fujicaResult = requestFujica.FujicaResult;
                if (!string.IsNullOrEmpty(fujicaResult))
                {
                    //解析返回参数json字符串 
                    GetRecordListResponse<PaymentRecordResponse> response = m_serializer.Deserialize<GetRecordListResponse<PaymentRecordResponse>>(fujicaResult);
                    //List<MCoupon> ls = m_serializer.Deserialize<List<MCoupon>>(dicFujicaResult["CouponList"].ToString());
                    //responseModel.CouponList = ls; 
                    responseModel = response;
                }
            }
            return responseModel;
        }

        /// <summary>
        /// 查询充值记录(报表)
        /// </summary>
        /// <param name="parkCode"></param>
        /// <returns></returns>
        public GetRecordListResponse<RechargeRecordResponse> SearchRechargeRecord(RechargeSearchRequest model)
        {
            GetRecordListResponse<RechargeRecordResponse> responseModel = null;
            RequestFujicaReportStatistical requestFujica = new RequestFujicaReportStatistical();
            //请求方法
            string servername = "SearchRecord/SearchRechargeRecord";
            //请求参数
            Dictionary<string, object> dicParam = new Dictionary<string, object>();
            dicParam["Id"] = model.Id;
            dicParam["ParkingCode"] = model.ParkingCode;
            dicParam["LicensePlate"] = model.LicensePlate;
            dicParam["TransactionCode"] = model.TransactionCode;
            dicParam["UserCode"] = model.UserCode;
            dicParam["OpenId"] = model.OpenId;
            dicParam["OwnerName"] = model.OwnerName;
            dicParam["PhoneNumber"] = model.PhoneNumber;
            dicParam["GroupParkingCode"] = model.GroupParkingCode;
            dicParam["RechargeOperator"] = model.RechargeOperator;
            dicParam["InvoiceCode"] = model.InvoiceCode;
            dicParam["RenewalType"] = model.RenewalType;
            dicParam["PaymentType"] = model.PaymentType;
            dicParam["OrderType"] = model.OrderType;
            dicParam["CardType"] = model.CardType; 
            dicParam["CarType"] = model.CarType;
            dicParam["OrderStauts"] = model.OrderStauts;
            dicParam["OperatType"] = model.OperatType;
            dicParam["InvoiceState"] = model.InvoiceState;
            dicParam["TransactionStartTime"] = model.TransactionStartTime;
            dicParam["TransactionEndTime"] = model.TransactionEndTime;
            dicParam["ExpirationStartTime"] = model.ExpirationStartTime;
            dicParam["ExpirationEndTime"] = model.ExpirationEndTime;
            dicParam["CreateStartTime"] = model.CreateStartTime;
            dicParam["CreateEndTime"] = model.CreateEndTime;
            dicParam["InvoiceStartTime"] = model.InvoiceStartTime;
            dicParam["InvoiceEndTime"] = model.InvoiceEndTime;
            dicParam["NextToken"] = model.NextToken; //就是第一次查询,需要制定当前页  页大小 ,需要下一页的时候,直接给这个nexttoken就可以了
            dicParam["PageIndex"] = model.PageIndex;
            dicParam["PageSize"] = model.PageSize;
            dicParam["Sort"] = model.Sort;
            dicParam["FieldSort"] = model.FieldSort;
            bool falg = requestFujica.RequestFujicaReport(servername, dicParam);
            if (falg)
            {
                string fujicaResult = requestFujica.FujicaResult;
                if (!string.IsNullOrEmpty(fujicaResult))
                {
                    //解析返回参数json字符串 
                    GetRecordListResponse<RechargeRecordResponse> response = m_serializer.Deserialize<GetRecordListResponse<RechargeRecordResponse>>(fujicaResult);
                    //List<MCoupon> ls = m_serializer.Deserialize<List<MCoupon>>(dicFujicaResult["CouponList"].ToString());
                    //responseModel.CouponList = ls; 
                    responseModel = response;
                }
            }
            return responseModel;
        }
        /// <summary>
        /// 读取异常开闸列表(报表)
        /// </summary>
        /// <param name="parkCode"></param>
        /// <returns></returns>
        public GetRecordListResponse<OpenGateRecordResponse> GetOpenGateRecordList(OpenGateSearchRequest model)
        {
            GetRecordListResponse<OpenGateRecordResponse> responseModel = null;
            RequestFujicaReportStatistical requestFujica = new RequestFujicaReportStatistical();
            //请求方法
            string servername = "SearchRecord/SearchOpenGateRecord";
            //请求参数
            Dictionary<string, object> dicParam = new Dictionary<string, object>();
            dicParam["ParkingCode"] = model.ParkingCode;
            dicParam["EntranceType"] = model.EntranceType;
            dicParam["OpenGateStartTime"] = model.OpenGateStartTime;
            dicParam["OpenGateEndTime"] = model.OpenGateEndTime;
            dicParam["OpenGateOperator"] = model.OpenGateOperator;
            dicParam["NextToken"] = model.NextToken; //就是第一次查询,需要制定当前页  页大小 ,需要下一页的时候,直接给这个nexttoken就可以了
            dicParam["PageIndex"] = model.PageIndex;
            dicParam["PageSize"] = model.PageSize;
            dicParam["Sort"] = model.Sort;
            dicParam["FieldSort"] = model.FieldSort;
            bool falg = requestFujica.RequestFujicaReport(servername, dicParam);
            if (falg)
            {
                string fujicaResult = requestFujica.FujicaResult;
                if (!string.IsNullOrEmpty(fujicaResult))
                {
                    //解析返回参数json字符串 
                    GetRecordListResponse<OpenGateRecordResponse> response = m_serializer.Deserialize<GetRecordListResponse<OpenGateRecordResponse>>(fujicaResult);
                     responseModel = response;
                }
            }
            return responseModel;
        }

        /// <summary>
        /// 车辆在场记录(报表)
        /// </summary>
        /// <param name="parkCode"></param>
        /// <returns></returns>
        public GetRecordListResponse<PresentRecordResponse> SearchPresentRecord(PresentSearchRequest model)
        {
            GetRecordListResponse<PresentRecordResponse> responseModel = null;
            RequestFujicaReportStatistical requestFujica = new RequestFujicaReportStatistical();
            //请求方法
            string servername = "SearchRecord/SearchPresentRecord";
            //请求参数
            Dictionary<string, object> dicParam = new Dictionary<string, object>();
            dicParam["ParkingCode"] = model.ParkingCode;
            if (model.LicensePlate != null) dicParam["LicensePlate"] = model.LicensePlate;
            if (model.SpecialCar != null) dicParam["SpecialCar"] = model.SpecialCar;
            if (model.ThroughType != 0) dicParam["ThroughType"] = model.ThroughType;
            if (model.ParkingOperator != null) dicParam["ParkingOperator"] = model.ParkingOperator;
            if (model.CarType != null) dicParam["CarType"] = model.CarType;
            if (model.CardType != null) dicParam["CardType"] = model.CardType;
            if (model.SpecialCar != null) dicParam["SpecialCar"] = model.AdmissionDateStartDate;
            if (model.AdmissionDateStartDate != null) dicParam["AdmissionDateStartDate"] = model.AdmissionDateStartDate;
            if (model.AdmissionDateEndDate != null) dicParam["AdmissionDateEndDate"] = model.AdmissionDateEndDate;
            if (model.AdmissionDateStartDate != null) dicParam["NextToken"] = model.NextToken; //就是第一次查询,需要制定当前页  页大小 ,需要下一页的时候,直接给这个nexttoken就可以了//就是第一次查询,需要制定当前页  页大小 ,需要下一页的时候,直接给这个nexttoken就可以了
            dicParam["PageIndex"] = model.PageIndex;
            dicParam["PageSize"] = model.PageSize;
            dicParam["Sort"] = model.Sort;
            dicParam["FieldSort"] = model.FieldSort;
            bool falg = requestFujica.RequestFujicaReport(servername, dicParam);
            if (falg)
            {
                string fujicaResult = requestFujica.FujicaResult;
                if (!string.IsNullOrEmpty(fujicaResult))
                {
                    //解析返回参数json字符串 
                    GetRecordListResponse<PresentRecordResponse> response = m_serializer.Deserialize<GetRecordListResponse<PresentRecordResponse>>(fujicaResult);

                    responseModel = response;
                }
            }
            return responseModel;
        }

        /// <summary>
        ///  补录数据(报表)
        /// </summary>
        /// <param name="parkcode"></param>
        /// <returns></returns>
        public List<AddRecordModel> AllAddRecord(RecordInSearch model)
        {
            //批量数据都从数据库获取 redis并不缓存此实体
            try
            {
                List<AddRecordModel> list = _iReportContext.AllAddRecord(model) as List<AddRecordModel>;
                return list;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        ///  车牌修正数据(报表)
        /// </summary>
        /// <param name="parkcode"></param>
        /// <returns></returns>
        public List<CorrectCarnoModel> AllCorrectCarno(CorrectCarnoSearch model)
        {
            //批量数据都从数据库获取 redis并不缓存此实体
            try
            {
                List<CorrectCarnoModel> list = _iReportContext.AllCorrectCarno(model) as List<CorrectCarnoModel>;
                return list;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


    }
}
