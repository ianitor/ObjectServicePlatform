import {Component, ElementRef, Input, OnInit, ViewChild} from '@angular/core';
import {ConstructionKitTypeDataSource} from "../../../shared/constructionKitTypeDataSource";
import {TenantBackendService} from "../../../services/tenant-backend.service";
import {fromEvent, merge} from "rxjs";
import {debounceTime, distinctUntilChanged, tap} from "rxjs/operators";
import {MatPaginator} from "@angular/material/paginator";
import {MatSort} from "@angular/material/sort";
import {SearchFilter, SearchFilterTypes, Sort, SortOrders} from "../../../graphQL/globalTypes";
import {MessageService} from "@ianitor/shared-services";

@Component({
  selector: 'app-ck-entities',
  templateUrl: './ck-entities.component.html',
  styleUrls: ['./ck-entities.component.css']
})
export class CkEntitiesComponent implements OnInit {

  @Input() tenantId: string;
  dataSource: ConstructionKitTypeDataSource;
  isMobile: boolean;
  dataColumns: string[] = ['ckId', 'baseType', 'scopeId', 'isAbstract', 'isFinal'];
  displayedColumns: string[] = [...this.dataColumns, 'actions'];
  searchFilterColumns: string[] = ['ckId', 'scopeId', 'isAbstract', 'isFinal'];

  @ViewChild(MatPaginator, {static: false}) paginator: MatPaginator;
  @ViewChild(MatSort, {static: false}) sort: MatSort;
  @ViewChild('input', {static: false}) input: ElementRef;

  constructor(private dataSourceBackendService: TenantBackendService, private messageService : MessageService) {
  }

  ngOnInit() {

    this.dataSource = new ConstructionKitTypeDataSource(this.dataSourceBackendService, this.messageService);
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
