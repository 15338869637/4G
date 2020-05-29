using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.Business.Model
{
    /// <summary>
    /// 临时车类型模型
    /// </summary>
    public class TempCarTypeModel
    {
        /// <summary>
        /// 车颜色类别 1:蓝牌车 2:黄牌车 3:白牌车 4:绿牌车 5:黑牌车
        /// </summary>
        public int CarColor { get; set; }

        /// <summary>
        /// 所属车类 收费(空表示按默认的)
        /// </summary>
        public string CarTypeGuid { get; set; }

        /// <summary>
        /// 开始时间-时间段1
        /// </summary>
        public string BeginTime1 { get; set; }
        /// <summary>
        /// 开始时间-时间段2
        /// </summary>
        public string BeginTime2 { get; set; }
        /// <summary>
        /// 开始时间-时间段3
        /// </summary>
        public string BeginTime3 { get; set; }

        /// <summary>
        /// 结束时间-时间段1
        /// </summary>
        public string EndTime1 { get; set; }
        /// <summary>
        /// 结束时间-时间段2
        /// </summary>
        public string EndTime2 { get; set; }
        /// <summary>
        /// 结束时间-时间段3
        /// </summary>
        public string EndTime3 { get; set; }
        /// <summary>
        /// 免费分钟数-时间段1
        /// </summary>
        public int FreeMinutes1 { get; set; }
        /// <summary>
        /// 免费分钟数-时间段2
        /// </summary>
        public int FreeMinutes2 { get; set; }
        /// <summary>
        /// 免费分钟数-时间段3
        /// </summary>
        public int FreeMinutes3 { get; set; }

        /// <summary>
        /// 离场超时分钟-时间段1
        /// 缴费后可享有的免费时间
        /// </summary>
        public int LeaveTimeout1 { get; set; }
        /// <summary>
        /// 离场超时分钟-时间段2
        /// 缴费后可享有的免费时间
        /// </summary>
        public int LeaveTimeout2 { get; set; }
        /// <summary>
        /// 离场超时分钟-时间段3
        /// 缴费后可享有的免费时间
        /// </summary>
        public int LeaveTimeout3 { get; set; }

    }
    
}
