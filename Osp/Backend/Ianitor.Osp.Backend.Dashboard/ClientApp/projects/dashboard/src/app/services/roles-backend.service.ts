import {Injectable} from '@angular/core';
import {HttpClient, HttpParams} from '@angular/common/http';
import {Observable} from 'rxjs';
import {map} from 'rxjs/operators';
import {ConfigurationService} from "./configuration.service";
import {PagedResultDto} from "@ianitor/shared-services";
import {RoleDto} from "../models/roleDto";

@Injectable({
  providedIn: 'root'
})
export class RolesBackendService {


  constructor(private httpClient: HttpClient, private configurationService: ConfigurationService) {
  }


  getRoles(filter: string, skip: number, take: number): Observable<PagedResultDto<RoleDto>> {

    const params = new HttpParams()
      .set('skip', "" + skip)
      .set('take', "" + take);

    if (filter)
    {
      params.set('filter', filter);
    }

    return this.httpClient.get<PagedResultDto<RoleDto>>(this.configurationService.config.issuer + 'system/v1/roles', {
      params: params,
      observe: 'response'
    }).pipe(
      map(res => {
        return res.body;
      }),
    );
  }

  getRoleDetails(roleName: string): Observable<RoleDto> {

    return this.httpClient.get<RoleDto>(this.configurationService.config.issuer + `system/v1/roles/names/${roleName}`, {
      observe: 'response'
    }).pipe(
      map(res => {
        return res.body;
      }),
    );
  }

  createRole(roleDto: RoleDto): Observable<any> {

    return this.httpClient.post<any>(this.configurationService.config.issuer + 'system/v1/roles', roleDto, {
      observe: 'response'
    });
  }

  updateRole(roleName: string, roleDto: RoleDto): Observable<any> {

    return this.httpClient.put<any>(this.configurationService.config.issuer + `system/v1/roles/${roleName}`, roleDto, {
      observe: 'response'
    });
  }

  deleteRole(roleName: string): Observable<any> {
    return this.httpClient.delete<any>(this.configurationService.config.issuer + `system/v1/roles/${roleName}`, {
      observe: 'response'
    });
  }


}
