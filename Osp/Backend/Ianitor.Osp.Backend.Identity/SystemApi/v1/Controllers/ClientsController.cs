using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Ianitor.Osp.Backend.Common.ApiErrors;
using Ianitor.Osp.Backend.DistributedCache;
using Ianitor.Osp.Backend.Persistence.SystemEntities;
using Ianitor.Osp.Backend.Persistence.SystemStores;
using Ianitor.Osp.Common.Shared;
using Ianitor.Osp.Common.Shared.DataTransferObjects;
using IdentityServer4.AccessTokenValidation;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ianitor.Osp.Backend.Identity.SystemApi.v1.Controllers
{
    /// <summary>
    /// REST Controller for client management
    /// </summary>
    [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
    [Route(IdentityServiceConstants.ApiPathPrefix + "/[controller]")]
    [ApiController]
    [ApiVersion(IdentityServiceConstants.ApiVersion1)]
    public class ClientsController : ControllerBase
    {
        private readonly IOspClientStore _ospClientStore;
        private readonly IDistributedWithPubSubCache _distributedCache;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ospClientStore">The storage service of clients</param>
        /// <param name="distributedCache">Distributed cache with REDIS</param>
        public ClientsController(IOspClientStore ospClientStore, IDistributedWithPubSubCache distributedCache)
        {
            _ospClientStore = ospClientStore;
            _distributedCache = distributedCache;
        }

        // GET: system/v1/clients
        /// <summary>
        /// Returns all client definitions
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(IdentityServiceConstants.IdentityApiReadOnlyPolicy)]
        public async Task<IEnumerable<ClientDto>> Get()
        {
            var clients = await _ospClientStore.GetClients();
            return clients.Select(CreateClientDto);
        }
        
        // GET system/v1/clients/getPaged
        /// <summary>
        /// Returns all existing users
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetPaged")]
        [Authorize(IdentityServiceConstants.IdentityApiReadOnlyPolicy)]
        public async Task<PagedResult<ClientDto>> Get([Required][FromQuery] PagingParams pagingParams)
        {
            var list = new List<ClientDto>();

            var clients = (await _ospClientStore.GetClients()).ToArray();
            
            foreach (var applicationUser in clients.Skip(pagingParams.Skip).Take(pagingParams.Take))
            {
                var clientDto = CreateClientDto(applicationUser);
                list.Add(clientDto);
            }
            
            var pagedResult = new PagedResult<ClientDto>(list, pagingParams.Skip, pagingParams.Take, clients.Count());
            
            Response.Headers.Add("X-Pagination", pagedResult.GetHeader().ToJson());  

            return pagedResult;
        }

        // GET api/Clients/5
        /// <summary>
        /// Returns client information based on it's client id
        /// </summary>
        /// <param name="id">Id of the client</param>
        /// <returns>An Object that describes the client.</returns>
        [HttpGet("{id}")]
        [Authorize(IdentityServiceConstants.IdentityApiReadOnlyPolicy)]
        public async Task<IActionResult> Get([Required]string id)
        {
            var client = await _ospClientStore.FindClientByIdAsync(id);
            if (client == null)
            {
                return NotFound();
            }

            return Ok(CreateClientDto(client));
        }

        /// <summary>
        /// Creates a new client
        /// </summary>
        /// <param name="clientDto">The client data transfer object instance</param>
        /// <returns></returns>
        // POST api/Clients
        [HttpPost]
        [Authorize(IdentityServiceConstants.IdentityApiReadWritePolicy)]
        public async Task<IActionResult> Post([Required][FromBody]ClientDto clientDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await _ospClientStore.FindClientByIdAsync(clientDto.ClientId) != null)
            {
                return Conflict($"Client with id '{clientDto.ClientId}' already exists.");
            }

            var appClient = new OspClient
            {

                RequirePkce = true,
                RequireClientSecret = false,

                AccessTokenType = AccessTokenType.Jwt,
                AllowAccessTokensViaBrowser = true,
                AlwaysIncludeUserClaimsInIdToken = true,
                RequireConsent = false
            };
            ApplyToClient(appClient, clientDto);

            try
            {
                await _ospClientStore.CreateAsync(appClient);
                await ClearCacheAsync();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(new InternalServerError(e.Message));
            }
        }

        // PUT api/Clients/5
        /// <summary>
        /// Updates a client
        /// </summary>
        /// <param name="id">Id of the client</param>
        /// <param name="clientDto">The client data transfer object instance</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Authorize(IdentityServiceConstants.IdentityApiReadWritePolicy)]
        public async Task<IActionResult> Put([Required]string id, [Required][FromBody]ClientDto clientDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var appClient = (OspClient)await _ospClientStore.FindClientByIdAsync(id);
            if (appClient == null)
            {
                return NotFound(new NotFoundError($"Client with id '{id}' does not exist."));
            }

            ApplyToClient(appClient, clientDto);

            try
            {
                await _ospClientStore.UpdateAsync(id, appClient);
                await ClearCacheAsync();
            }
            catch (Exception e)
            {
                return BadRequest(new InternalServerError(e.Message));
            }

            return Ok();
        }

        // DELETE api/Clients/5
        /// <summary>
        /// Deletes a client
        /// </summary>
        /// <param name="id">Id of the client</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(IdentityServiceConstants.IdentityApiReadWritePolicy)]
        public async Task<IActionResult> Delete([Required]string id)
        {
            var appClient = (OspClient)await _ospClientStore.FindClientByIdAsync(id);
            if (appClient == null)
            {
                return NotFound(new NotFoundError($"Client with id '{id}' does not exist."));
            }

            try
            {
                await _ospClientStore.DeleteAsync(id);
                await ClearCacheAsync();
            }
            catch (Exception e)
            {
                return BadRequest(new InternalServerError(e.Message));
            }

            return Ok();
        }

        private async Task ClearCacheAsync()
        {
            await _distributedCache.PublishAsync(CacheCommon.KeyCorsClients, Guid.NewGuid().ToString());
        }

        private ClientDto CreateClientDto(Client applicationClient)
        {
            var clientDto = new ClientDto
            {
                IsEnabled = applicationClient.Enabled,
                ClientId = applicationClient.ClientId,
                ClientName = applicationClient.ClientName,
                ClientUri = applicationClient.ClientUri,
                AllowedGrantTypes = applicationClient.AllowedGrantTypes,
                RedirectUris = applicationClient.RedirectUris,
                PostLogoutRedirectUris = applicationClient.PostLogoutRedirectUris,
                AllowedCorsOrigins = applicationClient.AllowedCorsOrigins,
                AllowedScopes = applicationClient.AllowedScopes,
                IsOfflineAccessEnabled = applicationClient.AllowOfflineAccess
            };
            return clientDto;
        }
        private void ApplyToClient(Client applicationClient, ClientDto clientDto)
        {
            applicationClient.Enabled = clientDto.IsEnabled;
            applicationClient.ClientId = clientDto.ClientId;
            applicationClient.ClientName = clientDto.ClientName;
            applicationClient.ClientUri = clientDto.ClientUri;
            applicationClient.AllowedGrantTypes = clientDto.AllowedGrantTypes.ToList();
            applicationClient.RedirectUris = clientDto.RedirectUris?.ToList();
            applicationClient.PostLogoutRedirectUris = clientDto.PostLogoutRedirectUris?.ToList();
            applicationClient.AllowedCorsOrigins = clientDto.AllowedCorsOrigins?.ToList();
            applicationClient.AllowedScopes = CommonConstants.OspDefaultScopes.Concat(clientDto.AllowedScopes).Distinct().ToList();
            applicationClient.AllowOfflineAccess = clientDto.IsOfflineAccessEnabled;

            if (!string.IsNullOrWhiteSpace(clientDto.ClientSecret))
            {
                applicationClient.ClientSecrets = new[]
                {
                    new Secret(clientDto.ClientSecret.Sha256())
                };
            }
        }
    }
}
