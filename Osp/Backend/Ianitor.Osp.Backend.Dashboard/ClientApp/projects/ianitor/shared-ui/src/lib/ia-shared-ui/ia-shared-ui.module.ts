import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {IaNotificationBarComponent} from './ia-notification-bar/ia-notification-bar.component';
import {MatToolbarModule} from "@angular/material/toolbar";
import {MatButtonModule} from "@angular/material/button";
import {MatSnackBarModule} from "@angular/material/snack-bar";
import {MatDialogModule} from "@angular/material/dialog";
import {MessageDetailsComponent} from "./message-details/message-details.component";
import {IaAutocompleteInput} from "./ia-autocomplete-input/ia-autocomplete-input";
import {MatAutocompleteModule} from "@angular/material/autocomplete";
import {MatInputModule} from "@angular/material/input";
import {MatProgressSpinnerModule} from "@angular/material/progress-spinner";
import {ReactiveFormsModule} from "@angular/forms";
import {IaEntitySelectInput} from './ia-entity-select-input/ia-entity-select-input.component';


@NgModule({
  declarations: [IaNotificationBarComponent, MessageDetailsComponent, IaAutocompleteInput, IaEntitySelectInput],
  exports: [
    IaNotificationBarComponent,
    IaAutocompleteInput,
    IaEntitySelectInput
  ],
  entryComponents: [
    MessageDetailsComponent,
    IaAutocompleteInput,
    IaEntitySelectInput
  ],
  imports: [
    CommonModule,
    MatToolbarModule,
    MatButtonModule,
    MatSnackBarModule,
    MatDialogModule,
    MatAutocompleteModule,
    MatInputModule,
    MatProgressSpinnerModule,
    ReactiveFormsModule
  ]
})
export class IaSharedUIModule {
}
