using Fujica.com.cn.Context.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.Context.IContext
{

    /// <summary>
    /// 开闸管理
    /// </summary>
    public interface IOpenGateReasonContext
    {
        /// <summary>
        /// 保存开闸原因
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool SaveOpenReason(OpenGateReasonModel model);

        /// <summary>
        /// 读取开闸原因
        /// </summary>
        /// <param name="drivewayGuid"></param>
        /// <returns></returns>
        OpenGateReasonModel GetOpenReason(string guid); 

        /// <summary>
        /// 删除开闸原因
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        bool DeleteOpenReason(string guid);

        /// <summary>
        /// 读取开闸原因列表
        /// </summary>
        /// <param name="parkCode"></param>
        /// <returns></returns>
        List<OpenGateReasonModel> GetOpenReasonList(string projectGuid);

    }
}
