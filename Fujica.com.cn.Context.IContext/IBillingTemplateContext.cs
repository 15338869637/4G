using Fujica.com.cn.Context.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.Context.IContext
{
    /// <summary>
    /// 计费模板管理
    /// </summary>
    public interface IBillingTemplateContext
    {
        /// <summary>
        /// 添加计费模板
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool AddBillingTemplate(BillingTemplateModel model);

        /// <summary>
        /// 修改计费模板
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool ModifyBillingTemplate(BillingTemplateModel model);

        /// <summary>
        /// 删除计费模板
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool DeleteBillingTemplate(BillingTemplateModel model);

        /// <summary>
        /// 获取某计费模板
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        BillingTemplateModel GetBillingTemplate(string carTypeGuid);



    }
}
