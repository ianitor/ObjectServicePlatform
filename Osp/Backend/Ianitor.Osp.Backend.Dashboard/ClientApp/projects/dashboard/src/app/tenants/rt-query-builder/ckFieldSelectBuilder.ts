import {EventEmitter} from "@angular/core";
import {Observable, of} from "rxjs";
import {CkEntityDetailDto} from "../../models/ckEntityDetailDto";
import {map} from "rxjs/operators";
import {TenantBackendService} from "../../services/tenant-backend.service";

export class CkFieldSelectBuilder{

  private _queryCkId: string;
  private _ckEntityDetails: CkEntityDetailDto;


  public queryCkIdChange = new EventEmitter<string>();

  public get queryCkId() {
    return this._queryCkId;
  }

  public set queryCkId(value) {
    if (this._queryCkId != value) {
      this._queryCkId = value;
      this.processCkIdChange().subscribe(_ => {
        this.queryCkIdChange.emit(value);
      });
    }
  }


  public get ckEntityDetails(){
    return this._ckEntityDetails;
  }

  constructor(protected tenantId: string, protected dataSourceBackendService: TenantBackendService) {
    this._queryCkId = null;
  }


  private processCkIdChange(): Observable<CkEntityDetailDto> {
    this.onCkIdPreLoad();

    if (this.queryCkId) {
      return this.dataSourceBackendService.getCkEntityDetails(this.tenantId, this.queryCkId).pipe(map(value => {

        this._ckEntityDetails = value;
        this.onCkIdLoaded(value);
        return value;
      }));
    } else {

      this._ckEntityDetails = null;
      this.onCkIdUnloaded();
      return of(null);
    }
  }

  protected onCkIdPreLoad()
  {

  }

  protected onCkIdLoaded(ckEntityDetails: CkEntityDetailDto) {

  }

  protected onCkIdUnloaded() {

  }
}
