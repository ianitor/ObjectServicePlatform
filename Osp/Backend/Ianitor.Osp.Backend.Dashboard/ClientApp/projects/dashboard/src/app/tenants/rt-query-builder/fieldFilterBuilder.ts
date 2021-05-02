import {FieldFilter} from "../../graphQL/globalTypes";
import {CkEntityDetailDto} from "../../models/ckEntityDetailDto";
import {TenantBackendService} from "../../services/tenant-backend.service";
import {CkFieldSelectBuilder} from "./ckFieldSelectBuilder";

export class FieldFilterBuilder extends CkFieldSelectBuilder {

  private _fieldFilters: FieldFilter[];
  private _dataColumns: string[];

  public get fieldFilters() {
    return this._fieldFilters;
  }

  public set fieldFilters(value) {
    if (this._fieldFilters != value) {
      this._fieldFilters = value;
    }
  }

  public get dataColumns() {
    return this._dataColumns;
  }

  public set dataColumns(value) {
    if (this._dataColumns != value) {
      this._dataColumns = value;
    }
  }

  constructor(tenantId: string, dataSourceBackendService: TenantBackendService) {
    super(tenantId, dataSourceBackendService);
    this.dataColumns = [];
    this.fieldFilters = [];
  }

  protected onCkIdPreLoad(){

    super.onCkIdPreLoad();

    this.dataColumns = [];
  }

  protected onCkIdLoaded(ckEntityDetails: CkEntityDetailDto) {

    super.onCkIdLoaded(ckEntityDetails);

    this.dataColumns = ckEntityDetails.attributes.map(value => value.attributeName).sort((n1, n2) => {
      if (n1 > n2) {
        return 1;
      }

      if (n1 < n2) {
        return -1;
      }

      return 0;
    });
  }

  protected onCkIdUnloaded() {

    super.onCkIdUnloaded();

    this.dataColumns = [];
    this.fieldFilters = [];
  }
}
