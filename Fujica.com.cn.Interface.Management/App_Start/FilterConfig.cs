﻿using Fujica.com.cn.Interface.Management.Filter;
using System.Web;
using System.Web.Mvc;

namespace Fujica.com.cn.Interface.Management
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
