using System;

namespace Ianitor.Osp.ManagementTool
{
    public class OspToolAuthenticationOptions
    {
        public string AccessToken { get; set; }
        public DateTime? AccessTokenExpiresAt { get; set; }
        public string RefreshToken { get; set; }
    }
}