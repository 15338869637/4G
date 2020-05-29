using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardImportTool
{
    /// <summary>
    /// 月卡车模型
    /// </summary>
    public class MonthCardModel
    {
        /// <summary>
        /// 车主姓名
        /// </summary>
        public string CarOwnerName { get; set; }

        /// <summary>
        /// 车牌
        /// </summary>
        public string CarNo { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool Delete { get; set; }

        /// <summary>
        /// 卡类型 所属车类唯一标识
        /// </summary>
        public string CarTypeGuid { get; set; }

        /// <summary>
        /// 禁止出场
        /// </summary>
        public bool Locked { get; set; }

        /// <summary>
        /// 起始日期 月卡时有效
        /// </summary>
        public string StartDate { get; set; }

        /// <summary>
        /// 截止日期 月卡时有效
        /// </summary>
        public string EndDate { get; set; }
    }
}
