namespace Ianitor.Osp.Backend.Identity.ViewModels.Shared
{
  public class UserInfoViewModel
  {
    public string UserName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string EMail { get; set; }

    public int AccessFailedCount { get; set; }
    public string Id { get; set; }

    public bool HasPassword { get; set; }
  }
}
