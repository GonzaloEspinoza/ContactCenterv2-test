using Backend.Models;
using System.Web;
using System.Web.Mvc;

namespace Backend
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new CCAuthorizeAttribute());
            filters.Add(new HandleErrorAttribute());
        }
    }
}
