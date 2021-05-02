import {Injectable} from "@angular/core";
import {HttpClient, HttpParams} from "@angular/common/http";
import {ConfigurationService} from "./configuration.service";
import {map} from "rxjs/operators";
import {JobDto} from "../models/jobDto";
import {Observable} from "rxjs";


@Injectable({
  providedIn: 'root'
})
export class JobsBackendService {

  constructor(private httpClient: HttpClient, private configurationService: ConfigurationService) {
  }

  downloadJobResultBinary(jobId: string) : Observable<Blob> {
    const params = new HttpParams()
      .set('id', jobId);

    return this.httpClient.get(this.configurationService.config.jobServices + 'system/v1/jobs/download', {
      params: params,
      responseType: 'blob'
    })
  }

  getJobStatus(jobId: string) : Observable<JobDto> {

    const params = new HttpParams()
      .set('id', jobId);

    return this.httpClient.get<JobDto>(this.configurationService.config.jobServices + 'system/v1/jobs', {
      params: params,
      observe: 'response'
    }).pipe(
      map(res => {
        return res.body;
      }),
    );
  }

}
