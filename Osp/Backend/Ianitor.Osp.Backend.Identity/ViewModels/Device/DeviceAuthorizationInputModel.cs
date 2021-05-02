using Ianitor.Osp.Backend.Identity.ViewModels.Consent;
#pragma warning disable 1591

namespace Ianitor.Osp.Backend.Identity.ViewModels.Device
{
    public class DeviceAuthorizationInputModel : ConsentInputModel
    {
        public string UserCode { get; set; }
    }
}