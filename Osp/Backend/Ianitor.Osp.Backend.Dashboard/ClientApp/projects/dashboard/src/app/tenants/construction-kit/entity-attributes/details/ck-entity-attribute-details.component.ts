import { Component, OnInit } from '@angular/core';
import {FormControl, FormGroup, Validators} from "@angular/forms";
import {ActivatedRoute} from "@angular/router";
import {TenantBackendService} from "../../../../services/tenant-backend.service";
import {ConfigurationService} from "../../../../services/configuration.service";
import {CkEntityAttributeDetailDto} from "../../../../models/ckEntityAttributeDetailDto";
import {MessageService} from "@ianitor/shared-services";

@Component({
  selector: 'app-ck-entity-attributes-details',
  templateUrl: './ck-entity-attribute-details.component.html',
  styleUrls: ['./ck-entity-attribute-details.component.css']
})
export class CkEntityAttributeDetailsComponent implements OnInit {

  public tenantId: string;
  public attributeName: string;
  public ckId: string;
  public loading: boolean;

  public ckEntityAttributeDetailDto: CkEntityAttributeDetailDto;

  public ownerForm: FormGroup;

  constructor(route: ActivatedRoute, private dataSourceBackendService: TenantBackendService, public configurationService: ConfigurationService, private messageService: MessageService) {

    route.params.subscribe(val => {
      // put the code from `ngOnInit` here
      this.tenantId = val.id;
      this.ckId = val.ckId;
      this.attributeName = val.attributeName;
      this.loading = true;

      this.dataSourceBackendService.getCkEntityAttributeDetails(this.tenantId, this.ckId, this.attributeName).subscribe(value => {
        this.ckEntityAttributeDetailDto = value;
        this.onLoaded();
        this.loading = false;
      });
    });
  }

  ngOnInit(): void {
  }

  onLoaded() {
    this.ownerForm = new FormGroup({
      'attributeName': new FormControl(this.ckEntityAttributeDetailDto.attributeName, [Validators.required]),
      'attributeId': new FormControl(this.ckEntityAttributeDetailDto.attributeId, [Validators.required]),
      'autoCompleteFilter': new FormControl(this.ckEntityAttributeDetailDto.autoCompleteFilter),
      'autoCompleteLimit': new FormControl(this.ckEntityAttributeDetailDto.autoCompleteLimit),
      'autoCompleteTexts': new FormControl(this.ckEntityAttributeDetailDto.autoCompleteTexts),
      'autoIncrementReference': new FormControl(this.ckEntityAttributeDetailDto.autoIncrementReference),
      'isAutoCompleteEnabled': new FormControl(this.ckEntityAttributeDetailDto.isAutoCompleteEnabled)
    });
  }

  public hasError = (controlName: string, errorName: string) => {
    return this.ownerForm.controls[controlName].hasError(errorName);
  };

  save() {

  }


}
