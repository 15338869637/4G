using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.Context.Model
{
    /// <summary>
    /// 月卡车模型
    /// </summary>
    public class MonthCarModel : IBaseModel
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
        /// 车主姓名
        /// </summary>
        public string CarOwnerName { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 车牌
        /// </summary>
        public string CarNo { get; set; }

        /// <summary>
        /// 车类编码
        /// </summary>
        public string CarTypeGuid { get; set; }

        /// <summary>
        /// 授权车道集合
        /// </summary>
        public List<string> DrivewayGuidList { get; set; }

        /// <summary>
        /// 基础费用
        /// </summary>
        public decimal Cost { get; set; }

        /// <summary>
        /// 单次续费月数 例如
        /// cost=100,untis=1,则单次只允许1个月一续费，费用100
        /// cost=1000,untis=12,则单次只允许12个月一续费，费用1000
        /// </summary>
        public uint Units { get; set; }

        /// <summary>
        /// 是否有效
        /// </summary>
        public bool Enable { get; set; }

        /// <summary>
        /// 是否锁定
        /// </summary>
        public bool Locked { get; set; }

        /// <summary>
        /// 起始日期 起始时间(开卡时间)
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 截止日期
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 报停日期 月卡时有效
        /// </summary>
        public DateTime PauseDate { get; set; }

        /// <summary>
        /// 激活日期 月卡时有效
        /// </summary>
        public DateTime ContinueDate { get; set; }
    }
}
