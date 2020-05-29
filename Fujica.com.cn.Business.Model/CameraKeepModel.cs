using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.Business.Model
{

    /// <summary>
    /// 相机锁定MQ模型
    /// </summary>
    public class CameraKeepModel
    {
        /// <summary>
        /// 设备地址
        /// </summary>
        public string DeviceIdentify { get; set; }

        /// <summary>
        /// 闸口状态 0：正常  1：开闸锁定   2：关闸锁定
        /// </summary>
        public int GateState { get; set; }
    }
}
