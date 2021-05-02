import {Component, Inject, OnInit} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialogRef} from "@angular/material/dialog";
import {FormControl, FormGroup, Validators} from "@angular/forms";

export interface QueryNameData {
  name: string;
}

export interface QueryNameResult {
  name: string;
}

@Component({
  selector: 'app-query-name',
  templateUrl: './query-name.component.html',
  styleUrls: ['./query-name.component.css']
})
export class QueryNameComponent implements OnInit {

  public readonly formGroup: FormGroup;


  constructor(private dialogRef: MatDialogRef<QueryNameComponent, QueryNameResult>,
              @Inject(MAT_DIALOG_DATA) private data: QueryNameData) {

    this.formGroup = new FormGroup({
      'name': new FormControl('', Validators.required)
    });
  }

  ngOnInit(): void {

    if (this.data.name) {
      this.formGroup.setValue(this.data);
    }
  }

  public hasError = (controlName: string, errorName: string) =>{
    return this.formGroup.controls[controlName].hasError(errorName);
  };

  onOkClick(): void{

    this.formGroup.disable();
    this.formGroup.updateValueAndValidity();

    const result = <QueryNameResult>{
      name: this.formGroup.get('name').value
    };

    this.dialogRef.close(result);
  }

  onCancelClick(): void {
    this.dialogRef.close();
  }
}
