import {Component, Input, OnChanges, SimpleChanges} from '@angular/core';
import {FormControl, FormGroup, Validators} from "@angular/forms";
import {TenantBackendService} from "../../../../services/tenant-backend.service";
import {CkFieldSelectBuilder} from "../../ckFieldSelectBuilder";
import {CkEntitySelectDataSource} from "../../../../shared/ckEntitySelectDataSource";
import {AbstractDetailsComponent} from "../../../../../../../ianitor/shared-ui/src/lib/shared/abstractDetailsComponent";

@Component({
  selector: 'app-rtQueryBuilder-ck-selector',
  templateUrl: './ck-selector.component.html',
  styleUrls: ['./ck-selector.component.css']
})
export class CkSelectorComponent extends AbstractDetailsComponent<any> implements OnChanges {

  @Input() tenantId: string;
  @Input() ckFieldSelectBuilder: CkFieldSelectBuilder;

  public ckEntitySelectDataSource: CkEntitySelectDataSource;

  constructor(private dataSourceBackendService: TenantBackendService) {
    super();

    super._ownerForm = new FormGroup({
      'ckIdSelect': new FormControl(null, Validators.required)
    });
  }

  ngOnInit(): void {

    this.ckEntitySelectDataSource = new CkEntitySelectDataSource(this.tenantId, this.dataSourceBackendService);

    this.ownerForm.get('ckIdSelect').valueChanges.subscribe(selectedCkEntity => {
      this.ckFieldSelectBuilder.queryCkId = selectedCkEntity?.ckId;
    });
  }

  ngOnChanges(changes: SimpleChanges): void {

    this.ckFieldSelectBuilder?.queryCkIdChange.subscribe(ckId => {
      if (this.ownerForm.get('ckIdSelect').value !== this.ckFieldSelectBuilder.ckEntityDetails) {
        this.ownerForm.get('ckIdSelect').setValue(this.ckFieldSelectBuilder.ckEntityDetails);
      }
    });
  }
}
