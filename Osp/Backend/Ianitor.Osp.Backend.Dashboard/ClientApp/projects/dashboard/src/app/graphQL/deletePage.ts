/* tslint:disable */
/* eslint-disable */
// @generated
// This file was automatically generated and should not be edited.

import { DeletionSystemUIPageInput } from "./globalTypes";

// ====================================================
// GraphQL mutation operation: deletePage
// ====================================================

export interface deletePage {
  /**
   * Deletes an entity of type 'SystemUIPage'.
   */
  deleteSystemUIPages: boolean | null;
}

export interface deletePageVariables {
  entities: (DeletionSystemUIPageInput | null)[];
}
