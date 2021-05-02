import { Component, OnInit } from '@angular/core';
import {ActivatedRoute} from "@angular/router";
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {TenantDto} from "../../../models/tenantDto";
import {CoreBackendServices} from "../../../services/core-backend.service";
import {MessageService, ObjectCloner} from "@ianitor/shared-services";
import {GraphQLCloneIgnoredProperties} from "@ianitor/osp-services";

@Component({
  selector: 'app-tenant-details',
  templateUrl: './tenant-details.component.html',
  styleUrls: ['./tenant-details.component.scss']
})
export class TenantDetailsComponent implements OnInit {

  private readonly tenantId: string;
  private readonly isAttach: boolean;
  public loading: boolean;

  public readonly tenantForm: FormGroup;

  private tenantDto: TenantDto;

  constructor(route: ActivatedRoute, private fb: FormBuilder, private coreBackendServices: CoreBackendServices, private messageService: MessageService) {
    this.tenantId = route.snapshot.paramMap.get('tenantId');
    this.isAttach = route.snapshot.url[1].path === "attach";

    this.tenantForm = this.fb.group({
      tenantId: ['', Validators.required],
      database: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    if (this.tenantId) {
      this.coreBackendServices.getTenantDetails(this.tenantId).subscribe(value => {
        this.tenantDto = value;

        this.tenantForm.setValue({
          tenantId: value.tenantId,
          database: value.database
        });
        this.loading = false;

      })
    } else {
      this.tenantDto = <TenantDto>{};

      this.tenantForm.setValue({tenantId: null, database: null});
      this.loading = false;
    }
  }

  save() {

    this.tenantForm.disable();
    this.tenantForm.updateValueAndValidity();

    const tenantDto = ObjectCloner.cloneObject<TenantDto, any>(this.tenantForm.value, [...GraphQLCloneIgnoredProperties]);


    if (this.isAttach) {

      this.coreBackendServices.attachTenant(tenantDto).subscribe(_ => {
        this.messageService.showInformation(`Tenant '${tenantDto.tenantId}' has been attached.`);

        this.tenantForm.reset();
        this.tenantForm.enable();

      }, error => {
        this.tenantForm.enable();
      })

    } else {
      this.coreBackendServices.createTenant(tenantDto).subscribe(_ => {
        this.messageService.showInformation(`Tenant '${tenantDto.tenantId}' has been created.`);

        this.tenantForm.reset();
        this.tenantForm.enable();

      }, error => {
        this.tenantForm.enable();
      })
    }
  }
}
