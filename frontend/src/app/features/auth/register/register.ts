import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AuthService } from '../../../core/services/auth';
import { RegisterRequest } from '../../../core/models/auth.model';
import { ErrorHandlerService, ValidationError } from '../../../core/services/error-handler.service';
import { ErrorDisplayComponent } from '../../../shared/components/error-display/error-display.component';

@Component({
  selector: 'app-register',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    ErrorDisplayComponent
  ],
  templateUrl: './register.html',
})
export class RegisterComponent {
  registerForm: FormGroup;
  loading = signal(false);
  error = signal<string | null>(null);
  validationErrors = signal<ValidationError[]>([]);

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private errorHandler: ErrorHandlerService
  ) {
    this.registerForm = this.fb.group({
      firstName: ['', [Validators.required, Validators.minLength(2)]],
      lastName: ['', [Validators.required, Validators.minLength(2)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(8), this.passwordStrengthValidator]],
      confirmPassword: ['', [Validators.required]]
    }, { validators: this.passwordMatchValidator });
  }

  passwordStrengthValidator(control: any) {
    const value = control.value;
    if (!value) return null;
    
    const hasLowerCase = /[a-z]/.test(value);
    const hasUpperCase = /[A-Z]/.test(value);
    const hasNumber = /\d/.test(value);
    
    if (!hasLowerCase || !hasUpperCase || !hasNumber) {
      return { passwordStrength: true };
    }
    
    return null;
  }

  passwordMatchValidator(form: FormGroup) {
    const password = form.get('password');
    const confirmPassword = form.get('confirmPassword');
    
    if (password && confirmPassword && password.value !== confirmPassword.value) {
      confirmPassword.setErrors({ passwordMismatch: true });
      return { passwordMismatch: true };
    }
    
    if (confirmPassword?.errors?.['passwordMismatch']) {
      delete confirmPassword.errors['passwordMismatch'];
      if (Object.keys(confirmPassword.errors).length === 0) {
        confirmPassword.setErrors(null);
      }
    }
    
    return null;
  }

  onSubmit(): void {
    if (this.registerForm.valid && !this.loading()) {
      this.loading.set(true);
      this.error.set(null);
      this.validationErrors.set([]);

      // Map form values to backend model format
      const formValues = this.registerForm.value;
      const request: RegisterRequest = {
        FirstName: formValues.firstName,
        LastName: formValues.lastName,
        Email: formValues.email,
        Password: formValues.password,
        ConfirmPassword: formValues.confirmPassword
      };
      
      this.authService.register(request).subscribe({
        next: () => {
          this.loading.set(false);
          this.router.navigate(['/game']);
        },
        error: (error) => {
          this.loading.set(false);
          console.error('Registration error:', error);
          
          // Use the error handler service to process the error
          const processedError = this.errorHandler.processError(error);
          
          if (processedError.isValidationError) {
            this.validationErrors.set(processedError.validationErrors);
          } else {
            this.error.set(processedError.message);
          }
        }
      });
    }
  }

  getErrorMessage(fieldName: string): string {
    const field = this.registerForm.get(fieldName);
    
    // First check for backend validation errors
    const backendFieldName = this.getBackendFieldName(fieldName);
    const backendError = this.errorHandler.formatValidationErrorsForField(
      this.validationErrors(), 
      backendFieldName
    );
    
    if (backendError) {
      return backendError;
    }
    
    // Then check for local form validation errors
    const fieldDisplayNames: { [key: string]: string } = {
      'firstName': 'First name',
      'lastName': 'Last name',
      'email': 'Email',
      'password': 'Password',
      'confirmPassword': 'Confirm password'
    };
    
    const displayName = fieldDisplayNames[fieldName] || fieldName.charAt(0).toUpperCase() + fieldName.slice(1);
    
    if (field?.hasError('required')) {
      return `${displayName} is required`;
    }
    if (field?.hasError('email')) {
      return 'Please enter a valid email';
    }
    if (field?.hasError('minlength')) {
      const requiredLength = field.errors?.['minlength']?.requiredLength;
      return `${displayName} must be at least ${requiredLength} characters`;
    }
    if (field?.hasError('passwordStrength')) {
      return 'Password must contain at least one lowercase letter, one uppercase letter, and one digit';
    }
    if (field?.hasError('passwordMismatch')) {
      return 'Passwords do not match';
    }
    return '';
  }

  /**
   * Maps frontend field names to backend field names
   */
  private getBackendFieldName(frontendFieldName: string): string {
    const fieldMapping: { [key: string]: string } = {
      'firstName': 'FirstName',
      'lastName': 'LastName',
      'email': 'Email',
      'password': 'Password',
      'confirmPassword': 'ConfirmPassword'
    };
    
    return fieldMapping[frontendFieldName] || frontendFieldName;
  }
}
