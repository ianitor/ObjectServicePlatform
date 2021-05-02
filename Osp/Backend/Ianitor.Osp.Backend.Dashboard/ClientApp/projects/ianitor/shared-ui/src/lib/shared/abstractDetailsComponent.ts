import {FormGroup} from "@angular/forms";
import {IsoDateTime} from "@ianitor/shared-services";

export abstract class AbstractDetailsComponent<TEntity>  {

  protected entity: TEntity;
  public loading: boolean;

  protected constructor() {
    this.loading = true;
    this.entity = null;
  }

  protected _ownerForm: FormGroup;

  public get isLoaded() : boolean{
    return this.entity !== null;
  }

  public get ownerForm() : FormGroup{
    return this._ownerForm;
  }

  public hasError = (controlName: string, errorName: string) =>{
    return this.ownerForm.controls[controlName].hasError(errorName);
  };

  public hasFormError = (errorName: string) =>{
    return this.ownerForm.hasError(errorName);
  };

  public updateDateTime(controlName: string){
    this.ownerForm.get(controlName).setValue(IsoDateTime.utcToLocalDateTimeIso(IsoDateTime.currentUtcDateTimeIso()));
  }

  public copyInputMessage(inputElement){
    inputElement.select();
    document.execCommand('copy');
    inputElement.setSelectionRange(0, 0);
  }

  protected onProgressStarting(){
    this.loading = true;
    this.ownerForm.disable();
    this.ownerForm.updateValueAndValidity();
  }

  protected onProgressCompleted(){
    this.ownerForm.enable();
    this.loading = false;
  }
}
