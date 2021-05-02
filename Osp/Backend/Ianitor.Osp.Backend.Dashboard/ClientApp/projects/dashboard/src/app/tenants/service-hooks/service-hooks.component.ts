import {Component, ElementRef, OnInit, ViewChild} from '@angular/core';
import {ActivatedRoute} from "@angular/router";
import {ServiceHookDataSource} from "../../shared/serviceHookDataSource";
import {TenantBackendService} from "../../services/tenant-backend.service";
import {ConfirmationService} from "@ianitor/shared-ui";
import {ServiceHookDto} from "../../models/serviceHookDto";
import {OspListNavigation, OspListNavigationDataInfo, SearchFilterTypes} from "@ianitor/osp-ui";
import {MatPaginator} from "@angular/material/paginator";
import {MatSort} from "@angular/material/sort";
import {MessageService} from "@ianitor/shared-services";

@Component({
  selector: 'app-service-hooks',
  templateUrl: './service-hooks.component.html',
  styleUrls: ['./service-hooks.component.css']
})
export class ServiceHooksComponent implements OnInit {

  private readonly tenantId: string;
  public readonly dataSource: ServiceHookDataSource;
  public readonly serviceHooksColumns = ["enabled", "name", "queryCkId", "serviceHookBaseUri", "serviceHookAction"];
  public readonly serviceHooksDisplayColumns = [...this.serviceHooksColumns, "actions"];
  private ospListNavigation: OspListNavigation;

  @ViewChild(MatPaginator, {static: false}) paginator: MatPaginator;
  @ViewChild(MatSort, {static: false}) sort: MatSort;
  @ViewChild('input', {static: false}) input: ElementRef;

  constructor(route: ActivatedRoute, private dataSourceBackendService: TenantBackendService, private messageService : MessageService, private confirmationService : ConfirmationService) {
    this.tenantId = route.snapshot.paramMap.get('tenantId');
    this.dataSource = new ServiceHookDataSource(dataSourceBackendService, messageService);
  }

  ngOnInit() {

    this.dataSource.loadData(this.tenantId);
  }

  ngAfterViewInit() {

    this.ospListNavigation = new OspListNavigation(this.paginator, this.sort, this.input, {
      language: 'en',
      searchFilterType: SearchFilterTypes.ATTRIBUTE_FILTER,
      searchFilterAttributeNames: this.serviceHooksColumns
    });
    this.ospListNavigation.loadDataRequest.subscribe((x) => this.loadData(x));
    this.ospListNavigation.init();
  }

  loadData(loadDataInfo: OspListNavigationDataInfo) {

    this.dataSource.loadData(this.tenantId,
      loadDataInfo.skip,
      loadDataInfo.take, loadDataInfo.searchFilter, null, loadDataInfo.sort);
  }

  onDelete(serviceHookDto: ServiceHookDto){


    this.confirmationService.showYesNoConfirmationDialog("Delete Service Hook", `Do you really want to delete service hook with name '${serviceHookDto.name}'`).subscribe(confirmResult => {

      if (confirmResult) {

        this.dataSourceBackendService.deleteServiceHook(this.tenantId, serviceHookDto).subscribe(_ => {

          this.messageService.showInformation("Service hook deleted.");

        }, error => {

          this.messageService.showError(error, "Error during deleting service hook.");
        })
      }
    });
  }
}
