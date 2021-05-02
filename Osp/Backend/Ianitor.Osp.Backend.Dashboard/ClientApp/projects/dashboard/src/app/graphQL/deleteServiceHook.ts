/* tslint:disable */
/* eslint-disable */
// @generated
// This file was automatically generated and should not be edited.

import { DeletionSystemServiceHookInput } from "./globalTypes";

// ====================================================
// GraphQL mutation operation: deleteServiceHook
// ====================================================

export interface deleteServiceHook {
  /**
   * Deletes an entity of type 'SystemServiceHook'.
   */
  deleteSystemServiceHooks: boolean | null;
}

export interface deleteServiceHookVariables {
  entities: (DeletionSystemServiceHookInput | null)[];
}
