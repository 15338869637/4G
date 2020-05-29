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
    /// 语音指令管理器
    /// </summary>
    public class VoiceCommandContext : IBasicContext, IVoiceCommandContext
    { 

        /// <summary>
        /// 数据库操作类
        /// </summary>
        private IBaseDataBaseOperate<VoiceCommandModel> databaseoperate = null;

        public VoiceCommandContext(IBaseDataBaseOperate<VoiceCommandModel> _databaseoperate)
        {
            databaseoperate = _databaseoperate;
        }

        /// <summary>
        /// 保存指令
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool SaveCommand(VoiceCommandModel model)
        {
            return databaseoperate.SaveToDataBase(model); 
             
        }

        /// <summary>
        /// 读取指令
        /// </summary>
        /// <param name="drivewayGuid">车道唯一id</param>
        /// <returns></returns>
        public VoiceCommandModel GetCommand(string drivewayGuid)
        {
            return databaseoperate.GetFromDataBase(drivewayGuid);
        }

    }
}
