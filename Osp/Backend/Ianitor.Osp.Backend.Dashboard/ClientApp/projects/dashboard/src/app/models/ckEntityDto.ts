import {Scopes} from "../graphQL/globalTypes";
import {CkEntityTypeInfo} from "./ckEntityTypeInfo";

export interface CkEntityDto {
  baseType: CkEntityTypeInfo | null;
  ckId: string | null;
  scopeId: Scopes | null;
  isAbstract: boolean;
  isFinal: boolean;
}
