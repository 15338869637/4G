/***************************************************************************************
 * *
 * *        File Name        : ParkLotManager.cs
 * *        Creator          : llp
 * *        Create Time      : 2019-09-12
 * *        Functional Description  : 车辆进出逻辑模块
 * *        Remark           :  
 * *
 * *  Copyright (c) 深圳市富士智能系统有限公司.  All rights reserved. 
 * ***************************************************************************************/
using Fujica.com.cn.Business.Model;
using Fujica.com.cn.Business.Base;
using Fujica.com.cn.Context.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fujica.com.cn.Business.ParkLot
{
    /// <summary>
    /// 车辆进出管理
    /// </summary>
    /// <remarks> 
    /// 2019.09.12: 增加 InVehicleDelete 在场车辆删除方法. llp <br/>  
    /// 2019.09.26: 修改 InVehicleDelete 在场车辆删除方法：增加判断当前相机是否断连业务. Ase <br/>  
    /// </remarks>
    partial class ParkLotManager
    {
        /// <summary>
        /// 拍照
        /// </summary>
        /// <param name="driveway">车道</param>
        /// <returns></returns>
        public bool Photograph(string parkcode, string devicemacaddress)
        {
            CommandEntity<PhotographModel> entity = new CommandEntity<PhotographModel>()
            {
                command = BussineCommand.Photograph,
                idMsg = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                message = new PhotographModel()
                {
                    DeviceIdentify = devicemacaddress
                }
            };
            return m_rabbitMQ.SendMessageForRabbitMQ("发送拍照命令", m_serializer.Serialize(entity), entity.idMsg, parkcode);
        }
   
        /// <summary>
        /// 车道闸口锁定
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool SetGateKeep(GateKeepListModel model)
        {
            if (model == null || model.List == null || model.List.Count <= 0) return false;

            bool flag = _iCarInOutContext.SetGateKeep(model);

            if (flag)
                foreach (var item in model.List)
                {
                    SendGateKeepToMq(new CameraKeepModel() { DeviceIdentify = item.DeviceMacAddress, GateState = item.GateState }, model.ParkingCode);
                }
            return true;
        }

        /// <summary>
        /// 读取车道闸口锁定集合
        /// </summary>
        /// <param name="parkingCode">停车场编码</param>
        /// <returns></returns>
        public GateKeepListModel GetGateKeep(string parkingCode)
        {
            return _iCarInOutContext.GetGateKeep(parkingCode);
        }

        /// <summary>
        /// 车牌修改重推
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool CarNumberRepushToCamera(CorrectCarnoModel model)
        {
            CarNumberRepushModel correctModel = new CarNumberRepushModel();
            correctModel.DeviceIdentify = model.DeviceIdentify;
            correctModel.ParkingCode = model.ParkingCode;
            correctModel.OldCarno = model.OldCarno;
            correctModel.NewCarno = model.NewCarno;  
            bool flag = SendCarNumberRepushToCameras(correctModel);
            if (flag)
            {
                DrivewayModel drivewayModel = _iDrivewayContext.GetDrivewayByMacAddress(model.DeviceIdentify);
                if (drivewayModel != null)
                { 
                    model.Discerncamera = drivewayModel.DeviceName;
                    model.ThroughName = drivewayModel.DrivewayName; 
                }
                ////保存至数据库,形成报表记录  
                flag = _iParkLotContext.AddCarnoRecorddatabaseoperate(model);
            }
            return flag;
        }

        /// <summary>
        /// 车牌修改重推 发送到相机
        /// </summary>
        /// <returns></returns>
        public bool SendCarNumberRepushToCameras(CarNumberRepushModel model)
        {
            CommandEntity<CarNumberRepushModel> entity = new CommandEntity<CarNumberRepushModel>()
            {
                command = BussineCommand.CarNumberRepush,
                idMsg = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                message = model
            };
            //将修改车牌重推数据通过mq交给相机去处理业务流程(出口相机按照新车牌重新进行推送，走正常业务)
            return m_rabbitMQ.SendMessageForRabbitMQ("发送修改车牌重推命令", m_serializer.Serialize(entity), entity.idMsg, model.ParkingCode); 

        }

        /// <summary>
        /// 车牌修正
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool CorrectToEntryCamera(CorrectCarnoModel model)
        {
            bool flag = SendCorrectToEntryCamera(model);
            if (flag)
            {
                DrivewayModel drivewayModel = _iDrivewayContext.GetDrivewayByMacAddress(model.DeviceIdentify);
                if (drivewayModel != null)
                {
                    model.ThroughName = drivewayModel.DrivewayName;
                    model.Discerncamera = drivewayModel.DeviceName;
                }

                ////保存至数据库,形成报表记录  
                flag = _iParkLotContext.AddCarnoRecorddatabaseoperate(model);
            }
            return flag;
        }

        /// <summary>
        /// 车牌修正 发送到相机
        /// </summary>
        /// <param name="model">修正车牌实体参数</param>
        /// <returns></returns>
        public bool SendCorrectToEntryCamera(CorrectCarnoModel model)
        {
            CommandEntity<CorrectCarnoModel> entity = new CommandEntity<CorrectCarnoModel>()
            {
                command = BussineCommand.Correct,
                idMsg = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                message = model
            };
            //将修正车牌数据通过mq交给相机去处理业务流程(新车牌入场、旧车牌出场)
            return m_rabbitMQ.SendMessageForRabbitMQ("发送修正车牌命令", m_serializer.Serialize(entity), entity.idMsg, model.ParkingCode);
        }


        /// <summary>
        /// 入场补录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool RecordInToEntryCamera(AddRecordModel model)
        {
            bool flag = SendRecordInToEntryCamera(model);
            if (flag)
            { 
                ////保存至数据库,形成报表记录  
                flag = _iParkLotContext.AddRecordToDatabase(model);
            }
            return flag;
        }
        /// <summary>
        /// 补发入场记录到入口相机
        /// </summary>
        /// <param name="model">补录车牌实体参数</param>
        /// <returns></returns>
        public bool SendRecordInToEntryCamera(AddRecordModel model)
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
        /// 获取车道拦截异常数据
        /// </summary>
        /// <param name="devicemacaddress">车道相机标识</param>
        /// <returns></returns>
        public GateCatchDetailModel GetGateCatch(string devicemacaddress)
        {
            return _iCarInOutContext.GetGateCatch(devicemacaddress);
        }

        /// <summary>
        /// 获取车道车辆进出抓拍数据
        /// </summary>
        /// <param name="parkingCode">车场编号</param>
        /// <param name="deviceMacAddress">车道相机标识</param>
        /// <returns></returns>
        public CaptureInOutModel GetGateData(string parkingCode, string deviceMacAddress)
        {
            return _iCarInOutContext.GetGateData(parkingCode, deviceMacAddress);
        }

        /// <summary>
        /// 车道闸口锁定
        /// </summary>
        /// <param name="model"></param>
        /// <param name="parkingCode"></param>
        /// <returns></returns>
        private bool SendGateKeepToMq(CameraKeepModel model, string parkingCode)
        {
            CommandEntity<CameraKeepModel> entity = new CommandEntity<CameraKeepModel>()
            {
                command = BussineCommand.GateKeep,
                idMsg = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                message = model
            };
            return m_rabbitMQ.SendMessageForRabbitMQ("发送车道闸口锁定命令", m_serializer.Serialize(entity), entity.idMsg, parkingCode);
        }

        /// <summary>
        /// 在场车辆删除 
        /// <param> 在场车辆删除实体参数</param> 
        /// </summary> 
        public bool InVehicleDelete(InVehicleDeleteModel model, string parkingCode)
        {
            string macAddress = string.Empty;
            //验证当前相机是否正常，如不可用则返回一个可用的正常相机地址
            bool drivewayStatus = CheckDrivewayStatus(parkingCode, model.DeviceMACAddress, out macAddress);
            if (!drivewayStatus)
                if (!string.IsNullOrEmpty(macAddress))
                    model.DeviceMACAddress = macAddress;
                else
                {
                    LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_MQ_SEND_ERROR.GetDesc();
                    return false;
                }
            
            return SendInVehicleDeleteToCamera(model, parkingCode);
        }

        /// <summary>
        /// 发送在场车辆删除命令给相机
        /// </summary>
        /// <param name="model">删除实体</param>
        /// <param name="parkingCode">停车场编码</param>
        /// <returns></returns>
        public bool SendInVehicleDeleteToCamera(InVehicleDeleteModel model, string parkingCode)
        {
            CommandEntity<InVehicleDeleteModel> entity = new CommandEntity<InVehicleDeleteModel>()
            {
                command = BussineCommand.FieldDelete,
                idMsg = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                message = model
            };
            return m_rabbitMQ.SendMessageForRabbitMQ("发送在场车辆删除命令", m_serializer.Serialize(entity), entity.idMsg, parkingCode);
        }

    }
}
