/* tslint:disable */
/* eslint-disable */
// @generated
// This file was automatically generated and should not be edited.

import { SystemQueryInput } from "./globalTypes";

// ====================================================
// GraphQL mutation operation: createQuery
// ====================================================

export interface createQuery_createSystemQuerys {
  __typename: "SystemQuery";
  rtId: OspOspObjectIdType | null;
  name: string | null;
  queryCkId: string | null;
  columns: string | null;
  sorting: string | null;
  fieldFilter: string | null;
  attributeSearchFilter: string | null;
  textSearchFilter: string | null;
}

export interface createQuery {
  /**
   * Creates new entities of type 'SystemQuery'.
   */
  createSystemQuerys: (createQuery_createSystemQuerys | null)[] | null;
}

export interface createQueryVariables {
  entities: (SystemQueryInput | null)[];
}
