using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.Context.Model
{ 
    /// <summary>
    /// 补录记录模型 查询模型
    /// </summary>
    public class RecordInRequest
    {
        /// <summary>
        /// 当前页码
        /// </summary>
       
        public int PageIndex { get; set; }

        /// <summary>
        /// 每页数量
        /// </summary>
        public int PageSize { get; set; } 
        /// <summary>
        /// 项目编码
        /// </summary>
        public string ProjectGuid { get; set; }
        /// <summary>
        /// 停车场编码
        /// </summary>
        public string ParkingCode { get; set; }

        /// <summary>
        /// 车牌
        /// </summary>
        public string CarNo { get; set; }

        /// <summary>
        /// 车类guid
        /// </summary>
        public string CarTypeGuid { get; set; }

        /// <summary>
        /// 补录开始时间
        /// </summary>
        public DateTime StrTime { get; set; }

        /// <summary>
        /// 补录结束时间
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        public string Operator { get; set; }

    }
}
