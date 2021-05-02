import {of} from "rxjs";
import {catchError} from "rxjs/operators";
import {TenantDto} from "../models/tenantDto";
import {CoreBackendServices} from "../services/core-backend.service";
import {DataSourceBase, MessageService, PagedResultDto} from "@ianitor/shared-services";

export class TenantDataSource extends DataSourceBase<TenantDto>{

  constructor(private coreBackendServices: CoreBackendServices, private messageService : MessageService) {
    super()
  }

  loadData(skip: number = 0, take: number = 10) {
    super.onBeginLoad();

    this.coreBackendServices.getTenants(skip, take).pipe(
      catchError((error) => {

        this.messageService.showError(error, "Error during load of data");

        return of(new PagedResultDto<TenantDto>())
      }))
      .subscribe(pagedResult => {

        super.onCompleteLoad(pagedResult);
      });
  }
}
