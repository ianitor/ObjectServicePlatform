using MongoDB.Driver;

namespace Ianitor.Osp.Backend.Persistence.DataAccess
{
    public class UpdateInfo<T> where T : class, new()
    {
        public UpdateTypes UpdateType { get; }
        public T Document { get; }
        
        public UpdateInfo(ChangeStreamDocument<T> changeStreamDocument)
        {
            switch (changeStreamDocument.OperationType)
            {
                case ChangeStreamOperationType.Insert:
                    UpdateType = UpdateTypes.Insert;
                    break;
                case ChangeStreamOperationType.Update:
                    UpdateType = UpdateTypes.Update;
                    break;
                case ChangeStreamOperationType.Replace:
                    UpdateType = UpdateTypes.Replace;
                    break;                
                case ChangeStreamOperationType.Delete:
                    UpdateType = UpdateTypes.Delete;
                    break;
                default:
                    UpdateType = UpdateTypes.Undefined;
                    break;
            }

            Document = changeStreamDocument.FullDocument;

        }
        
    }
}