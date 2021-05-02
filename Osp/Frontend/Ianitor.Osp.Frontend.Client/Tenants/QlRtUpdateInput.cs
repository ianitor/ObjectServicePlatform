namespace Ianitor.Osp.Frontend.Client.Tenants
{
    /// <summary>
    /// Update input data transfer object for runtime entities 
    /// </summary>
    /// <typeparam name="TDto"></typeparam>
    // ReSharper disable once UnusedType.Global
    public class QlRtUpdateInput<TDto>
    {   
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="rtId">GUID of entity that has to be updated</param>
        /// <param name="revision">Current revision key</param>
        /// <param name="item">New item definition that has to be updated</param>
        public QlRtUpdateInput(string rtId, string revision, TDto item)
        {
            RtId = rtId;
            Revision = revision;
            Item = item;
        }
        
        // ReSharper disable MemberCanBePrivate.Global
        // ReSharper disable UnusedAutoPropertyAccessor.Global
        /// <summary>
        /// Returns the update item object with properties that need to be changed.
        /// </summary>
        public TDto Item { get; }
        
        /// <summary>
        /// Returns the GUID of the runtime entity object
        /// </summary>
        public string RtId { get; }
        
        /// <summary>
        /// Returns the revision of the runtime entity object
        /// </summary>
        public string Revision { get; }
        
        // ReSharper restore MemberCanBePrivate.Global
        // ReSharper restore UnusedAutoPropertyAccessor.Global

    }
}