using System.Web;
using System.Web.Mvc;
#pragma warning disable CS1591

namespace BisAceAPI
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
