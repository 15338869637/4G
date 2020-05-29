using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.Business.Model
{
    /// <summary>
    /// 临时卡车模型
    /// </summary>
    public class TempCardModel
    {
        /// <summary>
        /// 当次停车记录唯一标识
        /// </summary>
        public string Guid { get; set; }

        /// <summary>
        /// 车牌号
        /// </summary>
        public string CarNo { get; set; }

        /// <summary>
        /// 入场时间
        /// </summary>
        public DateTime BeginTime { get; set; }

        /// <summary>
        /// 最晚离场时间 用于控制场内超时
        /// </summary>
        public DateTime LatestTime { get; set; }

        /// <summary>
        /// 是否已经付费
        /// </summary>
        public bool HavePaid { get; set; }
    }
}
