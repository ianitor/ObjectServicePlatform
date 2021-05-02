import {Injectable} from '@angular/core';
import {HttpClient, HttpParams} from '@angular/common/http';

import {TenantDto} from '../models/tenantDto';

import {Observable} from 'rxjs';
import {ConfigurationService} from "./configuration.service";
import {map} from "rxjs/operators";
import {PagedResultDto} from "@ianitor/shared-services";
import {ExportModelResponseDto} from "../models/exportModelResponseDto";

@Injectable({
  providedIn: 'root'
})
export class CoreBackendServices {

  constructor(private httpClient: HttpClient, private configurationService: ConfigurationService) {
  }

  getTenants(skip: number, take: number): Observable<PagedResultDto<TenantDto>> {

    const params = new HttpParams()
      .set('skip', "" + skip)
      .set('take', "" + take);

    return this.httpClient.get<PagedResultDto<TenantDto>>(this.configurationService.config.coreServices + 'system/v1/tenants', {
      params: params,
      observe: 'response'
    }).pipe(
      map(res => {
        return res.body;
      }),
    );
  }

  getTenantDetails(tenantId: string): Observable<TenantDto> {

    return this.httpClient.get<TenantDto>(this.configurationService.config.coreServices + `system/v1/tenant/${tenantId}`, {
      observe: 'response'
    }).pipe(
      map(res => {
        return res.body;
      }),
    );
  }

  createTenant(tenantDto: TenantDto) : Observable<any>{

    const params = new HttpParams()
      .set('tenantId', tenantDto.tenantId)
      .set('databaseName', tenantDto.database);

    return this.httpClient.post<any>(this.configurationService.config.coreServices + 'system/v1/tenants', null, {
      params: params,
      observe: 'response'
    });
  }

  attachTenant(dataSourceDto: TenantDto) : Observable<any>{

    const params = new HttpParams()
      .set('tenantId', dataSourceDto.tenantId)
      .set('databaseName', dataSourceDto.database);

    return this.httpClient.post<any>(this.configurationService.config.coreServices + 'system/v1/tenants/attach', null, {
      params: params,
      observe: 'response'
    });
  }

  detachTenant(tenantId: string) : Observable<any>{

    const params = new HttpParams()
      .set('tenantId', tenantId);

    return this.httpClient.post<any>(this.configurationService.config.coreServices + 'system/v1/tenants/detach', null, {
      params: params,
      observe: 'response'
    });
  }

  deleteTenant(tenantId: string) : Observable<any>{

    const params = new HttpParams()
      .set('tenantId', tenantId);

    return this.httpClient.delete<any>(this.configurationService.config.coreServices + 'system/v1/tenants', {
      params: params,
      observe: 'response'
    });
  }

  exportRtModel(tenantId: string, queryId: string) : Observable<string>{

    const params = new HttpParams()
      .set('tenantId', tenantId);

    return this.httpClient.post<ExportModelResponseDto>(this.configurationService.config.coreServices + 'system/v1/Models/ExportRt', {queryId: queryId}, {
      params: params,
      observe: 'response'
    }).pipe(
      map(res => {
        return res.body.jobId;
      }),
    );
  }
}
