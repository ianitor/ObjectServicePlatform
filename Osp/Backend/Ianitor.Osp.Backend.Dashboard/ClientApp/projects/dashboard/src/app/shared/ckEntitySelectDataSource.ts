import {EntitySelectDataSource, PagedResultDto} from "@ianitor/shared-services";
import {CkEntityDto} from "../models/ckEntityDto";
import {Observable} from "rxjs";
import {TenantBackendService} from "../services/tenant-backend.service";
import {SearchFilterTypes} from "../graphQL/globalTypes";

export class CkEntitySelectDataSource implements  EntitySelectDataSource<CkEntityDto>{

  constructor(private tenantId: string, private dataSourceBackendService: TenantBackendService) {
  }

  onDisplayEntity(entity: CkEntityDto): string {
    if (entity) {

      return entity.ckId;
    }
  }

  onFilter(filter: string): Observable<PagedResultDto<CkEntityDto>> {
    return this.dataSourceBackendService.getCkEntities(this.tenantId, null, null, null, {
      type: SearchFilterTypes.ATTRIBUTE_FILTER,
      attributeNames: ['ckId'],
      searchTerm: filter
    });
  }

}
