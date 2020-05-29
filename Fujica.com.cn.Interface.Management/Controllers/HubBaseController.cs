using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Web.Http;

namespace Fujica.com.cn.Interface.Management.Controllers
{
    /// <summary>
    ///  Hub 类继承于 Hub<T>，这样就可以指定你的客户端可以调用的方法，也可以在你的 Hub 方法里开启代码提示
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class HubBaseController<T> : ApiController where T : Hub
    {
        protected IHubConnectionContext<dynamic> Clients { get; private set; }
        protected IGroupManager Groups { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        protected HubBaseController()
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<T>();
            Clients = context.Clients;
            Groups = context.Groups;
        }
    }
}
