using Fujica.com.cn.Context.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.Context.IContext
{
    /// <summary>
    /// 黑名单管理
    /// </summary>
    public interface IBlacklistContext
    {
        /// <summary>
        /// 添加黑名单
        /// </summary>
        /// <returns></returns>
        bool AddBlacklist(BlacklistModel model);

        /// <summary>
        /// 修改黑名单
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool ModifyBlacklist(BlacklistModel model);

        /// <summary>
        /// 删除黑名单
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool DeleteBlacklist(BlacklistModel model);

        /// <summary>
        /// 获取黑名单
        /// </summary>
        /// <param name="parkingCode">黑名单编码</param>
        /// <returns>停黑名单实体</returns>
        BlacklistModel GetBlacklist(string parkcode);

        
    }
}
