using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.Context.Model
{
    /// <summary>
    /// 车场模型
    /// </summary>
    public class ParkLotModel: IBaseModel
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
        /// 停车场名称
        /// </summary>
        public string ParkName { get; set; }

        /// <summary>
        /// 车牌前缀 如粤B
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// 车位数
        /// </summary>
        public uint SpacesNumber { get; set; }

        /// <summary>
        /// 剩余车位
        /// </summary>
        public uint RemainingSpace { get; set; }

        /// <summary>
        /// 车场类型 0=商业 1=小区
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 车场地址
        /// </summary>
        public string SiteAddress { get; set; }

        /// <summary>
        /// 收款账户
        /// </summary>
        //public string gatheraccountGuid { get; set; }

        /// <summary>
        /// 该车场所有车道guid列表
        /// </summary>
        public List<string> DrivewayList { get; set; }

        /// <summary>
        /// 车场存续标志
        /// </summary>
        public bool Existence { get; set; }
    }
}
