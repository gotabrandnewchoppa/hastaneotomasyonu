using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HospitalManagementSystem.Web.Filters
{
    public class SessionCheckAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Eğer aksiyon veya controller [AllowAnonymous] ile işaretlenmişse session kontrolü yapma
            var hasAllowAnonymous = context.ActionDescriptor.EndpointMetadata
                .Any(em => em.GetType() == typeof(Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute));

            if (hasAllowAnonymous)
            {
                base.OnActionExecuting(context);
                return;
            }

            // Session kontrolü
            var userEmail = context.HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail))
            {
                // Kullanıcı giriş yapmamışsa Login sayfasına yönlendir
                context.Result = new RedirectToActionResult("Login", "Auth", null);
            }

            base.OnActionExecuting(context);
        }
    }
}
