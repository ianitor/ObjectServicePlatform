using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Ianitor.Osp.Backend.Common.ApiErrors;
using Ianitor.Osp.Backend.Persistence.SystemEntities;
using Ianitor.Osp.Common.Shared.DataTransferObjects;
using IdentityModel;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Ianitor.Osp.Backend.Identity.SystemApi.v1.Controllers
{
    /// <summary>
    /// REST Controller for user management
    /// </summary>
    [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
    [Route(IdentityServiceConstants.ApiPathPrefix + "/[controller]")]
    [ApiController]
    [ApiVersion(IdentityServiceConstants.ApiVersion1)]
    public class IdentitiesController : ControllerBase
    {
        private readonly UserManager<OspUser> _userManager;
        private readonly RoleManager<OspRole> _roleManager;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userManager">The storage service of users</param>
        /// <param name="roleManager">The storage service of roles</param>
        public IdentitiesController(
            UserManager<OspUser> userManager, RoleManager<OspRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET system/v1/identities
        /// <summary>
        /// Returns all existing users
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(IdentityServiceConstants.IdentityApiReadOnlyPolicy)]
        public async Task<IEnumerable<UserDto>> Get()
        {
            var list = new List<UserDto>();
            foreach (var applicationUser in _userManager.Users)
            {
                var userDto = await CreateUserDto(applicationUser, _userManager);
                list.Add(userDto);
            }

            return list;
        }
        
        // GET system/v1/identities/getPaged
        /// <summary>
        /// Returns all existing users
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetPaged")]
        [Authorize(IdentityServiceConstants.IdentityApiReadOnlyPolicy)]

        public async Task<PagedResult<UserDto>> Get([Required][FromQuery] PagingParams pagingParams)
        {
            var list = new List<UserDto>();
            foreach (var applicationUser in _userManager.Users.Skip(pagingParams.Skip).Take(pagingParams.Take))
            {
                var userDto = await CreateUserDto(applicationUser, _userManager);
                list.Add(userDto);
            }
            
            var pagedResult = new PagedResult<UserDto>(list, pagingParams.Skip, pagingParams.Take, _userManager.Users.Count());
            
            Response.Headers.Add("X-Pagination", pagedResult.GetHeader().ToJson());  

            return pagedResult;
        }
        
        // GET system/v1/identities/{userName}
        /// <summary>
        /// Returns user information based on it's id
        /// </summary>
        /// <param name="userName">Name of the user</param>
        /// <returns>An Object that describes the user.</returns>
        [HttpGet("{userName}")]
        [Authorize(IdentityServiceConstants.IdentityApiReadOnlyPolicy)]
        public async Task<IActionResult> Get([Required]string userName)
        {
            var ospUser = await _userManager.FindByNameAsync(userName);
            if (ospUser == null)
            {
                return NotFound();
            }

            return Ok(await CreateUserDto(ospUser, _userManager));
        }


        // POST system/v1/identities
        /// <summary>
        /// Creates a new user
        /// </summary>
        /// <param name="userDto">The user data transfer object instance</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(IdentityServiceConstants.IdentityApiReadWritePolicy)]
        public async Task<IActionResult> Post([Required] [FromBody] UserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var applicationUser = new OspUser();

            try
            {
                await ApplyToUser(applicationUser, userDto);
            }
            catch (RoleNotFoundException e)
            {
                return BadRequest(new OperationFailedError(e.Message));
            }

            try
            {
                var result = await _userManager.CreateAsync(applicationUser);
                if (result.Succeeded)
                {
                    return Ok();
                }

                return BadRequest(new OperationFailedError("Creation of user failed",
                    result.Errors.Select(x => new FailedDetails {Code = x.Code, Description = x.Description})));
            }
            catch (Exception e)
            {
                return BadRequest(new InternalServerError(e.Message));
            }
        }

        // PUT system/v1/identities/5
        /// <summary>
        /// Updates an user
        /// </summary>
        /// <param name="userName">The user name</param>
        /// <param name="userDto">The client data transfer object instance</param>
        /// <returns></returns>
        [HttpPut("{userName}")]
        [Authorize(IdentityServiceConstants.IdentityApiReadWritePolicy)]
        public async Task<IActionResult> Put([Required] string userName, [Required] [FromBody] UserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var applicationUser = await _userManager.FindByNameAsync(userName);
            if (applicationUser == null)
            {
                return NotFound(new NotFoundError($"User name '{userName}' not found."));
            }

            try
            {
                await ApplyToUser(applicationUser, userDto);
            }
            catch (RoleNotFoundException e)
            {
                return BadRequest(new OperationFailedError(e.Message));
            }

            try
            {
                var result = await _userManager.UpdateAsync(applicationUser);
                if (result.Succeeded)
                {
                    return Ok();
                }

                return BadRequest(new OperationFailedError("Update of user failed",
                    result.Errors.Select(x => new FailedDetails {Code = x.Code, Description = x.Description})));
            }
            catch (Exception e)
            {
                return BadRequest(new InternalServerError(e.Message));
            }
        }

        // POST: system/v1/identities/resetPassword
        /// <summary>
        /// Resets the password of an user
        /// </summary>
        /// <param name="userName">The user name</param>
        /// <param name="password">The new password</param>
        /// <returns></returns>
        [HttpPost]
        [Route("ResetPassword")]
        [Authorize(IdentityServiceConstants.IdentityApiReadWritePolicy)]
        public async Task<IActionResult> ResetPassword([Required] string userName, [Required] string password)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(userName);
                if (user == null)
                {
                    return NotFound(new NotFoundError($"User '{userName}' not found."));
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                var result = await _userManager.ResetPasswordAsync(user, token, password);
                if (result.Succeeded)
                {
                    return Ok();
                }

                return BadRequest(new OperationFailedError("Reset of password failed.",
                    result.Errors.Select(x => new FailedDetails {Code = x.Code, Description = x.Description})));
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(new InternalServerError(e.Message));
            }
        }

        // DELETE system/v1/identities/5
        /// <summary>
        /// Deletes an user
        /// </summary>
        /// <param name="userName">The user name</param>
        /// <returns></returns>
        [HttpDelete("{userName}")]
        [Authorize(IdentityServiceConstants.IdentityApiReadWritePolicy)]
        public async Task<IActionResult> Delete([Required] string userName)
        {
            var applicationUser = await _userManager.FindByNameAsync(userName);
            if (applicationUser == null)
            {
                return NotFound(new NotFoundError($"User name '{userName}' not found."));
            }

            try
            {
                var result = await _userManager.DeleteAsync(applicationUser);
                if (result.Succeeded)
                {
                    return Ok();
                }

                return BadRequest(new OperationFailedError("Delete of user failed",
                    result.Errors.Select(x => new FailedDetails {Code = x.Code, Description = x.Description})));
            }
            catch (Exception e)
            {
                return BadRequest(new InternalServerError(e.Message));
            }
        }

        private async Task<UserDto> CreateUserDto(OspUser applicationUser, UserManager<OspUser> userManager)
        {
            var userDto = new UserDto
            {
                UserId = applicationUser.Id.ToString(),
                FirstName = applicationUser.FirstName,
                LastName = applicationUser.LastName,
                Email = applicationUser.Email ?? applicationUser.Claims.FirstOrDefault(x => x.ClaimType == JwtClaimTypes.Email)?.ClaimValue,
                Name = applicationUser.UserName ?? applicationUser.Claims.FirstOrDefault(x => x.ClaimType == JwtClaimTypes.Name)?.ClaimValue
            };

            var roles = new List<RoleDto>();
            foreach (var role in await userManager.GetRolesAsync(applicationUser))
            {
                roles.Add(RolesController.CreateRoleDto(await _roleManager.FindByNameAsync(role)));
            }
            userDto.Roles = roles;
            
            return userDto;
        }

        private async Task ApplyToUser(OspUser applicationUser, UserDto userDto)
        {
            var roleIds = new List<string>();

            foreach (var roleDto in userDto.Roles)
            {
                if (roleDto == null)
                {
                    continue;
                }

                var role = await _roleManager.FindByIdAsync(roleDto.Id);
                if (role == null)
                {
                    throw new RoleNotFoundException($"Role '{roleDto.Name}' does not exist.");
                }

                roleIds.Add(role.Id.ToString());
            }


            if (!string.IsNullOrWhiteSpace(userDto.Name))
            {
                applicationUser.UserName = userDto.Name;
            }

            applicationUser.Roles.AddRange(roleIds);
            applicationUser.Email = userDto.Email;
            applicationUser.FirstName = userDto.FirstName;
            applicationUser.LastName = userDto.LastName;
        }
    }
}