import {AutoCompleteDataSource} from "@ianitor/shared-services";
import {BehaviorSubject, Observable} from "rxjs";
import {AutoCompleteResult} from "@ianitor/shared-services";

export class AutoCompleteEntitySelectDataSource implements AutoCompleteDataSource {

  private filterResultObservable = new BehaviorSubject<AutoCompleteResult>(null);
  private sourceList: string[] = [];


  constructor() {
  }

  public setSource(values: string[]) {
    if (values) {
      this.sourceList = values;
    } else {
      this.sourceList = new Array<string>();
    }
  }

  onFilter(filter: string): Observable<AutoCompleteResult> {

    const filterValue = filter.toLowerCase();
    const resultList = this.sourceList.filter(option => option.toLowerCase().includes(filterValue));

    this.filterResultObservable.next(<AutoCompleteResult>{
      searchTerm: filter,
      list: resultList
    });

    return this.filterResultObservable.asObservable();
  }

  onPreprocessSearchString(search: string): string {
    if (search.startsWith("%")) {
      return search.substr(8, 14);
    }
    return search;
  }
}
