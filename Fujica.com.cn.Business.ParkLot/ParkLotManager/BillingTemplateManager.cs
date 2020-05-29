/***************************************************************************************
 * *
 * *        File Name        : ParkLotManager.cs
 * *        Creator          : llp
 * *        Create Time      : 2019-09-21 
 * *        Remark           : 计费模板管理 逻辑类
 * *
 * *  Copyright (c) 深圳市富士智能系统有限公司.  All rights reserved. 
 * ***************************************************************************************/
using Fujica.com.cn.Business.Base;
using Fujica.com.cn.Context.Model;
using Fujica.com.cn.Tools;
using Fujica.com.cn.Business.Toll;
using System.Collections.Generic;

namespace Fujica.com.cn.Business.ParkLot
{ 
    /// <summary>
    /// 计费模板管理.
    /// </summary> 
    /// <remarks>
    /// 2019.09.21: 日志参数完善. llp <br/> 
    /// </remarks>   
    partial class ParkLotManager
    {

        /// <summary>
        /// 添加计费模板
        /// </summary>
        /// <returns></returns>
        public bool AddNewBillingTemplate(BillingTemplateModel model)
        {
            //当前车类是否存在
            CarTypeModel carTypeModel = _iCarTypeContext.GetCarType(model.CarTypeGuid);
            if (carTypeModel == null || (carTypeModel.CarType != CarTypeEnum.TempCar && carTypeModel.CarType != CarTypeEnum.ValueCar))
            {
                LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_NOTEXISTS_CARTYPE.GetDesc();
                return false;
            }

            
            bool flag = _iBillingTemplateContext.AddBillingTemplate(model);
            if (!flag)
            {
                LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_SAVE_BILLING.GetDesc();
                return false; 
            }
            
            //将数据进行格式化，再同步到主平台
            bool toFujicaResult = AddTemplateDataToFujica(model, carTypeModel.CarType);
            if (!toFujicaResult)
                LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_SAVE_BILLING_TOFUJICA.GetDesc();
            return toFujicaResult;
        }

        /// <summary>
        /// 修改计费模板
        /// </summary>
        /// <returns></returns>
        public bool ModifyBillingTemplate(BillingTemplateModel model)
        {
            //当前车类是否存在
            CarTypeModel carTypeModel = _iCarTypeContext.GetCarType(model.CarTypeGuid);
            if (carTypeModel == null || (carTypeModel.CarType != CarTypeEnum.TempCar && carTypeModel.CarType != CarTypeEnum.ValueCar))
            {
                LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_NOTEXISTS_CARTYPE.GetDesc();
                return false;
            }

            bool flag = _iBillingTemplateContext.ModifyBillingTemplate(model);
            if (!flag)
            {
                LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_SAVE_BILLING.GetDesc();
                return false;
            }

            //下发缴费模板给相机(车类对应缴费模板)
            SendTempCarTypeOfPlateColor(model.ProjectGuid, model.ParkCode);

            //将数据进行格式化，再同步到主平台
            bool toFujicaResult = EditTemplateDataToFujica(model, carTypeModel.CarType);
            if (!toFujicaResult)
                LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_SAVE_BILLING_TOFUJICA.GetDesc();
            return toFujicaResult;

        }

        /// <summary>
        /// 删除计费模板
        /// </summary>
        /// <returns></returns>
        public bool DeleteBillingTemplate(BillingTemplateModel model)
        {
            bool flag = _iBillingTemplateContext.DeleteBillingTemplate(model);
            if (!flag)
            {
                LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_DELETE_BILLING.GetDesc();
                return false;
            }

            //将删除数据进行格式化，再同步到主平台
            bool toFujicaResult = DeleteTemplateDataToFujica(model.CarTypeGuid);
            if (!toFujicaResult)
                LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_SAVE_BILLING_TOFUJICA.GetDesc();
            return toFujicaResult;
        }

        /// <summary>
        /// 获取计费模板
        /// </summary>
        /// <param name="carTypeGuid"></param>
        /// <returns></returns>
        public BillingTemplateModel GetBillingTemplate(string carTypeGuid)
        {
            return _iBillingTemplateContext.GetBillingTemplate(carTypeGuid);
        }

