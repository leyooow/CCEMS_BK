using Application.Contracts.Services;
using Application.Models.DTOs.Group;
using Application.Models.DTOs.User.role;
using Application.Models.DTOs.User.user;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAll()
        {
            var response = await _userService.GetAllAsync();
            return Ok(response);
        }
        [HttpGet("GetPaginatedUsers")]
        public async Task<IActionResult> GetPaginated([FromQuery] int? pageNumber = 1, [FromQuery] int? pageSize = 10, [FromQuery] string? searchTerm = null)
        {

            var response = await _userService.GetPaginatedAsync(pageNumber, pageSize, searchTerm);
            return Ok(response);
        }

        [HttpGet("GetUserById/{id}")]
        public async Task<ActionResult<UserDTO>> GetById(string id)
        {
            var response = await _userService.GetUserByIdAsync(id);
            return Ok(response);

        }

        [HttpGet("GetAllRoles")]
        public async Task<IActionResult> GetallRoles()
        {
            var response = await _userService.GetAllRolesAsync();
            return Ok(response);
        }


        [HttpGet("GetAllPermissionLookups")]
        public async Task<IActionResult> GetAllPermissionLookUps()
        {
            var response = await _userService.GetAllPermissionLookUpAsync();
            return Ok(response);
        }


        [HttpGet("GetPermissionsByRoleId/{roleId}")]
        public async Task<IActionResult> GetPermissionsByRoleId(int roleId)
        {
            var permissions = await _userService.GetPermissionsByRoleId(roleId);

            return Ok(permissions);
        }

        [HttpPost("AddPermissions")]
        public async Task<IActionResult> AddPermissions([FromBody] AddPermissionRequest addPermissionRequest)
        {
            var response = await _userService.AddPermissionsAsync(addPermissionRequest);

            return Ok(response);
        }

        [HttpGet("CheckAdUsername/{username}")]
        public async Task<IActionResult> CheckUserNameAsync(string username)
        {
            var response = await _userService.CheckUserNameAsync(username);
            return Ok(response);
        }

        [HttpPost("CreateUser")]
        public async Task<IActionResult> Create(UserCreateDTO userCreateDto)
        {
            var response = await _userService.AddUserAsync(userCreateDto);
            return Ok(response);
        }

        [HttpPut("UpdateUser")]
        public async Task<IActionResult> UpdateUser([FromBody] UserUpdateDTO userUpdateDTO)
        {
            var response = await _userService.UpdateUserAsync(userUpdateDTO);
            return Ok(response);
        }

        [HttpDelete("DeleteUser/{employeeId}")]
        public async Task<IActionResult> DeleteUser(string employeeId)
        {
            var response = await _userService.DeleteUserAsync(employeeId);
            return Ok(response);
        }
    }
}
