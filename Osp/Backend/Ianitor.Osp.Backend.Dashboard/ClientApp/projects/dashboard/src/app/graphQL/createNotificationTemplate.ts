/* tslint:disable */
/* eslint-disable */
// @generated
// This file was automatically generated and should not be edited.

import { SystemNotificationTemplateInput, AttributeValueType } from "./globalTypes";

// ====================================================
// GraphQL mutation operation: createNotificationTemplate
// ====================================================

export interface createNotificationTemplate_createSystemNotificationTemplates_constructionKitType_attributes_items_attribute_selectionValues {
  __typename: "CkSelectionValue";
  /**
   * AssociationId of the selection list value
   */
  key: number | null;
  /**
   * Name of the selection list value
   */
  name: string | null;
}

export interface createNotificationTemplate_createSystemNotificationTemplates_constructionKitType_attributes_items_attribute {
  __typename: "CkAttribute";
  /**
   * Selection values for the attribute.
   */
  selectionValues: (createNotificationTemplate_createSystemNotificationTemplates_constructionKitType_attributes_items_attribute_selectionValues | null)[] | null;
}

export interface createNotificationTemplate_createSystemNotificationTemplates_constructionKitType_attributes_items {
  __typename: "CkEntityAttribute";
  /**
   * Attribute name within the entity.
   */
  attributeName: string | null;
  /**
   * Attribute name within the entity.
   */
  attributeValueType: AttributeValueType | null;
  /**
   * The construction kit attribute definition
   */
  attribute: createNotificationTemplate_createSystemNotificationTemplates_constructionKitType_attributes_items_attribute | null;
}

export interface createNotificationTemplate_createSystemNotificationTemplates_constructionKitType_attributes {
  __typename: "CkEntityAttributeDtoConnection";
  /**
   * A list of all of the objects returned in the connection. This is a convenience field provided for quickly exploring the API; rather than querying for "{ edges { node } }" when no edge data is needed, this field can be used instead. Note that when clients like Relay need to fetch the "cursor" field on the edge to enable efficient pagination, this shortcut cannot be used, and the full "{ edges { node } } " version should be used instead.
   */
  items: (createNotificationTemplate_createSystemNotificationTemplates_constructionKitType_attributes_items | null)[] | null;
}

export interface createNotificationTemplate_createSystemNotificationTemplates_constructionKitType {
  __typename: "CkEntity";
  attributes: createNotificationTemplate_createSystemNotificationTemplates_constructionKitType_attributes | null;
}

export interface createNotificationTemplate_createSystemNotificationTemplates {
  __typename: "SystemNotificationTemplate";
  constructionKitType: createNotificationTemplate_createSystemNotificationTemplates_constructionKitType | null;
  rtId: OspOspObjectIdType | null;
  wellKnownName: string | null;
  type: number | null;
  subjectTemplate: string | null;
  bodyTemplate: string | null;
}

export interface createNotificationTemplate {
  /**
   * Creates new entities of type 'SystemNotificationTemplate'.
   */
  createSystemNotificationTemplates: (createNotificationTemplate_createSystemNotificationTemplates | null)[] | null;
}

export interface createNotificationTemplateVariables {
  entities: (SystemNotificationTemplateInput | null)[];
}
