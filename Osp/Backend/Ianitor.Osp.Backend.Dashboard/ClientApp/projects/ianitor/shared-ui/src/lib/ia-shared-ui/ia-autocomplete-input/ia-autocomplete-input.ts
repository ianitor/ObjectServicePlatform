import {
  Component,
  ElementRef,
  EventEmitter, forwardRef,
  HostBinding, Injector,
  Input,
  ViewChild
} from '@angular/core';
import {DoCheck} from '@angular/core';
import {OnInit} from '@angular/core';
import {OnDestroy} from '@angular/core';
import {MatFormFieldControl} from "@angular/material/form-field";
import {
  AbstractControl,
  ControlValueAccessor,
  FormControl, NG_VALIDATORS,
  NG_VALUE_ACCESSOR,
  NgControl,
  ValidationErrors, Validator
} from "@angular/forms";
import {coerceBooleanProperty} from "@angular/cdk/coercion";
import {debounceTime, filter, map, switchMap, tap} from "rxjs/operators";
import {MatAutocompleteActivatedEvent, MatAutocompleteSelectedEvent} from "@angular/material/autocomplete";
import {MatInput} from "@angular/material/input";
import {Subject} from "rxjs";
import {FocusMonitor} from "@angular/cdk/a11y";
import {AutoCompleteDataSource} from "@ianitor/shared-services";

@Component({
  selector: 'ia-autocomplete',
  templateUrl: './ia-autocomplete-input.html',
  styleUrls: ['./ia-autocomplete-input.css'],
  host: {
    '[id]': 'id',
    '[attr.aria-describedby]': 'describedBy'
  },
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => IaAutocompleteInput),
      multi: true
    },
    {
      provide: MatFormFieldControl,
      useExisting: IaAutocompleteInput
    },
    {
      provide: NG_VALIDATORS,
      useExisting: forwardRef(() => IaAutocompleteInput),
      multi: true
    }
  ]
})
export class IaAutocompleteInput implements OnInit, OnDestroy, DoCheck, ControlValueAccessor, MatFormFieldControl<any>, Validator {


  public readonly searchFormControl: FormControl;
  public isLoading: boolean;
  public filteredStrings: string[] = [];
  private _selectedString: string;
  @ViewChild('input') private inputField: MatInput;

  private _dataSource: AutoCompleteDataSource;

  private _propagateChange = (_: any) => {
  };
  private _onTouched = () => {
  };

  private static nextId = 0;
  private _disabled = false;
  private _placeholder: string;
  private _required = false;
  private _prefix: string;

  public ngControl: NgControl;
  public errorState: boolean;
  public focused: boolean;

  public readonly stateChanges = new Subject<void>();
  @HostBinding() public readonly id = `ia-autocomplete-${IaAutocompleteInput.nextId++}`;
  @HostBinding('attr.aria-describedby') private describedBy = '';

  constructor(public elRef: ElementRef, private injector: Injector, private fm: FocusMonitor) {

    this.searchFormControl = new FormControl();
    this.isLoading = false;
    this._disabled = false;
    this.focused = false;
    fm.monitor(elRef.nativeElement, true).subscribe(origin => {
      this.focused = !!origin;
      this.stateChanges.next();
    });
  }

  ngOnInit() {
    this.ngControl = this.injector.get(NgControl, null);
    if (this.ngControl != null) {
      this.ngControl.valueAccessor = this;
    }

    // If prefix defined, usually this is used for a code scanner.
    // The goal is to select the entity in direct way.
    if (this._prefix) {
      this.searchFormControl
        .valueChanges
        .pipe(
          debounceTime(300),
          filter(value => typeof value === 'string'),
          filter(value => value.startsWith(this._prefix)),
          tap(() => this.value = null),
          tap(() => this.isLoading = true),
          map(value => this._dataSource.onPreprocessSearchString(value)),
          switchMap(value => this._dataSource.onFilter(value))
        )
        .subscribe(resultSet => {

          if (resultSet.list.length === 1)
          {
            this.value = resultSet[0];
          }
          else
          {
            this.filteredStrings = resultSet.list;
            this.searchFormControl.patchValue(resultSet.searchTerm, {emitEvent:false});
          }

          this.isLoading = false;

        });
    }

    // This is the search functionality when search by human.
    this.searchFormControl
      .valueChanges
      .pipe(
        debounceTime(300),
        tap(value => {
            this.filteredStrings = [];
          }
        ),
        filter(value => value != null && value.toString().length >= 1),
        tap(()=> this.isLoading = true),
        map(value => this._dataSource.onPreprocessSearchString(value)),
        tap(value => this.value = value),
        switchMap(value => this._dataSource.onFilter(value))
      )
      .subscribe(resultSet => {
          this.filteredStrings = resultSet.list;
          this.searchFormControl.patchValue(resultSet.searchTerm, {emitEvent:false});
          this.isLoading = false;
        }
      );
  }

