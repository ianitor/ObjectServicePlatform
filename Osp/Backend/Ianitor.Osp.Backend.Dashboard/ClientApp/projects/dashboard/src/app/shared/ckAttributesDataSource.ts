import {TenantBackendService} from "../services/tenant-backend.service";
import {CkAttributeDto} from "../models/ckAttributeDto";
import {FieldFilter, SearchFilter, Sort} from "../graphQL/globalTypes";
import {Observable} from "rxjs";
import {GraphQLDataSource} from "@ianitor/osp-ui";
import {MessageService, PagedResultDto} from "@ianitor/shared-services";

export class CkAttributesDataSource extends GraphQLDataSource<CkAttributeDto> {

  constructor(private dataSourceBackendService: TenantBackendService, messageService: MessageService) {
    super(messageService);
  }

  protected executeLoad(tenantId: string, skip: number = 0, take: number = 10, searchFilter: SearchFilter = null, fieldFilter: FieldFilter[] = null, sort: Sort[] = null) : Observable<PagedResultDto<CkAttributeDto>>
  {
    return this.dataSourceBackendService.getCkAttributes(tenantId, skip, take, searchFilter, sort);
  }
}
