using Fujica.com.cn.Context.IContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fujica.com.cn.Context.Model;
using Fujica.com.cn.DataService.RedisCache;
using Fujica.com.cn.DataService.DataBase;
using System.Xml.Serialization;

namespace Fujica.com.cn.Context.ParkLot
{
    /// <summary>
    /// 报表管理
    /// </summary>
    public class ReportContext : IBasicContext,IReportContext
    {

        /// <summary>
        ///补录数据 数据库操作类
        /// </summary>
        private IBaseDataBaseOperate<AddRecordModel> recorddatabaseoperate = null;
        /// <summary>
        ///车牌修正记录 数据库操作类
        /// </summary>
        private IBaseDataBaseOperate<CorrectCarnoModel> carnoRecorddatabaseoperate = null;
   
        public ReportContext(IBaseDataBaseOperate<AddRecordModel> _recorddatabaseoperate,
            IBaseDataBaseOperate<CorrectCarnoModel> _carnoRecorddatabaseoperate)
        { 
            recorddatabaseoperate = _recorddatabaseoperate;
            carnoRecorddatabaseoperate = _carnoRecorddatabaseoperate;
        }

        /// <summary>
        ///  补录数据(报表)
        /// </summary>
        /// <param name="parkcode"></param>
        /// <returns></returns>
        public List<AddRecordModel> AllAddRecord(RecordInSearch model)
        {
            //批量数据都从数据库获取 redis并不缓存此实体
            try
            {
                List<AddRecordModel> list = recorddatabaseoperate.GetFromDataBaseByPage(model) as List<AddRecordModel>;
                return list;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        ///  车牌修正数据(报表)
        /// </summary>
        /// <param name="parkcode"></param>
        /// <returns></returns>
        public List<CorrectCarnoModel> AllCorrectCarno(CorrectCarnoSearch model)
        {
            //批量数据都从数据库获取 redis并不缓存此实体
            try
            {
                List<CorrectCarnoModel> list = carnoRecorddatabaseoperate.GetFromDataBaseByPage(model) as List<CorrectCarnoModel>;
                return list;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}
