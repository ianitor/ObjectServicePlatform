import {of} from "rxjs";
import {UserDto} from "../models/userDto";
import {IdentityBackendServices} from "../services/identity-backend.service";
import {catchError} from "rxjs/operators";
import {DataSourceBase, MessageService, PagedResultDto} from "@ianitor/shared-services";
import {Injectable} from "@angular/core";

@Injectable({
  providedIn: 'root'
})
export class UsersDataSource extends DataSourceBase<UserDto> {

  constructor(private identityBackendServices: IdentityBackendServices, private messageService : MessageService) {
    super();
  }

  loadData(skip: number = 0, take: number = 10) {
    super.onBeginLoad();

    this.identityBackendServices.getUsers(skip, take).pipe(
      catchError((error) => {

        this.messageService.showError(error.statusText, "Error during load of data");

        return of(new PagedResultDto<UserDto>())
      }))
      .subscribe(pagedResult => {

        super.onCompleteLoad(pagedResult);
      });
  }
}
