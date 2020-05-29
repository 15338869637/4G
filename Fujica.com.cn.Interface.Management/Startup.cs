using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;

[assembly: OwinStartup(typeof(Fujica.com.cn.Interface.Management.Startup))]

namespace Fujica.com.cn.Interface.Management
{
    /// <summary>
    /// 让客户端能够连接到 Hub ，当程序启动的时候需要调用 MapSignalR 方法。
    /// </summary>
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll); //支持跨域
            app.MapSignalR(); 
        

        }
    }
}
