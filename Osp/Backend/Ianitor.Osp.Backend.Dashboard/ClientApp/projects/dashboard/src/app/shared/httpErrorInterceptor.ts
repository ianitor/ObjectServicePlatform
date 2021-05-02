import {
  HttpEvent,
  HttpInterceptor,
  HttpHandler,
  HttpRequest,
  HttpErrorResponse
} from '@angular/common/http';
import {Observable, throwError} from 'rxjs';
import {retry, catchError} from 'rxjs/operators';
import {MessageService} from "@ianitor/shared-services";
import {ApiError} from "../models/apiError";

export class HttpErrorInterceptor implements HttpInterceptor {

  constructor(private messageService: MessageService) {
  }

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(request)
      .pipe(
        retry(1),
        catchError((error: HttpErrorResponse) => {
          let errorMessage = '';
          if (error.error.statusCode && error.error.statusDescription){
            // error provided by REST interface
            return throwError(<ApiError>error.error);
          }
          else if (error.error instanceof ErrorEvent) {
            // client-side error
            errorMessage = `Error: ${error.error.message}`;
          } else {
            // server-side error
            if (error.status === 404)
            {
              return throwError(<ApiError>{
                statusCode: error.status,
                statusDescription: error.statusText,
                message: error.message,
                details: [
                  {code: "NotFound", description: "The item does not exist."}
                ]
              });
            }

            errorMessage = `Error Code: ${error.status}\nMessage: ${error.message}`;
          }

          this.messageService.showError(errorMessage, "Error during communication with server");
          return throwError(errorMessage);
        })
      )
  }
}
