using System.ComponentModel.DataAnnotations;
using Ianitor.Osp.Common.Internationalization;

namespace Ianitor.Osp.Backend.Identity.ViewModels.Setup
{
  public class SetupViewModel
  {
    [Required(ErrorMessageResourceName = nameof(Texts.Backend_Identity_LogIn_Validation_Username),
      ErrorMessageResourceType = typeof(Texts))]
    [Display(ResourceType = typeof(Texts), Name = nameof(Texts.Backend_Identity_General_Username_Label))]
    public string EMailAddress { get; set; }

    [Required(ErrorMessageResourceName = nameof(Texts.Backend_Identity_ChangePassword_Validation_NewPassword),
      ErrorMessageResourceType = typeof(Texts))]
    [StringLength(100, ErrorMessage = nameof(Texts.Backend_Identity_ChangePassword_Validation_NewPassword_MinMax),
      MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = nameof(Texts.Backend_Identity_NewPassword_Label), ResourceType = typeof(Texts))]
    public string NewPassword { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = nameof(Texts.Backend_Identity_ConfirmNewPassword_Label), ResourceType = typeof(Texts))]
    [Compare("NewPassword", ErrorMessageResourceName = nameof(Texts.Backend_Identity_ChangePassword_Validation_Match),
      ErrorMessageResourceType = typeof(Texts))]
    public string ConfirmPassword { get; set; }
  }
}
