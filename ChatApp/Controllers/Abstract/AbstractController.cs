using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Back_End.Controllers.Abstract
{
    public class AbstractController : ControllerBase
    {
        protected int GetUserId()
        {
            return int.TryParse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value,
                out var userId)
                ? userId
                : throw new Exception("Not a valid user id!");
        }
    }
}