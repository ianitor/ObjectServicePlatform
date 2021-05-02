using System.Collections.Generic;

namespace Ianitor.Osp.Frontend.Client.Tenants
{
    public class QlItemsContainer<TTdoType>
    {
        public IEnumerable<TTdoType> Items { get; set; }
    }
}