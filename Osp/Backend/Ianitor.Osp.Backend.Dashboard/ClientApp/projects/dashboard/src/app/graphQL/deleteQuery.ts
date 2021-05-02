/* tslint:disable */
/* eslint-disable */
// @generated
// This file was automatically generated and should not be edited.

import { DeletionSystemQueryInput } from "./globalTypes";

// ====================================================
// GraphQL mutation operation: deleteQuery
// ====================================================

export interface deleteQuery {
  /**
   * Deletes an entity of type 'SystemQuery'.
   */
  deleteSystemQuerys: boolean | null;
}

export interface deleteQueryVariables {
  entities: (DeletionSystemQueryInput | null)[];
}
