using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.Business.Model
{
    /// <summary>
    /// 相机类型模型
    /// </summary>
    public class CameraTypeModel
    {
        /// <summary>
        /// 0=入口相机 1=出口相机
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 相机标识
        /// </summary>
        public string DeviceIdentify { get; set; }

        /// <summary>
        /// 停车场编号
        /// </summary>
        public string ParkingCode { get; set; }

        /// <summary>
        ///入口数量
        /// </summary>
        public int EntrywayCount { get; set; }

        /// <summary>
        /// 设备地址（原设备登录地址）
        /// </summary>
        public string DeviceEntranceURI { get; set; }

        /// <summary>
        /// 设备类型（原设备账户）
        /// </summary>
        public string DeviceAccount { get; set; }

        /// <summary>
        /// 城市编码
        /// </summary>
        public string CityCode
        {
            get
            {
                return ParkingCode.Length > 10 ? ParkingCode.Substring(6, 4) : "";
            }
        }
    }
}
