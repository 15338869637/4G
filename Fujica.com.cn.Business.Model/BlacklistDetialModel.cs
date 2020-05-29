using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.Business.Model
{
    /// <summary>
    /// 黑名单模型
    /// </summary>
    public class BlacklistDetialModel
    {
        /// <summary>
        /// 车牌号
        /// </summary>
        public string CarNo { get; set; }
        
        /// <summary>
        /// 是否永久限制
        /// </summary>
        public bool Always { get; set; }

        /// <summary>
        /// 日期禁止 12位长度，前6位yyMMdd表示开始，后6位yyMMdd表示结束,若12个0表示不按日期
        /// </summary>
        public string DateInterval { get; set; }

        /// <summary>
        /// 时间禁止 12位长度，前6位HHmmss表示开始，后6位HHmmss表示开始结束
        /// </summary>
        public string TimeInterval { get; set; }

        /// <summary>
        /// 周天禁止 7位长度,依次为周一到周末，为1表示禁止,例如 0010010 表示周三周六禁止
        /// </summary>
        public string WeekInterval { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDelete { get; set; }
    }
}
