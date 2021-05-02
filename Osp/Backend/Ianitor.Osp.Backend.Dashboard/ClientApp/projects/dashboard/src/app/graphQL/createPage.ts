/* tslint:disable */
/* eslint-disable */
// @generated
// This file was automatically generated and should not be edited.

import { SystemUIPageInput } from "./globalTypes";

// ====================================================
// GraphQL mutation operation: createPage
// ====================================================

export interface createPage_createSystemUIPages {
  __typename: "SystemUIPage";
  rtId: OspOspObjectIdType | null;
  wellKnownName: string | null;
  content: string | null;
}

export interface createPage {
  /**
   * Creates new entities of type 'SystemUIPage'.
   */
  createSystemUIPages: (createPage_createSystemUIPages | null)[] | null;
}

export interface createPageVariables {
  entities: (SystemUIPageInput | null)[];
}
