using System;

namespace Ianitor.Osp.Backend.Persistence
{
  [AttributeUsage(AttributeTargets.Class)]
  public class CollectionNameAttribute : Attribute
  {
    public string CollectionName { get; set; }

    public CollectionNameAttribute(string collectionName)
    {
      CollectionName = collectionName;
    }
  }
}
