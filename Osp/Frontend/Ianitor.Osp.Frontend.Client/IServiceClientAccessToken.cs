using System;

namespace Ianitor.Osp.Frontend.Client
{
    public interface IServiceClientAccessToken
    {
        event EventHandler AccessTokenUpdated;
        
        /// <summary>
        /// Returns the access token
        /// </summary>
        string AccessToken { get; set; }
    }
}