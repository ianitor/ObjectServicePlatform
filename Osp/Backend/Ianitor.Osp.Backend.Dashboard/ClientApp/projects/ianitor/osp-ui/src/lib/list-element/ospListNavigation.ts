import {MatPaginator} from "@angular/material/paginator";
import {MatSort, SortDirection} from "@angular/material/sort";
import {ElementRef, EventEmitter} from "@angular/core";
import {fromEvent, merge} from "rxjs";
import {debounceTime, distinctUntilChanged, tap} from "rxjs/operators";
import {SearchFilter, SearchFilterTypes, Sort, SortOrders} from "./globalTypes";


export class OspListNavigationDataInfo {
  skip: number;
  take: number;
  searchFilter?: SearchFilter;
  sort?: Sort[];
}

export class OspListNavigationOptions{
  language: string;
  searchFilterType?: SearchFilterTypes;
  searchFilterAttributeNames?: string[];
}

export class OspListNavigation {

  public loadDataRequest = new EventEmitter<OspListNavigationDataInfo>();

  lastSortDirection: SortDirection;
  lastSortField: string;
  lastSearchText: string;

  constructor(private paginator: MatPaginator, private sort: MatSort, private searchBox: ElementRef, private ospOptions: OspListNavigationOptions) {

  }

  init() {
    // server-side search
    fromEvent(this.searchBox.nativeElement, 'keyup')
      .pipe(
        debounceTime(500),
        distinctUntilChanged(),
        tap(() => {

          this.paginator.pageIndex = 0;

          const searchText = this.searchBox.nativeElement.value;

          if (!this.lastSearchText && searchText) {
            this.lastSortDirection = this.sort.direction;
            this.lastSortField = this.sort.active;

            // Reset sorting to see the score rating (default sorting returned from server)
            this.sort.sort({id: '', start: 'asc', disableClear: false});
          }

          this.lastSearchText = searchText;

          if (!searchText && this.lastSortField) {
            if (this.lastSortDirection == "asc") {
              this.sort.sort({id: this.lastSortField, start: 'asc', disableClear: true});
            } else if (this.lastSortDirection == "desc") {
              this.sort.sort({id: this.lastSortField, start: 'desc', disableClear: true});
            }
          }


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
      .subscribe()
  }

  public get loadDataInfo() {
    const filterString = this.searchBox.nativeElement.value;
    const sortField = this.sort.active;
    const sortDirection = this.sort.direction;

    let filter = null;
    if (filterString) {
      filter = <SearchFilter>{
        language: this.ospOptions.language,
        searchTerm: filterString,
        type: this.ospOptions.searchFilterType,
        attributeNames: this.ospOptions.searchFilterAttributeNames
      };
    }

    let sort = [];
    if (sortField && sortDirection) {
      sort.push(<Sort>{
        attributeName: sortField,
        sortOrder: sortDirection === "asc" ? SortOrders.ASCENDING : SortOrders.DESCENDING
      });
    }

    return <OspListNavigationDataInfo>{
      skip: this.paginator.pageIndex * this.paginator.pageSize,
      take: this.paginator.pageSize,
      searchFilter: filter,
      sort: sort
    };
  }

  private loadData() {

    this.loadDataRequest.emit(this.loadDataInfo);
  }
}
