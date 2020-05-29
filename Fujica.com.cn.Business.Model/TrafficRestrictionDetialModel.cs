using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.Business.Model
{
    /// <summary>
    /// 限制通行模型
    /// </summary>
    public class TrafficRestrictionDetialModel
    {
        /// <summary>
        /// 通行限制唯一标识
        /// </summary>
        public string Guid { get; set; }
        /// <summary>
        /// 限制车类集合
        /// </summary>
        public List<string> CarTypeGuid { get; set; }

        /// <summary>
        /// 相机标识
        /// </summary>
        public string DeviceIdentify { get; set; }

        /// <summary>
        /// 指定的禁止通行周天,长度为7,依次为周一到周末，为1表示禁止,例如 0010010 表示周三周六禁止
        /// </summary>
        public string AssignDays { get; set; }

        /// <summary>
        /// 禁行开始时间 HH:mm:ss
        /// </summary>
        public string StartTime { get; set; }

        /// <summary>
        /// 禁行结束时间 HH:mm:ss
        /// </summary>
        public string EndTime { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDelete { get; set; }
    }
}
