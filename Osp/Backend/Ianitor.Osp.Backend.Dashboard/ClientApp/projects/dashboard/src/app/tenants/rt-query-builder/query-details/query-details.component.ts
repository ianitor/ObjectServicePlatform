import {Component, Input, OnInit} from '@angular/core';
import {TenantBackendService} from "../../../services/tenant-backend.service";
import {CkEntityAttributeDetailDto} from "../../../models/ckEntityAttributeDetailDto";
import {
  ColumnSelectorComponent,
  ColumnSelectorData,
  ColumnSelectorResult
} from "./column-selector/column-selector.component";
import {MatDialog} from "@angular/material/dialog";
import {QueryNameComponent, QueryNameData, QueryNameResult} from "./query-name/query-name.component";
import {map, mergeMap} from "rxjs/operators";
import {RtQueryBuilder} from "../rtQueryBuilder";
import {SortSelectorComponent, SortSelectorData, SortSelectorResult} from "./sort-selector/sort-selector.component";
import {MessageService} from "@ianitor/shared-services";

@Component({
  selector: 'app-rtQueryBuilder-query-details',
  templateUrl: './query-details.component.html',
  styleUrls: ['./query-details.component.css']
})
export class QueryDetailsComponent implements OnInit {

  @Input() public tenantId: string;

  private _rtQueryBuilder: RtQueryBuilder;
  @Input() get rtQueryBuilder() {
    return this._rtQueryBuilder;
  }

  set rtQueryBuilder(value: RtQueryBuilder) {
    this._rtQueryBuilder = value;
  }

  constructor(private dataSourceBackendService: TenantBackendService, private dialog: MatDialog, private messageService: MessageService) {

  }

  ngOnInit(): void {


  }



  onSelectSorting() {

    let dialogRef = this.dialog.open<SortSelectorComponent, SortSelectorData, SortSelectorResult>(SortSelectorComponent, {
      width: '50vw',
      maxWidth: '50vw',
      data: <SortSelectorData>{
        sortedAttributes: this.rtQueryBuilder.sortColumns,
        ckEntityDetail: this.rtQueryBuilder.ckEntityDetails
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.rtQueryBuilder.sortColumns = result.sortedAttributes;
      }
    });
  }

  onSelectColumns() {

    let selectedAttributes: CkEntityAttributeDetailDto[] = null;
    if (this.rtQueryBuilder.selectedDataColumns) {
      selectedAttributes = this.rtQueryBuilder.ckEntityDetails.attributes.filter(a => this.rtQueryBuilder.selectedDataColumns.find(s => s == a.attributeName));
    }

    let dialogRef = this.dialog.open<ColumnSelectorComponent, ColumnSelectorData, ColumnSelectorResult>(ColumnSelectorComponent, {
      width: '50vw',
      maxWidth: '50vw',
      data: <ColumnSelectorData>{
        selectedAttributes: selectedAttributes,
        ckEntityDetail: this.rtQueryBuilder.ckEntityDetails
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.rtQueryBuilder.selectedDataColumns = result.selectedAttributes.map(value => value.attributeName);
      }
    });
  }

  onSave() {

    if (!this.rtQueryBuilder.queryName) {
      this.onSaveAs();
    } else {
      this.rtQueryBuilder.save().subscribe(_ => {
        this.messageService.showInformation("Query has been saved.");
      }, error => {
        this.messageService.showError(error, "Error during saving of query.");
      })
    }
  }

  onSaveAs() {

    this.askQueryName().pipe(mergeMap(_ => this.rtQueryBuilder.save())).subscribe(_ => {
      this.messageService.showInformation("Query has been saved.");
    }, error => {
      this.messageService.showError(error, "Error during saving of query.");
    })
  }

  askQueryName() {
    let dialogRef = this.dialog.open<QueryNameComponent, QueryNameData, QueryNameResult>(QueryNameComponent, {
      data: <QueryNameData>{
        name: this.rtQueryBuilder.queryName
      }
    });

    return dialogRef.afterClosed().pipe(map((value) => {
      this.rtQueryBuilder.queryName = value.name;
      return value.name;
    }));
  }


}
