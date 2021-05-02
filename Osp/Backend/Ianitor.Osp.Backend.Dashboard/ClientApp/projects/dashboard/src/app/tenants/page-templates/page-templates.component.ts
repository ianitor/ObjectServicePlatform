import {Component, ElementRef, OnInit, ViewChild} from '@angular/core';
import {ActivatedRoute} from "@angular/router";
import {OspListNavigation, OspListNavigationDataInfo, SearchFilterTypes} from "@ianitor/osp-ui";
import {MatPaginator} from "@angular/material/paginator";
import {MatSort} from "@angular/material/sort";
import {TenantBackendService} from "../../services/tenant-backend.service";
import {ConfirmationService} from "@ianitor/shared-ui";
import {PageDataSource} from "../../shared/pageDataSource";
import {PageDto} from "../../models/pageDto";
import {MessageService} from "@ianitor/shared-services";

@Component({
  selector: 'app-page-templates',
  templateUrl: './page-templates.component.html',
  styleUrls: ['./page-templates.component.css']
})
export class PageTemplatesComponent implements OnInit {

  private readonly tenantId: string;
  public readonly dataSource: PageDataSource;
  public readonly serviceHooksColumns = ["wellKnownName"];
  public readonly displayColumns = [...this.serviceHooksColumns, "actions"];
  private ospListNavigation: OspListNavigation;

  @ViewChild(MatPaginator, {static: false}) paginator: MatPaginator;
  @ViewChild(MatSort, {static: false}) sort: MatSort;
  @ViewChild('input', {static: false}) input: ElementRef;

  constructor(route: ActivatedRoute, private dataSourceBackendService: TenantBackendService, private messageService : MessageService, private confirmationService : ConfirmationService) {
    this.tenantId = route.snapshot.paramMap.get('tenantId');
    this.dataSource = new PageDataSource(dataSourceBackendService, messageService);
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

  onDelete(pageDto: PageDto){


    this.confirmationService.showYesNoConfirmationDialog("Delete Page", `Do you really want to delete page with name '${pageDto.wellKnownName}'?`).subscribe(confirmResult => {

      if (confirmResult) {
        this.dataSourceBackendService.deletePage(this.tenantId, pageDto).subscribe(_ => {

          this.messageService.showInformation("Page deleted.");

        }, error => {

          this.messageService.showError(error, "Error during deleting page.");
        })
      }
    });
  }

}
