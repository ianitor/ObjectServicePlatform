import {Component, Input, OnInit} from '@angular/core';
import {FormControl, FormGroup} from "@angular/forms";
import {TenantBackendService} from "../../../services/tenant-backend.service";
import {RtQueryBuilder} from "../rtQueryBuilder";
import {AbstractDetailsComponent} from "@ianitor/shared-ui";
import {QuerySelectDataSource} from "../../../shared/querySelectDataSource";

@Component({
  selector: 'app-rtQueryBuilder-query-selector',
  templateUrl: './query-selector.component.html',
  styleUrls: ['./query-selector.component.css']
})
export class QuerySelectorComponent extends AbstractDetailsComponent<any> implements OnInit {


  @Input() tenantId: string;

  private _rtQueryBuilder: RtQueryBuilder;
  @Input() get rtQueryBuilder(){
    return this._rtQueryBuilder;
  }

  set rtQueryBuilder(value: RtQueryBuilder){
    this._rtQueryBuilder = value;
  }

  public querySelectDataSource : QuerySelectDataSource;

  constructor(private dataSourceBackendService: TenantBackendService) {
    super();

    this._ownerForm = new FormGroup({
      'querySelect': new FormControl(null)
    });

    this.ownerForm.get('querySelect').valueChanges.subscribe(selectedQueryDto => {
      this.rtQueryBuilder.load(selectedQueryDto.rtId);
    });

  }

  ngOnInit(): void {

    this.querySelectDataSource = new QuerySelectDataSource(this.tenantId, this.dataSourceBackendService);

    this.rtQueryBuilder.queryChange.subscribe(queryDetailsDto=>{
      this.ownerForm.get('querySelect').setValue(queryDetailsDto);
    });
  }
}
