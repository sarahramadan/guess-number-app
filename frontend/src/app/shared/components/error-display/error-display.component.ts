import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ValidationError } from '../../../core/services/error-handler.service';

@Component({
  selector: 'app-error-display',
  standalone: true,
  imports: [CommonModule],
  template: `
    @if (errorMessage || validationErrors.length > 0) {
      <div class="app-alert alert-danger">
        <div class="d-flex align-items-start">
          <div class="flex-grow-1">
            @if (errorMessage) {
              <div class="error-message" [innerHTML]="formatErrorMessage(errorMessage)"></div>
            }
            
            @if (validationErrors.length > 0) {
              <div class="validation-errors">
                @for (fieldError of validationErrors; track fieldError.field) {
                  <div class="field-error mb-2">
                    <strong>{{ getFieldDisplayName(fieldError.field) }}:</strong>
                    <ul class="mb-0 mt-1">
                      @for (message of fieldError.messages; track message) {
                        <li>{{ message }}</li>
                      }
                    </ul>
                  </div>
                }
              </div>
            }
          </div>
        </div>
      </div>
    }
  `,
  styles: [`
    .error-message {
      white-space: pre-line;
    }
    
    .validation-errors .field-error {
      border-left: 3px solid #dc3545;
      padding-left: 12px;
    }
    
    .validation-errors ul {
      list-style-type: disc;
      padding-left: 20px;
    }
    
    .validation-errors ul li {
      margin-bottom: 4px;
    }
    
    .app-alert {
      border-radius: 8px;
      border: 1px solid;
      padding: 16px;
      margin-bottom: 20px;
    }
    
    .alert-danger {
      color: #721c24;
      background-color: #f8d7da;
      border-color: #f5c6cb;
    }
  `]
})
export class ErrorDisplayComponent {
  @Input() errorMessage: string | null = null;
  @Input() validationErrors: ValidationError[] = [];

  formatErrorMessage(message: string): string {
    // Convert line breaks to HTML breaks for display
    return message.replace(/\n/g, '<br>');
  }

  getFieldDisplayName(fieldName: string): string {
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
}