using System.Collections.Generic;
using Ianitor.Osp.Backend.Persistence.DatabaseEntities;
using Ianitor.Osp.Common.Shared;

namespace Ianitor.Osp.Backend.Persistence.Commands
{
    internal class TransientCkModel
    {
        public List<CkEntityAssociation> CkEntityAssociations { get; }
        public List<CkEntityInheritance> CkEntityInheritances { get; }
        public List<CkEntity> CkEntities { get; }
        public List<CkAttribute> CkAttributes { get; }

        public TransientCkModel()
        {
            CkEntityAssociations = new List<CkEntityAssociation>();
            CkEntityInheritances = new List<CkEntityInheritance>();
            CkEntities = new List<CkEntity>();
            CkAttributes = new List<CkAttribute>();
        }
    }
}