using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fujica.com.cn.Interface.Management.Models.OutPut
{
    /// <summary>
    /// 车场信息返回实体
    /// </summary>
    public class ParkLotResponse
    {
        /// <summary>
        /// 停车场编码
        /// </summary>
        public string ParkingCode { get; set; }

        /// <summary>
        /// 停车场名称
        /// </summary>
        public string ParkingName { get; set; }

        /// <summary>
        /// 车牌前缀 如:["粤","B"]
        /// </summary>
        public string[] CarNoPrefix { get; set; }

        /// <summary>
        /// 车位数
        /// </summary>
        public uint ParkingSpacesNumber { get; set; }

        /// <summary>
        /// 剩余车位数
        /// </summary>
        public uint RemainingSpace { get; set; }

        /// <summary>
        /// 车场类型 0=商业 1=小区
        /// </summary>
        public int ParkingType { get; set; }

        /// <summary>
        /// 车场地址
        /// </summary>
        public string ParkingSiteAddress { get; set; }

        /// <summary>
        /// 收款账户编码
        /// </summary>
        public string GatherAccountGuid { get; set; }

        /// <summary>
        /// 车场存续标志
        /// </summary>
        public bool Existence { get; set; }
    }
}