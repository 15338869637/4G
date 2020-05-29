/***************************************************************************************
 * *
 * *        File Name        : ScanningManager.cs
 * *        Creator          : llp
 * *        Create Time      : 2019-09-17 
 * *        Remark           : 扫码管理器 逻辑类 
 * *
 * *  Copyright (c) 深圳市富士智能系统有限公司.  All rights reserved. 
 * ***************************************************************************************/
using Fujica.com.cn.Business.Base;
using Fujica.com.cn.Context.Model;
using Fujica.com.cn.Logger;
using Fujica.com.cn.Tools;
using System;
using System.Collections.Generic;

namespace Fujica.com.cn.Business.ParkLot
{ 
    /// <summary>
    /// 扫码管理器.
    /// </summary>
    /// <remarks>
    /// 2019.09.21: 日志参数完善. llp <br/> 
    /// </remarks>   
    public class ScanningManager : IBaseBusiness
    {
        /// <summary>
        /// 日志记录器
        /// </summary>
        internal readonly ILogger m_logger;

        /// <summary>
        /// 序列化器
        /// </summary>
        internal readonly ISerializer m_serializer = null;

        public string LastErrorDescribe
        {
            get; set;
        }
        /// <summary>
        /// 停车场管理器
        /// </summary>
        public ParkLotManager _parkLotManager = null;

        public ScanningManager(ILogger logger, ISerializer serializer,
            ParkLotManager parkLotManager)
        {
            m_logger = logger;
            m_serializer = serializer;
            _parkLotManager = parkLotManager;
        }

        /// <summary>
        /// 主扫（用户扫描固定车道码缴费）
        /// </summary>
        public ActiveScanningResponseModel ActiveScanning(ActiveScanningModel model)
        {
            ActiveScanningResponseModel responseModel  = null;
            if (model == null || 
                string.IsNullOrEmpty(model.DeviceMACAddress) ||
                string.IsNullOrEmpty(model.Guid) ||
                string.IsNullOrEmpty(model.CarNo) ||
                model.LaneSenseDate == null)
            {
                LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_PARAM_ERROR.GetDesc();
                return responseModel;
            }
            //找到对应车道 
            DrivewayModel drivewayModel = _parkLotManager.GetDrivewayByMacAddress(model.DeviceMACAddress);
            if (drivewayModel == null)
            {
                LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_NOTEXISTS_DRIVEWAY.GetDesc();
                return responseModel;
            }

            //读取车辆的在场记录并验证停车标识是否一致
            VehicleInOutModel entermodel = _parkLotManager.GetEntryRecord(model.CarNo, drivewayModel.ParkCode);
            if (entermodel == null || entermodel.Guid != model.Guid)
            {
                LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_NOTEXISTS_CAR.GetDesc();
                return responseModel;
            }

            //请求Fujica Api 获取车道应缴费用和入场时间信息
            decimal actualAmount = 0;
            DateTime beginTime = DateTime.MinValue;
            bool fujicaResult = GetLaneCostInfoByFujica(drivewayModel.ParkCode, entermodel.CarNo, drivewayModel.DeviceMacAddress, model.LaneSenseDate, out actualAmount, out beginTime);
            if (fujicaResult)
            {
                responseModel = new ActiveScanningResponseModel();
                responseModel.CarNo = model.CarNo;
                responseModel.ParkingFee = actualAmount;
                responseModel.BeginTime = beginTime.ToString("yyyy-MM-dd HH:mm:ss");
                //93+车编+车道号
                responseModel.QRCode = "http://mops-test.fujica.com.cn/v2/Login/Index?key=FUJICA93" + drivewayModel.ParkCode + drivewayModel.DeviceMacAddress; 
                //responseModel.QRCode = "http://mops.fujica.com.cn/v2/Login/Index?key=FUJICA93" + drivewayModel.ParkCode + drivewayModel.DeviceMacAddress;//项目更新发布，这里一定要改回来
                responseModel.Remark = "";
                responseModel.Extend = "";
            }
            return responseModel;
        }

