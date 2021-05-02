using System;
using Ianitor.Osp.Frontend.Client.System;

namespace Ianitor.Osp.Frontend.Client.Tenants
{
    public class ServiceClientAccessToken : ITenantClientAccessToken, IJobServiceClientAccessToken, IIdentityServiceClientAccessToken, ICoreServiceClientAccessToken
    {
        private string _accessToken;

        public event EventHandler AccessTokenUpdated;

        public string AccessToken
        {
            get => _accessToken;
            set
            {
                if (_accessToken != value)
                {
                    _accessToken = value;
                    OnAccessTokenUpdated();
                }
            }
        }

        protected virtual void OnAccessTokenUpdated()
        {
            AccessTokenUpdated?.Invoke(this, EventArgs.Empty);
        }
    }
}    