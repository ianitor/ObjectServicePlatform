/* tslint:disable */
/* eslint-disable */
// @generated
// This file was automatically generated and should not be edited.

import { SearchFilter, FieldFilter, Sort, Scopes } from "./globalTypes";

// ====================================================
// GraphQL query operation: getCkEntities
// ====================================================

export interface getCkEntities_constructionKitTypes_items_baseType {
  __typename: "CkEntity";
  /**
   * Unique id of the object.
   */
  ckId: string | null;
}

export interface getCkEntities_constructionKitTypes_items {
  __typename: "CkEntity";
  baseType: getCkEntities_constructionKitTypes_items_baseType | null;
  /**
   * Unique id of the object.
   */
  ckId: string | null;
  scopeId: Scopes | null;
  isAbstract: boolean;
  isFinal: boolean;
}

export interface getCkEntities_constructionKitTypes {
  __typename: "CkEntityDtoConnection";
  /**
   * A count of the total number of objects in this connection, ignoring pagination. This allows a client to fetch the first five objects by passing "5" as the argument to `first`, then fetch the total count so it could display "5 of 83", for example. In cases where we employ infinite scrolling or don't have an exact count of entries, this field will return `null`.
   */
  totalCount: number | null;
  /**
   * A list of all of the objects returned in the connection. This is a convenience field provided for quickly exploring the API; rather than querying for "{ edges { node } }" when no edge data is needed, this field can be used instead. Note that when clients like Relay need to fetch the "cursor" field on the edge to enable efficient pagination, this shortcut cannot be used, and the full "{ edges { node } } " version should be used instead.
   */
  items: (getCkEntities_constructionKitTypes_items | null)[] | null;
}

export interface getCkEntities {
  constructionKitTypes: getCkEntities_constructionKitTypes | null;
}

export interface getCkEntitiesVariables {
  after?: string | null;
  first?: number | null;
  searchFilter?: SearchFilter | null;
  fieldFilters?: (FieldFilter | null)[] | null;
  sort?: (Sort | null)[] | null;
  ckId?: string | null;
}
