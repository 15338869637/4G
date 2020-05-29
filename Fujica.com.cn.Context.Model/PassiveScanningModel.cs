using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.Context.Model
{
    /// <summary>
    /// 被动扫码模型
    /// </summary>
    public class PassiveScanningModel
    {
        /// <summary>
        /// 车道相机设备地址
        /// </summary>
        public string DeviceMACAddress { get; set; }

        /// <summary>
        /// 当次停车唯一标识
        /// </summary>
        public string Guid { get; set; }

        /// <summary>
        /// 车牌号
        /// </summary>
        public string CarNo { get; set; }

        /// <summary>
        /// 压地感时间
        /// </summary>
        public DateTime LaneSenseDate { get; set; }

        /// <summary>
        /// 停车费用
        /// </summary>
        public decimal ParkingFee { get; set; }

        /// <summary>
        /// 支付授权码
        /// </summary>
        public string PayAuthCode { get; set; }
    }
}
