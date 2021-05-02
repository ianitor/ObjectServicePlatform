import {TenantBackendService} from "../services/tenant-backend.service";
import {PageDto} from "../models/pageDto";
import {FieldFilter, SearchFilter, Sort} from "../graphQL/globalTypes";
import {Observable} from "rxjs";
import {GraphQLDataSource} from "@ianitor/osp-ui";
import {MessageService, PagedResultDto} from "@ianitor/shared-services";

export class PageDataSource extends GraphQLDataSource<PageDto>{
  constructor(private dataSourceBackendService: TenantBackendService, messageService : MessageService) {
    super(messageService);
  }

  protected executeLoad(dataSource: string, skip: number = 0, take: number = 10, searchFilter: SearchFilter = null, fieldFilter: FieldFilter[] = null, sort: Sort[] = null) : Observable<PagedResultDto<PageDto>>
  {
    return this.dataSourceBackendService.getPages(dataSource, skip, take, searchFilter, fieldFilter, sort);
  }
}
