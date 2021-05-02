using Newtonsoft.Json.Serialization;

namespace Ianitor.Osp.Common.Shared
{
    public class ConstantCaseNamingStrategy : NamingStrategy
    {
        protected override string ResolvePropertyName(string name)
        {
            return name.ToConstantCase();
        }
    }
}