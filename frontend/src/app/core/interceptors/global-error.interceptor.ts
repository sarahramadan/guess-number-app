import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { ErrorHandlerService } from '../services/error-handler.service';

@Injectable()
export class GlobalErrorInterceptor implements HttpInterceptor {

  constructor(private errorHandler: ErrorHandlerService) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(
      catchError((error: HttpErrorResponse) => {
        // Process the error using our error handler service
        const processedError = this.errorHandler.processError(error);
        
        // Enhance the error object with processed information
        const enhancedError = {
          ...error,
          processedError: {
            message: processedError.message,
            validationErrors: processedError.validationErrors,
            isValidationError: processedError.isValidationError
          }
        };

        // Log the error for debugging
        console.error('HTTP Error:', {
          url: req.url,
          method: req.method,
          status: error.status,
          statusText: error.statusText,
          processedMessage: processedError.message,
          validationErrors: processedError.validationErrors
        });

        // Return the enhanced error
        return throwError(() => enhancedError);
      })
    );
  }
}