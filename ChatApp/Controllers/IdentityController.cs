using Back_End.Controllers.Abstract;
using Data.ViewModels.Identity.Models;
using Data.ViewModels.Token.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;

namespace Back_End.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityController : AbstractController
    {
        private readonly IIdentityService _identityService;

        public IdentityController(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            var result = await _identityService.LogoutAsync();
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.Message);
        }

        [HttpGet("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromHeader(Name = "Authorization")] string authorizationHeader)
        {
            var tokenViewModel = new TokenViewModel
            {
                Token = authorizationHeader.Replace("Bearer ", string.Empty)
            };


            var result = await _identityService.RefreshTokenAsync(tokenViewModel);
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.Message);
        }

        [HttpPost("google-login")]
        [AllowAnonymous]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginViewModel googleUserModel)
        {
            var result = await _identityService.GoogleLoginAsync(googleUserModel);
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.Message);
        }
    }
}