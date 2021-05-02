import {AttributeValueType} from "../graphQL/globalTypes";

export interface CkEntityAttributeDetailDto {

  attributeName: string | null;

  attributeValueType?: AttributeValueType | null;

  /**
   * OSP Identifier of the attribute.
   */
  attributeId: string | null;
  /**
   * Auto complete values for the attribute.
   */
  autoCompleteTexts?: (string | null)[] | null;
  /**
   * Returns true, when auto complete values are enabled.
   */
  isAutoCompleteEnabled: boolean | null;
  /**
   * Auto complete filter value for the attribute.
   */
  autoCompleteFilter?: string | null;
  /**
   * Auto complete max value count for the attribute.
   */
  autoCompleteLimit?: number | null;
  /**
   * Auto increment reference for the attribute.
   */
  autoIncrementReference?: string | null;
}
