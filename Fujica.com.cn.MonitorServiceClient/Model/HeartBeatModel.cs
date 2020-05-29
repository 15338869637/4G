using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.MonitorServiceClient.Model
{
    /// <summary>
    /// 相机心跳原始发送数据实体
    /// </summary>
    public class HeartBeatModel
    {
        /// <summary>
        /// 时间
        /// </summary>
        public string TimeStamp { get; set; }

        /// <summary>
        /// 停车场编码
        /// </summary>
        public string ParkingCode { get; set; }

        /// <summary>
        /// 相机mac地址
        /// </summary>
        public string DeviceIdentify { get; set; }
         
    }
}
