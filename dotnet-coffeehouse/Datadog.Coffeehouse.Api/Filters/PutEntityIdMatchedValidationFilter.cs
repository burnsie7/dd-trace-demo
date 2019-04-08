using Datadog.Coffeehouse.Core.Extensions;
using Datadog.Coffeehouse.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Datadog.Coffeehouse.Api.Filters
{
    public class PutEntityIdMatchedValidationFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            if (!actionContext.ActionArguments.TryGetValue("id", out var objId) || !(objId is string stringId) || !stringId.IsNotNullOrEmpty())
            {
                return;
            }

            if (!actionContext.ActionArguments.TryGetValue("value", out var objValue) || !(objValue is IHasStringId postObjId))
            {
                return;
            }

            if (!stringId.EqualsOrdinalIgnoreCase(postObjId.Id))
            {
                actionContext.Result = new BadRequestObjectResult("PUT data ID mismatch");
            }
        }
    }
}
