import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, BehaviorSubject, tap, catchError, throwError } from 'rxjs';
import { environment } from '../../../environments/environment.js';
import { 
  User, 
  LoginRequest, 
  RegisterRequest, 
  AuthResponse 
} from '../models/auth.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly TOKEN_KEY = 'guess_game_token';
  private readonly REFRESH_TOKEN_KEY = 'guess_game_refresh_token';
  private readonly USER_KEY = 'guess_game_user';
  private readonly apiUrl = `${environment.apiUrl}/v1.0/Auth`;

  private currentUserSubject = new BehaviorSubject<User | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();
  
  // Angular signals for reactive state
  public isAuthenticated = signal<boolean>(false);
  public currentUser = signal<User | null>(null);

  constructor(
    private http: HttpClient,
    private router: Router
  ) {
    this.initializeAuth();
  }

  private initializeAuth(): void {
    const token = localStorage.getItem(this.TOKEN_KEY);
    const userData = localStorage.getItem(this.USER_KEY);
    
    if (token && userData) {
      try {
        const user: User = JSON.parse(userData);
        this.setCurrentUser(user);
      } catch (error) {
        this.clearAuthData();
      }
    }
  }

  register(request: RegisterRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/register`, request)
      .pipe(
        tap(response => this.handleAuthSuccess(response)),
        catchError(this.handleAuthError)
      );
  }

  login(request: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, request)
      .pipe(
        tap(response => this.handleAuthSuccess(response)),
        catchError(this.handleAuthError)
      );
  }

  logout(): Observable<any> {
    return this.http.post(`${this.apiUrl}/logout`, {})
      .pipe(
        tap(() => this.handleLogoutSuccess()),
        catchError((error) => {
          // Even if the API call fails, clear local data
          this.handleLogoutSuccess();
          return throwError(() => error);
        })
      );
  }

  logoutLocal(): void {
    this.clearAuthData();
    this.router.navigate(['/auth/login']);
  }

  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  getRefreshToken(): string | null {
    return localStorage.getItem(this.REFRESH_TOKEN_KEY);
  }

  isUserAuthenticated(): boolean {
    return this.isAuthenticated();
  }

  isTokenExpired(): boolean {
    const token = this.getToken();
    if (!token) return true;

    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const expiry = payload.exp;
      return (Math.floor((new Date()).getTime() / 1000)) >= expiry;
    } catch (error) {
      return true;
    }
  }

  private handleAuthSuccess(response: AuthResponse): void {
    if (response.success && response.data) {
      localStorage.setItem(this.TOKEN_KEY, response.data.accessToken);
      localStorage.setItem(this.REFRESH_TOKEN_KEY, response.data.refreshToken);
      localStorage.setItem(this.USER_KEY, JSON.stringify(response.data.user));
      this.setCurrentUser(response.data.user);
    } else {
      throw new Error(response.message || 'Authentication failed');
    }
  }

  private handleLogoutSuccess(): void {
    this.clearAuthData();
    this.router.navigate(['/auth/login']);
  }

  private setCurrentUser(user: User): void {
    this.currentUser.set(user);
    this.isAuthenticated.set(true);
    this.currentUserSubject.next(user);
  }

  private clearAuthData(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.REFRESH_TOKEN_KEY);
    localStorage.removeItem(this.USER_KEY);
    this.currentUser.set(null);
    this.isAuthenticated.set(false);
    this.currentUserSubject.next(null);
  }

  private handleAuthError(error: any): Observable<never> {
    console.error('Authentication error:', error);
    return throwError(() => error);
  }
}
