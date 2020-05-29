using Autofac;
using Autofac.Integration.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.Tools
{
    public interface IServiceGetter
    {
        T GetByName<T>(string name);
    }
    public class AutofacHelper : IServiceGetter
    {
        public T GetByName<T>(string name)
        {
            return AutofacDependencyResolver.Current.RequestLifetimeScope.ResolveNamed<T>(name);
        }
    }
}
