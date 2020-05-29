using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using Fujica.com.cn.Container;
using Fujica.com.cn.Logger;
using Fujica.com.cn.Security.AdmissionControl;
using Fujica.com.cn.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Fujica.com.cn.Interface.Management
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        public static ContainerBuilder builder;
        public static IContainer _container; //可用于手工反转出一个对象 _container.Resolve<ParkLotManager>();

        protected void Application_Start()
        {

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register); //api路由
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

            //RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);  

            //设定api的返回格式
            GlobalConfiguration.Configuration.Formatters.Insert(0, new JsonMediaTypeFormatter());
            //去除掉xml格式支持
            GlobalConfiguration.Configuration.Formatters.XmlFormatter.SupportedMediaTypes.Clear();
            //去掉form表单格式支持
            GlobalConfiguration.Configuration.Formatters.FormUrlEncodedFormatter.SupportedMediaTypes.Clear();
/*****************************************************************************************************************************/
            //IOC容器绑定器
            builder = new ContainerBuilder();

            //注册下层公用对象到公共容器
            CommonContainer.RegTools(builder);
            CommonContainer.RegSecurity(builder);
            CommonContainer.RegCommunication(builder);
            CommonContainer.RegContext(builder);
            CommonContainer.RegDataService(builder);
            CommonContainer.RegBusiness(builder);

            //注册本项目API接口
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly()).InstancePerRequest();
            builder.RegisterWebApiFilterProvider(GlobalConfiguration.Configuration);
            builder.RegisterWebApiModelBinderProvider();

            //创建容器
            _container = builder.Build();

            //注入
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(_container);
            DependencyResolver.SetResolver(new AutofacDependencyResolver(_container));
            /*****************************************************************************************************************************/
        }
    }
}
