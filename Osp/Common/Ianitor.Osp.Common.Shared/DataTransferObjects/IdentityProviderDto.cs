using System.ComponentModel.DataAnnotations;
using JsonSubTypes;
using Newtonsoft.Json;
using static Ianitor.Osp.Common.Shared.DataTransferObjects.ValidationConstants;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Ianitor.Osp.Common.Shared.DataTransferObjects
{
    /// <summary>
    /// Data Transfer Object of a OSP identity provider
    /// </summary>
    [JsonConverter(typeof(JsonSubtypes), TypeJsonName)]
    [JsonSubtypes.KnownSubType(typeof(GoogleIdentityProviderDto), IdentityProviderTypesDto.Google)]
    [JsonSubtypes.KnownSubType(typeof(MicrosoftIdentityProviderDto), IdentityProviderTypesDto.Microsoft)]
    public class IdentityProviderDto
    {
        /// <summary>
        /// The key for the identity provider type as represented in the JSON.
        /// </summary>
        public const string TypeJsonName = "type";

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

        public const int AliasMinLength = 3;
        public const int AliasMaxLength = TextDefaultMaxLength;

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Indicates if an identity provider is enabled
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Unique ID for the IdentityProviderConfiguration. Do not set this property when creating a new configuration.
        /// The API automatically returns an ID once the configuration has been created.
        /// </summary>
        [StringLength(TextDefaultMaxLength)]
        public string Id { get; set; }

        /// <summary>
        /// The source type of the identity provider (e.g. AzureAD, OpenLDAP ...).
        /// </summary>
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        [Required]
        [JsonProperty(TypeJsonName)]
        public IdentityProviderTypesDto Type { get; set; }

        /// <summary>
        /// Free definable for all different identity provider types.
        /// </summary>
        [Required]
        [StringLength(AliasMaxLength, MinimumLength = AliasMinLength)]
        public string Alias { get; set; }

        /// <summary>
        /// An arbitrary long text describing the identity provider configuration in detail.
        /// </summary>
        [StringLength(DescriptionDefaultMaxLength)]
        public string Description { get; set; }
    }
}