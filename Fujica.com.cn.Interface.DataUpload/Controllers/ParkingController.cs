using System;
using System.Web.Http;
using Fujica.com.cn.Logger;
using Fujica.com.cn.Security.AdmissionControl;
using Fujica.com.cn.Tools;
using System.Web.Http.Description;
using Fujica.com.cn.Interface.DataUpload.Models.OutPut;
using Fujica.com.cn.Interface.DataUpload.Models.InPut;
using Fujica.com.cn.Context.Model;
using Fujica.com.cn.Business.ParkLot;

namespace Fujica.com.cn.Interface.DataUpload.Controllers
{
    /// <summary>
    /// 停车数据接口
    /// </summary>
    public class ParkingController
    {
        //private VehicleInOutManager vehicleinoutmanager = null;
        ///// <summary>
        ///// 唯一构造函数
        ///// </summary>
        ///// <param name="_logger">日志接口器</param>
        ///// <param name="_serializer">序列化接口器</param>
        ///// <param name="_apiaccesscontrol">api接入控制器</param>
        ///// <param name="_vehicleinoutmanager">进出场管理器</param>
        //public ParkingController(ILogger _logger, ISerializer _serializer, APIAccessControl _apiaccesscontrol,
        //    VehicleInOutManager _vehicleinoutmanager) : base(_logger, _serializer, _apiaccesscontrol)
        //{
        //    vehicleinoutmanager = _vehicleinoutmanager;
        //}

        ///// <summary>
        ///// 车辆入场接口
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost]
        //[ResponseType(typeof(ResponseBaseCommon))]
        //public IHttpActionResult VehicleEntry(VehicleInRequest model)
        //{
        //    ResponseBaseCommon response = new ResponseBaseCommon()
        //    {
        //        IsSuccess = true,
        //        MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
        //        MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
        //    };

        //    if (string.IsNullOrWhiteSpace(model.Guid) ||
        //        string.IsNullOrWhiteSpace(model.DriveWayMAC) ||
        //        string.IsNullOrWhiteSpace(model.CarTypeGuid) ||
        //        string.IsNullOrWhiteSpace(model.CarNo) ||
        //        model.InTime == default(DateTime) )
        //    {
        //        response.IsSuccess = false;
        //        response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
        //        response.MessageContent = "必要参数缺失,请检查";
        //        Logger.LogWarn(LoggerLogicEnum.Interface, "", "", "",
        //            "Fujica.com.cn.Interface.DataUpload.Controllers.ParkingController.VehicleEntry",
        //            string.Format("车辆入场必要参数不全，入参:{0}", Serializer.Serialize(model)));
        //        return Ok(response);
        //    }

        //    VehicleInOutModel content = new VehicleInOutModel()
        //    {
        //        Guid = model.Guid,
        //        DriveWayMAC = model.DriveWayMAC,
        //        CarNo = model.CarNo,
        //        ImgUrl = model.ImgUrl,
        //        EventTime = model.InTime,
        //        CarTypeGuid = model.CarTypeGuid,
        //        Remark = model.Remark
        //    };

        //    bool flag = vehicleinoutmanager.Enter(content, Serializer.Serialize(model));
        //    if (!flag)
        //    {
        //        response.IsSuccess = false;
        //        response.MessageCode = (int)ApiBaseErrorCode.API_FAIL;
        //        response.MessageContent = ApiBaseErrorCode.API_FAIL.ToString();
        //    }
        //    return Ok(response);
        //}

        ///// <summary>
        ///// 车辆出场接口
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost]
        //[ResponseType(typeof(ResponseBaseCommon))]
        //public IHttpActionResult VehicleExit(VehicleOutRequest model)
        //{
        //    ResponseBaseCommon response = new ResponseBaseCommon()
        //    {
        //        IsSuccess = true,
        //        MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
        //        MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
        //    };

        //    if (string.IsNullOrWhiteSpace(model.Guid) ||
        //        string.IsNullOrWhiteSpace(model.DriveWayMAC) ||
        //        string.IsNullOrWhiteSpace(model.CarTypeGuid) ||
        //        string.IsNullOrWhiteSpace(model.CarNo) ||
        //        model.OutTime == default(DateTime))
        //    {
        //        response.IsSuccess = false;
        //        response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
        //        response.MessageContent = "必要参数缺失,请检查";
        //        Logger.LogWarn(LoggerLogicEnum.Interface, "", "", "",
        //            "Fujica.com.cn.Interface.DataUpload.Controllers.ParkingController.VehicleExit",
        //            string.Format("车辆出场必要参数不全，入参:{0}", Serializer.Serialize(model)));
        //        return Ok(response);
        //    }

        //    VehicleInOutModel content = new VehicleInOutModel()
        //    {
        //        Guid = model.Guid,
        //        DriveWayMAC = model.DriveWayMAC,
        //        CarNo = model.CarNo,
        //        ImgUrl = model.ImgUrl,
        //        EventTime = model.OutTime,
        //        CarTypeGuid = model.CarTypeGuid,
        //        Remark = model.Remark
        //    };

        //    bool flag = vehicleinoutmanager.Exit(content);
        //    if (!flag)
        //    {
        //        response.IsSuccess = false;
        //        response.MessageCode = (int)ApiBaseErrorCode.API_FAIL;
        //        response.MessageContent = ApiBaseErrorCode.API_FAIL.ToString();
        //    }
        //    return Ok(response);
        //}

    }
}