        /// <summary>
        /// 被扫（用户出示微信/支付宝缴费二维码）
        /// </summary>
        public PassiveScanningResponseModel PassiveScanning(PassiveScanningModel model)
        {
            PassiveScanningResponseModel responseModel = null;
            if (model == null ||
                string.IsNullOrEmpty(model.DeviceMACAddress) ||
                string.IsNullOrEmpty(model.Guid) ||
                string.IsNullOrEmpty(model.CarNo) ||
                model.LaneSenseDate == null)
            {
                LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_PARAM_ERROR.GetDesc();
                return responseModel;
            }
            responseModel = new PassiveScanningResponseModel()
            {
                CarNo = model.CarNo,
            };
            //找到对应车道 
            DrivewayModel drivewayModel = _parkLotManager.GetDrivewayByMacAddress(model.DeviceMACAddress);
            if (drivewayModel == null)
            {
                LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_NOTEXISTS_DRIVEWAY.GetDesc();
                return responseModel;
            }

            //读取车辆的在场记录并验证停车标识是否一致
            VehicleInOutModel entermodel = _parkLotManager.GetEntryRecord(model.CarNo, drivewayModel.ParkCode);
            if (entermodel == null || entermodel.Guid != model.Guid)
            {
                LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_NOTEXISTS_CAR.GetDesc();
                return responseModel;
            }

            //请求Fujica Pay Api 发送支付授权码，申请扣费
            bool result = SendAuthCodeToFujica(drivewayModel.ParkCode, entermodel.CarNo, model.ParkingFee, model.PayAuthCode);
            if (result)
                responseModel.PayState = true;
            else
                responseModel.PayState = false;

            responseModel.Remark = "";
            responseModel.Extend = "";

            return responseModel;
        }

        /// <summary>
        /// Fujica Api 获取车道费用信息
        /// </summary>
        /// <param name="parkingCode">停车场编号</param>
        /// <param name="carNumber">车牌号码</param>
        /// <param name="deviceMACAddress">相机mac地址（车道号）</param>
        /// <param name="laneSenseDate">压地感时间</param>
        /// <param name="actualAmount">实际金额</param>
        /// <param name="beginTime">入场时间</param>
        /// <returns></returns>
        private bool GetLaneCostInfoByFujica(string parkingCode, string carNumber, string deviceMACAddress, DateTime laneSenseDate, out  decimal actualAmount, out DateTime beginTime)
        {
            actualAmount = 0;
            beginTime = DateTime.MinValue;

            //请求Fujica Api 获取车道费用信息
            RequestFujicaStandard requestFujica = new RequestFujicaStandard();
            //请求方法
            string servername = "/Park/GetLaneCostInfo";

            //请求参数
            Dictionary<string, object> dicParam = new Dictionary<string, object>();
            dicParam["ParkingCode"] = parkingCode;
            dicParam["CarNo"] = carNumber;
            dicParam["ExportCode"] = deviceMACAddress;
            dicParam["LandSenseDate"] = laneSenseDate;
            dicParam["LineDate"] = DateTime.Now;

            bool result = requestFujica.RequestInterfaceV2(servername, dicParam);
            if (result)
            {
                Dictionary<string, object> fujicaResultDic = m_serializer.Deserialize<Dictionary<string, object>>(requestFujica.FujicaResult);
                //解析result结果
                Dictionary<string, object> resultDic = m_serializer.Deserialize<Dictionary<string, object>>(fujicaResultDic["Result"].ToString()); 
                //应缴费用
                decimal.TryParse(Convert.ToString(resultDic["ActualAmount"]), out actualAmount);
                //入场时间
                DateTime.TryParse(Convert.ToString(resultDic["BeginTime"]), out beginTime);
                return true;
            }
            return false;
        }


        /// <summary>
        /// Fujica Pay Api 发送支付授权码，申请扣费
        /// </summary>
        /// <param name="parkingCode">停车场编码</param>
        /// <param name="carNumber">车牌号码</param>
        /// <param name="parkingFee">停车费用</param>
        /// <param name="authCode">授权码</param>
        /// <returns></returns>
        private bool SendAuthCodeToFujica(string parkingCode, string carNumber, decimal parkingFee,string authCode)
        {
            //请求Fujica Api 获取车道费用信息
            RequestFujicaStandard requestFujica = new RequestFujicaStandard();
            //请求方法
            string servername = "/Pay/StandardPayAuthCodeCommon";

            //请求参数
            Dictionary<string, object> dicParam = new Dictionary<string, object>();
            dicParam["ParkingCode"] = parkingCode;
            dicParam["CarNoOrCard"] = carNumber;
            dicParam["Price"] = parkingFee;
            dicParam["AuthCode"] = authCode;
            dicParam["PayStyle"] = 0;
            //dicParam["NotifyUrl"] = "";

            bool result = requestFujica.RequestInterfaceV2Pay(servername, dicParam);
            if (result)
                return true;
            else
                return false;
        }

    }
}
