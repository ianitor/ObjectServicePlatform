import {TenantBackendService} from "../services/tenant-backend.service";
import {FieldFilter, SearchFilter, Sort} from "../graphQL/globalTypes";
import {Observable} from "rxjs";
import {NotificationTemplateDto} from "../models/notificationTemplateDto";
import {GraphQLDataSource} from "@ianitor/osp-ui";
import {MessageService, PagedResultDto} from "@ianitor/shared-services";

export class NotificationTemplatesDataSource extends GraphQLDataSource<NotificationTemplateDto> {
  constructor(private dataSourceBackendService: TenantBackendService, messageService : MessageService) {
    super(messageService);
  }

  protected executeLoad(tenantId: string, skip: number = 0, take: number = 10, searchFilter: SearchFilter = null, fieldFilter: FieldFilter[] = null, sort: Sort[] = null) : Observable<PagedResultDto<NotificationTemplateDto>>
  {
    return this.dataSourceBackendService.getNotificationTemplates(tenantId, skip, take, searchFilter, sort);
  }
}
