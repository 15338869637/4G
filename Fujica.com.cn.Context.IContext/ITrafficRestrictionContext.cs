using Fujica.com.cn.Context.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.Context.IContext
{

    /// <summary>
    /// 通行设置管理器
    /// </summary>
    public interface ITrafficRestrictionContext
    {
        /// <summary>
        /// 保存通行设置
        /// </summary>
        /// <param name="model"></param>
        /// <param name="drivewayList">该停车场所有的车道</param>
        /// <returns></returns>
        bool SaveTrafficRestriction(TrafficRestrictionModel model);

        /// <summary>
        /// 读取通行设置
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        TrafficRestrictionModel GetTrafficRestriction(string guid);

        /// <summary>
        /// 删除通行设置
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        bool DeleteTrafficRestriction(TrafficRestrictionModel model);

        /// <summary>
        /// 读取通行设置列表
        /// </summary>
        /// <param name="projectGuid"></param>
        /// <returns></returns>
        List<TrafficRestrictionModel> GetTrafficRestrictionList(string projectGuid);
    }
}
