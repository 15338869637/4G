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
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

/***************************************************************************************
 * *
 * *        File Name        : ParkLotController.cs
 * *        Creator          : llp
 * *        Create Time      : 2019-09-03
 * *        Functional Description  : 车场业务模块
 * *        Remark           :  
 * *
 * *  Copyright (c) 深圳市富士智能系统有限公司.  All rights reserved. 
 * ***************************************************************************************/
namespace Fujica.com.cn.Interface.Management.Controllers
{
    /// <summary>
    ///车场业务模块
    /// </summary> 
    /// <remarks> 
    /// 2019.09.12: 增加InVehicleDelete在场车辆删除接口. llp <br/>  
    /// 2019.10.08: 修改实体名称InVehicleDelete为InVehicleDeleteModel. Ase <br/>  
    /// 2018.10.29：新增GetGateData获取车道车辆进出抓拍数据接口. Ase <br/>
    /// </remarks>
    public class ParkLotController : BaseController
    {
        /// <summary>
        /// 停车场管理器
        /// </summary>
        private ParkLotManager _parkLotManager = null;
        /// <summary>
        /// 扫码管理器
        /// </summary>
        private ScanningManager _scanningManager = null;

        /// <summary>
        /// 卡务管理
        /// </summary>
        private CardServiceManager _cardServiceManager = null;
        /// <summary>
        /// 唯一构造函数
        /// </summary>
        /// <param name="logger">日志接口器</param>
        /// <param name="serializer">序列化接口器</param>
        /// <param name="apiaccesscontrol">api接入控制器</param>
        /// <param name="parkLotManager">车场管理器</param>
        /// <param name="scanningManager">扫码管理器</param>
        /// <param name="cardServiceManager">卡务信息管理</param>        
        public ParkLotController(ILogger logger, ISerializer serializer, APIAccessControl apiaccesscontrol,
            ParkLotManager parkLotManager,
            ScanningManager scanningManager,
            CardServiceManager cardServiceManager) :
            base(logger, serializer, apiaccesscontrol)
        {
            _parkLotManager = parkLotManager;
            _scanningManager = scanningManager;
            _cardServiceManager = cardServiceManager;
        }

        #region 车场 

        /// <summary>
        /// 添加车场
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [ResponseType(typeof(ResponseBaseCommon))]
        public IHttpActionResult AddNewParkLot(ParkLotRequest model)
        {
            ResponseBaseCommon response = new ResponseBaseCommon()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString(),
            };

            if(string.IsNullOrWhiteSpace(model.ParkingName) || 
                string.IsNullOrWhiteSpace(model.ParkingSiteAddress) ||
                string.IsNullOrWhiteSpace(model.ParkingCode) ||
                string.IsNullOrWhiteSpace(model.ProjectGuid))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "必要参数缺失,请检查";
                return Ok(response);
            }

