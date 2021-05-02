import {EntitySelectDataSource, PagedResultDto} from "@ianitor/shared-services";
import {CkEntityAttributeDetailDto} from "../models/ckEntityAttributeDetailDto";
import {Observable, of} from "rxjs";
import {CkEntityDetailDto} from "../models/ckEntityDetailDto";

export class CkAttributeEntitySelectDataSource implements EntitySelectDataSource<CkEntityAttributeDetailDto>{

  constructor(private ckEntityDetail : CkEntityDetailDto) {
  }

  onDisplayEntity(entity: CkEntityAttributeDetailDto): string {
    if (entity) {

      return entity.attributeName;
    }
  }

  onFilter(filter: string): Observable<PagedResultDto<CkEntityAttributeDetailDto>> {

    const result = new PagedResultDto<CkEntityAttributeDetailDto>();
    result.list = this.ckEntityDetail.attributes.filter(attribute => attribute.attributeName.toLocaleLowerCase().includes(filter.toLowerCase()));
    result.totalCount = result.list.length;

    return of(result);
  }

}
