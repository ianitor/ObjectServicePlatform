import {of} from "rxjs";
import {catchError} from "rxjs/operators";
import {CkEntityDto} from "../models/ckEntityDto";
import {TenantBackendService} from "../services/tenant-backend.service";
import {SearchFilter, Sort, SortOrders} from "../graphQL/globalTypes";
import {DataSourceBase, MessageService, PagedResultDto} from "@ianitor/shared-services";

export class ConstructionKitTypeDataSource extends DataSourceBase<CkEntityDto> {

  constructor(private tenantBackendService: TenantBackendService, private messageService: MessageService) {
    super();
  }

  loadData(tenantId: string, skip: number = 0, take: number = 10, searchFilter: SearchFilter = null, sort: Sort[] = null) {
    super.onBeginLoad();

    if (!sort || sort && sort.length === 0) {
      sort = new Array<Sort>();
      sort.push(<Sort>{
        attributeName: 'ckId',
        sortOrder: SortOrders.ASCENDING
      });
    }

    this.tenantBackendService.getCkEntities(tenantId, skip, take, null, searchFilter, sort).pipe(
      catchError((error) => {

        this.messageService.showError(error, "Error during load of data");

        return of(new PagedResultDto<CkEntityDto>())
      }))
      .subscribe(pagedResult => {

        super.onCompleteLoad(pagedResult);
      });
  }
}
