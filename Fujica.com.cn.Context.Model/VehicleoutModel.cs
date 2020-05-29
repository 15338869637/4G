using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.Context.Model
{
    /// <summary>
    /// 车辆出场请求实体
    /// </summary>
    public class VehicleOutModel
    {
        /// <summary>
        /// 当次停车唯一标识
        /// </summary>
        public string Guid { get; set; }

        /// <summary>
        /// 车道相机设备地址
        /// </summary>
        public string DriveWayMAC { get; set; }

        /// <summary>
        /// 车牌号
        /// </summary>
        public string CarNo { get; set; }

        /// <summary>
        /// 车辆图片地址
        /// </summary>
        public string ImgUrl { get; set; }

        /// <summary>
        /// 出场时间
        /// </summary>
        public DateTime OutTime { get; set; }

        /// <summary>
        /// 车类
        /// </summary>
        public string CarTypeGuid { get; set; }

        /// <summary>
        /// 备注说明
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 开闸类型 1、自动开闸  2、手动开闸  3、免费开闸   4、收费放行
        /// </summary>
        public int OpenType { get; set; }

        /// <summary>
        /// 操作员
        /// </summary>
        public string Operator { get; set; }

        /// <summary>
        /// 车牌图片url地址
        /// </summary>
        public string PlateUrl { get; set; }
    }
}
