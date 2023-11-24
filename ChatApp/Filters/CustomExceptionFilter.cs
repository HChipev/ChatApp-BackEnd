using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Back_End.Filters
{
    public class CustomExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            var message = context.Exception.Message;

            var targetSide = context.Exception.TargetSite.DeclaringType.FullName;

            var stackTrace = context.Exception.StackTrace;

            var logMessage = $"*******{targetSide}-------{message}*******{stackTrace}*******{Environment.NewLine}";

            context.Result = new BadRequestResult();
        }
    }
}