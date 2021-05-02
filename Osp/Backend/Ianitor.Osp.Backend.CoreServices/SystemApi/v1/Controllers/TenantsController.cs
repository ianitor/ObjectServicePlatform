using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Ianitor.Osp.Backend.CoreServices.GraphQL.Caches;
using Ianitor.Osp.Backend.CoreServices.Services;
using Ianitor.Osp.Backend.Persistence;
using Ianitor.Osp.Common.Shared.DataTransferObjects;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ianitor.Osp.Backend.CoreServices.SystemApi.v1.Controllers
{
    /// <summary>
    /// REST Controller for tenants management
    /// </summary>
    [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
    [Route("system/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class TenantsController : ControllerBase
    {
        private readonly IOspService _ospService;
        private readonly ISchemaContext _schemaContext;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ospService">OSP service for tenant management</param>
        /// <param name="schemaContext">The Schema service of graph ql</param>
        public TenantsController(IOspService ospService, ISchemaContext schemaContext)
        {
            _ospService = ospService;
            _schemaContext = schemaContext;
        }
        
        // GET system/v1/tenants
        /// <summary>
        /// Returns all existing tenants using pages
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(CoreServiceConstants.SystemApiReadOnlyPolicy)]
        public async Task<IActionResult> Get([FromQuery] PagingParams pagingParams)
        {
            using var session = await _ospService.SystemContext.StartSystemSessionAsync();
            session.StartTransaction();
            
            var result = await _ospService.SystemContext.GetTenantsAsync(session, pagingParams?.Skip, pagingParams?.Take);

            if (pagingParams != null)
            {
                var pagedResult = new PagedResult<TenantDto>(result.List.Select(CreateTenantDto),
                    pagingParams.Skip, pagingParams.Take, result.TotalCount);

                Response.Headers.Add("X-Pagination", pagedResult.GetHeader().ToJson());
                
                return Ok(pagedResult);
            }
            
            await session.CommitTransactionAsync();
            
            return Ok(result.List.Select(CreateTenantDto));
        }
        
        // GET system/v1/tenants/{id}
        /// <summary>
        /// Returns client information based on it's client id
        /// </summary>
        /// <param name="id">Id of the client</param>
        /// <returns>An Object that describes the client.</returns>
        [HttpGet("{id}")]
        [Authorize(CoreServiceConstants.SystemApiReadOnlyPolicy)]
        public async Task<IActionResult> Get([Required]string id)
        {
            using var session = await _ospService.SystemContext.StartSystemSessionAsync();
            session.StartTransaction();
            
            var ospTenant = await _ospService.SystemContext.GetTenantAsync(session, id);
            if (ospTenant == null)
            {
                return NotFound();
            }
            
            await session.CommitTransactionAsync();

            return Ok(CreateTenantDto(ospTenant));
        }

        // POST: system/v1/tenants?tenantId=abc&databaseName=xyz
        /// <summary>
        /// Creates new tenants
        /// </summary>
        /// <param name="tenantId">Id of tenant</param>
        /// <param name="databaseName">Name of database</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(CoreServiceConstants.SystemApiReadWritePolicy)]
        public async Task<IActionResult> Post([Required] string tenantId, [Required] string databaseName)
        {
            using var session = await _ospService.SystemContext.StartSystemSessionAsync();
            session.StartTransaction();
            
            try
            {
                await _ospService.SystemContext.CreateTenantAsync(session, databaseName, tenantId);
                await session.CommitTransactionAsync();
                return NoContent();
            }
            catch (DatabaseException e)
            {
                return Conflict(e.Message);
            }
            catch (TenantException e)
            {
                return Conflict(e.Message);
            }
        }
        
        // POST: system/v1/tenants/attach?tenantId=abc&databaseName=xyz
        /// <summary>
        /// Appends an existing database as tenant
        /// </summary>
        /// <param name="tenantId">Id of tenant</param>
        /// <param name="databaseName">Name of database (have to exist)</param>
        /// <returns></returns>
        [HttpPost("attach")]
        [Authorize(CoreServiceConstants.SystemApiReadWritePolicy)]
        public async Task<IActionResult> Attach([Required] string tenantId, [Required] string databaseName)
        {
            using var session = await _ospService.SystemContext.StartSystemSessionAsync();
            session.StartTransaction();
            
            try
            {
                await _ospService.SystemContext.AttachTenantAsync(session, databaseName, tenantId);
                await session.CommitTransactionAsync();
                return NoContent();
            }
            catch (DatabaseException e)
            {
                return Conflict(e.Message);
            }
            catch (TenantException e)
            {
                return Conflict(e.Message);
            }
        }
        
        // POST: system/v1/tenants/detach?tenantId=abc&databaseName=xyz
        /// <summary>
        /// Appends an existing database as tenant
        /// </summary>
        /// <param name="tenantId">Id of tenant</param>
        /// <returns></returns>
        [HttpPost("detach")]
        [Authorize(CoreServiceConstants.SystemApiReadWritePolicy)]
        public async Task<IActionResult> Detach([Required] string tenantId)
        {
            using var session = await _ospService.SystemContext.StartSystemSessionAsync();
            session.StartTransaction();
            
            try
            {
                await _ospService.SystemContext.DetachTenantAsync(session, tenantId);
                await session.CommitTransactionAsync();
                return NoContent();
            }
            catch (DatabaseException e)
            {
                return Conflict(e.Message);
            }
            catch (TenantException e)
            {
                return Conflict(e.Message);
            }
        }

        // PUT: system/test/clear/TestOsp
        /// <summary>
        /// Clears the content of a tenant
        /// </summary>
        /// <param name="tenantId">Name of tenant</param>
        /// <returns></returns>
        [HttpPut("clear")]
        [Authorize(CoreServiceConstants.SystemApiReadWritePolicy)]
        public async Task<IActionResult> Clear([Required] string tenantId)
        {
            using var session = await _ospService.SystemContext.StartSystemSessionAsync();
            session.StartTransaction();
            
            try
            {
                await _ospService.SystemContext.ClearTenantAsync(session, tenantId);
                await session.CommitTransactionAsync();
                return Ok();
            }
            catch (TenantException e)
            {
                return Conflict(e.Message);
            }            
        }
        
        
        // PUT: system/test/clear/TestOsp
        /// <summary>
        /// Clears the caches of a tenant
        /// </summary>
        /// <param name="tenantId">ID of tenant</param>
        /// <returns></returns>
        [HttpPut("clearCache")]
        [Authorize(CoreServiceConstants.SystemApiReadWritePolicy)]
        public IActionResult ClearCache([Required] string tenantId)
        {
            try
            {
                // Clear GraphQL cache
                _schemaContext.Invalidate(tenantId);
                
                // Dispose data context
                if (_ospService.SystemContext.TryGetCkCache(tenantId, out var ckCache))
                {
                    ckCache.Dispose();
                }
                
                return Ok("Cache cleared");
            }
            catch (Exception e)
            {
                return Conflict(e.Message);
            }            
        }

        // DELETE: system/tenants/TestOsp
        /// <summary>
        /// Deletes a tenant
        /// </summary>
        /// <param name="tenantId">Id of tenant</param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize(CoreServiceConstants.SystemApiReadWritePolicy)]
        public async Task<IActionResult> Delete([Required] string tenantId)
        {
            using var session = await _ospService.SystemContext.StartSystemSessionAsync();
            session.StartTransaction();
            
            try
            {
                await _ospService.SystemContext.DropTenantAsync(session, tenantId);
                await session.CommitTransactionAsync();
                return Ok();
            }
            catch (TenantException e)
            {
                return NotFound(e.Message);
            }
        }

        private TenantDto CreateTenantDto(OspTenant ospTenant)
        {
            var tenantDto = new TenantDto
            {
                TenantId = ospTenant.TenantId,
                Database = ospTenant.DatabaseName
            };
            return tenantDto;
        }
    }
}