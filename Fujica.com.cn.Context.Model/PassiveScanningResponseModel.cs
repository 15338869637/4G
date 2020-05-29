using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.Context.Model
{
    /// <summary>
    /// 被动扫码返回模型
    /// </summary>
    public class PassiveScanningResponseModel : IBaseModel
    {
        /// <summary>
        /// 车牌号
        /// </summary>
        public string CarNo { get; set; }

        /// <summary>
        /// 付款状态 true：成功  false：失败
        /// </summary>
        public bool PayState { get; set; }

        /// <summary>
        /// 备注字段
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 预留扩展字段
        /// </summary>
        public string Extend { get; set; }
    }
}
