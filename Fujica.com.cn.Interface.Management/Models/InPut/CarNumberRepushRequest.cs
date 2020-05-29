using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fujica.com.cn.Interface.Management.Models.InPut
{
    /// <summary>
    /// 车牌修改重推 请求实体
    /// </summary>
    public class CarNumberRepushRequest: RequestBase
    {
        /// <summary>
        /// 车道设备地址
        /// </summary>
        public string DriveWayMAC { get; set; }

        /// <summary>
        /// 停车场编码
        /// </summary>
        public string ParkingCode { get; set; }

        /// <summary>
        /// 旧车牌
        /// </summary>
        public string OldCarNo { get; set; }

        /// <summary>
        /// 新车牌
        /// </summary>
        public string NewCarNo { get; set; } 
        /// <summary>
        /// 识别照片
        /// </summary>
        public string ImgUrl { get; set; }
        /// <summary>
        /// 操作员
        /// </summary>
        public string Operator { get; set; }
    }
}