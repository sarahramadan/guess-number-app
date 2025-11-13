import { Injectable } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';

export interface ValidationError {
  field: string;
  messages: string[];
}

export interface ErrorResponse {
  type?: string;
  title?: string;
  status?: number;
  errors?: { [key: string]: string[] };
  message?: string;
  error?: string;
  traceId?: string;
}

@Injectable({
  providedIn: 'root'
})
export class ErrorHandlerService {

  /**
   * Processes HTTP error responses and extracts validation errors
   * @param error The HTTP error response
   * @returns Formatted error information
   */
  processError(error: HttpErrorResponse): {
    message: string;
    validationErrors: ValidationError[];
    isValidationError: boolean;
  } {
    const errorData: ErrorResponse = error.error;
    
    // Check if this is a validation error with structured errors object
    if (error.status === 400 && errorData?.errors) {
      return this.processValidationErrors(errorData.errors);
    }

    // Handle other types of errors
    return {
      message: this.getGeneralErrorMessage(error, errorData),
      validationErrors: [],
      isValidationError: false
    };
  }

  /**
   * Processes validation errors from the backend
   * @param errors The errors object from backend validation response
   * @returns Formatted validation error information
   */
  private processValidationErrors(errors: { [key: string]: string[] }): {
    message: string;
    validationErrors: ValidationError[];
    isValidationError: boolean;
  } {
    const validationErrors: ValidationError[] = [];
    let message = 'Please fix the following issues:\n\n';

    Object.keys(errors).forEach(fieldName => {
      const fieldErrors = errors[fieldName];
      if (fieldErrors && Array.isArray(fieldErrors)) {
        validationErrors.push({
          field: fieldName,
          messages: fieldErrors
        });

        // Add to general message
        const fieldDisplayName = this.getFieldDisplayName(fieldName);
        message += `${fieldDisplayName}:\n`;
        fieldErrors.forEach(errorMsg => {
          message += `• ${errorMsg}\n`;
        });
        message += '\n';
      }
    });

    return {
      message: message.trim(),
      validationErrors,
      isValidationError: true
    };
  }

  /**
   * Gets a user-friendly field display name
   * @param fieldName The backend field name
   * @returns User-friendly field name
   */
  private getFieldDisplayName(fieldName: string): string {
    const fieldDisplayNames: { [key: string]: string } = {
      'FirstName': 'First Name',
      'LastName': 'Last Name',
      'Email': 'Email',
      'Password': 'Password',
      'ConfirmPassword': 'Confirm Password',
      'UserName': 'Username',
      'CurrentPassword': 'Current Password',
      'NewPassword': 'New Password',
      'PhoneNumber': 'Phone Number',
      'DisplayName': 'Display Name'
    };

    return fieldDisplayNames[fieldName] || fieldName;
  }

  /**
   * Gets a general error message for non-validation errors
   * @param error The HTTP error response
   * @param errorData The parsed error data
   * @returns General error message
   */
  private getGeneralErrorMessage(error: HttpErrorResponse, errorData: ErrorResponse): string {
    if (errorData?.message) {
      return errorData.message;
    }

    if (errorData?.title) {
      return errorData.title;
    }

    if (errorData?.error) {
      return errorData.error;
    }

    // Default messages based on status codes
    switch (error.status) {
      case 400:
        return 'Bad request. Please check your input and try again.';
      case 401:
        return 'Unauthorized. Please log in and try again.';
      case 403:
        return 'Forbidden. You don\'t have permission to perform this action.';
      case 404:
        return 'Resource not found.';
      case 409:
        return 'Conflict. The resource already exists or there\'s a conflict with the current state.';
      case 422:
        return 'Validation failed. Please check your input.';
      case 500:
        return 'Internal server error. Please try again later.';
      case 503:
        return 'Service unavailable. Please try again later.';
      default:
        return 'An unexpected error occurred. Please try again.';
    }
  }

  /**
   * Formats validation errors for form display
   * @param validationErrors Array of validation errors
   * @param targetField Optional field to filter errors for
   * @returns Formatted error message for the specified field or all fields
   */
  formatValidationErrorsForField(validationErrors: ValidationError[], targetField?: string): string {
    if (!targetField) {
      // Return all errors formatted
      return validationErrors
        .map(ve => `${this.getFieldDisplayName(ve.field)}: ${ve.messages.join(', ')}`)
        .join('\n');
    }

    // Find errors for specific field (case-insensitive match)
    const fieldError = validationErrors.find(ve => 
      ve.field.toLowerCase() === targetField.toLowerCase() ||
      this.getFieldDisplayName(ve.field).toLowerCase().replace(/\s/g, '') === targetField.toLowerCase()
    );

    return fieldError ? fieldError.messages.join('\n') : '';
  }

  /**
   * Maps backend field names to frontend form control names
   * @param backendFieldName Backend field name
   * @returns Frontend form control name
   */
  mapToFrontendFieldName(backendFieldName: string): string {
    const fieldMapping: { [key: string]: string } = {
      'FirstName': 'firstName',
      'LastName': 'lastName',
      'Email': 'email',
      'Password': 'password',
      'ConfirmPassword': 'confirmPassword',
      'UserName': 'userName',
      'CurrentPassword': 'currentPassword',
      'NewPassword': 'newPassword'
    };

    return fieldMapping[backendFieldName] || backendFieldName.toLowerCase();
  }
}