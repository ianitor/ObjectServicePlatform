/* tslint:disable */
/* eslint-disable */
// @generated
// This file was automatically generated and should not be edited.

// ====================================================
// GraphQL query operation: getCkEntityAttribute
// ====================================================

export interface getCkEntityAttribute_constructionKitTypes_items_attributes_items {
  __typename: "CkEntityAttribute";
  /**
   * Attribute name within the entity.
   */
  attributeName: string | null;
  /**
   * OSP Identifier of the attribute.
   */
  attributeId: string | null;
  /**
   * Auto complete values for the attribute.
   */
  autoCompleteTexts: (string | null)[] | null;
  /**
   * Returns true, when auto complete values are enabled.
   */
  isAutoCompleteEnabled: boolean | null;
  /**
   * Auto complete filter value for the attribute.
   */
  autoCompleteFilter: string | null;
  /**
   * Auto complete max value count for the attribute.
   */
  autoCompleteLimit: number | null;
  /**
   * Auto increment reference for the attribute.
   */
  autoIncrementReference: string | null;
}

export interface getCkEntityAttribute_constructionKitTypes_items_attributes {
  __typename: "CkEntityAttributeDtoConnection";
  /**
   * A list of all of the objects returned in the connection. This is a convenience field provided for quickly exploring the API; rather than querying for "{ edges { node } }" when no edge data is needed, this field can be used instead. Note that when clients like Relay need to fetch the "cursor" field on the edge to enable efficient pagination, this shortcut cannot be used, and the full "{ edges { node } } " version should be used instead.
   */
  items: (getCkEntityAttribute_constructionKitTypes_items_attributes_items | null)[] | null;
}

export interface getCkEntityAttribute_constructionKitTypes_items {
  __typename: "CkEntity";
  /**
   * Unique id of the object.
   */
  ckId: string | null;
  attributes: getCkEntityAttribute_constructionKitTypes_items_attributes | null;
}

export interface getCkEntityAttribute_constructionKitTypes {
  __typename: "CkEntityDtoConnection";
  /**
   * A list of all of the objects returned in the connection. This is a convenience field provided for quickly exploring the API; rather than querying for "{ edges { node } }" when no edge data is needed, this field can be used instead. Note that when clients like Relay need to fetch the "cursor" field on the edge to enable efficient pagination, this shortcut cannot be used, and the full "{ edges { node } } " version should be used instead.
   */
  items: (getCkEntityAttribute_constructionKitTypes_items | null)[] | null;
}

export interface getCkEntityAttribute {
  constructionKitTypes: getCkEntityAttribute_constructionKitTypes | null;
}

export interface getCkEntityAttributeVariables {
  ckId: string;
  attributeName: string;
}
