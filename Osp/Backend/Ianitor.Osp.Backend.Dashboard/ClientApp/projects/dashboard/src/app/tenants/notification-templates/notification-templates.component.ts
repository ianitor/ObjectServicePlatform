import {Component, ElementRef, OnInit, ViewChild} from '@angular/core';
import {ActivatedRoute} from "@angular/router";
import {TenantBackendService} from "../../services/tenant-backend.service";
import {NotificationTemplatesDataSource} from "../../shared/notificationTemplatesDataSource";
import {ConfirmationService} from "@ianitor/shared-ui";
import {OspListNavigation, OspListNavigationDataInfo, SearchFilterTypes} from "@ianitor/osp-ui";
import {MatPaginator} from "@angular/material/paginator";
import {MatSort} from "@angular/material/sort";
import {NotificationTemplateDto} from "../../models/notificationTemplateDto";
import {MessageService} from "@ianitor/shared-services";

@Component({
  selector: 'app-notification-templates',
  templateUrl: './notification-templates.component.html',
  styleUrls: ['./notification-templates.component.css']
})
export class NotificationTemplatesComponent implements OnInit {

  public readonly tenantId: string;
  public readonly dataSource: NotificationTemplatesDataSource;
  public readonly dataColumns: string[] = ['wellKnownName', 'subjectTemplate'];
  public readonly displayedColumns: string[] = [...this.dataColumns, 'actions'];

  private ospListNavigation: OspListNavigation;

  @ViewChild(MatPaginator, {static: false}) paginator: MatPaginator;
  @ViewChild(MatSort, {static: false}) sort: MatSort;
  @ViewChild('input', {static: false}) input: ElementRef;

  constructor(route: ActivatedRoute, private dataSourceBackendService: TenantBackendService,
              private messageService : MessageService, private confirmationService : ConfirmationService) {
    this.tenantId = route.snapshot.paramMap.get('id');
    this.dataSource = new NotificationTemplatesDataSource(this.dataSourceBackendService, this.messageService);
  }

  ngOnInit() {

    this.dataSource.loadData(this.tenantId);
  }

  ngAfterViewInit() {

    this.ospListNavigation = new OspListNavigation(this.paginator, this.sort, this.input, {
      language: 'en',
      searchFilterType: SearchFilterTypes.ATTRIBUTE_FILTER,
      searchFilterAttributeNames: this.displayedColumns
    });
    this.ospListNavigation.loadDataRequest.subscribe((x) => this.loadData(x));
    this.ospListNavigation.init();
  }

  loadData(loadDataInfo: OspListNavigationDataInfo) {

    this.dataSource.loadData(this.tenantId,
      loadDataInfo.skip,
      loadDataInfo.take, loadDataInfo.searchFilter, null, loadDataInfo.sort);
  }

  onDelete(notificationTemplateDto: NotificationTemplateDto){


    this.confirmationService.showYesNoConfirmationDialog("Delete Notification Template", `Do you really want to delete notification template with name '${notificationTemplateDto.wellKnownName}'?`).subscribe(confirmResult => {

      if (confirmResult) {
        this.dataSourceBackendService.deleteNotificationTemplate(this.tenantId, notificationTemplateDto).subscribe(_ => {

          this.messageService.showInformation("Notification template deleted.");

        }, error => {

          this.messageService.showError(error, "Error during deleting notification template.");
        })
      }
    });
  }
}
