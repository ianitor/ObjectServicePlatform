import { Component, OnInit } from '@angular/core';
import {ActivatedRoute} from "@angular/router";
import {TenantBackendService} from "../../../services/tenant-backend.service";
import {ConfigurationService} from "../../../services/configuration.service";
import {MessageService, ObjectCloner} from "@ianitor/shared-services";
import {FormControl, FormGroup, Validators} from "@angular/forms";
import {ServiceHookDetailsDto} from "../../../models/serviceHookDetailsDto";
import {FieldFilterBuilder} from "../../rt-query-builder/fieldFilterBuilder";
import {GraphQLCloneIgnoredProperties} from "../../../../../../ianitor/osp-services/src/lib/shared/graphQL";
import {CommonValidators} from "@ianitor/shared-ui";

@Component({
  selector: 'app-service-hook-details',
  templateUrl: './service-hook-details.component.html',
  styleUrls: ['./service-hook-details.component.scss']
})
export class ServiceHookDetailsComponent implements OnInit {

  public tenantId : string;
  private serviceHookId : string;
  public loading: boolean;
  private serviceHookDetailsDto : ServiceHookDetailsDto;
  public readonly ownerForm: FormGroup;
  public fieldFilterBuilder: FieldFilterBuilder;

  constructor(route: ActivatedRoute, private dataSourceBackendService: TenantBackendService, public configurationService: ConfigurationService, private messageService: MessageService) {

    this.ownerForm = new FormGroup({
      'enabled': new FormControl('', [Validators.required]),
      'name': new FormControl('', [Validators.required]),
      'serviceHookBaseUri': new FormControl('', [Validators.required, CommonValidators.httpUri()]),
      'serviceHookAction': new FormControl('', Validators.required)
    });

    route.params.subscribe(val => {
      // put the code from `ngOnInit` here
      this.tenantId = val.tenantId;
      this.serviceHookId = val.serviceHookId;
      this.loading = true;

      this.fieldFilterBuilder = new FieldFilterBuilder(this.tenantId, dataSourceBackendService);

      if (this.serviceHookId)
      {
        this.dataSourceBackendService.getServiceHookDetails(this.tenantId, this.serviceHookId).subscribe(value => {
          this.serviceHookDetailsDto = value;
          this.fieldFilterBuilder.queryCkId = value.queryCkId;
          this.fieldFilterBuilder.fieldFilters = JSON.parse(value.fieldFilter);
          this.ownerForm.setValue(ObjectCloner.cloneObject(value, [...GraphQLCloneIgnoredProperties, "queryCkId", "fieldFilter"]));
          this.onLoaded();
          this.loading = false;
        });
      }
      else {
        this.loading = false;

        this.ownerForm.setValue(<ServiceHookDetailsDto>{
          enabled: true,
          name: null,
          serviceHookBaseUri: null,
          serviceHookAction: null
        });

      }

    });



  }

  ngOnInit(): void {
  }

  onLoaded(){

  }

  public hasError = (controlName: string, errorName: string) => {
    return this.ownerForm.controls[controlName].hasError(errorName);
  };

  onSave() {

    this.loading = true;

    const queryDetails = <ServiceHookDetailsDto>{
      queryCkId: this.fieldFilterBuilder.queryCkId,
      fieldFilter: JSON.stringify(this.fieldFilterBuilder.fieldFilters)
    };

    const transient = <ServiceHookDetailsDto> this.ownerForm.value;

    Object.assign(queryDetails, transient);

    if (this.serviceHookDetailsDto?.rtId)
    {
      queryDetails.rtId = this.serviceHookDetailsDto.rtId;
      this.dataSourceBackendService.updateServiceHook(this.tenantId, queryDetails).subscribe(value => {

        this.serviceHookDetailsDto = value;
        this.loading = false;
        this.messageService.showInformation("Service Hook saved.");
      }, error => {

        this.loading = false;
        this.messageService.showError(error, "Error during saving of service hook.");
      });
    }
    else {
      this.dataSourceBackendService.createServiceHook(this.tenantId, queryDetails).subscribe(value => {

        this.serviceHookDetailsDto = value;
        this.loading = false;

        this.messageService.showInformation("Service Hook created.");

      }, error => {

        this.loading = false;
        this.messageService.showError(error, "Error during saving of service hook.");
      });
    }
  }
}
