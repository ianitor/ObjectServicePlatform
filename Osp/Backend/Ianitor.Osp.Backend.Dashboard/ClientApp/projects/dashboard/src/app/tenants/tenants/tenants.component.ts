import { Component, OnInit } from '@angular/core';
import {TenantDataSource} from "../../shared/tenantDataSource";
import {CoreBackendServices} from "../../services/core-backend.service";
import {ConfigurationService} from "../../services/configuration.service";
import {ConfirmationService} from "@ianitor/shared-ui";
import {TenantDto} from "../../models/tenantDto";
import {MessageService} from "@ianitor/shared-services";

@Component({
  selector: 'app-tenants',
  templateUrl: './tenants.component.html',
  styleUrls: ['./tenants.component.css']
})
export class TenantsComponent implements OnInit {

  dataSource: TenantDataSource;
  isMobile: boolean;
  dataColumns: string[] = ['tenantId', 'database'];
  displayedColumns: string[] = [...this.dataColumns, 'actions'];

  constructor(private coreBackendServices: CoreBackendServices, public configurationService: ConfigurationService,
              private messageService : MessageService,
              private confirmationService: ConfirmationService) { }

  ngOnInit() {

    this.dataSource = new TenantDataSource(this.coreBackendServices, this.messageService);
    this.dataSource.loadData();
  }

  onDetach(tenantDto: TenantDto){
    this.confirmationService.showYesNoConfirmationDialog("Detach tenant", `Do you really want to detach tenant with name '${tenantDto.tenantId}'?`).subscribe(confirmResult => {

      if (confirmResult) {

        this.coreBackendServices.detachTenant(tenantDto.tenantId).subscribe(_ => {

          this.messageService.showInformation("Tenant detached.");

          this.dataSource.loadData();

        }, error => {

          this.messageService.showError(error, "Error during detaching tenant.");
        })
      }
    });
  }

  onDelete(tenantDto: TenantDto){

    this.confirmationService.showYesNoConfirmationDialog("Delete Tenant", `Do you really want to delete tenant with name '${tenantDto.tenantId}'?`).subscribe(confirmResult => {

      if (confirmResult) {

        this.coreBackendServices.deleteTenant(tenantDto.tenantId).subscribe(_ => {

          this.messageService.showInformation("Tenant deleted.");

          this.dataSource.loadData();

        }, error => {

          this.messageService.showError(error, "Error during deleting tenant.");
        })
      }
    });
  }
}
