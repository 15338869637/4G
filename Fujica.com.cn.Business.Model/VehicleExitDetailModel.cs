using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.Business.Model
{
    /// <summary>
    /// 出场记录实体详情
    /// </summary>
    public class VehicleExitDetailModel
    {
        /// <summary>
        /// 当次停车记录唯一标识
        /// </summary>
        public string RecordGuid { get; set; }

        /// <summary>
        /// 车牌号
        /// </summary>
        public string CarNo { get; set; }

        /// <summary>
        /// 车场编码
        /// </summary>
        public string ParkingCode { get; set; }

        /// <summary>
        /// 车场名称
        /// </summary>
        public string ParkingName { get; set; }

        /// <summary>
        /// 车道MAC地址
        /// </summary>
        public string DriveWayMAC { get; set; }

        /// <summary>
        /// 出口车道名
        /// </summary>
        public string Exit { get; set; }

        /// <summary>
        /// 出口相机名
        /// </summary>
        public string ExitCamera { get; set; }

        /// <summary>
        /// 出场图片
        /// </summary>
        public string OutImgUrl { get; set; }

        /// <summary>
        /// 出场时间
        /// </summary>
        public DateTime LeaveTime { get; set; }

        /// <summary>
        /// 出场说明
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 开闸类型 1、自动开闸  2、手动开闸  3、免费开闸   4、收费放行
        /// </summary>
        public int OpenType { get; set; }

        /// <summary>
        /// 操作员
        /// </summary>
        public string Operator { get; set; }
    }
}