        /// <summary>
        /// 获取计费模板
        /// 具体计费方式的基础模型（包含免费时长、超时出场时长）
        /// </summary>
        /// <param name="carTypeGuid"></param>
        /// <returns></returns>
        public BillingTemplateChargeModel GetBillingTemplateChargeModel(string carTypeGuid)
        {
            BillingTemplateModel model = GetBillingTemplate(carTypeGuid);
            if (model == null) return null;

            BillingTemplateChargeModel chargeModel = new BillingTemplateChargeModel();
            chargeModel.CarTypeGuid = carTypeGuid;

            switch (model.ChargeMode)
            {
                case 1:
                    HourlyTollModel hourlyModel = m_serializer.Deserialize<HourlyTollModel>(model.TemplateJson);
                    chargeModel.FreeMinutes1 = hourlyModel.FreeMinutes;
                    chargeModel.LeaveTimeout1 = hourlyModel.LeaveTimeout;
                    chargeModel.BeginTime1 = "00:00";
                    chargeModel.EndTime1 = "00:00";
                    break;
                case 2:
                    SegmentTollModel segmentModel = m_serializer.Deserialize<SegmentTollModel>(model.TemplateJson);
                    chargeModel.FreeMinutes1 = segmentModel.FreeMinute1;
                    chargeModel.FreeMinutes2 = segmentModel.FreeMinute2;
                    chargeModel.FreeMinutes3 = segmentModel.FreeMinute3;
                    chargeModel.LeaveTimeout1 = segmentModel.LeaveTimeout;
                    chargeModel.LeaveTimeout2 = segmentModel.LeaveTimeout;
                    chargeModel.LeaveTimeout3 = segmentModel.LeaveTimeout;
                    chargeModel.BeginTime1 = segmentModel.BeginTime1.ToShortTimeString();
                    chargeModel.BeginTime2 = segmentModel.BeginTime2.ToShortTimeString();
                    chargeModel.BeginTime3 = segmentModel.BeginTime3.ToShortTimeString();
                    chargeModel.EndTime1 = segmentModel.EndTime1.ToShortTimeString();
                    chargeModel.EndTime2 = segmentModel.EndTime2.ToShortTimeString();
                    chargeModel.EndTime3 = segmentModel.EndTime3.ToShortTimeString();
                    break;
                case 3:
                    ShenZhengTollModel shezhenModel= m_serializer.Deserialize<ShenZhengTollModel>(model.TemplateJson);
                    chargeModel.FreeMinutes1 = shezhenModel.FreeMinutes;
                    chargeModel.LeaveTimeout1 = shezhenModel.LeaveTimeout;
                    chargeModel.BeginTime1 = "00:00";
                    chargeModel.EndTime1 = "00:00";
                    break;
                case 4:
                    HalfHourTollModel halfHourModel = m_serializer.Deserialize<HalfHourTollModel>(model.TemplateJson);
                    chargeModel.FreeMinutes1 = halfHourModel.DayFreeMinutes;
                    chargeModel.LeaveTimeout1 = halfHourModel.LeaveTimeout;
                    chargeModel.BeginTime1 = halfHourModel.DayBeginTime.ToShortTimeString();
                    chargeModel.EndTime1 = halfHourModel.DayEndTime.ToShortTimeString();
                    chargeModel.FreeMinutes2 = halfHourModel.NightFreeMinutes;
                    chargeModel.LeaveTimeout2 = halfHourModel.LeaveTimeout;
                    chargeModel.BeginTime2 = halfHourModel.DayEndTime.ToShortTimeString();
                    chargeModel.EndTime2 = halfHourModel.DayBeginTime.ToShortTimeString();
                    break;
                case 5:
                    SimpleSegmentTollModel simpleSegmentModel = m_serializer.Deserialize<SimpleSegmentTollModel>(model.TemplateJson);
                    chargeModel.FreeMinutes1 = simpleSegmentModel.FreeMinutes;
                    chargeModel.LeaveTimeout1 = simpleSegmentModel.LeaveTimeout;
                    chargeModel.BeginTime1 = "00:00";
                    chargeModel.EndTime1 = "00:00";
                    break;
                case 6:
                    SegmentHourlyTollModel sementHourlyModel = m_serializer.Deserialize<SegmentHourlyTollModel>(model.TemplateJson);
                    chargeModel.FreeMinutes1 = sementHourlyModel.FreeMinutes;
                    chargeModel.LeaveTimeout1 = sementHourlyModel.LeaveTimeout;
                    chargeModel.BeginTime1 = "00:00";
                    chargeModel.EndTime1 = "00:00";
                    break;
                case 7:
                    SegmentNoneHalfHourTollModel segmentNoneHalfHourModel = m_serializer.Deserialize<SegmentNoneHalfHourTollModel>(model.TemplateJson);
                    chargeModel.FreeMinutes1 = segmentNoneHalfHourModel.FreeMinutes;
                    chargeModel.LeaveTimeout1 = segmentNoneHalfHourModel.LeaveTimeout;
                    chargeModel.BeginTime1 = "00:00";
                    chargeModel.EndTime1 = "00:00";
                    break;
                case 8:
                    SegmentHalfHourTollModel segmentHalfHourModel = m_serializer.Deserialize<SegmentHalfHourTollModel>(model.TemplateJson);
                    chargeModel.FreeMinutes1 = segmentHalfHourModel.FreeMinutes;
                    chargeModel.LeaveTimeout1 = segmentHalfHourModel.LeaveTimeout;
                    chargeModel.BeginTime1 = "00:00";
                    chargeModel.EndTime1 = "00:00";
                    break;
                case 9:
                    NewSegmentTollModel newSegmentModel = m_serializer.Deserialize<NewSegmentTollModel>(model.TemplateJson);
                    chargeModel.FreeMinutes1 = newSegmentModel.FreeMinutes;
                    chargeModel.LeaveTimeout1 = newSegmentModel.LeaveTimeout;
                    chargeModel.BeginTime1 = "00:00";
                    chargeModel.EndTime1 = "00:00";
                    break;
                case 10:
                    SegmentQuarterHourTollModel segmentQuarterHourModel = m_serializer.Deserialize<SegmentQuarterHourTollModel>(model.TemplateJson);
                    chargeModel.FreeMinutes1 = segmentQuarterHourModel.DayFreeMinutes;
                    chargeModel.LeaveTimeout1 = segmentQuarterHourModel.LeaveTimeout;
                    chargeModel.BeginTime1 = segmentQuarterHourModel.DayBeginTime.ToShortTimeString();
                    chargeModel.EndTime1 = segmentQuarterHourModel.DayEndTime.ToShortTimeString();
                    chargeModel.FreeMinutes2 = segmentQuarterHourModel.NightFreeMinutes;
                    chargeModel.LeaveTimeout2 = segmentQuarterHourModel.LeaveTimeout;
                    chargeModel.BeginTime2 = segmentQuarterHourModel.DayEndTime.ToShortTimeString();
                    chargeModel.EndTime2 = segmentQuarterHourModel.DayBeginTime.ToShortTimeString();
                    break;
            }
            return chargeModel;
        }

