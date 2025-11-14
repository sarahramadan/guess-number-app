import { Injectable, signal } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable, catchError, throwError, map } from 'rxjs';
import { environment } from '../../../environments/environment';
import { 
  CreateGameRequest,
  GameSession,
  MakeGuessRequest,
  MakeGuessResponse,
  UserStats,
  PagedResult,
  GameHistoryFilter,
  GameApiResponse,
  GuessApiResponse,
  GameHistoryApiResponse,
  UserStatsApiResponse
} from '../models/game.model';
import { AuthService } from './auth';

@Injectable({
  providedIn: 'root'
})
export class GameService {
  private readonly apiUrl = `${environment.apiUrl}/v1.0/game`;
  
  // Current game state signals
  public currentGame = signal<GameSession | null>(null);
  public gameLoading = signal<boolean>(false);

  constructor(
    private http: HttpClient,
    private authService: AuthService
  ) {}

  private getHttpOptions(): { headers: HttpHeaders } {
    const token = this.authService.getToken();
    return {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`
      })
    };
  }

  createGame(request: CreateGameRequest): Observable<GameSession> {
    return this.http.post<GameApiResponse>(this.apiUrl, request, this.getHttpOptions())
      .pipe(
        map(response => {
          if (response.success && response.data) {
            return response.data;
          } else {
            throw new Error(response.message || 'Failed to create game');
          }
        }),
        catchError(this.handleError)
      );
  }

  makeGuess(gameSessionId: string, request: MakeGuessRequest): Observable<MakeGuessResponse> {
    return this.http.post<GuessApiResponse>(
      `${this.apiUrl}/${gameSessionId}/guess`, 
      request, 
      this.getHttpOptions()
    ).pipe(
      map(response => {
        if (response.success && response.data) {
          return response.data;
        } else {
          throw new Error(response.message || 'Failed to make guess');
        }
      }),
      catchError(this.handleError)
    );
  }

  getUserStats(): Observable<UserStats> {
    return this.http.get<UserStatsApiResponse>(`${this.apiUrl}/stats`, this.getHttpOptions())
      .pipe(
        map(response => {
          if (response.success) {
            return response.data;
          } else {
            throw new Error(response.message || 'Failed to load user stats');
          }
        }),
        catchError(this.handleError)
      );
  }

  getGameHistory(filter: GameHistoryFilter = {}): Observable<PagedResult<GameSession>> {
    let params = new HttpParams();
    
    if (filter.page) params = params.set('page', filter.page.toString());
    if (filter.pageSize) params = params.set('pageSize', filter.pageSize.toString());
    if (filter.status) params = params.set('status', filter.status);
    if (filter.difficulty) params = params.set('difficulty', filter.difficulty);

    return this.http.get<GameHistoryApiResponse>(
      `${this.apiUrl}/history`, 
      { ...this.getHttpOptions(), params }
    ).pipe(
      map(response => {
        if (response.success) {
          return response.data;
        } else {
          throw new Error(response.message || 'Failed to load game history');
        }
      }),
      catchError(this.handleError)
    );
  }

  getGameSession(gameSessionId: string): Observable<GameSession> {
    return this.http.get<GameApiResponse>(`${this.apiUrl}/${gameSessionId}`, this.getHttpOptions())
      .pipe(
        map(response => {
          if (response.success && response.data) {
            return response.data;
          } else {
            throw new Error(response.message || 'Failed to get game session');
          }
        }),
        catchError(this.handleError)
      );
  }

  // Helper methods for game state management
  setCurrentGame(game: GameSession | null): void {
    this.currentGame.set(game);
  }

  setGameLoading(loading: boolean): void {
    this.gameLoading.set(loading);
  }

  clearCurrentGame(): void {
    this.currentGame.set(null);
  }

  private handleError(error: any): Observable<never> {
    console.error('Game service error:', error);
    return throwError(() => error);
  }
}
