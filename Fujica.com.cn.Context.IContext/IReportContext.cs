using Fujica.com.cn.Context.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.Context.IContext
{
    /// <summary>
    /// 报表管理
    /// </summary>
    public interface IReportContext
    { 
        /// <summary>
        ///  补录报表数据
        /// </summary>
        /// <param name="parkcode"></param>
        /// <returns></returns>
        List<AddRecordModel> AllAddRecord(RecordInSearch model);
        /// <summary>
        ///  车牌修正报表数据
        /// </summary>
        /// <param name="parkcode"></param>
        /// <returns></returns>
        List<CorrectCarnoModel> AllCorrectCarno(CorrectCarnoSearch model);
    }
}
