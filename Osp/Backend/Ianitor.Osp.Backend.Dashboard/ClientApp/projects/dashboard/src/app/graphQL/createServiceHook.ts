/* tslint:disable */
/* eslint-disable */
// @generated
// This file was automatically generated and should not be edited.

import { SystemServiceHookInput } from "./globalTypes";

// ====================================================
// GraphQL mutation operation: createServiceHook
// ====================================================

export interface createServiceHook_createSystemServiceHooks {
  __typename: "SystemServiceHook";
  rtId: OspOspObjectIdType | null;
  enabled: boolean | null;
  name: string | null;
  queryCkId: string | null;
  serviceHookAction: string | null;
  serviceHookBaseUri: string | null;
  fieldFilter: string | null;
}

export interface createServiceHook {
  /**
   * Creates new entities of type 'SystemServiceHook'.
   */
  createSystemServiceHooks: (createServiceHook_createSystemServiceHooks | null)[] | null;
}

export interface createServiceHookVariables {
  entities: (SystemServiceHookInput | null)[];
}
