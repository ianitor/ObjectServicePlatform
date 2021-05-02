import {Observable, of} from "rxjs";
import {catchError} from "rxjs/operators";
import {FieldFilter, SearchFilter, Sort} from "./globalTypes";
import {DataSourceBase, MessageService, PagedResultDto} from "@ianitor/shared-services";

export class GraphQLDataSource<TDto> extends DataSourceBase<TDto>{

  constructor(protected messageService : MessageService) {
    super();
  }

  protected executeLoad(tenantId: string, skip: number = 0, take: number = 10, searchFilter: SearchFilter = null, fieldFilter: FieldFilter[] = null, sort: Sort[] = null) : Observable<PagedResultDto<TDto>>
  {
    return of(new PagedResultDto<TDto>())
  }

  public loadData(tenantId: string, skip: number = 0, take: number = 10, searchFilter: SearchFilter = null, fieldFilter: FieldFilter[] = null, sort: Sort[] = null) {

    super.onBeginLoad();

    this.executeLoad(tenantId, skip, take, searchFilter, fieldFilter, sort).pipe(
      catchError((error) => {

        this.messageService.showError(error, "Error during load of data");

        return of(new PagedResultDto<TDto>())
      }))
      .subscribe(pagedResult => {

        super.onCompleteLoad(pagedResult);
      });
  }
}
