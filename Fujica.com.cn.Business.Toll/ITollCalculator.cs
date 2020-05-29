using Fujica.com.cn.Context.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.Business.Toll
{
    /// <summary>
    /// 计费模板与算费接口
    /// </summary>
    public interface ITollCalculator
    {
        /// <summary>
        /// 计算费用 -1:计算失败
        /// </summary>
        /// <param name="beginTime">计费起始时间</param>
        /// <param name="endTime">计费截止时间</param>
        /// <param name="hasFreeMinute">是否有免费分钟</param>
        /// <returns></returns>
        decimal GetFees(BillingTemplateModel model,DateTime beginTime, DateTime endTime, bool hasFreeMinute);

        /// <summary>
        /// 获取出场超时时长
        /// </summary>
        /// <returns></returns>
        int GetLeaveTimeOut(BillingTemplateModel model);

        /// <summary>
        /// 获取收费模板字符串
        /// </summary>
        /// <returns></returns>
        string GetTollFeesTemplateStr(BillingTemplateModel model);
    }
}
