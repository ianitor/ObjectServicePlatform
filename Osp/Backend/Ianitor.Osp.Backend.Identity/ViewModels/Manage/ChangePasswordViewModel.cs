using System.ComponentModel.DataAnnotations;
using Ianitor.Osp.Common.Internationalization;

namespace Ianitor.Osp.Backend.Identity.ViewModels.Manage
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessageResourceName = nameof(Texts.Backend_Identity_ChangePassword_Validation_CurrentPassword),
            ErrorMessageResourceType = typeof(Texts))]
        [DataType(DataType.Password)]
        [Display(Name = nameof(Texts.Backend_Identity_CurrentPassword_Label), ResourceType = typeof(Texts))]
        public string OldPassword { get; set; }

        [Required(ErrorMessageResourceName = nameof(Texts.Backend_Identity_ChangePassword_Validation_NewPassword),
            ErrorMessageResourceType = typeof(Texts))]
        [StringLength(100, ErrorMessage = nameof(Texts.Backend_Identity_ChangePassword_Validation_NewPassword_MinMax),
            MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = nameof(Texts.Backend_Identity_NewPassword_Label), ResourceType = typeof(Texts))]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = nameof(Texts.Backend_Identity_ConfirmNewPassword_Label), ResourceType = typeof(Texts))]
        [Compare("NewPassword",
            ErrorMessageResourceName = nameof(Texts.Backend_Identity_ChangePassword_Validation_Match),
            ErrorMessageResourceType = typeof(Texts))]
        public string ConfirmPassword { get; set; }
    }
}