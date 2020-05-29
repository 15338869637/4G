using System.Web;
using System.Web.Mvc;

namespace Fujica.com.cn.Interface.DataUpload
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
