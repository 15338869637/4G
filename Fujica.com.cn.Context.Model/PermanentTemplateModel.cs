using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.Context.Model
{
    /// <summary>
    /// 固定车模板模型
    /// </summary>
    public class PermanentTemplateModel:IBaseModel
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
        /// 卡类型(对应的车类型的guid)
        /// </summary>
        public string CarTypeGuid { get; set; }

        /// <summary>
        /// 月数 单次续费的月数
        /// </summary>
        public uint Months { get; set; }

        /// <summary>
        /// 金额 单次续费的金额数 单位元
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 最后一次操作时间
        /// </summary>
        public string OperateTime { get; set; }

        /// <summary>
        /// 最后一次操作员
        /// </summary>
        public string OperateUser { get; set; }
    }
}
