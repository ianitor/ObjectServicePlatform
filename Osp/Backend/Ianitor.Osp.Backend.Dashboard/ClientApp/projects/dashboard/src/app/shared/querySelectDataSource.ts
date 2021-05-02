import {EntitySelectDataSource, PagedResultDto} from "@ianitor/shared-services";
import {Observable} from "rxjs";
import {TenantBackendService} from "../services/tenant-backend.service";
import {SearchFilterTypes} from "../graphQL/globalTypes";
import {QueryDto} from "../models/queryDto";

export class QuerySelectDataSource implements  EntitySelectDataSource<QueryDto>{

  constructor(private tenantId: string, private dataSourceBackendService: TenantBackendService) {
  }

  onDisplayEntity(entity: QueryDto): string {
    if (entity) {

      return entity.name;
    }
  }

  onFilter(filter: string): Observable<PagedResultDto<QueryDto>> {
    return this.dataSourceBackendService.getQueries(this.tenantId, null, null, {
      type: SearchFilterTypes.ATTRIBUTE_FILTER,
      attributeNames: ['name'],
      searchTerm: filter
    });
  }

}
