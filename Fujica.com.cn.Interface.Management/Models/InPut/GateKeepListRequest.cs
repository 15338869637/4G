using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Fujica.com.cn.Interface.Management.Models.InPut
{
    /// <summary>
    /// 车道闸口锁定请求实体
    /// </summary>
    public class GateKeepListRequest : RequestBase
    {
        /// <summary>
        /// 停车场编码
        /// </summary>
        [Required(ErrorMessage = "停车场编码不能为空")]
        public string ParkingCode { get; set; }

        /// <summary>
        /// 车道闸口锁定集合
        /// </summary>
        [Required(ErrorMessage = "车道闸口集合不能为空")]
        public List<GateKeepRequest> GateKeepList { get; set; }
    }


    /// <summary>
    /// 车道闸口锁定请求实体
    /// </summary>
    public class GateKeepRequest
    {
        /// <summary>
        /// 车道guid
        /// </summary>
        public string DrivewayGuid { get; set; }

        /// <summary>
        /// 设备地址
        /// </summary>
        public string DeviceMacAddress { get; set; }

        /// <summary>
        /// 闸口状态 0：正常  1：开闸锁定   2：关闸锁定
        /// </summary>
        public int GateState { get; set; }
    }
}