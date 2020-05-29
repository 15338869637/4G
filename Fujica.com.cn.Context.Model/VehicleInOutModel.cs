using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.Context.Model
{
    /// <summary>
    /// 车辆出入场实体
    /// </summary>
    public class VehicleInOutModel:IBaseModel
    {
        /// <summary>
        /// 当次停车唯一标识
        /// </summary>
        public string Guid { get; set; }

        /// <summary>
        /// 车道设备地址
        /// </summary>
        public string DriveWayMAC { get; set; }

        /// <summary>
        /// 入口名
        /// </summary>
        public string Entrance { get; set; }
        
        /// <summary>
        /// 车牌号
        /// </summary>
        public string CarNo { get; set; }

        /// <summary>
        /// 车辆图片地址
        /// </summary>
        public string ImgUrl { get; set; }

        /// <summary>
        /// 出入事件触发时间
        /// </summary>
        public DateTime EventTime { get; set; }

        /// <summary>
        /// 车类
        /// </summary>
        public string CarTypeGuid { get; set; }

        /// <summary>
        /// 车类名称
        /// </summary>
        public string CarTypeName { get; set; }

        /// <summary>
        /// 备注说明
        /// </summary>
        public string Remark { get; set; }

      
    }


    ///// <summary>
    ///// 车辆入场实体
    ///// </summary>
    //public class VehicleInModel : IBaseModel
    //{
    //    /// <summary>
    //    /// 当次停车唯一标识
    //    /// </summary>
    //    public string Guid { get; set; }

    //    /// <summary>
    //    /// 车道设备地址
    //    /// </summary>
    //    public string DriveWayMAC { get; set; }

    //    /// <summary>
    //    /// 车牌号
    //    /// </summary>
    //    public string CarNo { get; set; }

    //    /// <summary>
    //    /// 入场时间
    //    /// </summary>
    //    public DateTime InTime { get; set; }

    //    /// <summary>
    //    /// 车辆图片地址
    //    /// </summary>
    //    public string ImgUrl { get; set; }

    //    /// <summary>
    //    /// 车类编号
    //    /// </summary>
    //    public string CarTypeGuid { get; set; }

    //    /// <summary>
    //    /// 车类0=时租车 1=月租车 2=储值车 3=贵宾车
    //    /// </summary>
    //    public string CarType { get; set; }

    //    /// <summary>
    //    /// 备注说明
    //    /// </summary>
    //    public string Remark { get; set; }
    //}
}
