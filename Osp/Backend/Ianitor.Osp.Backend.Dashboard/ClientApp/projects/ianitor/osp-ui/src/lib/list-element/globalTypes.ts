/* tslint:disable */
/* eslint-disable */
// @generated
// This file was automatically generated and should not be edited.

//==============================================================
// START Enums and Input Objects
//==============================================================

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
  SYSTEM = "SYSTEM",
}

/**
 * The type of search that is used (a text based search using text analysis (high
 * performance, scoring, maybe more false positives) or filtering of attributes
 * (lower performance, more exact results)
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

export interface FieldFilter {
  attributeName: string;
  operator?: FieldFilterOperators | null;
  comparisonValue?: string | number | boolean | null;
}

export interface SearchFilter {
  language?: string | null;
  searchTerm: string;
  type?: SearchFilterTypes | null;
  attributeNames?: (string | null)[] | null;
}

export interface Sort {
  sortOrder?: SortOrders | null;
  attributeName: string;
}

//==============================================================
// END Enums and Input Objects
//==============================================================