            //GatherAccountModel account = gatheraccountmanager.GetGatherAccount(model.GatherAccountGuid);
            ParkLotModel content = new ParkLotModel()
            {  
                Existence = true,
                ParkName= model.ParkingName,
                ParkCode= model.ParkingCode,
                Prefix=model.CarNoPrefix,
                SpacesNumber=model.ParkingSpacesNumber,
                RemainingSpace=model.RemainingSpace,
                Type=model.ParkingType,
                SiteAddress=model.ParkingSiteAddress,
                ProjectGuid=model.ProjectGuid,
                //gatheraccountGuid= (account == null ? "" : account.guid)
            };
            bool flag = _parkLotManager.AddNewParkLot(content, model.RoleGuid);
            if (!flag)
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiParkLotErrorCode.API_DATA_SAVE_ERROR;
                response.MessageContent = _parkLotManager.LastErrorDescribe;
            }
            return Ok(response);
        }

        /// <summary>
        /// 修改车场
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(ResponseBaseCommon))]
        public IHttpActionResult ModifyParkLot(ParkLotRequest model)
        {
            ResponseBaseCommon response = new ResponseBaseCommon()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };

            if (string.IsNullOrWhiteSpace(model.ParkingCode) ||
                string.IsNullOrWhiteSpace(model.ParkingName) ||
                string.IsNullOrWhiteSpace(model.ParkingSiteAddress) ||
                string.IsNullOrWhiteSpace(model.ProjectGuid))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "必要参数缺失,请检查";
                return Ok(response);
            }

            ParkLotModel content = _parkLotManager.GetParkLot(model.ParkingCode);
            if (content != null)
            {
                if (content.ProjectGuid == model.ProjectGuid)
                {
                    //GatherAccountModel account = gatheraccountmanager.GetGatherAccount(model.GatherAccountGuid);
                    content.Existence = model.Existence;
                    content.ParkName = model.ParkingName;
                    content.ParkCode = model.ParkingCode;
                    content.Prefix = model.CarNoPrefix;
                    content.SpacesNumber = model.ParkingSpacesNumber;
                    content.RemainingSpace = model.RemainingSpace;
                    content.Type = model.ParkingType;
                    content.SiteAddress = model.ParkingSiteAddress;
                    content.ProjectGuid = model.ProjectGuid;
                    //content.gatheraccountGuid = (account == null ? "" : account.guid);
                    bool flag = _parkLotManager.ModifyParkLot(content);
                    if (!flag)
                    {
                        response.IsSuccess = false;
                        response.MessageCode = (int)ApiParkLotErrorCode.API_DATA_SAVE_ERROR;
                        response.MessageContent = _parkLotManager.LastErrorDescribe;
                    }
                }else
                {
                    response.IsSuccess = false;
                    response.MessageCode = (int)ApiBaseErrorCode.API_INTERFACENAME_ERROR;
                    response.MessageContent = ApiBaseErrorCode.API_INTERFACENAME_ERROR.ToString();
                }
            }else
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiParkLotErrorCode.API_DATA_NULL_ERROR;
                response.MessageContent = ApiParkLotErrorCode.API_DATA_NULL_ERROR.ToString();
            }
            return Ok(response);
        }

        /// <summary>
        /// 删除车场
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(ResponseBaseCommon))]
        public IHttpActionResult RemoveParkLot(DeleteParkLotRequest model)
        {
            ResponseBaseCommon response = new ResponseBaseCommon()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };

            if (string.IsNullOrWhiteSpace(model.ParkingCode) ||
                string.IsNullOrWhiteSpace(model.ProjectGuid))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "必要参数缺失,请检查";
                return Ok(response);
            }

            ParkLotModel content = _parkLotManager.GetParkLot(model.ParkingCode);
            if (content != null)
            {
                if (content.ProjectGuid == model.ProjectGuid)
                {
                    if (!_parkLotManager.CancelParkLot(content))
                    {
                        response.IsSuccess = false;
                        response.MessageCode = (int)ApiParkLotErrorCode.API_DATA_SAVE_ERROR;
                        response.MessageContent = _parkLotManager.LastErrorDescribe;
                    }
                }
                else
                {
                    response.IsSuccess = false;
                    response.MessageCode = (int)ApiBaseErrorCode.API_INTERFACENAME_ERROR;
                    response.MessageContent = ApiBaseErrorCode.API_INTERFACENAME_ERROR.ToString();
                }
            }
            else
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiParkLotErrorCode.API_DATA_NULL_ERROR;
                response.MessageContent = ApiParkLotErrorCode.API_DATA_NULL_ERROR.ToString();
            }

            return Ok(response);
        }

        /// <summary>
        /// 获取车场
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(ResponseBase<ParkLotModel>))]
        public IHttpActionResult GetParkLot(string ParkingCode)
        {
            ResponseBase<ParkLotModel> response = new ResponseBase<ParkLotModel>()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };
            if (string.IsNullOrWhiteSpace(ParkingCode))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "停车场编码不能为空,请检查";
                return Ok(response);
            }
            ParkLotModel content = _parkLotManager.GetParkLot(ParkingCode);
            response.Result = content;

            return Ok(response);
        }

        /// <summary>
        /// 获取车场列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(ResponseBaseList<ParkLotModel>))]
        public IHttpActionResult GetParkLotList(string ProjectGuid)
        {
            ResponseBaseList<ParkLotModel> response = new ResponseBaseList<ParkLotModel>()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };
            if (string.IsNullOrWhiteSpace(ProjectGuid))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "项目编码不能为空,请检查";
                return Ok(response);
            }
            List<ParkLotModel> content = _parkLotManager.AllParklot(ProjectGuid);
            response.Result = content;

            return Ok(response);
        }
         
         /// <summary>
        ///获取车场 剩余车位数量 
        /// </summary>
        /// <returns></returns> 
        [HttpGet]
        [ResponseType(typeof(ResponseBase<SpaceNumberModel>))]
        public IHttpActionResult GetRemainingSpace(string ParkingCode)
        {
            ResponseBase<SpaceNumberModel> response = new ResponseBase<SpaceNumberModel>()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };
            if (string.IsNullOrWhiteSpace(ParkingCode))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "停车场编码不能为空,请检查";
                return Ok(response);
            }
            SpaceNumberModel content = _parkLotManager.GetRemainingSpace(ParkingCode);
            response.Result = content; 
            return Ok(response);
        }


        #endregion

        #region 车道
        /// <summary>
        /// 设置/新增车道
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(ResponseBaseCommon))]
        public IHttpActionResult AddNewDriveWay(DriveWayRequest model)
        {
            ResponseBaseCommon response = new ResponseBaseCommon()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };

            if (string.IsNullOrWhiteSpace(model.ParkingCode) ||
                string.IsNullOrWhiteSpace(model.DriveWayName) ||
                string.IsNullOrWhiteSpace(model.DeviceName) ||
                string.IsNullOrWhiteSpace(model.DeviceMACAddress) ||
                string.IsNullOrWhiteSpace(model.ProjectGuid))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "必要参数缺失,请检查";
                return Ok(response);
            }

            DrivewayModel content = new DrivewayModel()
            {
                ProjectGuid = model.ProjectGuid,
                ParkCode = model.ParkingCode,
                Guid = Guid.NewGuid().ToString("N"),
                DrivewayName=model.DriveWayName,
                Type=(DrivewayType)model.DrivewayType,
                DeviceName=model.DeviceName,
                DeviceMacAddress=model.DeviceMACAddress,
                DeviceEntranceURI=model.DeviceEntranceURI,
                DeviceAccount=model.DeviceAccount,
                DeviceVerification=model.DeviceVerification
            };

            //验证访问权限
            bool authState = _parkLotManager.ParkLotAccessPermission(model.ParkingCode, model.ProjectGuid);
            if (authState)
            {
                bool flag = _parkLotManager.AddDriveway(content);
                if (!flag)
                {
                    response.IsSuccess = false;
                    response.MessageCode = (int)ApiParkLotErrorCode.API_DATA_SAVE_ERROR;
                    response.MessageContent = _parkLotManager.LastErrorDescribe;
                }
                else
                {
                    //新增相机成功，同步数据
                    _parkLotManager.SendMqDataByAddorEditCamera(content.ProjectGuid, content.ParkCode, content.DeviceMacAddress);
                }
            }
            else
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_INTERFACENAME_ERROR;
                response.MessageContent = _parkLotManager.LastErrorDescribe;
            }
            return Ok(response);
        }

        /// <summary>
        /// 修改车道
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(ResponseBaseCommon))]
        public IHttpActionResult ModifyDriveWay(ModifyDriveWayRequest model)
        {
            ResponseBaseCommon response = new ResponseBaseCommon()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };

            if (string.IsNullOrWhiteSpace(model.ParkingCode) ||
                string.IsNullOrWhiteSpace(model.Guid) ||
                string.IsNullOrWhiteSpace(model.DriveWayName) ||
                string.IsNullOrWhiteSpace(model.DeviceName) ||
                string.IsNullOrWhiteSpace(model.DeviceMACAddress) ||
                string.IsNullOrWhiteSpace(model.ProjectGuid))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "必要参数缺失,请检查";
                return Ok(response);
            }

            DrivewayModel content = _parkLotManager.GetDriveway(model.Guid);
            if (content != null)
            {
                if (content.ProjectGuid == model.ProjectGuid && content.ParkCode == model.ParkingCode)
                {
                    string oldDeviceMacAddress = content.DeviceMacAddress;

                    content.DrivewayName = model.DriveWayName;
                    content.Type = (DrivewayType)model.DrivewayType;
                    content.DeviceName = model.DeviceName;
                    content.DeviceMacAddress = model.DeviceMACAddress.ToLower();
                    content.DeviceEntranceURI = model.DeviceEntranceURI;
                    content.DeviceAccount = model.DeviceAccount;
                    content.DeviceVerification = model.DeviceVerification;
                    bool flag = _parkLotManager.ModifyDriveway(content);
                    if (!flag)
                    {
                        response.IsSuccess = false;
                        response.MessageCode = (int)ApiParkLotErrorCode.API_DATA_SAVE_ERROR;
                        response.MessageContent = _parkLotManager.LastErrorDescribe;
                    }
                    else
                    {
                        //修改相机成功并且修改了相机的mac地址，识别换了新相机，进行数据同步
                        if (oldDeviceMacAddress != model.DeviceMACAddress)
                        {
                            //修改相机成功，同步数据
                            _parkLotManager.SendMqDataByAddorEditCamera(content.ProjectGuid, content.ParkCode, content.DeviceMacAddress);
                        }
                    }
                }
                else
                {
                    response.IsSuccess = false;
                    response.MessageCode = (int)ApiBaseErrorCode.API_INTERFACENAME_ERROR;
                    response.MessageContent = ApiBaseErrorCode.API_INTERFACENAME_ERROR.ToString();
                }
            }
            else
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiParkLotErrorCode.API_DATA_NULL_ERROR;
                response.MessageContent = ApiParkLotErrorCode.API_DATA_NULL_ERROR.ToString();
            }
            return Ok(response);
        }

        /// <summary>
        /// 删除车道
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(ResponseBaseCommon))]
        public IHttpActionResult RemoveDriveWay(DeleteDriveWayRequest model)
        {
            ResponseBaseCommon response = new ResponseBaseCommon()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };

            if (string.IsNullOrWhiteSpace(model.ParkingCode) ||
                string.IsNullOrWhiteSpace(model.Guid) ||
                string.IsNullOrWhiteSpace(model.ProjectGuid))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "必要参数缺失,请检查";
                return Ok(response);
            }

            DrivewayModel content = _parkLotManager.GetDriveway(model.Guid);
            if (content != null)
            {
                if (content.ProjectGuid == model.ProjectGuid && content.ParkCode == model.ParkingCode)
                {
                    if (!_parkLotManager.DeleteDriveway(content))
                    {
                        response.IsSuccess = false;
                        response.MessageCode = (int)ApiParkLotErrorCode.API_DATA_SAVE_ERROR;
                        response.MessageContent = _parkLotManager.LastErrorDescribe;
                    }
                }
                else
                {
                    response.IsSuccess = false;
                    response.MessageCode = (int)ApiBaseErrorCode.API_INTERFACENAME_ERROR;
                    response.MessageContent = ApiBaseErrorCode.API_INTERFACENAME_ERROR.ToString();
                }
            }
            else
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiParkLotErrorCode.API_DATA_NULL_ERROR;
                response.MessageContent = ApiParkLotErrorCode.API_DATA_NULL_ERROR.ToString();
            }

            return Ok(response);
        }

        /// <summary>
        /// 获取车道
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(ResponseBase<DrivewayModel>))]
        public IHttpActionResult GetDriveWay(string Guid)
        {
            ResponseBase<DrivewayModel> response = new ResponseBase<DrivewayModel>()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };

            if (string.IsNullOrWhiteSpace(Guid))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "车道标识不能为空";
                return Ok(response);
            }

            DrivewayModel content = _parkLotManager.GetDriveway(Guid);
            response.Result = content;

            return Ok(response);
        }

        /// <summary>
        /// 获取车道列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(ResponseBaseList<DrivewayModel>))]
        public IHttpActionResult GetDriveWayList(string ParkingCode)
        {
            ResponseBaseList<DrivewayModel> response = new ResponseBaseList<DrivewayModel>()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };

            if (string.IsNullOrWhiteSpace(ParkingCode))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "停车场编码不能为空";
                return Ok(response);
            }

            List<DrivewayModel> content = _parkLotManager.AllDriveway(ParkingCode);
            response.Result = content;

            return Ok(response);
        }

        /// <summary>
        /// 获取车场所有车道的链接状态
        /// </summary>
        /// <param name="parkingCode">停车场编码</param>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(ResponseBaseList<DrivewayConnStatusModel>))]
        public IHttpActionResult GetDriveWayStatusList(string parkingCode)
        {
            ResponseBaseList<DrivewayConnStatusModel> response = new ResponseBaseList<DrivewayConnStatusModel>()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };

            if (string.IsNullOrWhiteSpace(parkingCode))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "停车场编码不能为空";
                return Ok(response);
            }

            IList<DrivewayConnStatusModel> list = _parkLotManager.AllDrivewayConnStatus(parkingCode);
            response.Result = list as List<DrivewayConnStatusModel>;

            return Ok(response);
        }

        /// <summary>
        /// 车道数据同步
        /// </summary>
        /// <param name="projectGuid"></param>
        /// <param name="parkingCode"></param>
        /// <param name="deviceMacAddress"></param>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(ResponseBaseCommon))]
        public IHttpActionResult SyncDriveWayData(string projectGuid, string parkingCode, string deviceMacAddress)
        {
            ResponseBaseCommon response = new ResponseBaseCommon()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };

            if (string.IsNullOrWhiteSpace(projectGuid)||
                string.IsNullOrWhiteSpace(parkingCode) ||
                string.IsNullOrWhiteSpace(deviceMacAddress))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "必要参数缺失,请检查";
                return Ok(response);
            }

            bool flag = _parkLotManager.SendMqDataByAddorEditCamera(projectGuid, parkingCode, deviceMacAddress);
            if (!flag)
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiParkLotErrorCode.API_DATA_SAVE_ERROR;
                response.MessageContent = _parkLotManager.LastErrorDescribe;
            }

            return Ok(response);
        }

        #endregion

        #region 车类
        /// <summary>
        /// 设置/新增车类
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(ResponseBaseCommon))]
        public IHttpActionResult AddCarType(CarTypeRequest model)
        {
            ResponseBaseCommon response = new ResponseBaseCommon()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };

            if (string.IsNullOrWhiteSpace(model.ParkingCode) ||
                string.IsNullOrWhiteSpace(model.TypeName) ||
                string.IsNullOrWhiteSpace(model.ProjectGuid))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "必要参数缺失,请检查";
                return Ok(response);
            }

            CarTypeModel content = new CarTypeModel()
            {
                ProjectGuid = model.ProjectGuid,
                ParkCode = model.ParkingCode,
                Guid = Guid.NewGuid().ToString("N"),
                CarType = model.PaymentMode,
                CarTypeName = model.TypeName,
                Enable = model.Enable,
                DefaultType = false 
            };

            //验证访问权限
            bool authState = _parkLotManager.ParkLotAccessPermission(model.ParkingCode, model.ProjectGuid);
            if (authState)
            {
                bool flag = _parkLotManager.AddNewCarType(content);
                if (!flag)
                {
                    response.IsSuccess = false;
                    response.MessageCode = (int)ApiParkLotErrorCode.API_DATA_SAVE_ERROR;
                    response.MessageContent = _parkLotManager.LastErrorDescribe;
                }
            }
            else
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_INTERFACENAME_ERROR;
                response.MessageContent = _parkLotManager.LastErrorDescribe;
            }
            return Ok(response);
        }

        /// <summary>
        /// 修改车类
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(ResponseBaseCommon))]
        public IHttpActionResult ModifyCarType(ModifyCarTypeRequest model)
        {
            ResponseBaseCommon response = new ResponseBaseCommon()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };

            if (string.IsNullOrWhiteSpace(model.Guid) ||
                string.IsNullOrWhiteSpace(model.ParkingCode) ||
                string.IsNullOrWhiteSpace(model.TypeName) ||
                string.IsNullOrWhiteSpace(model.ProjectGuid))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "必要参数缺失,请检查";
                return Ok(response);
            }

            CarTypeModel content = _parkLotManager.GetCarType(model.Guid);
            if (content != null)
            {
                if (content.ProjectGuid == model.ProjectGuid && content.ParkCode == model.ParkingCode)
                {
                    content.CarType = model.PaymentMode;
                    content.CarTypeName = model.TypeName;
                    content.Enable = model.Enable; 
                    bool flag = _parkLotManager.ModifyCarType(content);
                    if (!flag)
                    {
                        response.IsSuccess = false;
                        response.MessageCode = (int)ApiParkLotErrorCode.API_DATA_SAVE_ERROR;
                        response.MessageContent = _parkLotManager.LastErrorDescribe;
                    }
                }
                else
                {
                    response.IsSuccess = false;
                    response.MessageCode = (int)ApiBaseErrorCode.API_INTERFACENAME_ERROR;
                    response.MessageContent = ApiBaseErrorCode.API_INTERFACENAME_ERROR.ToString();
                }
            }
            else
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiParkLotErrorCode.API_DATA_NULL_ERROR;
                response.MessageContent = ApiParkLotErrorCode.API_DATA_NULL_ERROR.ToString();
            }
            return Ok(response);
        }

        /// <summary>
        /// 删除车类
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(ResponseBaseCommon))]
        public IHttpActionResult RemoveCarType(ModifyCarTypeRequest model)
        {
            ResponseBaseCommon response = new ResponseBaseCommon()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };

            if (string.IsNullOrWhiteSpace(model.ParkingCode) ||
                string.IsNullOrWhiteSpace(model.Guid) ||
                string.IsNullOrWhiteSpace(model.ProjectGuid))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "必要参数缺失,请检查";
                return Ok(response);
            }

            CarTypeModel content = _parkLotManager.GetCarType(model.Guid);
            if (content != null)
            {
                if (content.ProjectGuid == model.ProjectGuid && content.ParkCode == model.ParkingCode)
                {
                    bool flag = _parkLotManager.DeleteCarType(content);
                    if (!flag)
                    {
                        response.IsSuccess = false;
                        response.MessageCode = (int)ApiParkLotErrorCode.API_DATA_SAVE_ERROR;
                        response.MessageContent = _parkLotManager.LastErrorDescribe;
                    }
                }
                else
                {
                    response.IsSuccess = false;
                    response.MessageCode = (int)ApiBaseErrorCode.API_INTERFACENAME_ERROR;
                    response.MessageContent = ApiBaseErrorCode.API_INTERFACENAME_ERROR.ToString();
                }
            }
            else
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiParkLotErrorCode.API_DATA_NULL_ERROR;
                response.MessageContent = ApiParkLotErrorCode.API_DATA_NULL_ERROR.ToString();
            }
            return Ok(response);
        }

        /// <summary>
        /// 获取车类
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(ResponseBase<CarTypeModel>))]
        public IHttpActionResult GetCarType(string Guid)
        {
            ResponseBase<CarTypeModel> response = new ResponseBase<CarTypeModel>()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };

            if (string.IsNullOrWhiteSpace(Guid))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "车类标识不能为空,请检查";
                return Ok(response);
            }

            CarTypeModel content = _parkLotManager.GetCarType(Guid);
            response.Result = content;

            return Ok(response);
        }

        /// <summary>
        /// 获取车类列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(ResponseBaseList<CarTypeModel>))]
        public IHttpActionResult GetCarTypeList(string ParkingCode,string projectGuid)
        {
            ResponseBaseList<CarTypeModel> response = new ResponseBaseList<CarTypeModel>()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };

            if (string.IsNullOrWhiteSpace(ParkingCode))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "车场编码不能为空,请检查";
                return Ok(response);
            }

            List<CarTypeModel> content = _parkLotManager.AllCarType(ParkingCode, projectGuid);
            content.Sort((a, b) => a.Idx.CompareTo(b.Idx));
            response.Result = content;

            return Ok(response);
        }

        /// <summary>
        /// 设置默认车类
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ResponseType(typeof(ResponseBaseCommon))]
        public IHttpActionResult SetDefaultCarType(ModifyCarTypeRequest model)
        {
            ResponseBaseCommon response = new ResponseBaseCommon()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };

            if (string.IsNullOrWhiteSpace(model.ParkingCode) ||
                string.IsNullOrWhiteSpace(model.ProjectGuid))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "必要参数缺失,请检查";
                return Ok(response);
            }

            bool flag = _parkLotManager.SetDefaultCarType(model.Guid);
            if (!flag)
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiParkLotErrorCode.API_DATA_SAVE_ERROR;
                response.MessageContent = ApiParkLotErrorCode.API_DATA_SAVE_ERROR.ToString();
            }
            return Ok(response);
        }
        #endregion

        #region 收费
        /// <summary>
        /// 新增计费模板
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(ResponseBaseCommon))]
        public IHttpActionResult AddBillingTemplate(BillingTemplateRequest model)
        {
            ResponseBaseCommon response = new ResponseBaseCommon()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };

            if (string.IsNullOrWhiteSpace(model.ParkingCode) ||
                string.IsNullOrWhiteSpace(model.ProjectGuid) ||
                string.IsNullOrWhiteSpace(model.CarTypeGuid))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "必要参数缺失,请检查";
                return Ok(response);
            }

            BillingTemplateModel content = new BillingTemplateModel()
            {
                ProjectGuid = model.ProjectGuid,
                ParkCode = model.ParkingCode,
                CarTypeGuid = model.CarTypeGuid,
                ChargeMode=model.ChargeMode,
                TemplateJson=model.TemplateJson
            };

            //验证访问权限
            bool authState = _parkLotManager.ParkLotAccessPermission(model.ParkingCode, model.ProjectGuid);
            if (authState)
            {
                if (!_parkLotManager.AddNewBillingTemplate(content))
                {
                    response.IsSuccess = false;
                    response.MessageCode = (int)ApiParkLotErrorCode.API_DATA_SAVE_ERROR;
                    response.MessageContent = _parkLotManager.LastErrorDescribe;
                }
            }
            else
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_INTERFACENAME_ERROR;
                response.MessageContent = _parkLotManager.LastErrorDescribe;
            }

            return Ok(response);
        }
        
        /// <summary>
        /// 修改计费模板
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(ResponseBaseCommon))]
        public IHttpActionResult ModifyBillingTemplate(BillingTemplateRequest model)
        {
            ResponseBaseCommon response = new ResponseBaseCommon()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };

            if (string.IsNullOrWhiteSpace(model.ParkingCode) ||
                string.IsNullOrWhiteSpace(model.ProjectGuid) ||
                string.IsNullOrWhiteSpace(model.CarTypeGuid))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "必要参数缺失,请检查";
                return Ok(response);
            }
            BillingTemplateModel content = _parkLotManager.GetBillingTemplate(model.CarTypeGuid);
            if (content != null)
            {
                if (content.ProjectGuid == model.ProjectGuid && content.ParkCode == model.ParkingCode)
                {
                    content.ChargeMode = model.ChargeMode;
                    content.TemplateJson = model.TemplateJson;

                    if (!_parkLotManager.ModifyBillingTemplate(content))
                    {
                        response.IsSuccess = false;
                        response.MessageCode = (int)ApiParkLotErrorCode.API_DATA_SAVE_ERROR;
                        response.MessageContent = _parkLotManager.LastErrorDescribe;
                    }
                }
                else
                {
                    response.IsSuccess = false;
                    response.MessageCode = (int)ApiBaseErrorCode.API_INTERFACENAME_ERROR;
                    response.MessageContent = ApiBaseErrorCode.API_INTERFACENAME_ERROR.ToString();
                }
            }
            else
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiParkLotErrorCode.API_DATA_NULL_ERROR;
                response.MessageContent = ApiParkLotErrorCode.API_DATA_NULL_ERROR.ToString();
            }
            return Ok(response);
        }

        /// <summary>
        /// 删除计费模板
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(ResponseBaseCommon))]
        public IHttpActionResult RemoveBillingTemplate(BillingTemplateRequest model)
        {
            ResponseBaseCommon response = new ResponseBaseCommon()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };

            if (string.IsNullOrWhiteSpace(model.ParkingCode) ||
                string.IsNullOrWhiteSpace(model.ProjectGuid) ||
                string.IsNullOrWhiteSpace(model.CarTypeGuid))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "必要参数缺失,请检查";
                return Ok(response);
            }
            BillingTemplateModel content = _parkLotManager.GetBillingTemplate(model.CarTypeGuid);
            if (content != null)
            {
                if (content.ProjectGuid == model.ProjectGuid && content.ParkCode == model.ParkingCode)
                {
                    if (!_parkLotManager.DeleteBillingTemplate(content))
                    {
                        response.IsSuccess = false;
                        response.MessageCode = (int)ApiParkLotErrorCode.API_DATA_SAVE_ERROR;
                        response.MessageContent = _parkLotManager.LastErrorDescribe; ;
                    }
                }
                else
                {
                    response.IsSuccess = false;
                    response.MessageCode = (int)ApiBaseErrorCode.API_INTERFACENAME_ERROR;
                    response.MessageContent = ApiBaseErrorCode.API_INTERFACENAME_ERROR.ToString();
                }
            }
            else
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiParkLotErrorCode.API_DATA_NULL_ERROR;
                response.MessageContent = ApiParkLotErrorCode.API_DATA_NULL_ERROR.ToString();
            }
            return Ok(response);
        }

        /// <summary>
        /// 获取计费模板
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(ResponseBase<BillingTemplateModel>))]
        public IHttpActionResult GetBillingTemplate(string carTypeGuid)
        {
            ResponseBase<BillingTemplateModel> response = new ResponseBase<BillingTemplateModel>()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };

            BillingTemplateModel content = _parkLotManager.GetBillingTemplate(carTypeGuid);
            response.Result = content;

            return Ok(response);
        }

        /// <summary>
        /// 固定车延期模板设置
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(ResponseBaseCommon))]
        public IHttpActionResult SetPostponeTemplate(PermanentTemplateRequest model)
        {
            ResponseBaseCommon response = new ResponseBaseCommon()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };

            if (string.IsNullOrWhiteSpace(model.ParkingCode) ||
                string.IsNullOrWhiteSpace(model.ProjectGuid) ||
                string.IsNullOrWhiteSpace(model.CarTypeGuid))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "必要参数缺失,请检查";
                return Ok(response);
            }

            PermanentTemplateModel content = new PermanentTemplateModel()
            {
                ProjectGuid = model.ProjectGuid,
                ParkCode = model.ParkingCode,
                CarTypeGuid = model.CarTypeGuid,
                Months = 1,//默认固定一个月
                Amount = model.Amount,
                OperateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                OperateUser = model.OperateUser
            };

            if (!_parkLotManager.SetPermanentTemplate(content))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_FAIL;
                response.MessageContent = _parkLotManager.LastErrorDescribe;
            }

            return Ok(response);
        }

        /// <summary>
        /// 固定车延期模板移除
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(ResponseBaseCommon))]
        public IHttpActionResult RemovePostponeTemplate(PermanentTemplateRequest model)
        {
            ResponseBaseCommon response = new ResponseBaseCommon()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };

            if (string.IsNullOrWhiteSpace(model.ParkingCode) ||
                string.IsNullOrWhiteSpace(model.ProjectGuid) ||
                string.IsNullOrWhiteSpace(model.CarTypeGuid))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "必要参数缺失,请检查";
                return Ok(response);
            }

            PermanentTemplateModel content = new PermanentTemplateModel()
            {
                ProjectGuid = model.ProjectGuid,
                ParkCode = model.ParkingCode,
                CarTypeGuid = model.CarTypeGuid,
                OperateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                OperateUser = model.OperateUser
            };

            if (!_parkLotManager.DeletePermanentTemplate(content))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_FAIL;
                response.MessageContent = _parkLotManager.LastErrorDescribe;
            }

            return Ok(response);
        }

        /// <summary>
        /// 获取固定车延期模板
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(ResponseBase<PermanentTemplateModel>))]
        public IHttpActionResult GetPostponeTemplate(string CarTypeGuid)
        {
            ResponseBase<PermanentTemplateModel> response = new ResponseBase<PermanentTemplateModel>()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };

            if (string.IsNullOrWhiteSpace(CarTypeGuid))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "延期模板标识不能为空";
                return Ok(response);
            }

            PermanentTemplateModel content = _parkLotManager.GetPermanentTemplate(CarTypeGuid);
            response.Result = content;

            return Ok(response);
        }

        /// <summary>
        /// 获取固定车延期模板列表
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(ResponseBaseList<PermanentTemplateResponse>))]
        public IHttpActionResult GetPostponeTemplateList(string ParkingCode)
        {
            ResponseBaseList<PermanentTemplateResponse> response = new ResponseBaseList<PermanentTemplateResponse>()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };

            if (string.IsNullOrWhiteSpace(ParkingCode))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "停车场编码不能为空";
                return Ok(response);
            }

            List<PermanentTemplateModel> content = _parkLotManager.AllPermanentTemplate(ParkingCode);
            response.Result = new List<PermanentTemplateResponse>();
            foreach (var item in content) {
                response.Result.Add(new PermanentTemplateResponse() {
                    ProjectGuid = item.ProjectGuid,
                    ParkCode = item.ParkCode,
                    CarTypeGuid = item.CarTypeGuid,
                    CarTypeName = _parkLotManager.GetCarType(item.CarTypeGuid)?.CarTypeName,
                    Months=item.Months,
                    Amount=item.Amount,
                    OperateTime=item.OperateTime,
                    OperateUser=item.OperateUser
                });
            }

            return Ok(response);
        }
        #endregion

        #region 其它
        /// <summary>
        /// 获取其它设置页面数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(ResponseBase<FunctionPointModel>))]
        public IHttpActionResult GetFunctionPoint(string ParkingCode)
        {
            ResponseBase<FunctionPointModel> response = new ResponseBase<FunctionPointModel>()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };

            if (string.IsNullOrWhiteSpace(ParkingCode))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "停车场编码不能为空";
                return Ok(response);
            }

            ParkLotModel parklotmodel = _parkLotManager.GetParkLot(ParkingCode);
            if (parklotmodel != null)
            {
                FunctionPointModel content = _parkLotManager.GetFunctionPoint(parklotmodel.ProjectGuid, ParkingCode);
                response.Result = content;
            }
            else
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "无效的停车场编码";
            }

            return Ok(response);
        }

        /// <summary>
        /// 修改其他设置页面数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ResponseType(typeof(ResponseBaseCommon))]
        public IHttpActionResult SetFunctionPoint(FunctionPointRequest model)
        {
            ResponseBaseCommon response = new ResponseBaseCommon()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };

            try
            {
                FunctionPointModel content = _parkLotManager.GetFunctionPoint(model.ProjectGuid, model.ParkingCode);
                if (content != null)
                {
                    if (content.ProjectGuid == model.ProjectGuid)
                    {
                        content.PostponeMode = model.PostponeMode;
                        content.PastDueMode = model.PastDueMode;
                        content.BarredEntryCarTypeOnParkingFull = model.BarredEntryCarTypeOnParkingFull;
                        content.RemainingSpaceCarType = model.RemainingSpaceCarType;
                        content.BluePlateCarTypeGuid = model.BluePlateCarTypeGuid;
                        content.YellowPlateCarTypeGuid = model.YellowPlateCarTypeGuid;
                        content.WhitePlateCarTypeGuid = model.WhitePlateCarTypeGuid;
                        content.GreenPlateCarTypeGuid = model.GreenPlateCarTypeGuid;
                        content.BlackPlateCarTypeGuid = model.BlackPlateCarTypeGuid;
                        content.MinDays = model.MinDays;
                        content.MinBalance = model.MinBalance;
                        content.ManualOpenGateGuid = model.ManualOpenGateGuid;

                        bool flag = _parkLotManager.SetFunctionPoint(content);
                        if (!flag)
                        {
                            response.IsSuccess = false;
                            response.MessageCode = (int)ApiParkLotErrorCode.API_DATA_SAVE_ERROR;
                            response.MessageContent = _parkLotManager.LastErrorDescribe;
                        }
                    }
                    else
                    {
                        response.IsSuccess = false;
                        response.MessageCode = (int)ApiBaseErrorCode.API_INTERFACENAME_ERROR;
                        response.MessageContent = ApiBaseErrorCode.API_INTERFACENAME_ERROR.ToString();
                    }
                }
                else
                {
                    response.IsSuccess = false;
                    response.MessageCode = (int)ApiParkLotErrorCode.API_DATA_NULL_ERROR;
                    response.MessageContent = ApiParkLotErrorCode.API_DATA_NULL_ERROR.ToString();
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = ApiBaseErrorCode.API_PARAM_ERROR.ToString();
                Logger.LogFatal(LoggerLogicEnum.Interface, "", model.ParkingCode, "", model.ProjectGuid, "设置固定车延期方式时发生异常", ex.ToString());
            }
            return Ok(response);
        }

        #endregion

        #region 进出
        /// <summary>
        /// 重新拍照
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(ResponseBaseCommon))]
        public IHttpActionResult PhotographCar(DeviceRequest model)
        {
            //考虑用异步的接口，然后实现返回拍照后的图片地址
            ResponseBaseCommon response = new ResponseBaseCommon()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };

            if (string.IsNullOrWhiteSpace(model.ParkingCode) ||
                string.IsNullOrWhiteSpace(model.DeviceMacAddress)  
               ||  string.IsNullOrWhiteSpace(model.ProjectGuid))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "必要参数缺失,请检查";
                return Ok(response);
            }

            if (!_parkLotManager.Photograph(model.ParkingCode, model.DeviceMacAddress))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_FAIL;
                response.MessageContent = ApiBaseErrorCode.API_FAIL.ToString();
            }
            return Ok(response);
        }


        /// <summary>
         /// 设置车道闸口锁定
         /// </summary>
         /// <returns></returns>
        [HttpPost]
        [ResponseType(typeof(ResponseBaseCommon))]
        public IHttpActionResult SetGateKeep(GateKeepListRequest model)
        {
            ResponseBaseCommon response = new ResponseBaseCommon()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };

            if (string.IsNullOrWhiteSpace(model.ParkingCode) ||                
                string.IsNullOrWhiteSpace(model.ProjectGuid) ||
                model.GateKeepList == null)
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "必要参数缺失,请检查";
                return Ok(response);
            }

            GateKeepListModel content = new GateKeepListModel();
            content.ProjectGuid = model.ProjectGuid;
            content.ParkingCode = model.ParkingCode;
            content.List = new List<GateKeepModel>();
            foreach (var item in model.GateKeepList)
            {
                content.List.Add(new GateKeepModel
                {
                    DrivewayGuid = item.DrivewayGuid,
                    DeviceMacAddress = item.DeviceMacAddress,
                    GateState = item.GateState                                     
                });
            }
            if (!_parkLotManager.SetGateKeep(content))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_FAIL;
                response.MessageContent = ApiBaseErrorCode.API_FAIL.ToString();
            }
            return Ok(response);
        }

        /// <summary>
        /// 获取车道闸口锁定
        /// </summary>
        /// <param name="projectGuid">项目guid</param>
        /// <param name="parkingCode">停车场编码</param>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(ResponseBase<GateKeepListModel>))]
        public IHttpActionResult GetGateKeep(string projectGuid, string parkingCode)
        {
            ResponseBase<GateKeepListModel> response = new ResponseBase<GateKeepListModel>()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };

            if (string.IsNullOrWhiteSpace(parkingCode) || string.IsNullOrWhiteSpace(projectGuid))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "必要参数缺失,请检查";
                return Ok(response);
            }

            GateKeepListModel content = _parkLotManager.GetGateKeep(parkingCode);

            if (content != null && content.ProjectGuid == projectGuid)
            {
                response.Result = content;
            }
            else
            {
                response.Result = new GateKeepListModel()
                {
                    ProjectGuid = projectGuid,
                    ParkingCode = parkingCode,
                    List = new List<GateKeepModel>()
                };
            }

            return Ok(response);
        }


        ///// <summary>
        ///// 获取选择车道半小时内经过的车辆列表/carNo参数可选填
        ///// </summary>
        ///// <param name="drivewayguid"></param>
        ///// <param name="ParkingCode"></param>
        ///// <param name="carNo">可选填参数</param>
        ///// <returns></returns>
        //[HttpGet]
        //[ResponseType(typeof(ResponseBaseList<VehicleInOutModel>))]
        //public IHttpActionResult GetDriveWayHalfHourCars(string drivewayguid, string ParkingCode,string carNo = null)
        //{
        //    ResponseBaseList<VehicleInOutModel> response = new ResponseBaseList<VehicleInOutModel>()
        //    {
        //        IsSuccess = true,
        //        MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
        //        MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
        //    };

        //    if (string.IsNullOrWhiteSpace(drivewayguid))
        //    {
        //        response.IsSuccess = false;
        //        response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
        //        response.MessageContent = "必要参数缺失,请检查";
        //        return Ok(response);
        //    }
        //    response.Result = _parkLotManager.DriveWayHalfHourCarsRecord(drivewayguid,ParkingCode,carNo);
        //    return Ok(response);
        //}

        /// <summary>
        /// 根据车场编号和车牌号码获取当前车辆的停车费用信息
        /// </summary>
        /// <param name="parkingCode">停车场编码</param>
        /// <param name="carNumber">车牌号码</param>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(ResponseBase<TempCarParkingModel>))]
        public IHttpActionResult GetTempCarParkingInfo(string parkingCode,string carNumber)
        {
            ResponseBase<TempCarParkingModel> response = new ResponseBase<TempCarParkingModel>()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };

            TempCarParkingModel responseModel = null;  
            responseModel = _cardServiceManager.GetTempCarParkingInfo(parkingCode, carNumber); 
            response.Result = responseModel; 
            return Ok(response);
        }

        /// <summary>
        /// 车牌修改重推
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(ResponseBaseCommon))]
        public IHttpActionResult CarNumberRepush(CarNumberRepushRequest model)
        {
            ResponseBaseCommon response = new ResponseBaseCommon()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };

            if (string.IsNullOrWhiteSpace(model.ParkingCode) ||
                string.IsNullOrWhiteSpace(model.DriveWayMAC) ||
                string.IsNullOrWhiteSpace(model.OldCarNo) ||
                string.IsNullOrWhiteSpace(model.NewCarNo))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "必要参数缺失,请检查";
                return Ok(response);
            }
            ////读取车辆的在场记录
            //VehicleInOutModel entermodel = _parkLotManager.GetEntryRecord(model.OldCarNo, model.ParkingCode);
            //if (entermodel == null)
            //{
            //    response.IsSuccess = false;
            //    response.MessageCode = (int)ApiBaseErrorCode.API_FAIL;
            //    response.MessageContent = "未找到当前车辆在场记录";
            //    return Ok(response);
            //}
            //封装 修正车牌模型实体
            CorrectCarnoModel carnoMode = new CorrectCarnoModel();
            carnoMode.ProjectGuid = model.ProjectGuid;
            carnoMode.DeviceIdentify = model.DriveWayMAC;
            carnoMode.ParkingCode = model.ParkingCode;
            carnoMode.OldCarno = model.OldCarNo;
            carnoMode.NewCarno = model.NewCarNo;
            carnoMode.InTime = DateTime.Now;  
            carnoMode.ImgUrl = model.ImgUrl;
            carnoMode.OperationType = 1;
            carnoMode.Operator = model.Operator; 
            //将修改车牌重推数据通过mq交给相机去处理业务流程(出口相机按照新车牌重新进行推送，走正常业务)
            bool correctResult = _parkLotManager.CarNumberRepushToCamera(carnoMode); 
            return Ok(response);
        }

        /// <summary>
        /// 车道拦截数据刷新
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(ResponseBase<GateCatchDetailModel>))]
        public IHttpActionResult GateCatchRefresh(string deviceMacAddress)
        {
            ResponseBase<GateCatchDetailModel> response = new ResponseBase<GateCatchDetailModel>()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };

            if (string.IsNullOrWhiteSpace(deviceMacAddress))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "必要参数缺失,请检查";
                return Ok(response);
            }

            GateCatchDetailModel content = _parkLotManager.GetGateCatch(deviceMacAddress);

            if (content != null)
            {
                response.Result = content;
            }

            return Ok(response);
        }

        /// <summary>
        /// 获取车道车辆进出抓拍数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(ResponseBase<CaptureInOutModel>))]
        public IHttpActionResult GetGateData(string parkingCode, string deviceMacAddress)
        {
            ResponseBase<CaptureInOutModel> response = new ResponseBase<CaptureInOutModel>()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };

            if (string.IsNullOrWhiteSpace(parkingCode) || string.IsNullOrWhiteSpace(deviceMacAddress))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "必要参数缺失,请检查";
                return Ok(response);
            }

            CaptureInOutModel content = _parkLotManager.GetGateData(parkingCode, deviceMacAddress);

            if (content != null)
            {
                response.Result = content;
            }

            return Ok(response);
        }

        /// <summary>
        /// 获取储值车停车费信息
        /// </summary>  
        /// <returns></returns>
        [HttpPost]
        [ResponseType(typeof(ResponseBase<ValueCardFeeModel>))]
        public IHttpActionResult GetValueCardFeeInfoByCarNo(ValueCardFeeRequest model)
        {
            ResponseBase<ValueCardFeeModel> response = new ResponseBase<ValueCardFeeModel>()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };
            if (model == null || string.IsNullOrWhiteSpace(model.ParkCode) ||
                string.IsNullOrWhiteSpace(model.CarNo))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "必要参数缺失,请检查";
                return Ok(response);
            }
            ValueCardFeeModel responseModel = null; 
            //判断车辆是否在场
            VehicleInOutModel entryModel = _parkLotManager.GetEntryRecord(model.CarNo, model.ParkCode); 
            if (entryModel != null)
            {
                ValueCardFeeModel valuecardfeemodel = new ValueCardFeeModel();
                valuecardfeemodel.ParkingCode = model.ParkCode;
                valuecardfeemodel.CarNo = model.CarNo;
                valuecardfeemodel.Remarks = model.ErrorCode;
                responseModel = _cardServiceManager.GetValueCardFeeInfoByCarNo(valuecardfeemodel); 
            }
            else
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_FAIL;
                response.MessageContent = "未找到该车辆的入场信息";
            }
            response.Result = responseModel;

            return Ok(response);
        }

        /// <summary>
        ///  执行储值车扣费
        /// </summary>  
        /// <returns></returns>
        [HttpPost] 
        [ResponseType(typeof(ResponseBase<ValueCardFeeModel>))]
        public IHttpActionResult ValueCardCalculationFee(ValueCardCalculationFeeRequest model)
        {
            ResponseBase<ValueCardFeeModel> response = new ResponseBase<ValueCardFeeModel>()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };
            if (model == null || string.IsNullOrWhiteSpace(model.ParkCode) ||
                string.IsNullOrWhiteSpace(model.CarNo))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "必要参数缺失,请检查";
                return Ok(response);
            }
            ValueCardFeeModel responseModel = null; //停车费用信息 
            VehicleInOutModel entryModel = _parkLotManager.GetEntryRecord(model.CarNo, model.ParkCode); //判断车辆是否在场 
            if (entryModel != null)
            {   
                ValueCardFeeModel valuecardfeemodel = new ValueCardFeeModel();
                valuecardfeemodel.ParkingCode = model.ParkCode;
                valuecardfeemodel.CarNo = model.CarNo;
                //获取储值车停车费用信息
                responseModel = _cardServiceManager.GetValueCardFeeInfoByCarNo(valuecardfeemodel);
                if (responseModel != null)
                {
                    if (responseModel.Balance < responseModel.ParkingFee)
                    {
                        responseModel.IsPaid = false;
                        response.IsSuccess = true;
                        response.MessageCode = (int)ApiBaseErrorCode.API_SUCCESS;
                        response.MessageContent = "余额不足,请充值";
                    } 
                    else
                    {
                        //执行扣费
                        CardServiceModel cardservicemodel = new CardServiceModel()
                        {
                            ParkCode= responseModel.ParkingCode,
                            CarNo= responseModel.CarNo,
                            PayAmount= responseModel.ParkingFee,
                            AdmissionDate=Convert.ToDateTime(responseModel.BeginTime) 
                        };
                         bool flag= _cardServiceManager.DoValueCardFee(cardservicemodel, responseModel.LineRecordCode);
                        if(flag)
                        {
                            responseModel.IsPaid = true;
                            responseModel.Balance -= responseModel.ParkingFee; 
                        }
                        else
                        {
                            responseModel.IsPaid = false; 
                        }
                    }
                }
            }
            else
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_FAIL;
                response.MessageContent = "未找到该车辆的入场信息";
            }
            response.Result = responseModel;

            return Ok(response);
        } 
        #endregion

        #region 在场
        /// <summary>
        /// 车牌修正
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(ResponseBaseCommon))]
        public IHttpActionResult CorrectCarNo(ModifyCarNoRequest model)
        {
            //考虑用异步的接口，然后实现返回拍照后的图片地址
            ResponseBaseCommon response = new ResponseBaseCommon()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };

            if (string.IsNullOrWhiteSpace(model.ParkingCode) ||
                string.IsNullOrWhiteSpace(model.OldCarNo) ||
                string.IsNullOrWhiteSpace(model.NewCarNo) ||
                string.IsNullOrWhiteSpace(model.ProjectGuid))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "必要参数缺失,请检查";
                return Ok(response);
            }
            //读取车辆的在场记录
            VehicleInOutModel entermodel = _parkLotManager.GetEntryRecord(model.OldCarNo, model.ParkingCode);
            if (entermodel == null)
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_FAIL;
                response.MessageContent = "未找到原车辆在场记录";
                return Ok(response);
            }

            CarTypeModel carTypeModel = _parkLotManager.GetCarType(entermodel.CarTypeGuid);
            if (carTypeModel == null)
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_FAIL;
                response.MessageContent = "当前车类不存在";
                return Ok(response);
            }

            //封装 修正车牌模型实体
            CorrectCarnoModel correctModel = new CorrectCarnoModel();
            correctModel.ProjectGuid = model.ProjectGuid;
            correctModel.DeviceIdentify = entermodel.DriveWayMAC;
            correctModel.ParkingCode = model.ParkingCode;
            correctModel.OldGuid = entermodel.Guid;
            correctModel.OldCarno = entermodel.CarNo;
            correctModel.NewCarno = model.NewCarNo;
            correctModel.ImgUrl = entermodel.ImgUrl;
            correctModel.InTime = entermodel.EventTime;
            correctModel.CarTypeGuid = entermodel.CarTypeGuid;
            correctModel.CarType = carTypeModel.CarType;
            correctModel.Remark = $"管理后台车牌修正：原车牌{correctModel.OldCarno}改为{correctModel.NewCarno}";
            correctModel.ThroughName = entermodel.Entrance;
            correctModel.OperationType = 0;
            correctModel.Operator = model.Operator;
            //将修正车牌数据通过mq交给相机去处理业务流程(新车牌入场、旧车牌出场)
            bool correctResult = _parkLotManager.CorrectToEntryCamera(correctModel); 
            return Ok(response);
        }

        ///// <summary>
        ///// 在场车辆
        ///// </summary>
        ///// <param name="Base64args">base64编码的参数 参数格式{"ProjectGuid":"","ParkingCode":"","PageIndex":"","PageSize":"","CarNo":"","CarTypeGuid":"","MinInTime":"","MaxInTime":""}</param>
        ///// <returns></returns>
        //[HttpGet]
        //[ResponseType(typeof(ResponseBaseList<PresenceOfVehicleModel>))]
        //public IHttpActionResult PresenceOfVehicle(string Base64args)
        //{
        //    //编码前的地址{"ProjectGuid":"","ParkingCode":"","PageIndex":,"PageSize":,"CarNo":"","CarTypeGuid":"","MinInTime":"","MaxInTime":""}
        //    ResponseBaseList<PresenceOfVehicleModel> response = new ResponseBaseList<PresenceOfVehicleModel>()
        //    {
        //        IsSuccess = true,
        //        MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
        //        MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString(),
        //        Result = new List<PresenceOfVehicleModel>()
        //    };
        //    try
        //    {
        //        string jsonstr = HttpUtility.UrlDecode(Encoding.UTF8.GetString(Convert.FromBase64String(Base64args)));
        //        Dictionary<string, object> dicparam = Serializer.Deserialize<Dictionary<string, object>>(jsonstr);
        //        //必有
        //        string ProjectGuid = dicparam["ProjectGuid"].ToString();
        //        string ParkingCode = dicparam["ParkingCode"].ToString();
        //        int PageIndex = int.Parse(dicparam["PageIndex"].ToString());
        //        int PageSize = int.Parse(dicparam["PageSize"].ToString());
        //        //可选
        //        string CarNo = dicparam.ContainsKey("CarNo") ? dicparam["CarNo"].ToString() : "";
        //        string CarTypeGuid = dicparam.ContainsKey("CarTypeGuid") ? dicparam["CarTypeGuid"].ToString() : "";
        //        DateTime MinInTime = dicparam.ContainsKey("MinInTime") ? (dicparam["MinInTime"].ToString() == "" ? default(DateTime) : DateTime.Parse(dicparam["MinInTime"].ToString())) : default(DateTime);
        //        DateTime MaxInTime = dicparam.ContainsKey("MaxInTime") ? (dicparam["MaxInTime"].ToString() == "" ? default(DateTime) : DateTime.Parse(dicparam["MaxInTime"].ToString())) : default(DateTime);

        //        if (string.IsNullOrWhiteSpace(ParkingCode) ||
        //            PageIndex <= 0 || PageSize <= 0)
        //        {
        //            response.IsSuccess = false;
        //            response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
        //            response.MessageContent = "必要参数格式不对，请检查";
        //            return Ok(response);
        //        }
        //        ParkLotModel parklot = _parkLotManager.GetParkLot(ParkingCode);
        //        if (parklot != null)
        //        {
        //            //校验
        //            if (parklot.ProjectGuid == ProjectGuid)
        //            {
        //                List<PresenceOfVehicleModel> content = _parkLotManager.AllPresenceOfVehicle(ParkingCode);
        //                //筛选
        //                if (!string.IsNullOrWhiteSpace(CarNo)) content = content.FindAll(o => o.CarNo.Contains(CarNo));
        //                if (!string.IsNullOrWhiteSpace(CarTypeGuid)) content = content.FindAll(o => o.CarTypeGuid == CarTypeGuid);
        //                if (default(DateTime) != MinInTime) content = content.FindAll(o => o.InTime > MinInTime);
        //                if (default(DateTime) != MaxInTime) content = content.FindAll(o => o.InTime < MaxInTime);
        //                //分页跳过
        //                content = content.Skip((PageIndex - 1) * PageSize).ToList();
        //                //拿取一页
        //                content = content.Take(PageSize).ToList();

        //                response.Result = content;
        //            }
        //            else
        //            {
        //                response.IsSuccess = false;
        //                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
        //                response.MessageContent = "项目编码校验失败";
        //            }
        //        }
        //        else
        //        {
        //            response.IsSuccess = false;
        //            response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
        //            response.MessageContent = "无该车场信息";
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        response.IsSuccess = false;
        //        response.MessageCode = (int)ApiBaseErrorCode.API_FAIL;
        //        response.MessageContent = "请检查参数格式是否符合";
        //    }
        //    return Ok(response);
        //}

        /// <summary>
        /// 入场补录
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(ResponseBaseCommon))]
        public IHttpActionResult AddInRecord(AddInRecordRequest model)
        {
            ResponseBaseCommon response = new ResponseBaseCommon()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };

            if (string.IsNullOrWhiteSpace(model.ParkingCode) ||
                string.IsNullOrWhiteSpace(model.CarNo) ||
                string.IsNullOrWhiteSpace(model.DrivewayGuid) ||
                string.IsNullOrWhiteSpace(model.CarTypeGuid) ||
                (model.InTime == default(DateTime)) ||
                string.IsNullOrWhiteSpace(model.ImgUrl) ||
                string.IsNullOrWhiteSpace(model.ProjectGuid))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "必要参数缺失,请检查";
                return Ok(response);
            }

            if (model.InTime > DateTime.Now)
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "入场时间不能大于当前时间";
                return Ok(response);
            }

            DrivewayModel drivewayModel = _parkLotManager.GetDriveway(model.DrivewayGuid);
            if (drivewayModel == null)
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "车道不存在，请检查";
                return Ok(response);
            }
            CarTypeModel carTypeModel = _parkLotManager.GetCarType(model.CarTypeGuid);
            if (carTypeModel == null)
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "车类不存在，请检查";
                return Ok(response);
            }

            AddRecordModel recordModel = new AddRecordModel();
            recordModel.DeviceIdentify = drivewayModel.DeviceMacAddress;
            recordModel.Guid = Guid.NewGuid().ToString("N");
            recordModel.ParkingCode = model.ParkingCode;
            recordModel.CarNo = model.CarNo;
            recordModel.CarTypeGuid = model.CarTypeGuid;
            recordModel.ImgUrl = model.ImgUrl;
            recordModel.InTime = model.InTime;
            recordModel.Remark = "管理后台手工补录车辆入场";
            recordModel.CarType = carTypeModel.CarType;
            recordModel.ProjectGuid = model.ProjectGuid;
            recordModel.Entrance = model.Entrance;
            recordModel.Operator = model.Operator;
            bool addResult = _parkLotManager.RecordInToEntryCamera(recordModel);
            if (!addResult)
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiParkLotErrorCode.API_DATA_SAVE_ERROR;
                response.MessageContent = _parkLotManager.LastErrorDescribe;
            }
            return Ok(response);
        }

        /// <summary>
        /// 在场车辆删除
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(ResponseBaseCommon))]
        public IHttpActionResult InVehicleDelete(InVehicleDeleteRequest requestModel)
        {
            ResponseBaseCommon response = new ResponseBaseCommon()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            }; 
            if (  string.IsNullOrWhiteSpace(requestModel.CarNo) || 
                string.IsNullOrWhiteSpace(requestModel.Operator) || 
                string.IsNullOrWhiteSpace(requestModel.ImgUrl)  )
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "必要参数缺失,请检查";
                return Ok(response);
            }
            //读取车辆的在场记录
            VehicleInOutModel entermodel = _parkLotManager.GetEntryRecord(requestModel.CarNo, requestModel.ParkingCode);
            if (entermodel == null)
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_FAIL;
                response.MessageContent = "未找到原车辆在场记录";
                return Ok(response);
            }
            InVehicleDeleteModel deleteModel = new InVehicleDeleteModel()
            {
                RecordGuid = requestModel.RecordGuid,
                CarNo= requestModel.CarNo,
                DeviceMACAddress= entermodel.DriveWayMAC,
                ImgUrl= requestModel.ImgUrl,
                Operator= requestModel.Operator,
                Remark= "删除在场车辆",
                CarTypeGuid=entermodel.CarTypeGuid,
                OpenType = OpenTypeEnum.FieldDelete
            };
            bool delResult = _parkLotManager.InVehicleDelete(deleteModel, requestModel.ParkingCode);
            if (!delResult)
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiParkLotErrorCode.API_DATA_SAVE_ERROR;
                response.MessageContent = _parkLotManager.LastErrorDescribe;
            }
            return Ok(response);
        } 

        #endregion

        #region 黑名单
        /// <summary>
        /// 添加黑名单
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(ResponseBaseCommon))]
        public IHttpActionResult AddBlacklist(BlacklistRequest model)
        {
            //考虑用异步的接口，然后实现返回拍照后的图片地址
            ResponseBaseCommon response = new ResponseBaseCommon()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };
            string[] carnos = (model.CarNoList ?? "").Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            if (string.IsNullOrWhiteSpace(model.ParkingCode) ||
                carnos == null ||
                carnos.Count() <= 0 ||
                string.IsNullOrWhiteSpace(model.ProjectGuid))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "必要参数缺失,请检查";
                return Ok(response);
            }

            BlacklistModel content = new BlacklistModel()
            {
                ProjectGuid = model.ProjectGuid,
                ParkCode = model.ParkingCode,
                List = new List<BlacklistSingleModel>()
            };
            
            foreach (string carno in carnos)
            {
                if (!string.IsNullOrWhiteSpace(carno))
                {
                    BlacklistSingleModel innercontent = new BlacklistSingleModel()
                    {
                        CarNo = carno,
                        Enable = model.Enable,
                        Always = model.Always,
                        ByDate = model.ByDate,
                        StartDate = string.Format("{0:d}", model.StartDate),
                        EndDate = string.Format("{0:d}", model.EndDate),
                        ByTime = model.ByTime,
                        StatrtTime = model.StatrtTime,
                        EndTime = model.EndTime,
                        ByWeek = model.ByWeek,
                        AssignDay = model.AssignDay,
                        Remark = model.Remark
                    };
                    content.List.Add(innercontent);
                }
            }

            if (!_parkLotManager.AddNewBlacklist(content))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_FAIL;
                response.MessageContent = _parkLotManager.LastErrorDescribe;
            }
            return Ok(response);
        }

        /// <summary>
        /// 修改黑名单
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(ResponseBaseCommon))]
        public IHttpActionResult ModifyBlacklist(BlacklistRequest model)
        {
            ResponseBaseCommon response = new ResponseBaseCommon()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };

            if (string.IsNullOrWhiteSpace(model.ParkingCode) ||
                model.CarNoList == null ||
                string.IsNullOrWhiteSpace(model.ProjectGuid))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "必要参数缺失,请检查";
                return Ok(response);
            }

            BlacklistModel content = _parkLotManager.GetBlacklist(model.ParkingCode);
            if (content != null)
            {
                if (content.ProjectGuid == model.ProjectGuid && content.ParkCode == model.ParkingCode)
                {
                    string[] carnos = (model.CarNoList ?? "").Split(',');
                    if (carnos.Length > 0)
                    {
                        BlacklistSingleModel innercontent = content.List.FirstOrDefault(o => o.CarNo == carnos[0]);
                        if (innercontent != null)
                        {
                            content.List.RemoveAll(o => o.CarNo != carnos[0]);

                            innercontent.Enable = model.Enable;
                            innercontent.Always = model.Always;
                            innercontent.ByDate = model.ByDate;
                            innercontent.StartDate = string.Format("{0:d}", model.StartDate);
                            innercontent.EndDate = string.Format("{0:d}", model.EndDate);
                            innercontent.ByTime = model.ByTime;
                            innercontent.StatrtTime = model.StatrtTime;
                            innercontent.EndTime = model.EndTime;
                            innercontent.ByWeek = model.ByWeek;
                            innercontent.AssignDay = model.AssignDay;
                            innercontent.Remark = model.Remark;

                            bool flag = _parkLotManager.ModifyBlacklist(content);
                            if (!flag)
                            {
                                response.IsSuccess = false;
                                response.MessageCode = (int)ApiParkLotErrorCode.API_DATA_SAVE_ERROR;
                                response.MessageContent = _parkLotManager.LastErrorDescribe;
                            }
                        }else
                        {
                            response.IsSuccess = false;
                            response.MessageCode = (int)ApiParkLotErrorCode.API_DATA_NULL_ERROR;
                            response.MessageContent = "无效的车牌请求";
                        }
                    }else
                    {
                        response.IsSuccess = false;
                        response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                        response.MessageContent = "车牌参数缺失,请检查";
                    }
                }
                else
                {
                    response.IsSuccess = false;
                    response.MessageCode = (int)ApiBaseErrorCode.API_INTERFACENAME_ERROR;
                    response.MessageContent = ApiBaseErrorCode.API_INTERFACENAME_ERROR.ToString();
                }
            }
            else
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiParkLotErrorCode.API_DATA_NULL_ERROR;
                response.MessageContent = ApiParkLotErrorCode.API_DATA_NULL_ERROR.ToString();
            }
            return Ok(response);
        }

        /// <summary>
        /// 从黑名单列表移除某黑名单
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(ResponseBaseCommon))]
        public IHttpActionResult RemoveCarNoInBlacklist(BlacklistRemoveRequest model)
        {
            ResponseBaseCommon response = new ResponseBaseCommon()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.GetRemark()
            };

            if (string.IsNullOrWhiteSpace(model.ParkingCode) ||
                model.CarNo == null ||
                string.IsNullOrWhiteSpace(model.ProjectGuid))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "必要参数缺失,请检查";
                return Ok(response);
            }

            BlacklistModel content = _parkLotManager.GetBlacklist(model.ParkingCode);
            if (content != null)
            {
                if (content.ProjectGuid == model.ProjectGuid && content.ParkCode == model.ParkingCode)
                {
                    BlacklistSingleModel blacklistsingle = content.List.SingleOrDefault(o => o.CarNo == model.CarNo);
                    if (blacklistsingle != null)
                    {
                        content.List.RemoveAll(m => m.CarNo != model.CarNo);
                        if (!_parkLotManager.DeleteBlacklist(content))
                        {
                            response.IsSuccess = false;
                            response.MessageCode = (int)ApiBaseErrorCode.API_FAIL;
                            response.MessageContent = _parkLotManager.LastErrorDescribe;
                        }
                        else
                        {
                            response.IsSuccess = true;
                            response.MessageCode = (int)ApiBaseErrorCode.API_SUCCESS;
                            response.MessageContent = ApiBaseErrorCode.API_SUCCESS.GetRemark();
                        }
                    }
                    else
                    {
                        response.IsSuccess = false;
                        response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                        response.MessageContent = ApiBaseErrorCode.API_PARAM_ERROR.GetRemark();
                    }
                }
                else
                {
                    response.IsSuccess = false;
                    response.MessageCode = (int)ApiBaseErrorCode.API_INTERFACENAME_ERROR;
                    response.MessageContent = ApiBaseErrorCode.API_INTERFACENAME_ERROR.GetRemark();
                }
            }
            else
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiParkLotErrorCode.API_DATA_NULL_ERROR;
                response.MessageContent = ApiParkLotErrorCode.API_DATA_NULL_ERROR.GetRemark();
            }
            return Ok(response);
        }

        /// <summary>
        /// 获取黑名单
        /// </summary>
        /// <param name="Base64args">base64编码的参数 参数格式{"ProjectGuid":"","ParkingCode":"","CarNo":"","Type":0=永久时间 1=自定义时间}</param>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(ResponseBase<BlacklistModel>))]
        public IHttpActionResult GetBlacklist(string Base64args)
        {
            ResponseBase<BlacklistModel> response = new ResponseBase<BlacklistModel>()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };
            string jsonstr = HttpUtility.UrlDecode(Encoding.UTF8.GetString(Convert.FromBase64String(Base64args)));
            Dictionary<string, object> dicparam = Serializer.Deserialize<Dictionary<string, object>>(jsonstr);
            
            string ProjectGuid = dicparam["ProjectGuid"].ToString();
            string ParkingCode = dicparam["ParkingCode"].ToString();
            string CarNo = dicparam["CarNo"].ToString();
            int Type = int.Parse(dicparam["Type"].ToString());

            if (string.IsNullOrWhiteSpace(ParkingCode))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "停车场编码不能为空";
                return Ok(response);
            }

            BlacklistModel content = _parkLotManager.GetBlacklist(ParkingCode);
            if (content != null)
            {
                if (Type == 0) content.List.RemoveAll(o => o.Always);
                if (Type == 1) content.List.RemoveAll(o => !o.Always); // content.FindAll(o => o.CarNo.Contains(CarNo));
                if (!string.IsNullOrWhiteSpace(CarNo)) content.List=content.List.FindAll(o => o.CarNo.Contains(CarNo.ToUpper().ToString()));
                response.Result = content;
            }else
            {
                response.Result = new BlacklistModel()
                {
                    ProjectGuid = ProjectGuid,
                    ParkCode = ParkingCode,
                    List = new List<BlacklistSingleModel>()
                };
            }

            return Ok(response);
        }
        #endregion


        #region 主扫、被扫

        /// <summary>
        /// 主扫
        /// 用户扫描固定车道码缴费
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>\
        [HttpPost]
        [ResponseType(typeof(ResponseBase<ActiveScanningResponseModel>))]
        public IHttpActionResult ActiveScanning(ActiveScanningRequest model)
        {
            ResponseBase<ActiveScanningResponseModel> response = new ResponseBase<ActiveScanningResponseModel>()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString(),
            };

            if (model == null ||
                string.IsNullOrWhiteSpace(model.DeviceMACAddress) ||
                string.IsNullOrWhiteSpace(model.Guid) ||
                string.IsNullOrWhiteSpace(model.CarNo) ||
                model.LaneSenseDate == null ||
                model.LaneSenseDate == DateTime.MinValue)
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "必要参数缺失,请检查";
                return Ok(response);
            }

            ActiveScanningModel content = new ActiveScanningModel()
            {
                DeviceMACAddress = model.DeviceMACAddress,
                Guid = model.Guid,
                CarNo = model.CarNo,
                LaneSenseDate = model.LaneSenseDate
            };
            ActiveScanningResponseModel responseModel = _scanningManager.ActiveScanning(content);
            if (responseModel == null)
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_FAIL;
                response.MessageContent = _parkLotManager.LastErrorDescribe;
            }
            else
            {
                response.Result = responseModel;
            }
            return Ok(response);
        }

        /// <summary>
        /// 被扫
        /// 户出示微信/支付宝缴费二维码
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ResponseType(typeof(ResponseBase<PassiveScanningResponseModel>))]
        public IHttpActionResult PassiveScanning(PassiveScanningRequest model)
        {
            ResponseBase<PassiveScanningResponseModel> response = new ResponseBase<PassiveScanningResponseModel>()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString(),
            };

            if (string.IsNullOrWhiteSpace(model.DeviceMACAddress) ||
                string.IsNullOrWhiteSpace(model.Guid) ||
                string.IsNullOrWhiteSpace(model.CarNo) ||
                model.LaneSenseDate == null ||
                model.LaneSenseDate == DateTime.MinValue ||
                model.ParkingFee <= 0 || 
                string.IsNullOrEmpty(model.PayAuthCode))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "必要参数缺失,请检查";
                return Ok(response);
            }

            PassiveScanningModel content = new PassiveScanningModel()
            {
                DeviceMACAddress = model.DeviceMACAddress,
                Guid = model.Guid,
                CarNo = model.CarNo,
                LaneSenseDate = model.LaneSenseDate,
                ParkingFee = model.ParkingFee,
                PayAuthCode = model.PayAuthCode
            };
            PassiveScanningResponseModel responseModel = _scanningManager.PassiveScanning(content);
            if (responseModel == null)
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_FAIL;
                response.MessageContent = _parkLotManager.LastErrorDescribe;
            }
            else
            {
                response.Result = responseModel;
            }

            return Ok(response);
        }

        #endregion
    }
}