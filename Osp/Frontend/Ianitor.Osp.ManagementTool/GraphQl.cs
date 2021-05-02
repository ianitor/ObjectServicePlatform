namespace Ianitor.Osp.ManagementTool
{
    public class GraphQl
    {
        internal const string CreateServiceHook = @"mutation createServiceHook($entities: [SystemServiceHookInput]!) {
          createSystemServiceHooks(entities: $entities) {
            revision
            rtId
            enabled
            name
            queryCkId
            serviceHookAction
            serviceHookBaseUri
            fieldFilter
          }
        }";

        internal const string UpdateServiceHook =
            @"mutation updateServiceHook($entities: [UpdateSystemServiceHookInput]!) {
              updateSystemServiceHooks(entities: $entities) {
                revision
                rtId
                enabled
                name
                queryCkId
                serviceHookAction
                serviceHookBaseUri
                fieldFilter
              }
            }
            ";

        internal const string DeleteServiceHook =
            @"mutation deleteServiceHook($entities: [DeletionSystemServiceHookInput]!) {
            deleteSystemServiceHooks(entities: $entities)
          }";

        internal const string GetServiceHook = @"query getServiceHooks(
          $after: String,
          $first: Int,
          $rtIds:[OspObjectIdType],
          $searchFilter: SearchFilter,
          $fieldFilters: [FieldFilter],
          $sort: [Sort]) {
          systemServiceHookConnection(
            after: $after
            first: $first
            rtIds: $rtIds
            searchFilter: $searchFilter
            fieldFilter: $fieldFilters
            sortOrder: $sort
          ) {
            totalCount

            items {
              revision
              rtId
              enabled
              name
              queryCkId
              serviceHookAction
              serviceHookBaseUri
            }
          }
        }
        ";

        internal const string GetServiceHookDetails = @"query getServiceHookDetails($rtId: String!) {
          systemServiceHookConnection(rtId: $rtId) {
            totalCount

            items {
              revision
              rtId
              enabled
              name
              queryCkId
              serviceHookAction
              serviceHookBaseUri
              fieldFilter
            }
          }
        }";

        internal const string GetNotifications = @"query getNotifications(
          $after: String
          $first: Int
          $searchFilter: SearchFilter
          $fieldFilters: [FieldFilter]
          $sort: [Sort]
        ) {
          systemNotificationMessageConnection(
            after: $after
            first: $first
            searchFilter: $searchFilter
            fieldFilter: $fieldFilters
            sortOrder: $sort
          ) {
            totalCount
            items {
              rtId
            }
          }
        }";
    }
}