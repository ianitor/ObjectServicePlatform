namespace Ianitor.Osp.Frontend.Client.Authentication
{
    public class EnsureAuthenticatedData
    {
        public bool IsRefreshDone { get; set; }
        public AuthenticationData RefreshedAuthenticationData { get; set; }
    }
}