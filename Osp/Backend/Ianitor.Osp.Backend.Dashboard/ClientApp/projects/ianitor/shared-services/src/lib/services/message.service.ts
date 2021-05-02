import {Injectable} from '@angular/core';
import {MatSnackBar} from "@angular/material/snack-bar";
import {BehaviorSubject, Observable} from "rxjs";
import {ErrorMessage} from "../models/errorMessage";

@Injectable()
export class MessageService {

  latestErrorMessage : BehaviorSubject<ErrorMessage>;

  errorMessages: Array<ErrorMessage>;


  constructor(private snackBar: MatSnackBar) {

    this.latestErrorMessage = new BehaviorSubject<ErrorMessage>(null);
    this.errorMessages = new Array<ErrorMessage>();
  }

  getErrorMessageCount()
  {
    return this.errorMessages.length;
  }

  getErrorMessage(index: number)
  {
    return this.errorMessages[index];
  }

  getLatestErrorMessage() : Observable<ErrorMessage>
  {
    return this.latestErrorMessage.asObservable();
  }

  showError(message: string, title: string) {
    console.error(message);

    const errorMessage = <ErrorMessage> {
      title: title,
      message: message
    };
    this.errorMessages.push(errorMessage);

    this.latestErrorMessage.next(errorMessage);
  }

  showInformation(message: string)
  {
    this.snackBar.open(message, null, {
      duration: 3000,
      // here specify the position
      horizontalPosition: "center",
      verticalPosition: 'bottom'
    });
  }
}
