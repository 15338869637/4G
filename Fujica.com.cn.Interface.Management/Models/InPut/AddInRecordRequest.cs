using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Fujica.com.cn.Interface.Management.Models.InPut
{
    /// <summary>
    /// 补入场记录请求实体
    /// </summary>
    public class AddInRecordRequest:RequestBase
    {
        /// <summary>
        /// 停车场编码
        /// </summary>
        [Required(ErrorMessage = "停车场编码不能为空")]
        public string ParkingCode { get; set; }

        /// <summary>
        /// 车牌号
        /// </summary>
        [Required(ErrorMessage = "车牌号不能为空")]
        public string CarNo { get; set; }

        /// <summary>
        /// 入场时间
        /// </summary>
        [Required(ErrorMessage = "入场时间不能为空")]
        public DateTime InTime { get; set; }
        
        /// <summary>
        /// 入场图片
        /// </summary>
        [Required(ErrorMessage = "入场图片不能为空")]
        public string ImgUrl { get; set; }

        /// <summary>
        /// 入场通道
        /// </summary>
        [Required(ErrorMessage = "入场通道不能为空")]
        public string Entrance { get; set; } 

        /// <summary>
        /// 车类
        /// </summary>
        [Required(ErrorMessage = "入场车类不能为空")]
        public string CarTypeGuid { get; set; }

        /// <summary>
        /// 车道标识
        /// </summary>
        [Required(ErrorMessage = "车道标识不能为空")]
        public string DrivewayGuid { get; set; }

        /// <summary>
        /// 操作员
        /// </summary>
        public string Operator { get; set; }
    }
}