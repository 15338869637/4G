using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.Business.Model
{
    /// <summary>
    /// 显示屏与语音模板模型
    /// </summary>
    public class VoiceCommandDetialModel
    {
        /// <summary>
        /// 命令类型(名称)
        /// </summary>
        public string CommandType { get; set; }

        /// <summary>
        /// 语音展示
        /// </summary>
        public string ShowVoice { get; set; }

        /// <summary>
        /// 文字展示
        /// </summary>
        public string ShowText { get; set; }

        /// <summary>
        /// 相机标识
        /// </summary>
        public string DeviceIdentify { get; set; }
    }
}
