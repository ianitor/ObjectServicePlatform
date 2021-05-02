import {Component, Input, OnChanges, OnInit, SimpleChanges} from '@angular/core';
import {SelectionModel} from "@angular/cdk/collections";
import {FieldFilter, FieldFilterOperators} from "../../../../graphQL/globalTypes";
import {FieldFilterBuilder} from "../../fieldFilterBuilder";
import {GenericDataSource} from "@ianitor/shared-services";

@Component({
  selector: 'app-rtQueryBuilder-field-filter-editor',
  templateUrl: './field-filter-editor.component.html',
  styleUrls: ['./field-filter-editor.component.scss']
})
export class FieldFilterEditorComponent implements OnChanges {

  public readonly fieldFilterOperator = Object.values(FieldFilterOperators);


  public selection: SelectionModel<FieldFilter>;
  public fieldFilterDataSource: GenericDataSource<FieldFilter>;

  public readonly fieldFilterColumns = ["attributeName", "operator", "comparisonValue"];
  public readonly fieldFilterDisplayColumns = ["select", ...this.fieldFilterColumns];

  private _fieldFilterBuilder: FieldFilterBuilder;

  @Input()
  public get fieldFilterBuilder(){
    return this._fieldFilterBuilder;
  }
  public set fieldFilterBuilder(value){
    if (this._fieldFilterBuilder != value) {
      this._fieldFilterBuilder = value;
    }
  }

  constructor() {
    const initialSelection = [];
    const allowMultiSelect = true;
    this.selection = new SelectionModel<FieldFilter>(allowMultiSelect, initialSelection);
  }

  ngOnInit(): void {


  }

  ngOnChanges(changes: SimpleChanges): void {

    this.fieldFilterBuilder?.queryCkIdChange.subscribe(_ => {
      this.fieldFilterDataSource = new GenericDataSource<FieldFilter>(this.fieldFilterBuilder.fieldFilters);
    });
  }

  /** Whether the number of selected elements matches the total number of rows. */
  isAllSelected() {
    const numSelected = this.selection.selected.length;
    const numRows = this.fieldFilterDataSource.data.length;
    return numSelected == numRows;
  }

  /** Selects all rows if they are not all selected; otherwise clear selection. */
  masterToggle() {
    this.isAllSelected() ?
      this.selection.clear() :
      this.fieldFilterDataSource.data.forEach(row => this.selection.select(row));
  }


  addNewRow() {

    this.fieldFilterBuilder.fieldFilters.push(<FieldFilter>{operator: FieldFilterOperators.EQUALS});
    this.fieldFilterDataSource.emitChanged();
  }

  removeRow(){

    this.selection.selected.forEach(value => {

      const index =  this.fieldFilterBuilder.fieldFilters.indexOf(value, 0);
      if (index > -1) {
        this.fieldFilterBuilder.fieldFilters.splice(index, 1);
      }
    });

    this.fieldFilterDataSource.emitChanged();
  }
}
