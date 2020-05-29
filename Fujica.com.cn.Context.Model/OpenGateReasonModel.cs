using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.Context.Model
{
    /// <summary>
    /// 开闸原因模型
    /// </summary>
    public class OpenGateReasonModel : IBaseModel
    {
        /// <summary>
        /// 项目编码
        /// </summary>
        public string ProjectGuid { get; set; }

        /// <summary>
        /// 唯一标识
        /// </summary>
        public string Guid { get; set; }

        /// <summary>
        /// 开闸类型 0=手动 1=免费
        /// </summary>
        public int OpenType { get; set; }

        /// <summary>
        /// 开闸原因注明
        /// </summary>
        public string ReasonRemark { get; set; }
    }
}
