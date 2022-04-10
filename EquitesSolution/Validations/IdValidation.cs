using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Validations
{
    public class IdValidation : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if ((int)context.ActionArguments["id"] <= 0)
                context.Result = new BadRequestResult();
        }
    }
}
