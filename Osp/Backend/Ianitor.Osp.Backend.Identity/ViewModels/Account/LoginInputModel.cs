using System.ComponentModel.DataAnnotations;
using Ianitor.Osp.Common.Internationalization;

namespace Ianitor.Osp.Backend.Identity.ViewModels.Account
{
    public class LoginInputModel
    {
        [Required(ErrorMessageResourceName = nameof(Texts.Backend_Identity_LogIn_Validation_Username),
            ErrorMessageResourceType = typeof(Texts))]
        [Display(ResourceType = typeof(Texts), Name = nameof(Texts.Backend_Identity_General_Username_Label))]
        public string Username { get; set; }

        [Required(ErrorMessageResourceName = nameof(Texts.Backend_Identity_LogIn_Validation_Password),
            ErrorMessageResourceType = typeof(Texts))]
        [Display(ResourceType = typeof(Texts), Name = nameof(Texts.Backend_Identity_General_Password_Label))]
        public string Password { get; set; }

        public bool RememberLogin { get; set; }

        public string ReturnUrl { get; set; }
    }
}