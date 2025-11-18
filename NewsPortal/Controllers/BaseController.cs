using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class BaseController : Controller
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var rqf = Request.HttpContext.Features.Get<IRequestCultureFeature>();
        var currentCulture = rqf.RequestCulture.Culture.Name;

        ViewBag.CurrentCulture = currentCulture.ToUpper();
        ViewBag.SwitchCulture = currentCulture == "ru" ? "en" : "ru";

        base.OnActionExecuting(context);
    }
}
