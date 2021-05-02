import {Injectable} from '@angular/core';
import {HttpClient, HttpParams} from '@angular/common/http';
import {UserDto} from '../models/userDto';
import {DiagnosticsModel} from '../models/diagnosticsModel';
import {Observable} from 'rxjs';
import {map} from 'rxjs/operators';
import {ConfigurationService} from "./configuration.service";
import {PagedResultDto} from "@ianitor/shared-services";
import {ClientDto} from "../models/clientDto";


@Injectable({
  providedIn: 'root'
})
export class IdentityBackendServices {


  constructor(private httpClient: HttpClient, private configurationService: ConfigurationService) {
  }


  userDiagnostics(): Observable<DiagnosticsModel> {

    const result: Observable<DiagnosticsModel> = this.httpClient.get<DiagnosticsModel>(this.configurationService.config.issuer + 'system/v1/Diagnostics');
    result.subscribe(res => console.log('HTTP response', res), err => console.log("HTTP error", err), () => console.log("HTTP request completed"));

    return result;
  }

  getUsers(skip: number, take: number): Observable<PagedResultDto<UserDto>> {

    const params = new HttpParams()
      .set('skip', "" + skip)
      .set('take', "" + take);

    return this.httpClient.get<PagedResultDto<UserDto>>(this.configurationService.config.issuer + 'system/v1/Identities/getPaged', {
      params: params,
      observe: 'response'
    }).pipe(
      map(res => {
        return res.body;
      }),
    );
  }

  getUserDetails(userName: string): Observable<UserDto> {

    return this.httpClient.get<UserDto>(this.configurationService.config.issuer + `system/v1/Identities/${userName}`, {
      observe: 'response'
    }).pipe(
      map(res => {
        return res.body;
      }),
    );
  }

  createUser(userDto: UserDto): Observable<any> {

    return this.httpClient.post<any>(this.configurationService.config.issuer + 'system/v1/Identities', userDto, {
      observe: 'response'
    });
  }

  updateUser(userName: string, userDto: UserDto): Observable<any> {

    return this.httpClient.put<any>(this.configurationService.config.issuer + `system/v1/Identities/${userName}`, userDto, {
      observe: 'response'
    });
  }

  deleteUser(userName: string): Observable<any> {
    return this.httpClient.delete<any>(this.configurationService.config.issuer + `system/v1/Identities/${userName}`, {
      observe: 'response'
    });
  }

  resetPassword(userName: string, password: string): Observable<any> {

    const params = new HttpParams()
      .set('userName', userName)
      .set('password', password);

    return this.httpClient.post<any>(this.configurationService.config.issuer + 'system/v1/Identities/ResetPassword', null, {
      params: params,
      observe: 'response'
    }).pipe(
      map(res => {
        return res.body;
      }),
    );
  }


  getClients(skip: number, take: number): Observable<PagedResultDto<ClientDto>> {

    const params = new HttpParams()
      .set('skip', "" + skip)
      .set('take', "" + take);

    return this.httpClient.get<PagedResultDto<ClientDto>>(this.configurationService.config.issuer + 'system/v1/clients/getPaged', {
      params: params,
      observe: 'response'
    }).pipe(
      map(res => {
        return res.body;
      }),
    );
  }

  getClientDetails(clientId: string): Observable<ClientDto> {

    return this.httpClient.get<ClientDto>(this.configurationService.config.issuer + `system/v1/clients/${clientId}`, {
      observe: 'response'
    }).pipe(
      map(res => {
        return res.body;
      }),
    );
  }

  createClient(clientDto: ClientDto): Observable<any> {

    return this.httpClient.post<any>(this.configurationService.config.issuer + 'system/v1/clients', clientDto, {
      observe: 'response'
    });
  }

  updateClient(clientId: string, clientDto: ClientDto): Observable<any> {

    return this.httpClient.put<any>(this.configurationService.config.issuer + `system/v1/clients/${clientId}`, clientDto, {
      observe: 'response'
    });
  }

  deleteClient(clientId: string): Observable<any> {
    return this.httpClient.delete<any>(this.configurationService.config.issuer + `system/v1/clients/${clientId}`, {
      observe: 'response'
    });
  }
}
