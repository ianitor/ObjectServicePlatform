namespace Ianitor.Osp.Backend.Persistence
{
    public static class Constants
    {
        public const string RegexMongoDbHost = @"^([a-zA-Z0-9_.-]+)(:[0-9]{1,5})?$";
        public const string RegexWithoutWhitespaces = @"^[^\s]+$";

        internal const string AttributesName = "attributes";

        internal const string SystemSchemaVersion = "SystemSchemaVersion";

        internal const int SystemSchemaVersionValue = 1;

        public const string IdField = "_id";

        internal static readonly string[] KnownAnalyzerLanguages =
            {"de", "en", "es", "fi", "fr", "it", "nl", "no", "pt", "ru", "sv", "zh"};

        internal const string OspTextAnalyzer = "text_osp_";
        internal const string OspTextAnalyzerEn = "text_osp_en";
        internal const string OspTextAnalyzerDe = "text_osp_de";

        internal static readonly string[] TextAnalyzerFeatures = {"frequency", "norm", "position"};
        internal static readonly string[] SystemReservedAttributeNames = {"CkId", "RtId", "WellKnownName"};

        // ********************************************************************
        // Well-known construction kit identifier
        // ********************************************************************
        public const string SystemServiceHookCkId = "System.ServiceHook";
        public const string SystemNotificationMessageCkId = "System.Notification.Message";
        public const string SystemAutoIncrementCkId = "System.AutoIncrement";
        public const string SystemQueryCkId = "System.Query";
        
        
        // ********************************************************************
        // Well-known association identifier
        // ********************************************************************
        public const string RelatedRoleId = "System.Related";

        // ********************************************************************
        // Well-known attribute names
        // ********************************************************************
        public const string QueryCkIdAttribute = "QueryCkId";
        public const string ServiceHookBaseUriAttribute = "ServiceHookBaseUri";
        public const string ServiceHookActionAttribute = "ServiceHookAction";
        public const string FieldFilterAttribute = "FieldFilter";
        public const string EnabledAttribute = "Enabled";
    }
}