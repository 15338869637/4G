using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.Context.Model
{
    /// <summary>
    /// 免费放行请求实体
    /// </summary>
    public class FreeOpenGateModel 
    {
        /// <summary>
        /// 停车场编码
        /// </summary>  
        public string ParkingCode { get; set; }
        /// <summary>
        /// 设备标识
        /// </summary> 
        public string DeviceIdentify { get; set; }
        /// <summary>
        /// 车牌号
        /// </summary>
        public string CarNo { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 操作员
        /// </summary> 
        public string TolloPerator { get; set; }

    }
}
