import {Component, OnInit} from '@angular/core';
import {ActivatedRoute} from "@angular/router";
import {TenantBackendService} from "../../../../services/tenant-backend.service";
import {ConfigurationService} from "../../../../services/configuration.service";
import {FormControl, FormGroup, Validators} from "@angular/forms";
import {CkEntityDetailDto} from "../../../../models/ckEntityDetailDto";
import {Scopes} from "../../../../graphQL/globalTypes";
import {EnumReader, EnumTuple} from "@ianitor/shared-services";
import {AbstractDetailsComponent} from "@ianitor/shared-ui";

@Component({
  selector: 'app-ck-entities-details',
  templateUrl: './details.component.html',
  styleUrls: ['./details.component.css']
})

export class CkEntitiesDetailsComponent extends AbstractDetailsComponent<CkEntityDetailDto> implements OnInit {

  public tenantId: string;
  public ckId: string;
  public loading: boolean;

  public constructionKitTypeDetail: CkEntityDetailDto;

  public scopeSelectionValues: EnumTuple[] = [];

  constructor(route: ActivatedRoute, private dataSourceBackendService: TenantBackendService, public configurationService: ConfigurationService) {

    super();

    this._ownerForm = new FormGroup({
      'baseType': new FormControl(),
      'ckId': new FormControl(null, [Validators.required]),
      'isAbstract': new FormControl(null),
      'isFinal': new FormControl(null),
      'scopeId': new FormControl(null, Validators.required),
    });

    route.params.subscribe(val => {
      // put the code from `ngOnInit` here
      this.tenantId = val.id;
      this.ckId = val.ckId;
      this.loading = true;

      this.dataSourceBackendService.getCkEntityDetails(this.tenantId, this.ckId).subscribe(value => {
        this.constructionKitTypeDetail = value;
        this.onLoaded();
        this.loading = false;
      });
    });

  }

  ngOnInit(): void {

    this.scopeSelectionValues = new EnumReader(Scopes).getNamesAndValues();
  }

  onLoaded() {

    this.ownerForm.setValue({
      'baseType': this.constructionKitTypeDetail.baseType?.ckId != null ? this.constructionKitTypeDetail.baseType?.ckId : "",
      'ckId': this.constructionKitTypeDetail.ckId,
      'isAbstract': this.constructionKitTypeDetail.isAbstract,
      'isFinal': this.constructionKitTypeDetail.isFinal,
      'scopeId': this.constructionKitTypeDetail.scopeId,
    });
  }

  save() {

  }
}
