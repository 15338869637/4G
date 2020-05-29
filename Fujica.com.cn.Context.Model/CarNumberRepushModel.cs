using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.Context.Model
{
    /// <summary>
    /// 车牌修改重推模型
    /// </summary>
    public class CarNumberRepushModel
    {
        /// <summary>
        /// 设备标识
        /// </summary>
        public string DeviceIdentify { get; set; }

        /// <summary>
        /// 停车场编码
        /// </summary>
        public string ParkingCode { get; set; }

        /// <summary>
        /// 旧车牌
        /// </summary>
        public string OldCarno { get; set; }

        /// <summary>
        /// 新车牌
        /// </summary>
        public string NewCarno { get; set; }

    }
}
