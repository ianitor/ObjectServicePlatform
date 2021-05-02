using System.ComponentModel.DataAnnotations;
using Ianitor.Osp.Common.Internationalization;

namespace Ianitor.Osp.Backend.Identity.ViewModels.Device
{
  public class DeviceUserCodeCaptureViewModel
  {
    public DeviceUserCodeCaptureViewModel()
    {
    }

    [Required(ErrorMessageResourceName = nameof(Texts.Backend_Identity_DeviceCaptureUserCode_Validation_UserCode),
      ErrorMessageResourceType = typeof(Texts))]
    [Display(ResourceType = typeof(Texts), Name = nameof(Texts.Backend_Identity_UserCode_Header))]
    public string UserCode { get; set; }
  }
}
