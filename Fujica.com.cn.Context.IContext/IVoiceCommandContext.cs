using Fujica.com.cn.Context.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.Context.IContext
{

    /// <summary>
    /// 语音指令管理器
    /// </summary>
    public interface IVoiceCommandContext
    {
        /// <summary>
        /// 保存指令
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
         bool SaveCommand(VoiceCommandModel model);

        /// <summary>
        /// 读取指令
        /// </summary>
        /// <param name="drivewayGuid">车道唯一id</param>
        /// <returns></returns>
        VoiceCommandModel GetCommand(string drivewayGuid);

    }
}
