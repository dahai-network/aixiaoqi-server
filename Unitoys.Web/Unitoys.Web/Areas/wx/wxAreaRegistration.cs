using System.Web.Mvc;

namespace Unitoys.Web.Areas.wx
{
    public class wxAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "wx";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "wx_default",
                "wx/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}