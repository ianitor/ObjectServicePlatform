/* tslint:disable */
/* eslint-disable */
// @generated
// This file was automatically generated and should not be edited.

// ====================================================
// GraphQL query operation: getSelectionValuesOfCkType
// ====================================================

export interface getSelectionValuesOfCkType_constructionKitTypes_items_attributes_items_attribute_selectionValues {
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

export interface getSelectionValuesOfCkType_constructionKitTypes_items_attributes_items_attribute {
  __typename: "CkAttribute";
  /**
   * Selection values for the attribute.
   */
  selectionValues: (getSelectionValuesOfCkType_constructionKitTypes_items_attributes_items_attribute_selectionValues | null)[] | null;
}

export interface getSelectionValuesOfCkType_constructionKitTypes_items_attributes_items {
  __typename: "CkEntityAttribute";
  /**
   * Attribute name within the entity.
   */
  attributeName: string | null;
  /**
   * The construction kit attribute definition
   */
  attribute: getSelectionValuesOfCkType_constructionKitTypes_items_attributes_items_attribute | null;
}

export interface getSelectionValuesOfCkType_constructionKitTypes_items_attributes {
  __typename: "CkEntityAttributeDtoConnection";
  /**
   * A list of all of the objects returned in the connection. This is a convenience field provided for quickly exploring the API; rather than querying for "{ edges { node } }" when no edge data is needed, this field can be used instead. Note that when clients like Relay need to fetch the "cursor" field on the edge to enable efficient pagination, this shortcut cannot be used, and the full "{ edges { node } } " version should be used instead.
   */
  items: (getSelectionValuesOfCkType_constructionKitTypes_items_attributes_items | null)[] | null;
}

export interface getSelectionValuesOfCkType_constructionKitTypes_items {
  __typename: "CkEntity";
  /**
   * Unique id of the object.
   */
  ckId: string | null;
  attributes: getSelectionValuesOfCkType_constructionKitTypes_items_attributes | null;
}

export interface getSelectionValuesOfCkType_constructionKitTypes {
  __typename: "CkEntityDtoConnection";
  /**
   * A list of all of the objects returned in the connection. This is a convenience field provided for quickly exploring the API; rather than querying for "{ edges { node } }" when no edge data is needed, this field can be used instead. Note that when clients like Relay need to fetch the "cursor" field on the edge to enable efficient pagination, this shortcut cannot be used, and the full "{ edges { node } } " version should be used instead.
   */
  items: (getSelectionValuesOfCkType_constructionKitTypes_items | null)[] | null;
}

export interface getSelectionValuesOfCkType {
  constructionKitTypes: getSelectionValuesOfCkType_constructionKitTypes | null;
}

export interface getSelectionValuesOfCkTypeVariables {
  ckId: string;
  attributeNames: (string | null)[];
}
