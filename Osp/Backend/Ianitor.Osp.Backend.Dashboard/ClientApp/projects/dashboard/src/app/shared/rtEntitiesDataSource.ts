import {of} from "rxjs";
import {catchError} from "rxjs/operators";
import {TenantBackendService} from "../services/tenant-backend.service";
import {RtEntityDto} from "../models/rtEntityDto";
import {FieldFilter, SearchFilter, Sort} from "../graphQL/globalTypes";
import {DataSourceBase, MessageService, PagedResultDto} from "@ianitor/shared-services";

export class RtEntitiesDataSource extends DataSourceBase<RtEntityDto> {

  constructor(private dataSourceBackendService: TenantBackendService, private messageService: MessageService) {
    super()
  }


  loadData(tenantId: string, ckId : string, attributeNames : Array<string>, skip: number = 0, take: number = 10, searchFilter: SearchFilter = null, fieldFilter: FieldFilter[] = null, sort: Sort[] = null) {
    super.onBeginLoad();

    this.dataSourceBackendService.getRtEntities(tenantId, ckId, attributeNames, skip, take, searchFilter, fieldFilter, sort).pipe(
      catchError((error) => {

        this.messageService.showError(error, "Error during load of data");

        return of(new PagedResultDto<RtEntityDto>())
      }))
      .subscribe(pagedResult => {

        super.onCompleteLoad(pagedResult);
      });
  }
}