        #region 内部业务私有方法

        /// <summary>
        /// 模板数据进行格式化
        /// </summary>
        /// <param name="chargeMode">计费方式</param>
        /// <returns></returns>
        private ITollCalculator TemplateDataFormat(BillingTemplateModel model)
        {
            ITollCalculator tollModel = null;
            switch (model.ChargeMode)
            {
                case 1:
                    tollModel = new TollCalculator_Hourly(m_logger, m_serializer);                    
                    break;
                case 2:
                    tollModel = new TollCalculator_Segment(m_logger, m_serializer);
                    break;
                case 3:
                    tollModel = new TollCalculator_ShenZheng(m_logger, m_serializer);
                    break;
                case 4:
                    tollModel = new TollCalculator_HalfHourly(m_logger, m_serializer);
                    break;
                case 5:
                    tollModel = new TollCalculator_SimpleSegment(m_logger, m_serializer);
                    break;
                case 6:
                    tollModel = new TollCalculator_SegmentHourly(m_logger, m_serializer);
                    break;
                case 7:
                    tollModel = new TollCalculator_SegmentNone(m_logger, m_serializer);
                    break;
                case 8:
                    tollModel = new TollCalculator_SegmentHalfHour(m_logger, m_serializer);
                    break;
                case 9:
                    tollModel = new TollCalculator_NewSegment(m_logger, m_serializer);
                    break;
                case 10:
                    tollModel = new TollCalculator_SegmentQuarterHour(m_logger, m_serializer);
                    break;
            }
            return tollModel;
        }

