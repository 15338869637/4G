using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.Context.Model
{
    /// <summary>
    /// 出入场车辆抓拍
    /// </summary>
   public  class CaptureInOutModel : IBaseModel
    {
        /// <summary>
        /// 当次停车唯一标识
        /// </summary>
        public string Guid { get; set; }


        /// <summary>
        /// 停车场编码
        /// </summary>
        public string ParkCode { get; set; }

        /// <summary>
        /// 车道入口名
        /// </summary>
        public string Entrance { get; set; }

        /// <summary>
        /// 车道 出口名
        /// </summary>
        public string Exit { get; set; }

        /// <summary>
        /// 车道设备地址
        /// </summary>
        public string DriveWayMAC { get; set; }

        /// <summary>
        /// 剩余车位数量
        /// </summary>
        public string RemainingNumber { get; set; }
        

        /// <summary>
        /// 车牌号
        /// </summary>
        public string CarNo { get; set; }

        /// <summary>
        /// 进出场类型 0入场 1出场
        /// </summary>
        public string  EntryType { get; set; }

        /// <summary>
        /// 入场图片地址
        /// </summary>
        public string InImgUrl { get; set; }

        /// <summary>
        /// 出场图片地址
        /// </summary>
        public string OutImgUrl { get; set; } 

        /// <summary>
        /// 入场时间
        /// </summary>
        public DateTime? InTime { get; set; } 

        /// <summary>
        /// 出场时间
        /// </summary>
        public DateTime? OutTime { get; set; }


        /// <summary>
        /// 车类，0=时租车 1=月租车 2=储值车
        /// </summary>
        public int CarType { get; set; }

        /// <summary>
        /// 车类名称
        /// </summary>
        public string CarTypeName { get; set; }

        /// <summary>
        /// 车类标识
        /// </summary>
        public string CarTypeGuid { get; set; }
        
        /// <summary>
        /// 备注说明
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 异常说明
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
        /// 408是储值卡余额不足
        /// 409临时车锁定
        /// 410储值卡锁定
        /// </summary>
        public string ErrorCode { get; set; }
   

        
    }
}
