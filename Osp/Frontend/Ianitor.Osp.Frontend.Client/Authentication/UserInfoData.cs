using System.Collections.Generic;
using System.Security.Claims;

namespace Ianitor.Osp.Frontend.Client.Authentication
{
    public class UserInfoData
    {
        public IEnumerable<Claim> Claims { get; }
        public bool IsAuthenticated { get; }

        public UserInfoData(bool isAuthenticated, IEnumerable<Claim> claims)
        {
            IsAuthenticated = isAuthenticated;
            Claims = claims;
        }
    }
}