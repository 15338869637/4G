using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.Context.Model
{
    /// <summary>
    /// 入场车辆原始数据模型
    /// </summary>
    public class EntryOriginalModel
    {
        /// <summary>
        /// 设备标识
        /// </summary>
        public string DeviceIdentify { get; set; }

        /// <summary>
        /// 原始数据集合
        /// </summary>
        public List<VehicleInModel> OriginalList { get; set; }
    }
}
