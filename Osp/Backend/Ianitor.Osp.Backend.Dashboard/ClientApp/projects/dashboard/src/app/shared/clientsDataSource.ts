import {of} from "rxjs";
import {IdentityBackendServices} from "../services/identity-backend.service";
import {catchError} from "rxjs/operators";
import {DataSourceBase, MessageService, PagedResultDto} from "@ianitor/shared-services";
import {ClientDto} from "../models/clientDto";
import {Injectable} from "@angular/core";

@Injectable({
  providedIn: 'root'
})
export class ClientsDataSource extends DataSourceBase<ClientDto> {

  constructor(private identityBackendServices: IdentityBackendServices, private messageService : MessageService) {
    super();
  }

  loadData(skip: number = 0, take: number = 10) {
    super.onBeginLoad();

    this.identityBackendServices.getClients(skip, take).pipe(
      catchError((error) => {

        this.messageService.showError(error.statusText, "Error during load of data");

        return of(new PagedResultDto<ClientDto>())
      }))
      .subscribe(pagedResult => {

        super.onCompleteLoad(pagedResult);
      });
  }
}
