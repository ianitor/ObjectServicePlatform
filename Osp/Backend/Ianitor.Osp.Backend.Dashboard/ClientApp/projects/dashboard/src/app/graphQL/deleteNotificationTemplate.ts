/* tslint:disable */
/* eslint-disable */
// @generated
// This file was automatically generated and should not be edited.

import { DeletionSystemNotificationTemplateInput } from "./globalTypes";

// ====================================================
// GraphQL mutation operation: deleteNotificationTemplate
// ====================================================

export interface deleteNotificationTemplate {
  /**
   * Deletes an entity of type 'SystemNotificationTemplate'.
   */
  deleteSystemNotificationTemplates: boolean | null;
}

export interface deleteNotificationTemplateVariables {
  entities: (DeletionSystemNotificationTemplateInput | null)[];
}
