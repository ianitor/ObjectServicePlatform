using Ianitor.Osp.Backend.Identity.ViewModels.Consent;
#pragma warning disable 1591

namespace Ianitor.Osp.Backend.Identity.ViewModels.Device
{
    public class DeviceAuthorizationViewModel : ConsentViewModel
    {
        public string UserCode { get; set; }
        public bool ConfirmUserCode { get; set; }
    }
}