using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Ianitor.Osp.Backend.Common.ApiErrors;
using Ianitor.Osp.Backend.Persistence.SystemEntities;
using Ianitor.Osp.Common.Shared.DataTransferObjects;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace Ianitor.Osp.Backend.Identity.SystemApi.v1.Controllers
{
    /// <summary>
    /// REST Controller for role management
    /// </summary>
    [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
    [Route(IdentityServiceConstants.ApiPathPrefix + "/[controller]")]
    [ApiController]
    [ApiVersion(IdentityServiceConstants.ApiVersion1)]
    public class RolesController : ControllerBase
    {
        private readonly RoleManager<OspRole> _roleManager;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="roleManager">The storage service of roles</param>
        public RolesController(RoleManager<OspRole> roleManager)
        {
            _roleManager = roleManager;
        }

        // GET system/v1/roles
        /// <summary>
        /// Returns all existing roles
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(IdentityServiceConstants.IdentityApiReadOnlyPolicy)]
        public PagedResult<RoleDto> Get([Required] [FromQuery] PagingParams pagingParams)
        {
            var list = new List<RoleDto>();

            var query = _roleManager.Roles.AsQueryable();
            if (!string.IsNullOrWhiteSpace(pagingParams.Filter))
            {
                query = _roleManager.Roles.Where(x => x.Name.ToLower().Contains(pagingParams.Filter.ToLower()));
            }

            foreach (var ospRole in query.Skip(pagingParams.Skip).Take(pagingParams.Take))
            {
                var roleDto = CreateRoleDto(ospRole);
                list.Add(roleDto);
            }

            var pagedResult = new PagedResult<RoleDto>(list, pagingParams.Skip, pagingParams.Take, query.Count());

            Response.Headers.Add("X-Pagination", pagedResult.GetHeader().ToJson());

            return pagedResult;
        }

        // GET system/v1/roles/names/{roleName}
        /// <summary>
        /// Returns role information based on it's name
        /// </summary>
        /// <param name="roleName">Name of the role</param>
        /// <returns>An Object that describes the role.</returns>
        [HttpGet("names/{roleName}")]
        [Authorize(IdentityServiceConstants.IdentityApiReadOnlyPolicy)]
        public async Task<IActionResult> Get([Required] string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
            {
                return NotFound();
            }

            return Ok(CreateRoleDto(role));
        }

        // POST system/v1/roles
        /// <summary>
        /// Creates a new role
        /// </summary>
        /// <param name="roleDto">The role data transfer object instance</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(IdentityServiceConstants.IdentityApiReadWritePolicy)]
        public async Task<IActionResult> Post([Required] [FromBody] RoleDto roleDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ospRole = new OspRole();
            ApplyToRole(ospRole, roleDto);

            try
            {
                var result = await _roleManager.CreateAsync(ospRole);
                if (result.Succeeded)
                {
                    return Ok();
                }

                return BadRequest(new OperationFailedError("Creation of role failed",
                    result.Errors.Select(x => new FailedDetails {Code = x.Code, Description = x.Description})));
            }
            catch (Exception e)
            {
                return BadRequest(new InternalServerError(e.Message));
            }
        }

        // PUT system/v1/role/5
        /// <summary>
        /// Updates a role
        /// </summary>
        /// <param name="roleName">The role name</param>
        /// <param name="roleDto">The role data transfer object instance</param>
        /// <returns></returns>
        [HttpPut("{roleName}")]
        [Authorize(IdentityServiceConstants.IdentityApiReadWritePolicy)]
        public async Task<IActionResult> Put([Required] string roleName, [Required] [FromBody] RoleDto roleDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ospRole = await _roleManager.FindByNameAsync(roleName);
            if (ospRole == null)
            {
                return NotFound(new NotFoundError($"Role '{roleName}' not found."));
            }

            ApplyToRole(ospRole, roleDto);

            try
            {
                var result = await _roleManager.UpdateAsync(ospRole);
                if (result.Succeeded)
                {
                    return Ok();
                }

                return BadRequest(new OperationFailedError("Update of role failed",
                    result.Errors.Select(x => new FailedDetails {Code = x.Code, Description = x.Description})));
            }
            catch (Exception e)
            {
                return BadRequest(new InternalServerError(e.Message));
            }
        }

        // DELETE system/v1/role/5
        /// <summary>
        /// Deletes a role
        /// </summary>
        /// <param name="roleName">The role name</param>
        /// <returns></returns>
        [HttpDelete("{roleName}")]
        [Authorize(IdentityServiceConstants.IdentityApiReadWritePolicy)]
        public async Task<IActionResult> Delete([Required] string roleName)
        {
            var ospRole = await _roleManager.FindByNameAsync(roleName);
            if (ospRole == null)
            {
                return NotFound(new NotFoundError($"Role '{roleName}' not found."));
            }

            try
            {
                var result = await _roleManager.DeleteAsync(ospRole);
                if (result.Succeeded)
                {
                    return Ok();
                }

                return BadRequest(new OperationFailedError("Delete of role failed",
                    result.Errors.Select(x => new FailedDetails {Code = x.Code, Description = x.Description})));
            }
            catch (Exception e)
            {
                return BadRequest(new InternalServerError(e.Message));
            }
        }

        internal static RoleDto CreateRoleDto(OspRole ospRole)
        {
            var roleDto = new RoleDto
            {
                Id = ospRole.Id.ToString(),
                Name = ospRole.Name,
            };
            return roleDto;
        }

        private void ApplyToRole(OspRole ospRole, RoleDto roleDto)
        {
            if (!string.IsNullOrWhiteSpace(roleDto.Id))
            {
                ospRole.Id = new ObjectId(roleDto.Id);
            }

            ospRole.Name = roleDto.Name;
        }
    }
}