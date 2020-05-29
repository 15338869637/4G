using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.Context.Model
{
    /// <summary>
    /// 车道拦截相机发送实体
    /// </summary>
    public class GateCatchModel
    {
        /// <summary>
        /// 异常说明。
        /// 13-无入场记录
        /// 14-临时车未缴费
        /// 400-黑名单
        /// 401-通行限制
        /// 402-月卡被锁
        /// 403-月卡过期
        /// 404-禁止无牌车
        /// 405-手动开闸
        /// 406-满车位
        /// 407-无压地感车辆
        /// 408-是储值卡余额不足
        /// 500-非法开闸
        /// </summary>
        public int ErrorCode { get; set; }

        /// <summary>
        /// 车道相机 设备标识
        /// </summary>
        public string DeviceIdentify { get; set; }

        /// <summary>
        /// 车道当前照片
        /// </summary>
        public string ImgUrl { get; set; }

        /// <summary>
        /// 车牌号码
        /// </summary>
        public string CarNo { get; set; }

        /// <summary>
        /// 车类标识
        /// </summary>
        public string CarTypeGuid { get; set; }
    }

    /// <summary>
    /// 车道拦截车辆详细实体
    /// </summary>
    public class GateCatchDetailModel : IBaseModel
    {
        /// <summary>
        /// 当次停车记录唯一标识
        /// </summary>
        //public string RecordGuid { get; set; }

        /// <summary>
        /// 车牌号码
        /// </summary>
        public string CarNo { get; set; }

        /// <summary>
        /// 车场编码
        /// </summary>
        public string ParkingCode { get; set; }

        /// <summary>
        /// 车道名
        /// </summary>
        public string DrivewayName { get; set; }

        /// <summary>
        /// 车道相机 设备标识
        /// </summary>
        public string DriveWayMAC { get; set; }

        /// <summary>
        /// 车类，0=时租车 1=月租车 2=储值车 3=贵宾车
        /// </summary>
        public CarTypeEnum CarType { get; set; }

        /// <summary>
        /// 车类标识
        /// </summary>
        public string CarTypeGuid { get; set; }

        /// <summary>
        /// 车类名称
        /// </summary>
        public string CarTypeName { get; set; }

        /// <summary>
        /// 车道当前照片
        /// </summary>
        public string ImgUrl { get; set; }

    }

}
