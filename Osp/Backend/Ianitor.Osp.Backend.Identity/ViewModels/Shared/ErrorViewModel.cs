using IdentityServer4.Models;

#pragma warning disable 1591

namespace Ianitor.Osp.Backend.Identity.ViewModels.Shared
{
    public class ErrorViewModel
    {
        public ErrorViewModel()
        {
        }

        public ErrorViewModel(string error)
        {
            Error = new ErrorMessage { Error = error };
        }

        public ErrorMessage Error { get; set; }
    }
}