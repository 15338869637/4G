using Fujica.com.cn.Context.IContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fujica.com.cn.Context.Model;
using Fujica.com.cn.DataService.RedisCache;
using Fujica.com.cn.DataService.DataBase;

namespace Fujica.com.cn.Context.ParkLot
{
    /// <summary>
    /// 通行设置管理器
    /// </summary>
    public class TrafficRestrictionContext : IBasicContext, ITrafficRestrictionContext
    { 

        /// <summary>
        /// 数据库操作类
        /// </summary>
        private IBaseDataBaseOperate<TrafficRestrictionModel> databaseoperate = null; 

        public TrafficRestrictionContext(IBaseDataBaseOperate<TrafficRestrictionModel> _databaseoperate)
        {
            databaseoperate = _databaseoperate;
        }

        /// <summary>
        /// 保存通行设置 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="drivewayList">该停车场所有的车道</param>
        /// <returns></returns>
        public bool SaveTrafficRestriction(TrafficRestrictionModel model)
        {
            return databaseoperate.SaveToDataBase(model);  
        }

        /// <summary>
        /// 读取通行设置
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public TrafficRestrictionModel GetTrafficRestriction(string guid)
        {
            return databaseoperate.GetFromDataBase(guid);
        }

        /// <summary>
        /// 删除通行设置
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public bool DeleteTrafficRestriction(TrafficRestrictionModel model)
        {
            return databaseoperate.DeleteInDataBase(new TrafficRestrictionModel() { Guid = model.Guid });
            
        }

        /// <summary>
        /// 读取通行设置列表
        /// </summary>
        /// <param name="projectGuid"></param>
        /// <returns></returns>
        public List<TrafficRestrictionModel> GetTrafficRestrictionList(string projectGuid)
        {
            //批量数据都从数据库获取
            List<TrafficRestrictionModel> model = databaseoperate.GetMostFromDataBase(projectGuid) as List<TrafficRestrictionModel>;
            return model.OrderByDescending(a => a.StartTime).ToList();//降序;
        }


    }
}
