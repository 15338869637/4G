using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardImportTool
{
    public class CardServiceModel
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
        /// 授权车道集合
        /// </summary>
        public List<string> DrivewayGuidList { get; set; }

        /// <summary>
        /// 卡类型 所属车类唯一标识
        /// </summary>
        public string CarTypeGuid { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 支付金额
        /// </summary>
        public decimal PayAmount { get; set; }

        /// <summary>
        /// 支付方式，直接中文描述
        /// </summary>
        public string PayStyle { get; set; }

        /// <summary>
        /// 是否有效
        /// </summary>
        public bool Enable { get; set; }

        /// <summary>
        /// 是否锁定
        /// </summary>
        public bool Locked { get; set; }

        /// <summary>
        /// 账户余额 储值卡时有效
        /// </summary>
        public decimal Balance { get; set; }

        /// <summary>
        /// 月卡续费类型
        /// 1、天 2、月 3、年
        /// </summary>
        public int RenewalType { get; set; }

        /// <summary>
        /// 月卡续费值
        ///  </summary>
        public int RenewalValue { get; set; }
        /// <summary>
        /// 起始日期 月卡时有效（为开卡日期，不要修改）
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 截止日期 月卡时有效
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 延期前结束日期 月卡时有效
        /// </summary>
        public DateTime PrimaryEndDate { get; set; }

        /// <summary>
        /// 报停日期 月卡时有效
        /// </summary>
        public DateTime PauseDate { get; set; }

        /// <summary>
        /// 激活日期 月卡时有效
        /// </summary>
        public DateTime ContinueDate { get; set; }

        /// <summary>
        /// 入场时间 储值卡时有效
        /// </summary>
        public DateTime AdmissionDate { get; set; }

        /// <summary>
        /// 计费开始时间 储值卡时有效
        /// </summary>
        public DateTime BillingStartTime { get; set; }

        /// <summary>
        /// 收费人
        /// </summary>
        public string RechargeOperator { get; set; }
    }
}
