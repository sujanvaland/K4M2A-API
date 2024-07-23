using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SpiritualNetwork.API.AppContext;
using SpiritualNetwork.API;
using System.IdentityModel.Tokens.Jwt;
using SpiritualNetwork.Entities.CommonModel;

namespace SpiritualNetwork.API.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    public class ApiBaseController : Controller
    {
        protected AppDbContext DbContext => (AppDbContext)HttpContext.RequestServices.GetService(typeof(AppDbContext));
        protected int user_unique_id;
        protected string user_email;
        protected string username;
        protected string user_role;
        protected string token;

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            token = HttpContext.Request.Headers["Authorization"];
            GlobalVariables.Token = token;

            if (User.HasClaim(c => c.Type == "Id"))
            {
                string userid = User.Claims.SingleOrDefault(c => c.Type == "Id").Value.ToString();
                user_unique_id = int.Parse(userid);
                GlobalVariables.LoginUserId = user_unique_id;
            }

            if (User.HasClaim(c => c.Type == "Email"))
            {
                var email = User.Claims.SingleOrDefault(c => c.Type == "Email");
                user_email = email.Value.ToString();
                GlobalVariables.LoginUserEmail = user_email;
            }

            if (User.HasClaim(c => c.Type == "Username"))
            {
                var user = User.Claims.SingleOrDefault(c => c.Type == "Username");
                username = user.Value.ToString();
                GlobalVariables.LoginUserName = username;
            }

            if (User.HasClaim(c => c.Type == JwtRegisteredClaimNames.Exp))
            {
                var expClaim = User.Claims.SingleOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp)?.Value;
                if (expClaim == null)
                    return;

                var expDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expClaim)).UtcDateTime;
                if(DateTime.UtcNow > expDate)
                {
                    filterContext.Result = new UnauthorizedObjectResult(new JsonResponse(401, false, message: "Unauthorized request"));

                }
            }
        }

    }
}
