using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using Hangfire;
using Ianitor.Osp.Backend.Common.ApiErrors;
using Ianitor.Osp.Backend.DistributedCache;
using Ianitor.Osp.Backend.Jobs.Jobs;
using Ianitor.Osp.Common.Shared.DataTransferObjects;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ianitor.Osp.Backend.CoreServices.SystemApi.v1.Controllers
{
    /// <summary>
    /// REST Controller for CK and RT model management
    /// </summary>
    [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
    [Route("system/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class ModelsController : ControllerBase
    {
        private readonly IDistributedWithPubSubCache _distributedCache;
        private readonly IBackgroundJobClient _backgroundJobClient;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="distributedCache">Instance of distributed cache</param>
        /// <param name="backgroundJobClient">The hangfire job client</param>
        public ModelsController(IDistributedWithPubSubCache distributedCache, IBackgroundJobClient backgroundJobClient)
        {
            _distributedCache = distributedCache;
            _backgroundJobClient = backgroundJobClient;
        }

        // POST: system/Models/ExportRt
        /// <summary>
        /// Exports a runtime model
        /// </summary>
        /// <param name="tenantId">Id of tenant the request relies to</param>
        /// <param name="exportModelRequestDto">The query, whose result data should be exported</param>
        /// <returns></returns>
        [HttpPost]
        [Route("ExportRt")]
        [Authorize(CoreServiceConstants.SystemApiReadOnlyPolicy)]
        public IActionResult ExportRt([Required] string tenantId,
            [FromBody] ExportModelRequestDto exportModelRequestDto)
        {
            try
            {
                var id = _backgroundJobClient.Enqueue<ExportModelJob>(job =>
                    job.ExportRtAsync(tenantId, exportModelRequestDto.QueryId.ToString(), JobCancellationToken.Null));
                return Ok(new ExportModelResponseDto {JobId = id});
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(new InternalServerError(e.Message));
            }
        }


        // POST: system/Models/ImportRt
        /// <summary>
        /// Imports a runtime model
        /// </summary>
        /// <param name="tenantId">Id of tenant the request relies to</param>
        /// <param name="file">The file with the RT model definition</param>
        /// <returns></returns>
        [HttpPost]
        [RequestSizeLimit(300_000_000)]
        [Route("ImportRt")]
        [Authorize(CoreServiceConstants.SystemApiReadWritePolicy)]
        public async Task<IActionResult> ImportRt([Required] string tenantId, [FromForm] IFormFile file)
        {
            try
            {
                var cacheKey = await AddFileToCache(file);
                var id = _backgroundJobClient.Enqueue<ImportModelJob>(job =>
                    job.ImportRtAsync(tenantId, cacheKey, JobCancellationToken.Null));
                return Ok(id);
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(new InternalServerError(e.Message));
            }
        }

        // POST: system/Models/ImportCk
        /// <summary>
        /// Imports a construction kit model
        /// </summary>
        /// <param name="tenantId">Id of tenant the request relies to</param>
        /// <param name="scopeId">The scope id of the model to import</param>
        /// <param name="file">The file with the CK model definition</param>
        /// <returns></returns>
        [HttpPost]
        //[Consumes("application/zip", "application/zip")]
        [Route("ImportCk")]
        [Authorize(CoreServiceConstants.SystemApiReadWritePolicy)]
        public async Task<IActionResult> ImportCk([Required] string tenantId, ScopeIdsDto scopeId, [FromForm] IFormFile file)
        {
            try
            {
                var cacheKey = await AddFileToCache(file);
                var id = _backgroundJobClient.Enqueue<ImportModelJob>(job =>
                    job.ImportCkAsync(tenantId, cacheKey, scopeId, JobCancellationToken.Null));
                return Ok(id);
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(new InternalServerError(e.Message));
            }
        }

        private async Task<string> AddFileToCache(IFormFile file)
        {
            await using (MemoryStream memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                var key = Guid.NewGuid().ToString();
                await _distributedCache.Database.StringSetAsync(key + "contentType", file.ContentType);
                await _distributedCache.Database.StringSetAsync(key + "value", memoryStream.ToArray());
                return key;
            }
        }
    }
}