        /// <summary>
        /// 模板数据格式化字符串
        /// </summary>
        /// <param name="model">计费模板模型</param>
        /// <param name="carType">0：时租车  2：储值车</param>
        /// <returns></returns>
        private string TemplateDataFormatStr(BillingTemplateModel model, CarTypeEnum carType)
        {
            string templateStr = string.Empty;
            ITollCalculator tollModel = TemplateDataFormat(model);
            if (tollModel != null)
                templateStr = tollModel.GetTollFeesTemplateStr(model);

            //模板字符串前缀 说明：T0100015  T：临时卡 R：储值卡   01：第一套算费模板   00015：出场免费时间（单位分钟）
            string prefix = "";
            if (carType == CarTypeEnum.TempCar)
                prefix = "T";
            else if (carType == CarTypeEnum.ValueCar)
                prefix = "R";
            else
                return "";
            string templateIndex = model.ChargeMode.ToString().PadLeft(2, '0');
            string timeOut = tollModel.GetLeaveTimeOut(model).ToString().PadLeft(5, '0');
            prefix = prefix + templateIndex + timeOut;

            //示例 T0100015300102030405060708090A0A0A0A0A0A0A0A0A0A0A0A0A0A0AFFFFFF00000000
            templateStr = prefix + templateStr;
            return templateStr;
        }

        /// <summary>
        /// 模板数据同步到主平台Fujica
        /// </summary>
        /// <param name="carTypeGuid">车类唯一编码</param>
        /// <param name="actionType">新增:1 修改:2 删除:3</param>
        /// <param name="templateStr">格式化模板</param>
        /// <param name="carType">0：时租车  2：储值车</param>
        /// <returns></returns>
        private bool TemplateDataToFujica(string carTypeGuid, int actionType, string templateStr, CarTypeEnum carType)
        {
            CarTypeModel carTypeModel = _iCarTypeContext.GetCarType(carTypeGuid);
            RequestFujicaStandard requestFujica = new RequestFujicaStandard();
            //请求方法
            string servername = "/CalculationCost/";
            switch (actionType)
            {
                case 1://新增api
                case 2:
                    
                    if (carType == CarTypeEnum.TempCar)
                        //添加临时车卡模板
                        servername += "ApiAddTemporaryCardTemplate";
                    else if(carType == CarTypeEnum.ValueCar)
                        //添加储值卡模板
                        servername += "ApiAddValueCardTemplate";
                    break;
                case 3://删除api
                    servername += "ApiDeleteTemplate";
                    break;
            }

            //请求参数
            Dictionary<string, object> dicParam = new Dictionary<string, object>();
            dicParam["ParkingCode"] = carTypeModel.ParkCode;
            dicParam["CarType"] = carTypeModel.Idx;
            if (actionType != 3)
            {//删除api不需要这2个字段
                dicParam["CarName"] = carTypeModel.CarTypeName;
                dicParam["Template"] = templateStr;
            }
            else
            {
                dicParam["SmallParkingCode"] = "";
            }
            //返回fujica api计费模板添加、修改、删除请求结果
            return requestFujica.RequestInterfaceV2(servername, dicParam);

        }

        /// <summary>
        /// 新增-模板数据同步到主平台Fujica
        /// </summary>
        /// <param name="model">计费模板模型</param>
        /// <param name="carType">0：时租车  2：储值车</param>
        /// <returns></returns>
        private bool AddTemplateDataToFujica(BillingTemplateModel model, CarTypeEnum carType)
        {
            //1、根据不同的计费方式将数据进行格式化
            string templateStr = TemplateDataFormatStr(model, carType);

            //2、格式化后数据同步到主平台
            if (!string.IsNullOrEmpty(templateStr))
            {
                return TemplateDataToFujica(model.CarTypeGuid, 1, templateStr, carType);
            }
            return false;
        }

        /// <summary>
        /// 修改-模板数据同步到主平台Fujica
        /// </summary>
        /// <param name="model">计费模板模型</param>
        /// <param name="carType">0：时租车  2：储值车</param>
        /// <returns></returns>
        private bool EditTemplateDataToFujica(BillingTemplateModel model, CarTypeEnum carType)
        {
            //1、根据不同的计费方式将数据进行格式化
            string templateStr = TemplateDataFormatStr(model, carType);

            //2、格式化后数据同步到主平台
            if (!string.IsNullOrEmpty(templateStr))
            {
                return TemplateDataToFujica(model.CarTypeGuid, 2, templateStr, carType);
            }
            return false;
        }

        /// <summary>
        /// 删除-模板数据同步到主平台Fujica
        /// </summary>
        /// <param name="carTypeGuid">车类唯一编码</param>
        /// <returns></returns>
        private bool DeleteTemplateDataToFujica(string carTypeGuid)
        {
            return TemplateDataToFujica(carTypeGuid, 3, "", 0);
        }


        #endregion
    }
}
