using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.Context.Model
{
    /// <summary>
    /// 出口未缴费临时车辆 交互实体（相机上传）
    /// </summary>
    public class ExitTempCarPayModel
    {
        /// <summary>
        /// 车道相机设备地址
        /// </summary>
        public string DriveWayMAC { get; set; }

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

    }
}
