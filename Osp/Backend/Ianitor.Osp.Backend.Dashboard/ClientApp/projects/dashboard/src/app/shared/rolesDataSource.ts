import {DataSourceBase, EntitySelectDataSource, MessageService, PagedResultDto} from "@ianitor/shared-services";
import {RoleDto} from "../models/roleDto";
import {catchError} from "rxjs/operators";
import {Observable, of} from "rxjs";
import {RolesBackendService} from "../services/roles-backend.service";
import {Injectable} from "@angular/core";

@Injectable({
  providedIn: 'root'
})
export class RolesDataSource extends DataSourceBase<RoleDto> implements EntitySelectDataSource<RoleDto>{

  constructor(private rolesBackendService: RolesBackendService, private messageService : MessageService) {
    super();
  }

  public onFilter(filter : string) : Observable<PagedResultDto<RoleDto>>{

    return this.rolesBackendService.getRoles(filter, 0, 10).pipe(
      catchError((error) => {

        this.messageService.showError(error, "Error during load of data");

        return of(new PagedResultDto<RoleDto>())
      }));
  }

  public onDisplayEntity(entity: RoleDto): string{

    return entity?.name;

  }

  loadData(skip: number = 0, take: number = 10) {
    super.onBeginLoad();

    this.rolesBackendService.getRoles(null, skip, take).pipe(
      catchError((error) => {

        this.messageService.showError(error.statusText, "Error during load of data");

        return of(new PagedResultDto<RoleDto>())
      }))
      .subscribe(pagedResult => {

        super.onCompleteLoad(pagedResult);
      });
  }
}
