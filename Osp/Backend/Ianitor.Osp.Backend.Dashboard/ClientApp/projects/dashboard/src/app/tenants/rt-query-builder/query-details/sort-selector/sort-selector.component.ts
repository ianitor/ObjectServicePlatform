import {Component, Inject, OnInit} from '@angular/core';
import {CkEntityDetailDto} from "../../../../models/ckEntityDetailDto";
import {FormControl, FormGroup} from "@angular/forms";
import {MAT_DIALOG_DATA, MatDialogRef} from "@angular/material/dialog";
import {Sort, SortOrders} from "../../../../graphQL/globalTypes";
import {AbstractDetailsComponent} from "@ianitor/shared-ui";
import {CkAttributeEntitySelectDataSource} from "../../../../shared/ckAttributeEntitySelectDataSource";

export interface SortSelectorData {
  ckEntityDetail: CkEntityDetailDto;
  sortedAttributes: Array<Sort>;
}

export interface SortSelectorResult {
  sortedAttributes: Array<Sort>;
}

@Component({
  selector: 'app-sort-selector',
  templateUrl: './sort-selector.component.html',
  styleUrls: ['./sort-selector.component.css']
})
export class SortSelectorComponent extends AbstractDetailsComponent<any> implements OnInit {

  public sortedAttributes = new Array<Sort>();
  public readonly ckAttributeEntitySelectDataSource: CkAttributeEntitySelectDataSource;

  constructor(private dialogRef: MatDialogRef<SortSelectorComponent, SortSelectorResult>,
              @Inject(MAT_DIALOG_DATA) private data: SortSelectorData) {

    super();

    if (data.sortedAttributes) {
      this.sortedAttributes = data.sortedAttributes;
    }

    this.ckAttributeEntitySelectDataSource = new CkAttributeEntitySelectDataSource(this.data.ckEntityDetail);


    this._ownerForm = new FormGroup({
      'attributeSelect': new FormControl(null)
    });

    this.ownerForm.get('attributeSelect').valueChanges.subscribe(attribute => {
      if (attribute) {
        if (!this.sortedAttributes.find(v => v.attributeName == attribute.name)) {
          this.sortedAttributes.push(<Sort>{
            attributeName: attribute.attributeName,
            sortOrder: SortOrders.ASCENDING
          });
        }
        this._ownerForm.get("attributeSelect").setValue(null);
      }
    });
  }

  ngOnInit(): void {


  }


  isAscending(sort: Sort){
    return sort.sortOrder == SortOrders.ASCENDING;
  }

  remove(sort: Sort) {
    this.sortedAttributes = this.sortedAttributes.filter(value => value.attributeName != sort.attributeName);
  }


  up(sort: Sort) {
    const from = this.sortedAttributes.findIndex(value => value.attributeName == sort.attributeName);
    if (from >= 0)
    {
      const to = from - 1;
      if (to >= 0) {
        const removedElement = this.sortedAttributes.splice(from, 1)[0];
        this.sortedAttributes.splice(to, 0, removedElement);
      }
    }
  }

  down(sort: Sort) {

    const from = this.sortedAttributes.findIndex(value => value.attributeName == sort.attributeName);
    if (from >= 0)
    {
      const to = from + 1;
      if (to < this.sortedAttributes.length) {
        this.sortedAttributes.splice(to, 0, this.sortedAttributes.splice(from, 1)[0]);
      }
    }
  }

  sortAscending(sort: Sort){

    sort.sortOrder = SortOrders.ASCENDING;
  }

  sortDescending(sort: Sort){

    sort.sortOrder = SortOrders.DESCENDING;
  }

  onOkClick(): void{

    const result = <SortSelectorResult>{
      sortedAttributes: this.sortedAttributes
    };

    this.dialogRef.close(result);
  }

  onCancelClick(): void {
    this.dialogRef.close();
  }

}
