using Microsoft.AspNetCore.Mvc.Filters;

namespace VitalVues;

public class NoCacheHeadersAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var response = context.HttpContext.Response;
        response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
        response.Headers["Pragma"] = "no-cache";
        response.Headers["Expires"] = "0";

        base.OnActionExecuting(context);
    }
}
