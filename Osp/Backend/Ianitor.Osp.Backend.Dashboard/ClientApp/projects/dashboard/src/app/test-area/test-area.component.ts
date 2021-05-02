import {Component, OnInit} from '@angular/core';
import {AutoCompleteEntitySelectDataSource} from "./autoCompleteEntitySelectDataSource";
import {TestDto} from "./TestDto";
import {AbstractDetailsComponent} from "@ianitor/shared-ui";
import {FormControl, FormGroup, Validators} from "@angular/forms";
import {EntitySelectDataSource, PagedResultDto} from "@ianitor/shared-services";
import {BehaviorSubject, Observable} from "rxjs";

@Component({
  selector: 'app-test-area',
  templateUrl: './test-area.component.html',
  styleUrls: ['./test-area.component.scss']
})
export class TestAreaComponent extends AbstractDetailsComponent<TestDto> implements OnInit {

  public readonly senderDataSource: AutoCompleteEntitySelectDataSource;
  public readonly entityDataSource: EntitySelectDataSource<TestDto>;


  constructor() {
    super();

    this.senderDataSource = new AutoCompleteEntitySelectDataSource();
    this.entityDataSource = new class implements EntitySelectDataSource<TestDto> {
      private filterResultObservable = new BehaviorSubject<PagedResultDto<TestDto>>(null);

      private sourceList: TestDto[] = [
        {name: "Gerald"},
        {name: "Marlene"},
        {name: "Harald"},
        {name: "Marco"},
        {name: "Sandra"},
        {name: "Sebastian"},
        {name: "Heike"},
        {name: "Günther"},
        {name: "Markus"},
        {name: "Matthias"},
        {name: "Ernst"},
        {name: "Josef"},
        {name: "Waltraud"},
        {name: "Peter"},
        {name: "Daniela"},
        {name: "Tobias"},
        {name: "Bernadette"},
      ];

      onDisplayEntity(entity: TestDto): string {
        if (!entity){
          return null;
        }
        return entity.name;
      }

      onFilter(filter: string): Observable<PagedResultDto<TestDto>> {

        const filterValue = filter.toLowerCase();
        const resultList = this.sourceList.filter(option => option.name.toLowerCase().includes(filterValue));

        this.filterResultObservable.next(<PagedResultDto<TestDto>>{
          list: resultList,
          totalCount: resultList.length
        });

        return this.filterResultObservable.asObservable();
      }
    }

    this._ownerForm = new FormGroup({
      'sender': new FormControl(null, Validators.required),
      'firstname': new FormControl(null, Validators.required)
    });

  }

  ngOnInit(): void {

    this.senderDataSource.setSource(["The sky", "above", "the port", "was", "the color of television", "tuned", "to", "a dead channel", ".", "All", "this happened", "more or less", ".", "I", "had", "the story", "bit by bit", "from various people", "and", "as generally", "happens", "in such cases", "each time", "it", "was", "a different story", ".", "It", "was", "a pleasure", "to", "burn"]);

  }

  test() {
    alert(this.ownerForm.get("firstname").value);
  }

  reset(){
    this.ownerForm.reset();
  }

}
