using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.Context.Model
{
    /// <summary>
    /// 主动扫码返回模型
    /// </summary>
    public class ActiveScanningResponseModel : IBaseModel
    {
        /// <summary>
        /// 车牌号
        /// </summary>
        public string CarNo { get; set; }
        
        /// <summary>
        /// 停车费用
        /// </summary>
        public decimal ParkingFee { get; set; }

        /// <summary>
        /// 入场时间
        /// </summary>
        public string BeginTime { get; set; }

        /// <summary>
        /// 付款二维码
        /// </summary>
        public string QRCode { get; set; }

        /// <summary>
        /// 备注字段
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 预留扩展字段
        /// </summary>
        public string Extend { get; set; }
    }
}
