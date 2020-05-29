using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.Context.Model
{
    /// <summary>
    /// 车道连接状态实体
    /// </summary>
    public class DrivewayConnStatusModel : IBaseModel
    {
        /// <summary>
        /// 停车场编码
        /// </summary>
        public string ParkingCode { get; set; }

        /// <summary>
        /// 设备标识 如MAC地址等
        /// </summary>
        public string DeviceMacAddress { get; set; }

        /// <summary>
        /// 连接时间
        /// </summary>
        public DateTime ConnDate { get; set; }

        /// <summary>
        /// 设备连接状态 true：已连接 false：已断开
        /// </summary>
        public bool DeviceStatus
        {
            get
            {
                TimeSpan ts1 = new TimeSpan(ConnDate.Ticks);
                TimeSpan ts2 = new TimeSpan(DateTime.Now.Ticks);
                TimeSpan ts3 = ts1.Subtract(ts2).Duration();

                return ts3.TotalSeconds < 60;
            }
        }
    }
}
