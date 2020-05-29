using Fujica.com.cn.Context.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Fujica.com.cn.Context.Model.GatherAccountModel;

namespace Fujica.com.cn.Context.IContext
{

    /// <summary>
    /// 账户管理器
    /// </summary>
    public interface IGatherAccountContext
    { 
        /// <summary>
        /// 添加收款账户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool AddGatherAccount(GatherAccountModel model);

        /// <summary>
        /// 修改收款账户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool ModifyGatherAccount(GatherAccountModel model);

        /// <summary>
        /// 删除收款账户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool DeleteGatherAccount(GatherAccountModel model);

        /// <summary>
        /// 获取某收款账户
        /// </summary>
        /// <param name="guid">该账户guid</param>
        /// <returns></returns>
        GatherAccountModel GetGatherAccount(string guid);

        /// <summary>
        /// 获取收款账户列表
        /// </summary>
        /// <param name="projectguid"></param>
        /// <returns></returns>
        List<GatherAccountModel> GetGatherAccountList(string projectguid);

        /// <summary>
        /// 获取支付宝账户
        /// </summary>
        /// <param name="guid">该账户guid</param>
        /// <returns></returns>
         AliPayAccountModel GetAliPayAccount(string guid);

        /// <summary>
        /// 获取微信账户
        /// </summary>
        /// <param name="guid">该账户guid</param>
        /// <returns></returns>
         WeChatAccountModel GetWeChatAccount(string guid);

    }
}
