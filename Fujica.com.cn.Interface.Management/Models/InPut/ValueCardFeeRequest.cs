using System;

namespace Fujica.com.cn.Interface.Management.Models.InPut
{
    /// <summary>
    /// 查询 储值卡类停车费用
    /// </summary>
    public class ValueCardFeeRequest//: RequestBase
    {
        /// <summary>
        /// 停车场编号
        /// </summary>
        public string ParkCode { get; set; }
        
        /// <summary>
        /// 车牌号
        /// </summary>
        public string CarNo { get; set; }

        /// <summary>
        /// 车道相机设备地址
        /// </summary>
        public string DeviceMACAddress { get; set; }

        /// <summary>
        /// 压地感时间
        /// </summary>
        public DateTime LaneSenseDate { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string ErrorCode { get; set; }

       

    }

    /// <summary>
    ///执行储值卡扣费
    /// </summary>
    public class ValueCardCalculationFeeRequest  //: RequestBase
    {
        /// <summary>
        /// 停车场编号
        /// </summary>
        public string ParkCode { get; set; }

        /// <summary>
        /// 车牌号
        /// </summary>
        public string CarNo { get; set; }  

    }
}
