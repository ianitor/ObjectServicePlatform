using System;

namespace Ianitor.Osp.Frontend.Client.Authentication
{
    public class AuthenticationData
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        
        public DateTime ExpiresAt { get; set; }
    }
}