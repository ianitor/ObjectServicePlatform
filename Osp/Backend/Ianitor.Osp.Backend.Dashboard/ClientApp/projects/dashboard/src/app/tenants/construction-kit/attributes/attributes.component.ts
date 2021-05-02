import {Component, ElementRef, Input, OnInit, ViewChild} from '@angular/core';
import {TenantBackendService} from "../../../services/tenant-backend.service";
import {CkAttributesDataSource} from "../../../shared/ckAttributesDataSource";
import {fromEvent, merge} from "rxjs";
import {debounceTime, distinctUntilChanged, tap} from "rxjs/operators";
import {MatSort} from "@angular/material/sort";
import {MatPaginator} from "@angular/material/paginator";
import {SearchFilter, SearchFilterTypes, Sort, SortOrders} from "../../../graphQL/globalTypes";
import {MessageService} from "@ianitor/shared-services";

@Component({
  selector: 'app-ck-attributes',
  templateUrl: './attributes.component.html',
  styleUrls: ['./attributes.component.css']
})
export class AttributesComponent implements OnInit {

  @Input() tenantId: string;
  dataSource: CkAttributesDataSource;
  isMobile: boolean;
  dataColumns: string[] = ['attributeId', 'attributeValueType', 'scopeId'];
  displayedColumns: string[] = [...this.dataColumns, 'actions'];
  searchFilterColumns: string[] = this.dataColumns;

  @ViewChild(MatPaginator, {static: false}) paginator: MatPaginator;
  @ViewChild(MatSort, {static: false}) sort: MatSort;
  @ViewChild('input', {static: false}) input: ElementRef;

  constructor(private dataSourceBackendService: TenantBackendService, private messageService: MessageService) {
  }

  ngOnInit() {

    this.dataSource = new CkAttributesDataSource(this.dataSourceBackendService, this.messageService);
    this.dataSource.loadData(this.tenantId);
  }

  // noinspection JSUnusedGlobalSymbols
  ngAfterViewInit() {

    // server-side search
    fromEvent(this.input.nativeElement, 'keyup')
      .pipe(
        debounceTime(500),
        distinctUntilChanged(),
        tap(() => {
          this.paginator.pageIndex = 0;
          this.loadData();
        })
      )
      .subscribe();

    // reset the paginator after sorting
    this.sort.sortChange.subscribe(() => this.paginator.pageIndex = 0);

    merge(this.sort.sortChange, this.paginator.page)
      .pipe(
        tap(() => this.loadData())
      )
      .subscribe();
  }

  loadData() {

    const filterString = this.input.nativeElement.value;
    const field = this.sort.active;
    const direction = this.sort.direction;

    let filter = null;
    if (filterString) {
      filter = <SearchFilter>{
        type: SearchFilterTypes.ATTRIBUTE_FILTER,
        attributeNames: this.searchFilterColumns,
        searchTerm: filterString
      };
    }

    let sort = [];
    if (field) {
      sort.push(<Sort>{
        attributeName: field,
        sortOrder: direction === "asc" ? SortOrders.ASCENDING : SortOrders.DESCENDING
      });
    }

    this.dataSource.loadData(this.tenantId,this.paginator.pageIndex * this.paginator.pageSize,
      this.paginator.pageSize, filter, sort);
  }

}
