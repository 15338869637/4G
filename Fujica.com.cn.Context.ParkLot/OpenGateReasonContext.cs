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
    /// 开闸管理
    /// </summary>
    public class OpenGateReasonContext : IBasicContext, IOpenGateReasonContext
    { 
        /// <summary>
        /// 数据库操作类
        /// </summary>
        private IBaseDataBaseOperate<OpenGateReasonModel> databaseoperate = null; 

        public OpenGateReasonContext(IBaseDataBaseOperate<OpenGateReasonModel> _databaseoperate)
        {
            databaseoperate = _databaseoperate;
        }

        /// <summary>
        /// 保存开闸原因
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool SaveOpenReason(OpenGateReasonModel model)
        {            
            return databaseoperate.SaveToDataBase(model);
        }

        /// <summary>
        /// 读取开闸原因
        /// </summary>
        /// <param name="drivewayGuid"></param>
        /// <returns></returns>
        public OpenGateReasonModel GetOpenReason(string guid)
        {
            return databaseoperate.GetFromDataBase(guid);
        }

        /// <summary>
        /// 删除开闸原因
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public bool DeleteOpenReason(string guid)
        {
            return databaseoperate.DeleteInDataBase(new OpenGateReasonModel() { Guid = guid });
        }

        /// <summary>
        /// 读取开闸原因列表
        /// </summary>
        /// <param name="parkCode"></param>
        /// <returns></returns>
        public List<OpenGateReasonModel> GetOpenReasonList(string projectGuid)
        {
            //批量数据都从数据库获取
            List<OpenGateReasonModel> model = databaseoperate.GetMostFromDataBase(projectGuid) as List<OpenGateReasonModel>;
            return model;
        }


    }
}
