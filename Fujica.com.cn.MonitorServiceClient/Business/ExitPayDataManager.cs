/***************************************************************************************
 * *
 * *        File Name        : ExitPayDataManager.cs
 * *        Creator          : llp
 * *        Create Time      : 2019-09-21 
 * *        Remark           : 临时车出口压地感扫码缴费数据管理
 * *
 * *  Copyright (c) 深圳市富士智能系统有限公司.  All rights reserved. 
 * ***************************************************************************************/
using Fujica.com.cn.BaseConnect;
using Fujica.com.cn.Business.Model;
using Fujica.com.cn.Context.Model;
using Fujica.com.cn.Logger;
using Fujica.com.cn.Tools;
using StackExchange.Redis;
using System;
using System.Collections.Generic;

namespace Fujica.com.cn.MonitorServiceClient.Business
{
    /// <summary>
    /// 临时车出口压地感扫码缴费 数据管理.
    /// </summary>
    /// <remarks>
    /// 2019.09.21: 日志参数完善. llp <br/> 
    /// </remarks>  
    public class ExitPayDataManager
    {
        private static string mq_ListKey = "MQ_GroundSense";

        public static ResponseCommon DataHandle(ILogger m_ilogger, ISerializer m_serializer)
        {
            ResponseCommon response = new ResponseCommon()
            {
                IsSuccess = false,
                MsgType = MsgType.GroundSense
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

            //转换成 出口未缴费临时车辆 实体
            ExitTempCarPayModel exitTempCarModel = m_serializer.Deserialize<ExitTempCarPayModel>(redisContent);
            if (exitTempCarModel == null)
            {
                response.MessageContent = "redis数据库读取值转换成实体失败";
                return response;

            }
            if (string.IsNullOrEmpty(exitTempCarModel.Guid)
                || string.IsNullOrEmpty(exitTempCarModel.DriveWayMAC)
                || string.IsNullOrEmpty(exitTempCarModel.CarNo)
                || exitTempCarModel.LaneSenseDate == DateTime.MinValue)
            {
                response.MessageContent = "redis数据转换成实体后必要参数缺失";
                return response;
            }

            db = RedisHelper.GetDatabase(0);
            //根据相机MAC地址拿到车道的guid
            string drivewayguid = db.HashGet("DrivewayLinkMACList", exitTempCarModel.DriveWayMAC);
            //得到相对应的车道实体
            DrivewayModel drivewaymodel = m_serializer.Deserialize<DrivewayModel>(db.HashGet("DrivewayList", drivewayguid ?? ""));
            if (drivewaymodel == null)
            {
                response.MessageContent = "根据车道相机设备MAC地址，读取车道模型为空";
                return response;
            }

            //验证当前车牌是否在场
            string carNumber = exitTempCarModel.CarNo;
            int dbIndex = Common.GetDatabaseNumber(carNumber);
            db = RedisHelper.GetDatabase(dbIndex);

            //去redis中查询车辆是否在场，返回在场实体
            VehicleEntryDetailModel entryModel = m_serializer.Deserialize<VehicleEntryDetailModel>(db.HashGet(carNumber, drivewaymodel.ParkCode));

            //为空，则代表车辆不在场
            if (entryModel == null)
            {
                response.MessageContent = "未找到当前车辆在场记录";
                return response;
            }

            //相机 临时车实体传过来的“当次停车唯一标识”和redis在场数据中的“当次停车唯一标识”值相等，确认是同一条记录
            if (exitTempCarModel.Guid != entryModel.RecordGuid)
            {
                response.MessageContent = "当次停车唯一标识与redis数据不一致";
                return response;
            }

            //拿到车类信息
            CarTypeModel cartypemodel = m_serializer.Deserialize<CarTypeModel>(db.HashGet("CarTypeList", entryModel.CarTypeGuid));
            if (cartypemodel == null)
            {
                response.MessageContent = "根据车类Guid，读取车类模型为空";
                return response;
            }

            try
            {
                //去fujica拿到当前停车的费用
                string fujicaTempCarModel = GetFujicaTempCarByCarNo(entryModel.CarNo, entryModel.ParkingCode, m_serializer);
                if (string.IsNullOrEmpty(fujicaTempCarModel))
                {
                    response.MessageContent = "fujicaApi/GetTempCarPaymentInfoByCarNo未查询到停车信息";
                    return response;
                }
                Dictionary<string, object> fujicaTempCarDic = m_serializer.Deserialize<Dictionary<string, object>>(fujicaTempCarModel);
                if (fujicaTempCarDic == null || fujicaTempCarDic["ParkingFee"] == null || fujicaTempCarDic["ActualAmount"] == null)
                {
                    response.MessageContent = "fujicaApi/GetTempCarPaymentInfoByCarNo返回结果解析失败" + fujicaTempCarModel;
                    return response;
                }

                //fujica返回停车费用
                decimal amount = Convert.ToDecimal(fujicaTempCarDic["ParkingFee"]);
                //fujica返回应缴费用
                decimal actualAmount = Convert.ToDecimal(fujicaTempCarDic["ActualAmount"]);
                //再将数据组装后，上传到fujica的车道码压地感车辆上传接口
                bool uploadResult = UploadExitPayDataToFujica(entryModel.CarNo, entryModel.ParkingCode, amount, actualAmount, exitTempCarModel.LaneSenseDate, entryModel.RecordGuid, cartypemodel.Idx, entryModel.InImgUrl, entryModel.BeginTime, exitTempCarModel.DriveWayMAC, drivewaymodel.DrivewayName);
                if (uploadResult)
                {
                    response.IsSuccess = true;
                    response.MessageContent = "车辆压地感数据上传Fujica成功";
                    return response;
                }
                else
                {
                    response.MessageContent = "车辆压地感数据上传Fujica失败";
                    return response;
                }
            }
            catch (Exception ex)
            {
                m_ilogger.LogFatal(LoggerLogicEnum.Tools, entryModel.RecordGuid, entryModel.ParkingCode, entryModel.CarNo, "Fujica.com.cn.MonitorServiceClient.Business.ExitPayDataManager.DataHandle", "未缴费临时车压地感上传fujica出现异常", ex.ToString());

                response.MessageContent = "车辆压地感数据发生异常：" + ex.ToString();
                return response;
            }

        }

        /// <summary>
        /// 获取主平台fujica临时车辆的停车信息
        /// </summary>
        /// <param name="carNumber">车牌号码</param>
        /// <param name="parkingCode">停车场编号</param>
        /// <param name="m_serializer"></param>
        /// <returns></returns>
        private static string GetFujicaTempCarByCarNo(string carNumber, string parkingCode, ISerializer m_serializer)
        {
            string result = string.Empty;

            RequestFujicaStandard requestFujica = new RequestFujicaStandard();
            //请求方法
            string servername = "/Park/GetTempCarPaymentInfoByCarNo";
            //请求参数
            Dictionary<string, object> dicParam = new Dictionary<string, object>();
            dicParam["CarNo"] = carNumber;//车牌号
            dicParam["ParkingCode"] = parkingCode;//停车场编码

            //返回fujica api接口的结果
            if (requestFujica.RequestInterfaceV2(servername, dicParam))
            {
                Dictionary<string, object> resultdic = m_serializer.Deserialize<Dictionary<string, object>>(requestFujica.FujicaResult);
                result = Convert.ToString(resultdic["Result"]);
            }

            return result;
        }

        /// <summary>
        /// 上传压地感车辆数据
        /// </summary>
        /// <param name="carNumber">车牌号</param>
        /// <param name="parkingCode">停车场编码</param>
        /// <param name="amount">应缴费用</param>
        /// <param name="actualAmount">实缴费用</param>
        /// <param name="landSenseDate">压地感时间</param>
        /// <param name="recordGuid">唯一订单号</param>
        /// <param name="carType">车类(算费模板编号)</param>
        /// <param name="inImg">入场图片</param>
        /// <param name="beginDate">入场时间</param>
        /// <param name="driveWayMAC">出口编号</param>
        /// <param name="drivewayName">出口名</param>
        /// <returns></returns>
        private static bool UploadExitPayDataToFujica(string carNumber, string parkingCode, decimal amount, decimal actualAmount, DateTime landSenseDate, string recordGuid, string carType, string inImg, DateTime beginDate, string driveWayMAC, string drivewayName)
        {
            RequestFujicaStandard requestFujica = new RequestFujicaStandard();
            //请求方法
            string servername = "/CalculationCost/ApiScanCode";
            //请求参数
            Dictionary<string, object> dicParam = new Dictionary<string, object>();
            dicParam["CarNo"] = carNumber;//车牌号
            dicParam["ParkingCode"] = parkingCode;//停车场编码
            dicParam["Amount"] = amount;//应缴费用
            dicParam["ActualAmount"] = actualAmount;//实缴费用
            dicParam["DiscountAmount"] = amount - actualAmount;//优惠费用
            dicParam["LineDate"] = DateTime.Now;//客户端时间
            dicParam["LandSenseDate"] = landSenseDate;//压地感时间
            dicParam["LineRecordCode"] = recordGuid;//唯一订单号
            dicParam["CarType"] = carType;//车类(算费模板编号)
            dicParam["Img"] = inImg;//入场图片
            dicParam["AdmissionDate"] = beginDate;//入场时间
            dicParam["ExportCode"] = driveWayMAC;//出口编号
            dicParam["Export"] = drivewayName;//出口名

            //fujica api上传出口压地感车辆信息 接口
            return requestFujica.RequestInterfaceV2(servername, dicParam);
        }

    }
}
