using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.Context.Model
{
    /// <summary>
    /// 黑名单模型
    /// </summary>
    public class BlacklistModel : IBaseModel
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
        /// 黑名单详情
        /// </summary>
        public List<BlacklistSingleModel> List { get; set; }

    }

    /// <summary>
    /// 黑名单某项详情模型
    /// </summary>
    public class BlacklistSingleModel
    {

        /// <summary>
        /// 车牌号
        /// </summary>
        public string CarNo { get; set; }

        /// <summary>
        /// 是否启用(该黑名单)
        /// </summary>
        public bool Enable { get; set; }

        /// <summary>
        /// 是否永久限制
        /// </summary>
        public bool Always { get; set; }


        /// <summary>
        /// 按日期禁止
        /// 其实可以按12位长度，每6位表示yyMMdd,一个为开始，一个为结束
        /// 但怕别人看不懂，只好定义多个字段
        /// </summary>
        public bool ByDate { get; set; }
        /// <summary>
        /// 开始日期
        /// </summary>
        public string StartDate { get; set; }
        /// <summary>
        /// 结束日期
        /// </summary>
        public string EndDate { get; set; }


        /// <summary>
        /// 按时间禁止
        /// 其实可以按12位长度，每6位表示HHmmss,一个为开始，一个为结束
        /// 但怕别人看不懂，只好定义多个字段
        /// </summary>
        public bool ByTime { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public string StatrtTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public string EndTime { get; set; }


        /// <summary>
        /// 按周天禁止
        /// </summary>
        public bool ByWeek { get; set; }
        /// <summary>
        /// 指定的禁止天,长度为7,依次为周一到周末，为1表示禁止,例如 0010010 表示周三周六禁止
        /// </summary>
        public string AssignDay { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
