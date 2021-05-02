import {AttributeValueType, Scopes} from "../graphQL/globalTypes";

export interface CkAttributeDto {
  attributeId: string | null;
  attributeValueType: AttributeValueType | null;
  scopeId?: Scopes | null;
}
