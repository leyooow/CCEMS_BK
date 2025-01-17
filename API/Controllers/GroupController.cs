using Application.Contracts;
using Application.Contracts.Services;
using Application.Models.DTOs;
using Application.Models.DTOs.Group;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class GroupsController : ControllerBase
    {
        private readonly IGroupService _groupService;

        public GroupsController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        [HttpGet("GetAllGroups")]
        public async Task<IActionResult> GetAll()
        {
            var response = await _groupService.GetAllAsync();
            return Ok(response);
        }
        [HttpGet ("GetPaginatedGroups")]
        public async Task<IActionResult> GetPaginated([FromQuery] int? pageNumber = 1, [FromQuery] int? pageSize = 10, [FromQuery] string? searchTerm = null)
        {

            var response = await _groupService.GetPaginatedAsync(pageNumber, pageSize, searchTerm);
            return Ok(response);
        }

        [HttpGet("GetGroupById/{id}")]
        public async Task<ActionResult<GroupDTO>> GetById(int id)
        {
            var response = await _groupService.GetGroupByIdAsync(id);
            return Ok(response);

        }

        [HttpGet("GetBranchDetails")]
        public async Task<IActionResult> GetBranchDetailsAsync([FromQuery] string? branchIds = "")
        {
            try
            {
                var branchIdsList = branchIds?.Split(','); // Split comma-separated values
                var branchDetails = await _groupService.GetBranchDetailsAsync(branchIdsList);
                return Ok(branchDetails);
            }
            catch (Exception ex)
            {
                // Handle and log the exception
                return StatusCode(500, new { message = $"Error retrieving branch details: {ex.Message}" });
            }
        }

        [HttpGet("GetBranchDropdown")]
        public async Task<IActionResult> GetBranchDropdown()
        {
            try 
            {
                var branchDetails = await _groupService.GetBranchDropdown();
                return Ok(branchDetails);
            }
            catch (Exception ex)
            {
                // Handle and log the exception
                return StatusCode(500, new { message = $"Error retrieving branch details: {ex.Message}" });
            }
        }

        [HttpPost("CreateGroup")]
        public async Task<IActionResult> Create(GroupCreateDTO groupCreateDto)
        {
            var response = await _groupService.AddGroupAsync(groupCreateDto);
            return Ok(response);

        }

        [HttpPut("UpdateGroup/{id}")]
        public async Task<IActionResult> Update(int id, GroupUpdateDTO groupUpdateDto)
        {
            var response = await _groupService.UpdateGroupAsync(groupUpdateDto);
            return Ok(response);
        }

        [HttpDelete("DeleteGroup/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _groupService.DeleteGroupAsync(id);
            return Ok(response);
        }
    }
}
