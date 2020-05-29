using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.Context.Model
{
    /// <summary>
    /// 通行限制模型
    /// </summary>
    public class TrafficRestrictionModel : IBaseModel
    {
        /// <summary>
        /// 项目编码
        /// </summary>
        public string ProjectGuid { get; set; }

        /// <summary>
        /// 停车场编码
        /// </summary>
        public string ParkCode { get; set; }

        /// <summary>
        /// 唯一区分标识
        /// </summary>
        public string Guid { get; set; }

        /// <summary>
        /// 限制车道集合
        /// </summary>
        public List<string> DrivewayGuid { get; set; }

        /// <summary>
        /// 限制车类集合
        /// </summary>
        public List<string> CarTypeGuid { get; set; }

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
    }
}
