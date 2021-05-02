import {Component, Inject, OnInit} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialogRef} from "@angular/material/dialog";
import {CkEntityDetailDto} from "../../../../models/ckEntityDetailDto";
import {CkEntityAttributeDetailDto} from "../../../../models/ckEntityAttributeDetailDto";
import {FormControl, FormGroup} from "@angular/forms";
import {CkAttributeEntitySelectDataSource} from "../../../../shared/ckAttributeEntitySelectDataSource";
import {AbstractDetailsComponent} from "@ianitor/shared-ui";

export interface ColumnSelectorData {
  ckEntityDetail: CkEntityDetailDto;
  selectedAttributes: Array<CkEntityAttributeDetailDto>;
}

export interface ColumnSelectorResult {
  selectedAttributes: Array<CkEntityAttributeDetailDto>;
}

@Component({
  selector: 'app-column-selector',
  templateUrl: './column-selector.component.html',
  styleUrls: ['./column-selector.component.css']
})
export class ColumnSelectorComponent extends AbstractDetailsComponent<any> implements OnInit {

  public selectedAttributes = new Array<CkEntityAttributeDetailDto>();
  public readonly ckEntitySelectDataSource: CkAttributeEntitySelectDataSource;

  constructor(private dialogRef: MatDialogRef<ColumnSelectorComponent, ColumnSelectorResult>,
              @Inject(MAT_DIALOG_DATA) private data: ColumnSelectorData) {

    super();

    if (data.selectedAttributes) {
      this.selectedAttributes = data.selectedAttributes;
    }

    this.ckEntitySelectDataSource = new CkAttributeEntitySelectDataSource(this.data.ckEntityDetail);


    this._ownerForm = new FormGroup({
      'attributeSelect': new FormControl(null)
    });
    this.ownerForm.get('attributeSelect').valueChanges.subscribe(value => {
      if (value) {
        if (this.selectedAttributes.indexOf(value) == -1) {
          this.selectedAttributes.push(value);
        }
        this.ownerForm.get("attributeSelect").setValue(null);

      }
    });
  }

  ngOnInit(): void {


  }

  remove(ckEntityAttributeDetailDto: CkEntityAttributeDetailDto) {
    this.selectedAttributes = this.selectedAttributes.filter(value => value.attributeId != ckEntityAttributeDetailDto.attributeId);
  }


  up(ckEntityAttributeDetailDto: CkEntityAttributeDetailDto) {
    const from = this.selectedAttributes.findIndex(value => value.attributeId == ckEntityAttributeDetailDto.attributeId);
    if (from >= 0)
    {
      const to = from - 1;
      if (to >= 0) {
        const removedElement = this.selectedAttributes.splice(from, 1)[0];
        this.selectedAttributes.splice(to, 0, removedElement);
      }
    }
  }

  down(ckEntityAttributeDetailDto: CkEntityAttributeDetailDto) {

    const from = this.selectedAttributes.findIndex(value => value.attributeId == ckEntityAttributeDetailDto.attributeId);
    if (from >= 0)
    {
      const to = from + 1;
      if (to < this.selectedAttributes.length) {
        this.selectedAttributes.splice(to, 0, this.selectedAttributes.splice(from, 1)[0]);
      }
    }
  }

  onOkClick(): void{

    const result = <ColumnSelectorResult>{
      selectedAttributes: this.selectedAttributes
    };

    this.dialogRef.close(result);
  }

  onCancelClick(): void {
    this.dialogRef.close();
  }
}
