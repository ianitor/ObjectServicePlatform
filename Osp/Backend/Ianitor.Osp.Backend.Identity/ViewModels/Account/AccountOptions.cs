#pragma warning disable 1591

using System;
using Ianitor.Osp.Common.Internationalization;

namespace Ianitor.Osp.Backend.Identity.ViewModels.Account
{
    public class AccountOptions
    {
        public static bool AllowLocalLogin = true;
        public static bool AllowRememberLogin = true;
        public static TimeSpan RememberMeLoginDuration = TimeSpan.FromDays(30);

        public static bool ShowLogoutPrompt = true;
        public static bool AutomaticRedirectAfterSignOut = true;

        // specify the Windows authentication scheme being used
        public static readonly string WindowsAuthenticationSchemeName = "Windows";
        // if user uses windows auth, should we load the groups from windows
        public static bool IncludeWindowsGroups = false;

        public static string InvalidCredentialsErrorMessage = Texts.Backend_Identity_Login_InvalidUserPassword;
    }
}
