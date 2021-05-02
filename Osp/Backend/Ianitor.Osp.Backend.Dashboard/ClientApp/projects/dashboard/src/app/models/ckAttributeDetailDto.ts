import {AttributeValueType, Scopes} from "../graphQL/globalTypes";
import {CkDefaultValue} from "./ckDefaultValue";
import {CkSelectionValue} from "./ckSelectionValue";

export interface CkAttributeDetailDto {
  attributeId: string | null;
  attributeValueType: AttributeValueType | null;
  scopeId: Scopes | null;
  selectionValues: CkSelectionValue[] | null;
  defaultValues: CkDefaultValue[] | null;
  defaultValue: CkDefaultValue | null;
}
