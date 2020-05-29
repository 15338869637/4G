using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.Context.Model
{
    /// <summary>
    /// 车道闸口锁定模型集合
    /// </summary>
    public class GateKeepListModel : IBaseModel
    {
        /// <summary>
        /// 项目编码
        /// </summary>
        public string ProjectGuid { get; set; }

        /// <summary>
        /// 停车场编码
        /// </summary>
        public string ParkingCode { get; set; }

        /// <summary>
        /// 数据集合
        /// </summary>
        public List<GateKeepModel> List { get; set; }
    }

    /// <summary>
    /// 车道闸口锁定模型
    /// </summary>
    public class GateKeepModel
    {
        /// <summary>
        /// 车道guid
        /// </summary>
        public string DrivewayGuid { get; set; }

        /// <summary>
        /// 设备地址
        /// </summary>
        public string DeviceMacAddress { get; set; }

        /// <summary>
        /// 闸口状态 0：正常  1：开闸锁定   2：关闸锁定
        /// </summary>
        public int GateState { get; set; }
    }

}
