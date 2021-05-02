import {BehaviorSubject, Observable} from "rxjs";
import {CollectionViewer, DataSource} from "@angular/cdk/collections";
import {PagedResultDto} from "../models/pagedResultDto";

export class DataSourceBase<TDto> implements DataSource<TDto> {
  private dataSubject = new BehaviorSubject<Array<TDto>>([]);
  private loadingSubject = new BehaviorSubject<boolean>(false);
  private totalCountSubject = new BehaviorSubject<number>(0);

  public readonly loading$ = this.loadingSubject.asObservable();
  public readonly totalCount$ = this.totalCountSubject.asObservable();

  constructor() {
  }

  connect(collectionViewer: CollectionViewer): Observable<TDto[]> {
    return this.dataSubject.asObservable();
  }

  disconnect(collectionViewer: CollectionViewer): void {
    this.dataSubject.complete();
    this.loadingSubject.complete();
  }

  clear(){
    this.loadingSubject.next(false);
    this.dataSubject.next([]);
    this.totalCountSubject.next(0);
  }

  protected onBeginLoad()
  {
    this.loadingSubject.next(true);
  }

  protected onCompleteLoad(pagedResult: PagedResultDto<TDto>)
  {
    this.loadingSubject.next(false);
    this.dataSubject.next(pagedResult.list);
    this.totalCountSubject.next(pagedResult.totalCount);
  }
}
