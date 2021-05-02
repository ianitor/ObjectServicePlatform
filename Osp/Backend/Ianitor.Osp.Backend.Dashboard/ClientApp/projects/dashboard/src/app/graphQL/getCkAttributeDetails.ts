/* tslint:disable */
/* eslint-disable */
// @generated
// This file was automatically generated and should not be edited.

import { AttributeValueType, Scopes } from "./globalTypes";

// ====================================================
// GraphQL query operation: getCkAttributeDetails
// ====================================================

export interface getCkAttributeDetails_constructionKitAttributes_items_selectionValues {
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

export interface getCkAttributeDetails_constructionKitAttributes_items {
  __typename: "CkAttribute";
  /**
   * Unique id of the object.
   */
  attributeId: string | null;
  attributeValueType: AttributeValueType | null;
  scopeId: Scopes | null;
  /**
   * Selection values for the attribute.
   */
  selectionValues: (getCkAttributeDetails_constructionKitAttributes_items_selectionValues | null)[] | null;
  /**
   * Default values of a compound attribute.
   */
  defaultValues: (OspSimpleScalarType | null)[] | null;
  /**
   * Default value of a scalar attribute.
   */
  defaultValue: OspSimpleScalarType | null;
}

export interface getCkAttributeDetails_constructionKitAttributes {
  __typename: "CkAttributeDtoConnection";
  /**
   * A list of all of the objects returned in the connection. This is a convenience field provided for quickly exploring the API; rather than querying for "{ edges { node } }" when no edge data is needed, this field can be used instead. Note that when clients like Relay need to fetch the "cursor" field on the edge to enable efficient pagination, this shortcut cannot be used, and the full "{ edges { node } } " version should be used instead.
   */
  items: (getCkAttributeDetails_constructionKitAttributes_items | null)[] | null;
}

export interface getCkAttributeDetails {
  constructionKitAttributes: getCkAttributeDetails_constructionKitAttributes | null;
}

export interface getCkAttributeDetailsVariables {
  attributeId?: string | null;
}
