import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Params, Router} from "@angular/router";
import {TenantBackendService} from "../../services/tenant-backend.service";
import {ConfigurationService} from "../../services/configuration.service";
import {MatDialog} from "@angular/material/dialog";
import {RtQueryBuilder} from "./rtQueryBuilder";
import {MessageService} from "@ianitor/shared-services";

@Component({
  selector: 'app-rt-query-builder',
  templateUrl: './rt-query-builder.component.html',
  styleUrls: ['./rt-query-builder.component.css']
})
export class RtQueryBuilderComponent implements OnInit {

  public readonly rtQueryBuilder: RtQueryBuilder;

  public readonly tenantId: string;

  constructor(private route: ActivatedRoute, private router: Router, private dataSourceBackendService: TenantBackendService, public dialog: MatDialog, public configurationService: ConfigurationService, private messageService: MessageService) {
    this.tenantId = route.snapshot.paramMap.get('id');
    this.rtQueryBuilder = new RtQueryBuilder(this.tenantId, dataSourceBackendService, messageService);
  }

  ngOnInit(): void {

    this.rtQueryBuilder.queryChange.subscribe(query=> {

      if (query) {
        this.navigateToUri(query.rtId)
      }
    });

    this.route.queryParams.subscribe(value => {
      if (value.queryId) {
        this.rtQueryBuilder.load(value.queryId);
      } else {
        this.rtQueryBuilder.new();
      }

    });
  }

  navigateToUri(queryRtId: string) {
    let queryParams = <Params>
      {queryId: queryRtId};

    const urlTree = this.router.createUrlTree([], {
      queryParams: queryParams,
      queryParamsHandling: "merge",
      preserveFragment: true
    });

    this.router.navigateByUrl(urlTree);
  }

  onNewQuery() {
    this.navigateToUri(null);
  }

  onApply() {

    this.rtQueryBuilder.apply();
  }

}
