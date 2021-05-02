/* tslint:disable */
/* eslint-disable */
// @generated
// This file was automatically generated and should not be edited.

import { UpdateSystemQueryInput } from "./globalTypes";

// ====================================================
// GraphQL mutation operation: updateQueryDetails
// ====================================================

export interface updateQueryDetails_updateSystemQuerys {
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

export interface updateQueryDetails {
  /**
   * Updates existing entity of type 'SystemQuery'.
   */
  updateSystemQuerys: (updateQueryDetails_updateSystemQuerys | null)[] | null;
}

export interface updateQueryDetailsVariables {
  entities: (UpdateSystemQueryInput | null)[];
}
