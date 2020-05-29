using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;

[assembly: OwinStartup(typeof(demp3.Startup))]

namespace demp3
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
              app.MapSignalR();


            ////Web API的跨域支持组件System.Web.Cors
            //app.Map("/signalr", map =>
            //{
            //    map.UseCors(CorsOptions.AllowAll);
            //    var hubConfiguration = new HubConfiguration
            //    {
            //        EnableJSONP = true

            //    };
            //      map.RunSignalR(hubConfiguration);
            //});
        }
    }
}
