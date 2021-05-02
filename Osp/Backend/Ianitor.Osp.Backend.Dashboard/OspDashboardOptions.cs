namespace Ianitor.Osp.Backend.Dashboard
{
    public class OspDashboardOptions
    {
        public OspDashboardOptions()
        {
            CoreServiceUrl = "https://localhost:5001";
            JobServiceUrl = "https://localhost:5009";
            AuthorityUrl = "https://localhost:5003";
            PublicUrl = "https://localhost:5005";
        }

        public string CoreServiceUrl { get; set; }
        public string JobServiceUrl { get; set; }
        public string AuthorityUrl { get; set; }
        public string PublicUrl { get; set; }
    }
}
