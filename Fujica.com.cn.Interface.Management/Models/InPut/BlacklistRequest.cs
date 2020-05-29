using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Fujica.com.cn.Interface.Management.Models.InPut
{
    /// <summary>
    /// 黑名单请求实体
    /// </summary>
    public class BlacklistRequest: RequestBase
    {
        /// <summary>
        /// 停车场编码
        /// </summary>
        [Required(ErrorMessage = "停车场编码不能为空")]
        public string ParkingCode { get; set; }

        /// <summary>
        /// 车牌号集合,逗号分隔
        /// </summary>
        [Required(ErrorMessage = "车牌号码不能为空")]
        public string CarNoList { get; set; }

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
        /// 指定的禁止天,长度为7,依次为周一到周末，为1表示禁止
        /// </summary>
        public string AssignDay { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }

    /// <summary>
    /// 黑名单移除实体
    /// </summary>
    public class BlacklistRemoveRequest: RequestBase
    {
        /// <summary>
        /// 停车场编码
        /// </summary>
        [Required(ErrorMessage = "停车场编码不能为空")]
        public string ParkingCode { get; set; }

        /// <summary>
        /// 车牌号
        /// </summary>
        [Required(ErrorMessage = "车牌号码不能为空")]
        public string CarNo { get; set; }
    }
}