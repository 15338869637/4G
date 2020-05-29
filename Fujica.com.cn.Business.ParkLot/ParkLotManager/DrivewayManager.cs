/***************************************************************************************
 * *
 * *        File Name        : ParkLotManager.cs
 * *        Creator          : llp
 * *        Create Time      : 2019-09-21 
 * *        Remark           : 车道管理 业务逻辑层
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
    /// 添加车道
    /// </summary> 
    /// <remarks>
    /// 2019.09.21: 新增注释信息. llp <br/> 
    /// 2019.09.23: 修改SendMqDataByAddorEditCamera方法的错误返回值. Ase <br/>
    /// 2019.09.26: 增加CheckDrivewayStatus方法. Ase <br/>
    /// </remarks>  
    partial class ParkLotManager
    { 
        public bool AddDriveway(DrivewayModel model)
        {
            ParkLotModel contentParkLot = GetParkLot(model.ParkCode);

            //验证当前设备mac地址是否已存在
            DrivewayModel content = _iDrivewayContext.GetDrivewayByMacAddress(model.DeviceMacAddress);
            if (content != null)
            {
                LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_EXISTS_MACADDRESS_DRIVEWAY.GetDesc();
                return false;
            }

            //添加车道
            bool flag = _iDrivewayContext.AddDriveway(model);
            if (!flag)
            {
                LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_SAVE_DRIVEWAY.GetDesc();
                return false;
            }
            
            //修改车场信息中车道列表
            if (contentParkLot.DrivewayList == null)
            {
                contentParkLot.DrivewayList = new List<string>();
            }
            contentParkLot.DrivewayList.Add(model.Guid);
            flag = _iParkLotContext.ModifyParkLot(contentParkLot);
            if (!flag)
            {
                return false;
            }

            //初始化语音指令
            VoiceCommandModel voiceCommandModel = new VoiceCommandModel()
            {
                ProjectGuid = model.ProjectGuid,
                ParkCode = model.ParkCode,
                DrivewayGuid = model.Guid,
                DeviceMacAddress = model.DeviceMacAddress,
                CommandList = null
            };
            //语音指令返回值不影响车道保存结果
            _voiceCommandManager.InitVoiceCommand(voiceCommandModel);
            
            int entrywayCount = GetCameraCount(model.ParkCode, DrivewayType.In);
            //初始化相机 
            bool mqReturn = SendDriveWayType(model, entrywayCount, BussineCommand.CameraInfo);
            if (!mqReturn)
            {
                LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_MQ_SEND_ERROR.GetDesc();
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 修改车道
        /// </summary>
        /// <param name="model"></param>
        /// <param name="oldDeviceMacAddress">修改前的相机mac地址，用于判断相机是否替换</param>
        /// <returns></returns>
        public bool ModifyDriveway(DrivewayModel model)
        {
            //验证当前输入的设备mac地址是否已存在
            DrivewayModel content = _iDrivewayContext.GetDrivewayByMacAddress(model.DeviceMacAddress);
            if (content != null)
            {
                //guid是否一致
                if (content.Guid != model.Guid)
                {
                    LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_EXISTS_MACADDRESS_DRIVEWAY.GetDesc();
                    return false;
                }
            }

            //修改车道
            bool flag = _iDrivewayContext.ModifyDriveway(model);
            if (!flag)
            {
                //数据库不成功就不要往下执行了
                LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_SAVE_DRIVEWAY.GetDesc();
                return false; 
            }
            
            //获取停车场入口相机数量
            int entrywayCount = GetCameraCount(model.ParkCode, DrivewayType.In);
            //发送车道信息给相机
            bool mqReturn = SendDriveWayType(model, entrywayCount, BussineCommand.CameraEdit);
            if (!mqReturn)
            {
                LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_MQ_SEND_ERROR.GetDesc();
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 删除车道
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool DeleteDriveway(DrivewayModel model)
        {
            bool flag = _iDrivewayContext.DeleteDriveway(model);
            if (!flag)
            {
                //数据库不成功就不要往下执行了
                LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_DELETE_DRIVEWAY.GetDesc(); 
                return false; 
            }

            //修改车场信息中车道列表
            ParkLotModel content = _iParkLotContext.GetParkLot(model.ParkCode);
            if (content != null)
            {
                if (content.DrivewayList != null)
                {
                    if (content.DrivewayList.Remove(model.Guid))
                    {
                        flag = _iParkLotContext.ModifyParkLot(content);
                        if (!flag) return false;
                    }
                }
            }

            //获得入口相机数量
            int entrywayCount = GetCameraCount(model.ParkCode, DrivewayType.In);
            //删除相机 
            bool mqReturn = SendDriveWayType(model, entrywayCount, BussineCommand.CameraDelete);
            if (!mqReturn)
            {
                LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_MQ_SEND_ERROR.GetDesc();
                return false;
            }
            else
            {
                return true;
            }

        }

        /// <summary>
        /// 根据车道类型获取车场的相机数量
        /// </summary>
        /// <param name="parkingCode">停车场编号</param>
        /// <param name="type">车道类型</param>
        /// <returns></returns>
        private int GetCameraCount(string parkingCode , DrivewayType type)
        {
            //停车场所有车道 
            List<DrivewayModel> list = AllDriveway(parkingCode);
            if (list == null)
                return 0;
            return list.Where(m => m.Type == type).Count();
        }

        /// <summary>
        /// 某车场的所有车道
        /// </summary>
        /// <param name="parkcode"></param>
        /// <returns></returns>
        public List<DrivewayModel> AllDriveway(string parkcode)
        {
            List<DrivewayModel> model = _iDrivewayContext.AllDriveway(parkcode);
            return model;
        }

        /// <summary>
        /// 获取车场所有车道的链接状态
        /// </summary>
        /// <param name="parkingCode">停车场编码</param>
        /// <returns></returns>
        public IList<DrivewayConnStatusModel> AllDrivewayConnStatus(string parkingCode)
        {
            IList<DrivewayConnStatusModel> list = _iDrivewayContext.AllDrivewayConnStatus(parkingCode);

            return list;
        }

        /// <summary>
        /// 验证车道状态，并分配一个可用的设备编号
        /// </summary>
        /// <param name="parkingCode">停车场编号</param>
        /// <param name="deviceMACAddress">设备标识</param>
        /// <param name="activeDeviceMACAddress">返回有效的设备标识</param>
        /// <returns>传入的设备标识状态 true:正常  false:连接中</returns>
        public bool CheckDrivewayStatus(string parkingCode, string deviceMACAddress, out string activeDeviceMACAddress)
        {
            activeDeviceMACAddress = string.Empty;
            List<DrivewayConnStatusModel> list = AllDrivewayConnStatus(parkingCode) as List<DrivewayConnStatusModel>;
            if (list != null && list.Count > 0)
            {
                //判断当前相机是否断开连接
                DrivewayConnStatusModel currentStatusModel = list.Where(m => m.DeviceMacAddress == deviceMACAddress && m.DeviceStatus).FirstOrDefault();
                //未断开
                if (currentStatusModel != null)
                {
                    activeDeviceMACAddress = deviceMACAddress;
                    return true;
                }
                else
                {
                    //该相机断开连接，分配一个当前连接其他可用的相机
                    DrivewayConnStatusModel otherStatusModel = list.Where(m => m.DeviceStatus).FirstOrDefault();
                    if (otherStatusModel != null)
                    {
                        activeDeviceMACAddress = otherStatusModel.DeviceMacAddress;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 获取某车道
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public DrivewayModel GetDriveway(string guid)
        {
            return _iDrivewayContext.GetDriveway(guid);
        }

        /// <summary>
        /// 通过车道设备MAC地址获取车道实体
        /// </summary>
        /// <param name="macaddress"></param>
        /// <returns></returns>
        public DrivewayModel GetDrivewayByMacAddress(string macaddress)
        {
            return _iDrivewayContext.GetDrivewayByMacAddress(macaddress);
        }

        /// <summary>
        /// 发送车道类型到设备
        /// </summary>
        /// <param name="model">车道实体</param>
        /// <param name="count">入口数量</param>
        /// <param name="commandType">业务类型</param>
        /// <returns></returns>
        public bool SendDriveWayType(DrivewayModel model, int entrywayCount, BussineCommand commandType)
        {
            try
            {
                if (model != null)
                {
                    CameraTypeModel sendmodel = new CameraTypeModel()
                    {
                        Type = (int)model.Type,
                        DeviceIdentify = model.DeviceMacAddress,
                        ParkingCode = model.ParkCode,
                        EntrywayCount = entrywayCount,
                        DeviceEntranceURI = model.DeviceEntranceURI,
                        DeviceAccount = model.DeviceAccount
                    };
                    CommandEntity<CameraTypeModel> entity = new CommandEntity<CameraTypeModel>()
                    {
                        command = commandType,
                        idMsg = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                        message = sendmodel
                    };
                    string remark = string.Empty;
                    switch (commandType)
                    {   
                        case BussineCommand.CameraInfo:
                            remark = "发送相机初始化类型指令命令";
                            break;
                        case BussineCommand.CameraDelete:
                            remark = "发送相机删除类型指令命令";
                            break;
                        default:
                            remark = "发送相机初始化类型指令命令";
                            break;
                    }
                    return m_rabbitMQ.SendMessageForRabbitMQ(remark, m_serializer.Serialize(entity), entity.idMsg, model.ParkCode, "CameraParkInit", "FuJiCaYunCameraParkInit.fanout");
                }
            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.Bussiness, "",model.ParkCode, "", "Fujica.com.cn.Business.ParkLot.DrivewayManager.SendDriveWayType", "下发车道相机类型发生异常", ex.ToString());
            }
            return false;
        }

        /// <summary>
        /// 新增、修改相机时下发数据集合进行相机数据同步
        /// 月卡、储值卡、黑名单、其他设置页（不同颜色车牌车类、满位禁入车类、月租车过期方式、月租车延期提醒天数）、入场数据
        /// </summary>
        public bool SendMqDataByAddorEditCamera(string projectGuid, string parkingCode, string deviceMACAddress)
        {
            LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_MQ_SEND_ERROR.GetDesc();

            //同步临时卡
            int tempCarCount = _cardServiceManager.SendTempCarAllToCameras(parkingCode);
            //if (tempCarCount <= 0) return false;
            //同步月卡
            int monthCarCount = _cardServiceManager.SendMonthCarAllToCameras(parkingCode);
            //if (monthCarCount <= 0) return false;
            //同步储值卡
            int valueCarCount = _cardServiceManager.SendValueCarAllToCameras(parkingCode);
            //if (valueCarCount <= 0) return false;
            //同步黑名单
            bool blackResult = SendTrafficRestriction(parkingCode);
            if (!blackResult)
            {
                LastErrorDescribe += "（黑名单）";
                return false;
            }
            //其他设置页-不同颜色车牌对应车类
            bool colorResult = SendTempCarTypeOfPlateColor(projectGuid, parkingCode);
            if (!colorResult)
            {
                LastErrorDescribe += "（颜色车类）";
                return false;
            }
            //其他设置页-满位禁入车类
            bool fullResult = SendBarredEntryCarTypeOnParkingFull(projectGuid, parkingCode);
            if (!fullResult)
            {
                LastErrorDescribe += "（满位禁入车类）";
                return false;
            }
            //其他设置页-月租车过期方式
            bool expiremodeResult = SendMonthCarExpireMode(projectGuid, parkingCode);
            if (!expiremodeResult)
            {
                LastErrorDescribe += "（月租车过期方式）";
                return false;
            }
            //其他设置页-月租车延期提醒天数
            bool reminddayResult = SendMonthCarExpireRemindDay(projectGuid, parkingCode);
            if (!reminddayResult)
            {
                LastErrorDescribe += "（月租车延期提醒）";
                return false;
            }
            //车辆入场记录（临时车、月卡车、储值车...）
            bool syncparkingResult = SendAllPresenceOfVehicleByOriginal(parkingCode, deviceMACAddress);
            if (!syncparkingResult)
            {
                LastErrorDescribe += "（入场记录）";
                return false;
            }
            return true;
        }

    }
}
