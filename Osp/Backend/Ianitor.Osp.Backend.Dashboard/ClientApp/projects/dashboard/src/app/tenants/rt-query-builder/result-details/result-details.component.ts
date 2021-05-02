import {Component, Input, OnInit, ViewChild} from '@angular/core';
import {RtQueryBuilder} from "../rtQueryBuilder";
import {merge, timer} from "rxjs";
import {switchMap, takeWhile, tap} from "rxjs/operators";
import {MatPaginator} from "@angular/material/paginator";
import {MatSort} from "@angular/material/sort";
import {CoreBackendServices} from "../../../services/core-backend.service";
import {JobsBackendService} from "../../../services/jobs-backend.service";
import {ProgressNotifierService} from "@ianitor/shared-ui";
import {MessageService} from "@ianitor/shared-services";

@Component({
  selector: 'app-rtQueryBuilder-result-details',
  templateUrl: './result-details.component.html',
  styleUrls: ['./result-details.component.css']
})
export class ResultDetailsComponent implements OnInit {


  @ViewChild(MatPaginator, {static: false}) paginator: MatPaginator;
  @ViewChild(MatSort, {static: false}) sort: MatSort;

  @Input() public tenantId: string;

  private _rtQueryBuilder: RtQueryBuilder;
  @Input() get rtQueryBuilder() {
    return this._rtQueryBuilder;
  }

  set rtQueryBuilder(value: RtQueryBuilder) {
    this._rtQueryBuilder = value;
  }

  constructor(private coreBackendServices: CoreBackendServices, private jobsBackendService: JobsBackendService, private messageService: MessageService, private progressNotifierService: ProgressNotifierService) {
  }

  ngOnInit(): void {
  }

  // noinspection JSUnusedGlobalSymbols
  ngAfterViewInit() {

    // reset the paginator after sorting
    this.sort.sortChange.subscribe(() => this.paginator.pageIndex = 0);

    merge(this.sort.sortChange, this.paginator.page)
      .pipe(
        tap(() => this.onApply())
      )
      .subscribe();
  }

  onApply() {

    // TODO: Temporary sort is not implemented
    this.rtQueryBuilder.skip = this.paginator.pageIndex * this.paginator.pageSize;
    this.rtQueryBuilder.take = this.paginator.pageSize;

    this.rtQueryBuilder.apply();
  }

  onExport() {

    this.progressNotifierService.start("Exporting runtime entities", false, true);

    this.coreBackendServices.exportRtModel(this.tenantId, this.rtQueryBuilder.query.rtId).subscribe(jobId => {

      timer(0, 2000).pipe(
        switchMap(_=> this.jobsBackendService.getJobStatus(jobId)),
        takeWhile(job => job.status != "Succeeded" && job.status != "Failed" && job.status != "Deleted" && !this.progressNotifierService.isCanceled)
      ).subscribe(jobDto => {

        this.progressNotifierService.reportProgressIndeterminate(`Operation '${jobDto.status}'. Please wait...`)

      }, error => {

        this.progressNotifierService.complete();
        this.messageService.showError(error, "Error during watching background task for exporting");

      }, () => {

        this.progressNotifierService.complete();
        this.messageService.showInformation("Operation completed. Download has been initialized.");

        this.jobsBackendService.downloadJobResultBinary(jobId).subscribe(b =>{
          const downloadURL = window.URL.createObjectURL(b);
          const link = document.createElement('a');
          link.href = downloadURL;
          link.download = `${this.rtQueryBuilder.query.name}.zip`;
          link.click();
        });
      } )

    }, error => {
      this.progressNotifierService.complete();
      this.messageService.showError(error, "Error during requesting export");
    }, () => {
      this.progressNotifierService.reportProgressIndeterminate("Operation requested. Please wait...")
    })
  }
}
