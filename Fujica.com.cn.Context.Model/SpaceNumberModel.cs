using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.Context.Model
{
    /// <summary>
    /// 车位数量实体
    /// </summary>
    public class SpaceNumberModel : IBaseModel
    {
        /// <summary>
        /// 停车场编码
        /// </summary>
        public string ParkCode { get; set; }

        /// <summary>
        /// 剩余车位数
        /// </summary>
        public uint RemainingSpace { get; set; }


    }
}
