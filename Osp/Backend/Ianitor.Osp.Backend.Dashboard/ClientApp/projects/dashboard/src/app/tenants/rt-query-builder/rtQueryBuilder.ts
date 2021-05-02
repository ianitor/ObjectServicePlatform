import {EventEmitter} from "@angular/core";
import {QueryDetailsDto} from "../../models/queryDetailsDto";
import {TenantBackendService} from "../../services/tenant-backend.service";
import {RtEntitiesDataSource} from "../../shared/rtEntitiesDataSource";
import {map} from "rxjs/operators";
import {Sort} from "../../graphQL/globalTypes";
import {MessageService} from "@ianitor/shared-services";
import {FieldFilterBuilder} from "./fieldFilterBuilder";
import {CkEntityDetailDto} from "../../models/ckEntityDetailDto";

export class RtQueryBuilder extends FieldFilterBuilder {

  private _query: QueryDetailsDto;
  private _queryName: string;
  private _selectedDataColumns: string[];
  private _sortColumns: Sort[];
  private _take: number;
  private _skip: number;
  public readonly dataSource: RtEntitiesDataSource;

  public  get take(){
    return this._take;
  }
  public set take(value: number){
    this._take = value;
  }

  public  get skip(){
    return this._skip;
  }
  public set skip(value: number){
    this._skip = value;
  }

  public queryChange = new EventEmitter<QueryDetailsDto>();
  public get query(){
    return this._query;
  }
  public set query(value){
    if (this._query != value) {
      this._query = value;
      this.queryChange.emit(value);
      this.processQueryChange();
    }
  }


  public queryNameChange = new EventEmitter<string>();
  public get queryName(){
    return this._queryName;
  }
  public set queryName(value){
    if (this._queryName != value) {
      this._queryName = value;
      this.queryNameChange.emit(value);
    }
  }

  public get selectedDataColumns(){
    return this._selectedDataColumns;
  }
  public set selectedDataColumns(value){
    if (this._selectedDataColumns != value) {
      this._selectedDataColumns = value;
    }
  }

  public get sortColumns(){
    return this._sortColumns;
  }
  public set sortColumns(value){
    if (this._sortColumns != value) {
      this._sortColumns = value;
    }
  }

  private processQueryChange(){

    if (this.query)
    {
      this.queryCkId = this.query.queryCkId;
      this.queryName = this.query.name;
    }
    else {
      this.queryCkId = null;
      this.queryName = null;
      this.dataSource.clear();
    }
  }

  protected onCkIdLoaded(ckEntityDetails: CkEntityDetailDto){

    super.onCkIdLoaded(ckEntityDetails);

    if (this.query) {
      this.selectedDataColumns = JSON.parse(this.query.columns);
      this.sortColumns = JSON.parse(this.query.sorting);
      if (this.query.fieldFilter) {
        this.fieldFilters = JSON.parse(this.query.fieldFilter);
        this.fieldFilters.forEach(value1 => {
          if (value1.attributeName === undefined) {
            value1.attributeName = null;
          }
        })
      }
    }
  }

  protected onCkIdUnloaded(){

    super.onCkIdUnloaded();

    this.selectedDataColumns = [];
    this.sortColumns = [];
  }

  constructor(tenantId: string, dataSourceBackendService: TenantBackendService, private messageService: MessageService) {

    super(tenantId, dataSourceBackendService);

    this.dataSource = new RtEntitiesDataSource(this.dataSourceBackendService, this.messageService);
    this.processQueryChange();
    this.selectedDataColumns = [];
    this.sortColumns = [];
  }

  new(){
    this.query = null;

  }

  load(queryId: string){
    if (queryId) {

      this.dataSourceBackendService.getQueryDetails(this.tenantId, queryId).subscribe(value => {
        this.query = value;
      }, error => {
        this.messageService.showError(error, `Query ID '${queryId}' in tenant '${this.tenantId}' failed to load.`)
      });
    } else {
      this.query = null;
    }
  }

  save(){

    if (!this.queryName){
      throw new Error("'queryName' must have a value.")
    }
    if (!this.queryCkId){
      throw new Error("'queryCkId' must have a value.")
    }
    if (this.selectedDataColumns.length === 0){
      throw new Error("'dataColumns' must contain at least one column.")
    }

    const queryDetails = <QueryDetailsDto>{
      name: this.queryName,
      queryCkId: this.queryCkId,
      columns: JSON.stringify(this.selectedDataColumns),
      sorting: JSON.stringify(this.sortColumns),
      fieldFilter: JSON.stringify(this.fieldFilters)
    };

    if (this.query?.rtId)
    {
      queryDetails.rtId = this.query.rtId;
      return this.dataSourceBackendService.updateQuery(this.tenantId, queryDetails).pipe(map(value => {
        this.query = value;
        return value;
      }));
    }

    return this.dataSourceBackendService.createQuery(this.tenantId, queryDetails).pipe(map(value => {
      this.query = value;
      return value;
    }));
  }

  apply() {

    this.dataSource.loadData(this.tenantId, this.queryCkId, this.selectedDataColumns, this.skip, this.take, null, this.fieldFilters, this.sortColumns);

  }
}
