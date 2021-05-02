/* tslint:disable */
/* eslint-disable */
// @generated
// This file was automatically generated and should not be edited.

import { UpdateSystemUIPageInput } from "./globalTypes";

// ====================================================
// GraphQL mutation operation: updatePage
// ====================================================

export interface updatePage_updateSystemUIPages {
  __typename: "SystemUIPage";
  rtId: OspOspObjectIdType | null;
  wellKnownName: string | null;
  content: string | null;
}

export interface updatePage {
  /**
   * Updates existing entity of type 'SystemUIPage'.
   */
  updateSystemUIPages: (updatePage_updateSystemUIPages | null)[] | null;
}

export interface updatePageVariables {
  entities: (UpdateSystemUIPageInput | null)[];
}