  ngOnDestroy(): void {
    this.stateChanges.complete();
    this.fm.stopMonitoring(this.elRef.nativeElement);
  }

  ngDoCheck(): void {
    if (this.ngControl) {
      this.errorState = this.ngControl.invalid && this.ngControl.touched;
      this.stateChanges.next();
    }
  }

  @Input()
  public set dataSource(value: AutoCompleteDataSource) {
    this._dataSource = value;
  }

  public get dataSource() {
    return this._dataSource;
  }

  public valueChange: EventEmitter<any> = new EventEmitter<any>();

  public get value(): any {
    return this._selectedString;
  }

  public set value(value: any) {
    if (value !== this._selectedString) {
      this._selectedString = value;
      this.searchFormControl.setValue(value);
      this.valueChange.emit(value);
      this._propagateChange(this._selectedString);
      this.stateChanges.next();
    }
  }

  public clear() {
    this.filteredStrings = [];
    this.searchFormControl.reset(null);
  }

  public focus() {
    this.elRef.nativeElement.querySelector('input').focus();
  }

  public onOptionSelected(event: MatAutocompleteSelectedEvent) {
    this.value = event.option.value;
    this.filteredStrings = [];
  }

  private activatedValue: any;

  public onOptionActivated(event: MatAutocompleteActivatedEvent) {
    this.activatedValue = event.option?.value;
  }

  public onAutoCompleteClosed() {

    if (this.activatedValue) {

      this.value = this.activatedValue;
      this.activatedValue = null;

    }
  }

  public reset() {
    this.value = null;
  }

  public onFocusOut() {

    if (this.filteredStrings.length === 1) {
      this.activatedValue = this.filteredStrings[0];
      this.value = this.filteredStrings[0];
    }
  }

  public onTouched() {

    this._onTouched();
    this.stateChanges.next();
  }

  public registerOnChange(fn: any): void {
    this._propagateChange = fn;
  }

  public registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }

  public writeValue(obj: any): void {
    this.clear();
    this.value = obj;
  }

  public setDisabledState(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }

  @Input()
  public get prefix(): string {
    return this._prefix;
  }

  public set prefix(value: string) {
    if (value !== this._prefix) {
      this._prefix = value;
    }
  }

  @Input()
  public get disabled() {
    return this._disabled;
  }

  public set disabled(dis) {
    this._disabled = coerceBooleanProperty(dis);
    this._disabled ? this.searchFormControl.disable() : this.searchFormControl.enable();
    this.stateChanges.next();
  }

  public get empty() {
    let n = this.searchFormControl.value;
    return !!!n;
  }

  @Input()
  public get placeholder() {
    return this._placeholder;
  }

  public set placeholder(plh) {
    this._placeholder = plh;
    this.stateChanges.next();
  }

  @Input()
  public get required() {
    return this._required;
  }

  public set required(req) {
    this._required = coerceBooleanProperty(req);
    this.inputField.required = this._required;
    this.stateChanges.next();
  }

  @HostBinding('class.floating')
  public get shouldLabelFloat() {
    return this.focused || !this.empty;
  }

  public onContainerClick(event: MouseEvent): void {
    if ((event.target as Element).tagName.toLowerCase() != 'input') {
      this.focus();
    }
  }

  public setDescribedByIds(ids: string[]): void {
    this.describedBy = ids.join(' ');
  }

  validate(control: AbstractControl): ValidationErrors | null {
    const selection: any = control.value;
    if (typeof selection === 'string' && (<string>selection).length < 1) {
      return {incorrect: true};
    }
    return null;
  }
}
