import { Component, OnInit } from '@angular/core';
import {ConfigurationService} from "../../services/configuration.service";
import {ConstructionKitTypeDataSource} from "../../shared/constructionKitTypeDataSource";
import {TenantBackendService} from "../../services/tenant-backend.service";
import {ActivatedRoute} from "@angular/router";
import {MessageService} from "@ianitor/shared-services";

@Component({
  selector: 'app-construction-kit',
  templateUrl: './construction-kit.component.html',
  styleUrls: ['./construction-kit.component.css']
})
export class ConstructionKitComponent implements OnInit {

  public readonly tenantId: string;
  dataSource: ConstructionKitTypeDataSource;
  isMobile: boolean;
  dataColumns: string[] = ['ckId', 'baseType', 'scopeId', 'isAbstract', 'isFinal'];
  displayedColumns: string[] = [...this.dataColumns, 'actions'];

  constructor(route: ActivatedRoute, private dataSourceBackendService: TenantBackendService, public configurationService: ConfigurationService, private messageService : MessageService) {
    this.tenantId = route.snapshot.paramMap.get('id');
  }

  ngOnInit() {

    this.dataSource = new ConstructionKitTypeDataSource(this.dataSourceBackendService, this.messageService);
    this.dataSource.loadData(this.tenantId);
  }
}
