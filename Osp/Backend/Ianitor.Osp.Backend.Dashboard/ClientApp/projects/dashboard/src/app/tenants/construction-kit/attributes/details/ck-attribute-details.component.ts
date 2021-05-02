import {Component, OnInit} from '@angular/core';
import {ActivatedRoute} from "@angular/router";
import {TenantBackendService} from "../../../../services/tenant-backend.service";
import {ConfigurationService} from "../../../../services/configuration.service";
import {FormControl, FormGroup, Validators} from "@angular/forms";
import {CkAttributeDetailDto} from "../../../../models/ckAttributeDetailDto";
import {AttributeValueType, Scopes} from "../../../../graphQL/globalTypes";
import {EnumReader, EnumTuple} from "@ianitor/shared-services";

@Component({
  selector: 'app-details',
  templateUrl: './ck-attribute-details.component.html',
  styleUrls: ['./ck-attribute-details.component.css']
})
export class CkAttributeDetailsComponent implements OnInit {

  public tenantId: string;
  public attributeId: string;
  public loading: boolean;

  public ckAttributeDetailDto: CkAttributeDetailDto;

  public ownerForm: FormGroup;

  public scopeSelectionValues: EnumTuple[] = [];
  public attributeValueTypes: EnumTuple[] = [];

  constructor(route: ActivatedRoute, private dataSourceBackendService: TenantBackendService, public configurationService: ConfigurationService) {

    route.params.subscribe(val => {
      // put the code from `ngOnInit` here
      this.tenantId = val.id;
      this.attributeId = val.attributeId;
      this.loading = true;

      this.dataSourceBackendService.getCkAttributeDetails(this.tenantId, this.attributeId).subscribe(value => {
        this.ckAttributeDetailDto = value;
        this.onLoaded();
        this.loading = false;
      });
    });
  }

  ngOnInit(): void {
    this.scopeSelectionValues = new EnumReader(Scopes).getNamesAndValues();
    this.attributeValueTypes = new EnumReader(AttributeValueType).getNamesAndValues();
  }

  onLoaded() {
    this.ownerForm = new FormGroup({
      'attributeId': new FormControl(this.ckAttributeDetailDto.attributeId, [Validators.required]),
      'attributeValueType': new FormControl(this.ckAttributeDetailDto.attributeValueType, [Validators.required]),
      'scopeId': new FormControl(this.ckAttributeDetailDto.scopeId, Validators.required),
      'defaultValue': new FormControl(this.ckAttributeDetailDto.defaultValue),
      'defaultValues': new FormControl(this.ckAttributeDetailDto.defaultValues),
      'selectionValues': new FormControl(this.ckAttributeDetailDto.selectionValues)
    });
  }

  public hasError = (controlName: string, errorName: string) => {
    return this.ownerForm.controls[controlName].hasError(errorName);
  };

  save() {

  }

}
