/* tslint:disable */
/* eslint-disable */
// @generated
// This file was automatically generated and should not be edited.

import { UpdateSystemServiceHookInput } from "./globalTypes";

// ====================================================
// GraphQL mutation operation: updateServiceHook
// ====================================================

export interface updateServiceHook_updateSystemServiceHooks {
  __typename: "SystemServiceHook";
  rtId: OspOspObjectIdType | null;
  enabled: boolean | null;
  name: string | null;
  queryCkId: string | null;
  serviceHookAction: string | null;
  serviceHookBaseUri: string | null;
  fieldFilter: string | null;
}

export interface updateServiceHook {
  /**
   * Updates existing entity of type 'SystemServiceHook'.
   */
  updateSystemServiceHooks: (updateServiceHook_updateSystemServiceHooks | null)[] | null;
}

export interface updateServiceHookVariables {
  entities: (UpdateSystemServiceHookInput | null)[];
}
