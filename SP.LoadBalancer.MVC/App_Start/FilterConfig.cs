﻿using System.Web;
using System.Web.Mvc;

namespace SP.LoadBalancer.MVC
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
