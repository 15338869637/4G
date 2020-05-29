using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.Interface.Management.Models.InPut
{
    /// <summary>
    /// 异常开闸记录
    /// </summary>
    public class OpenGateRecordRequest
    {
        /// <summary>
        /// 停车场编码
        /// </summary> 
        [Required(ErrorMessage = "停车场编码不能为空")]
        public string ParkingCode { get; set; }
        /// <summary>
        /// 车牌号
        /// </summary>
        public string CarNo { get; set; }

        /// <summary>
        /// 设备标识
        /// </summary> 
        public string DeviceIdentify { get; set; }
        /// <summary>
        /// 车类别
        /// </summary>
        public string CarType { get; set; }
        
        /// <summary>
        /// 进出类型
        /// </summary> 
        public int EntranceType { get; set; }

        /// <summary>
        /// 通道名称
        /// </summary>
        public string ThroughName { get; set; }
        /// <summary>
        /// 识别相机
        /// </summary>
        public string DiscernCamera { get; set; }
        /// <summary>
        /// 通行方式   
        /// 出场：1、自动开闸  2、手动开闸  3、免费开闸   4、收费放行
        /// </summary>
        public int ThroughType { get; set; }
        /// <summary>
        /// 开闸时间
        /// </summary>
        public DateTime OpenGateTime { get; set; }
        /// <summary>
        /// 开闸原因
        /// </summary>
        public string OpenGateReason { get; set; }
        /// <summary>
        /// 值班人员
        /// </summary>
        public string OpenGateOperator { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 图片地址
        /// </summary>
        public string ImageUrl { get; set; }
        /// <summary>
        /// 异常说明
        /// </summary>
        public string ErrorCode { get; set; }
        

    }

}
