/* tslint:disable */
/* eslint-disable */
// @generated
// This file was automatically generated and should not be edited.

import { SearchFilter, FieldFilter, Sort } from "./globalTypes";

// ====================================================
// GraphQL query operation: getServiceHooks
// ====================================================

export interface getServiceHooks_systemServiceHookConnection_items {
  __typename: "SystemServiceHook";
  rtId: OspOspObjectIdType | null;
  enabled: boolean | null;
  name: string | null;
  queryCkId: string | null;
  serviceHookAction: string | null;
  serviceHookBaseUri: string | null;
}

export interface getServiceHooks_systemServiceHookConnection {
  __typename: "SystemServiceHookConnection";
  /**
   * A count of the total number of objects in this connection, ignoring pagination. This allows a client to fetch the first five objects by passing "5" as the argument to `first`, then fetch the total count so it could display "5 of 83", for example. In cases where we employ infinite scrolling or don't have an exact count of entries, this field will return `null`.
   */
  totalCount: number | null;
  /**
   * A list of all of the objects returned in the connection. This is a convenience field provided for quickly exploring the API; rather than querying for "{ edges { node } }" when no edge data is needed, this field can be used instead. Note that when clients like Relay need to fetch the "cursor" field on the edge to enable efficient pagination, this shortcut cannot be used, and the full "{ edges { node } } " version should be used instead.
   */
  items: (getServiceHooks_systemServiceHookConnection_items | null)[] | null;
}

export interface getServiceHooks {
  systemServiceHookConnection: getServiceHooks_systemServiceHookConnection | null;
}

export interface getServiceHooksVariables {
  after?: string | null;
  first?: number | null;
  rtIds?: (OspOspObjectIdType | null)[] | null;
  searchFilter?: SearchFilter | null;
  fieldFilters?: (FieldFilter | null)[] | null;
  sort?: (Sort | null)[] | null;
}
