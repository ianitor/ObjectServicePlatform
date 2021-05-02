/* tslint:disable */
/* eslint-disable */
// @generated
// This file was automatically generated and should not be edited.

//==============================================================
// START Enums and Input Objects
//==============================================================

/**
 * Defines the type of modification during write operations
 */
export enum AssociationModOptions {
  CREATE = "CREATE",
  DELETE = "DELETE",
}

/**
 * Enum of valid attribute types
 */
export enum AttributeValueType {
  BINARY = "BINARY",
  BOOLEAN = "BOOLEAN",
  DATE_TIME = "DATE_TIME",
  DOUBLE = "DOUBLE",
  INT = "INT",
  INT_ARRAY = "INT_ARRAY",
  STRING = "STRING",
  STRING_ARRAY = "STRING_ARRAY",
}

/**
 * Defines the operator of field compare
 */
export enum FieldFilterOperators {
  EQUALS = "EQUALS",
  GREATER_EQUAL_THAN = "GREATER_EQUAL_THAN",
  GREATER_THAN = "GREATER_THAN",
  IN = "IN",
  LESS_EQUAL_THAN = "LESS_EQUAL_THAN",
  LESS_THAN = "LESS_THAN",
  LIKE = "LIKE",
  MATCH_REG_EX = "MATCH_REG_EX",
  NOT_EQUALS = "NOT_EQUALS",
  NOT_IN = "NOT_IN",
  NOT_MATCH_REG_EX = "NOT_MATCH_REG_EX",
}

/**
 * The scope of the construction kit model
 */
export enum Scopes {
  APPLICATION = "APPLICATION",
  LAYER_2 = "LAYER_2",
  LAYER_3 = "LAYER_3",
  LAYER_4 = "LAYER_4",
  SYSTEM = "SYSTEM",
}

/**
 * The type of search that is used (a text based search using text analysis (high performance, scoring, maybe more false positives) or filtering of attributes (lower performance, more exact results)
 */
export enum SearchFilterTypes {
  ATTRIBUTE_FILTER = "ATTRIBUTE_FILTER",
  TEXT_SEARCH = "TEXT_SEARCH",
}

/**
 * Defines the sort order
 */
export enum SortOrders {
  ASCENDING = "ASCENDING",
  DEFAULT = "DEFAULT",
  DESCENDING = "DESCENDING",
}

export interface DeletionSystemNotificationTemplateInput {
  rtId?: OspOspObjectIdType | null;
}

export interface DeletionSystemQueryInput {
  rtId?: OspOspObjectIdType | null;
}

export interface DeletionSystemServiceHookInput {
  rtId?: OspOspObjectIdType | null;
}

export interface DeletionSystemUIPageInput {
  rtId?: OspOspObjectIdType | null;
}

export interface FieldFilter {
  attributeName: string;
  comparisonValue?: OspSimpleScalarType | null;
  operator?: FieldFilterOperators | null;
}

/**
 * Input field for associations
 */
export interface RtAssociationInput {
  modOption?: AssociationModOptions | null;
  target: RtEntityId;
}

/**
 * Id information consists of CkId and RtId
 */
export interface RtEntityId {
  ckId: string;
  rtId: OspOspObjectIdType;
}

export interface SearchFilter {
  attributeNames?: (string | null)[] | null;
  language?: string | null;
  searchTerm: string;
  type?: SearchFilterTypes | null;
}

export interface Sort {
  attributeName: string;
  sortOrder?: SortOrders | null;
}

export interface SystemNotificationTemplateInput {
  bodyTemplate?: string | null;
  relatesFrom?: (RtAssociationInput | null)[] | null;
  subjectTemplate?: string | null;
  type?: number | null;
  wellKnownName?: string | null;
}

export interface SystemQueryInput {
  attributeSearchFilter?: string | null;
  columns?: string | null;
  fieldFilter?: string | null;
  name?: string | null;
  queryCkId?: string | null;
  relatesFrom?: (RtAssociationInput | null)[] | null;
  sorting?: string | null;
  textSearchFilter?: string | null;
  wellKnownName?: string | null;
}

export interface SystemServiceHookInput {
  enabled?: boolean | null;
  fieldFilter?: string | null;
  name?: string | null;
  queryCkId?: string | null;
  relatesFrom?: (RtAssociationInput | null)[] | null;
  serviceHookAction?: string | null;
  serviceHookBaseUri?: string | null;
  wellKnownName?: string | null;
}

export interface SystemUIPageInput {
  content?: string | null;
  relatesFrom?: (RtAssociationInput | null)[] | null;
  wellKnownName?: string | null;
}

export interface UpdateSystemNotificationTemplateInput {
  item: SystemNotificationTemplateInput;
  rtId?: OspOspObjectIdType | null;
}

export interface UpdateSystemQueryInput {
  item: SystemQueryInput;
  rtId?: OspOspObjectIdType | null;
}

export interface UpdateSystemServiceHookInput {
  item: SystemServiceHookInput;
  rtId?: OspOspObjectIdType | null;
}

export interface UpdateSystemUIPageInput {
  item: SystemUIPageInput;
  rtId?: OspOspObjectIdType | null;
}

//==============================================================
// END Enums and Input Objects
//==============================================================
