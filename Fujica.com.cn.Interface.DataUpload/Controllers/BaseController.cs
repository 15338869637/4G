using Fujica.com.cn.Interface.DataUpload.Filter;
using Fujica.com.cn.Logger;
using Fujica.com.cn.Security.AdmissionControl;
using Fujica.com.cn.Tools;
using System.Web.Http;

namespace Fujica.com.cn.Interface.DataUpload.Controllers
{
    [VerifyRequest]
    public class BaseController : ApiController
    {
        /// <summary>
        /// 日志记录器
        /// </summary>
        public readonly ILogger Logger;

        /// <summary>
        /// 序列化器
        /// </summary>
        public readonly ISerializer Serializer;

        /// <summary>
        /// api接入控制
        /// </summary>
        internal readonly APIAccessControl apiaccesscontrol;

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="_logger">日志器</param>
        /// <param name="_serializer">序列化器</param>
        /// <param name="_apiaccesscontrol">权限控制器</param>
        public BaseController(ILogger _logger, ISerializer _serializer, APIAccessControl _apiaccesscontrol)
        {
            Logger = _logger;
            Serializer = _serializer;
            apiaccesscontrol = _apiaccesscontrol;
        }
    }
}
