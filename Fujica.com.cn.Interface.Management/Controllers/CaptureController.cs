/***************************************************************************************
 * *
 * *        File Name        : CaptureController.cs
 * *        Creator          : llp
 * *        Create Time      : 2019-09-18 
 * *        Remark           :  
 * *
 * *  Copyright (c) 深圳市富士智能系统有限公司.  All rights reserved. 
 * ***************************************************************************************/
using Fujica.com.cn.Context.Model;
using Fujica.com.cn.Interface.Management.Models.OutPut;
using Fujica.com.cn.MonitorServiceClient.Model;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;

namespace Fujica.com.cn.Interface.Management.Controllers
{
    /// <summary>
    /// websocket通讯接口类.
    /// </summary>
    /// <remarks>
    /// 2019.09.18: 创建. llp <br/> 
    /// </remarks> 
    public class CaptureController :HubBaseController<ChatHub>
    {
        /// <summary>
        /// 移动岗亭数据
        /// </summary>
        /// <param name="model"></param> 
        [ResponseType(typeof(ResponseBaseCommon))]
        public IHttpActionResult CaptureSend(CaptureInOutModel model)
        { 
            ResponseBaseCommon response = new ResponseBaseCommon()
            {
                IsSuccess = false,
                MessageCode = (int)ApiBaseErrorCode.API_FAIL,
                MessageContent = "请求失败"
            };  
            List<string> laneIds = new List<string>() { };
            if (model.DriveWayMAC != null)
            {
                laneIds.Add(model.DriveWayMAC); 
            } 
            Clients.Groups(laneIds).broadcastMessage(model); //broadcastMessage 
            response.IsSuccess = true;
            response.MessageCode = (int)ApiBaseErrorCode.API_SUCCESS;
            response.MessageContent = "请求成功";
            return Ok(response);
        }

        /// <summary>
        /// 相机状态
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [ResponseType(typeof(ResponseBaseCommon))]
        public IHttpActionResult HeartBeatSend(HeartBeatModel model)
        {
            ResponseBaseCommon response = new ResponseBaseCommon()
            {
                IsSuccess = false,
                MessageCode = (int)ApiBaseErrorCode.API_FAIL,
                MessageContent = "请求失败"
            };
            List<string> laneIds = new List<string>() { };
            if (model.ParkingCode != null&&model.DeviceIdentify!=null)
            {
                laneIds.Add(model.ParkingCode);
            }
            Clients.Groups(laneIds).heartBeatMessage(model); //broadcastMessage 
            response.IsSuccess = true;
            response.MessageCode = (int)ApiBaseErrorCode.API_SUCCESS;
            response.MessageContent = "请求成功";
            return Ok(response);
        }



    }
}

 
