using Back_End.Controllers.Abstract;
using Common.Classes;
using Data.ViewModels.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;

namespace Back_End.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Roles.Admin)]
    public class AdminController : AbstractController
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("user/all")]
        public IActionResult GetUsers()
        {
            var result = _adminService.GetUsers();

            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Message);
        }

        [HttpDelete("user/delete/{userId}")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var result = await _adminService.DeleteUserAsync(userId, GetUserId());

            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Message);
        }

        [HttpDelete("role/delete/{roleId}")]
        public async Task<IActionResult> DeleteRole(int roleId)
        {
            var result = await _adminService.DeleteRoleAsync(roleId, GetUserId());

            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Message);
        }

        [HttpGet("role/all")]
        public IActionResult GetRoles()
        {
            var result = _adminService.GetRoles();

            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Message);
        }

        [HttpPost("role")]
        public async Task<IActionResult> AddRole([FromBody] RoleSimpleViewModel model)
        {
            var result = await _adminService.AddRoleAsync(model, GetUserId());

            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Message);
        }

        [HttpPut("user/role")]
        public async Task<IActionResult> UpdateUserRoles([FromBody] UserRolesViewModel model)
        {
            var result = await _adminService.UpdateUserRolesAsync(model, GetUserId());

            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Message);
        }
    }
}