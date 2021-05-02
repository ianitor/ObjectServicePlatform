/* tslint:disable */
/* eslint-disable */
// @generated
// This file was automatically generated and should not be edited.

import { Scopes, AttributeValueType } from "./globalTypes";

// ====================================================
// GraphQL query operation: getCkEntityDetails
// ====================================================

export interface getCkEntityDetails_constructionKitTypes_items_baseType {
  __typename: "CkEntity";
  /**
   * Unique id of the object.
   */
  ckId: string | null;
}

export interface getCkEntityDetails_constructionKitTypes_items_derivedTypes_items {
  __typename: "CkEntity";
  /**
   * Unique id of the object.
   */
  ckId: string | null;
}

export interface getCkEntityDetails_constructionKitTypes_items_derivedTypes {
  __typename: "CkEntityDtoConnection";
  /**
   * A list of all of the objects returned in the connection. This is a convenience field provided for quickly exploring the API; rather than querying for "{ edges { node } }" when no edge data is needed, this field can be used instead. Note that when clients like Relay need to fetch the "cursor" field on the edge to enable efficient pagination, this shortcut cannot be used, and the full "{ edges { node } } " version should be used instead.
   */
  items: (getCkEntityDetails_constructionKitTypes_items_derivedTypes_items | null)[] | null;
}

export interface getCkEntityDetails_constructionKitTypes_items_attributes_items {
  __typename: "CkEntityAttribute";
  /**
   * OSP Identifier of the attribute.
   */
  attributeId: string | null;
  /**
   * Attribute name within the entity.
   */
  attributeName: string | null;
  /**
   * Returns true, when auto complete values are enabled.
   */
  isAutoCompleteEnabled: boolean | null;
  /**
   * Attribute name within the entity.
   */
  attributeValueType: AttributeValueType | null;
}

export interface getCkEntityDetails_constructionKitTypes_items_attributes {
  __typename: "CkEntityAttributeDtoConnection";
  /**
   * A list of all of the objects returned in the connection. This is a convenience field provided for quickly exploring the API; rather than querying for "{ edges { node } }" when no edge data is needed, this field can be used instead. Note that when clients like Relay need to fetch the "cursor" field on the edge to enable efficient pagination, this shortcut cannot be used, and the full "{ edges { node } } " version should be used instead.
   */
  items: (getCkEntityDetails_constructionKitTypes_items_attributes_items | null)[] | null;
}

export interface getCkEntityDetails_constructionKitTypes_items {
  __typename: "CkEntity";
  baseType: getCkEntityDetails_constructionKitTypes_items_baseType | null;
  /**
   * Unique id of the object.
   */
  ckId: string | null;
  scopeId: Scopes | null;
  isAbstract: boolean;
  isFinal: boolean;
  derivedTypes: getCkEntityDetails_constructionKitTypes_items_derivedTypes | null;
  attributes: getCkEntityDetails_constructionKitTypes_items_attributes | null;
}

export interface getCkEntityDetails_constructionKitTypes {
  __typename: "CkEntityDtoConnection";
  /**
   * A list of all of the objects returned in the connection. This is a convenience field provided for quickly exploring the API; rather than querying for "{ edges { node } }" when no edge data is needed, this field can be used instead. Note that when clients like Relay need to fetch the "cursor" field on the edge to enable efficient pagination, this shortcut cannot be used, and the full "{ edges { node } } " version should be used instead.
   */
  items: (getCkEntityDetails_constructionKitTypes_items | null)[] | null;
}

export interface getCkEntityDetails {
  constructionKitTypes: getCkEntityDetails_constructionKitTypes | null;
}

export interface getCkEntityDetailsVariables {
  ckId?: string | null;
}
