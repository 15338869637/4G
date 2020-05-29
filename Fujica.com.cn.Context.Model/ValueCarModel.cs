using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.Context.Model
{
    /// <summary>
    /// 储值卡车模型
    /// </summary>
    public class ValueCarModel : IBaseModel
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
        /// 起始时间(开卡时间)
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 授权车道集合
        /// </summary>
        public List<string> DrivewayGuidList { get; set; }

        /// <summary>
        /// 是否有效
        /// </summary>
        public bool Enable { get; set; }

        /// <summary>
        /// 是否锁定
        /// </summary>
        public bool Locked { get; set; }

        /// <summary>
        /// 账户余额
        /// </summary>
        public decimal Balance { get; set; }
    }
}
