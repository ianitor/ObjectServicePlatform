using System.Collections.Generic;
using Ianitor.Osp.Backend.Identity.ViewModels.Shared;
using Microsoft.AspNetCore.Identity;

namespace Ianitor.Osp.Backend.Identity.ViewModels.Manage
{
    public class IndexViewModel : UserInfoViewModel
    {
        public IList<UserLoginInfo> Logins { get; set; }

        public bool BrowserRemembered { get; set; }
    }
}