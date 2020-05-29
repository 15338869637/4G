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
    public class CorrectCarnoRequest
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
        /// 旧车牌
        /// </summary>
        public string OldCarno { get; set; }

        /// <summary>
        /// 新车牌
        /// </summary>
        public string NewCarno { get; set; } 
        /// <summary>
        /// 操作开始时间
        /// </summary>
        public DateTime StrTime { get; set; }

        /// <summary>
        /// 操作结束时间
        /// </summary>
        public DateTime EndTime { get; set; } 

        /// <summary>
        /// 操作类型 0-入场 1-出场
        /// </summary>
        public int OperationType { get; set; } 

        /// <summary>
        /// 操作员
        /// </summary>
        public string Operator { get; set; }

    }
}
