import {IDictionary} from "./dictionary";
import {RtEntityAttributeDto} from "./rtEntityAttributeDto";

export interface RtEntityDto {
  rtId: string;
  wellKnownName: string | null;

  attributes: IDictionary<RtEntityAttributeDto>;

}
