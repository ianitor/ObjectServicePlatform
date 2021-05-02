import {Scopes} from "../graphQL/globalTypes";
import {CkEntityTypeInfo} from "./ckEntityTypeInfo";
import {CkEntityAttributeDetailDto} from "./ckEntityAttributeDetailDto";

export interface CkEntityDetailDto {
  baseType: CkEntityTypeInfo | null;
  ckId: string | null;
  scopeId: Scopes | null;
  isAbstract: boolean;
  isFinal: boolean;
  attributes: Array<CkEntityAttributeDetailDto> | null;
  derivedTypes: Array<CkEntityTypeInfo> | null;
}
