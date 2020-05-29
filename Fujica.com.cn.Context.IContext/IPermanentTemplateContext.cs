using Fujica.com.cn.Context.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.Context.IContext
{

    /// <summary>
    /// 收费设置(模板)管理
    /// </summary>
    public interface IPermanentTemplateContext
    {
        /// <summary>
        /// 设置固定车延期模板
        /// </summary>
        /// <returns></returns>
        bool SetPermanentTemplate(PermanentTemplateModel model);

        /// <summary>
        /// 删除固定车延期模板
        /// </summary>
        /// <returns></returns>
        bool DeletePermanentTemplate(PermanentTemplateModel model);

        /// <summary>
        /// 某车场所有固定车延期模板
        /// </summary>
        /// <param name="parkcode"></param>
        /// <returns></returns>
         List<PermanentTemplateModel> AllPermanentTemplate(string parkcode);

        /// <summary>
        /// 获取固定车延期模板
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        PermanentTemplateModel GetPermanentTemplate(string cartypeguid);

    }
}
