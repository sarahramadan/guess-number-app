import { Component, signal, OnInit, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { MatDividerModule } from '@angular/material/divider';
import { GameService } from '../../../core/services/game';
import { AuthService } from '../../../core/services/auth';
import { 
  GameSession, 
  GameDifficulty, 
  GameStatus, 
  GuessResult, 
  CreateGameRequest, 
  MakeGuessRequest,
  MakeGuessResponse,
  GameAttempt 
} from '../../../core/models/game.model';

@Component({
  selector: 'app-play',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatSelectModule,
    MatDividerModule
  ],
  templateUrl: './play.html',
  styleUrl: './play.scss',
})
export class PlayComponent implements OnInit {
  // Expose enums to template
  GameDifficulty = GameDifficulty;
  GameStatus = GameStatus;
  GuessResult = GuessResult;

  // Form controls
  newGameForm: FormGroup;
  guessForm: FormGroup;

  // State signals
  loading = signal(false);
  currentGame = signal<GameSession | null>(null);
  error = signal<string | null>(null);
  gameMessage = signal<string>('');
  showCelebration = signal(false);

  // Computed properties
  isGameActive = computed(() => {
    const game = this.currentGame();
    return game && game.status === GameStatus.InProgress;
  });

  attemptsCount = computed(() => {
    return this.currentGame()?.attemptsCount || 0;
  });

  lastGuess = computed(() => {
    const attempts = this.currentGame()?.attempts;
    return attempts && attempts.length > 0 ? attempts[attempts.length - 1] : null;
  });

  constructor(
    private fb: FormBuilder,
    private gameService: GameService,
    private authService: AuthService,
    private router: Router
  ) {
    this.newGameForm = this.fb.group({
      difficulty: [GameDifficulty.Easy, [Validators.required]]
    });

    this.guessForm = this.fb.group({
      guessedNumber: ['', [Validators.required, Validators.min(1), Validators.max(100)]]
    });
  }

  ngOnInit(): void {
    // Check if user is authenticated
    if (!this.authService.isUserAuthenticated()) {
      this.router.navigate(['/auth/login']);
      return;
    }
    
    // Load current game if exists
    this.loadCurrentGame();
  }

  private loadCurrentGame(): void {
    // Here you might want to check for an active game from localStorage
    // or make an API call to get the current active game
    const savedGame = localStorage.getItem('currentGame');
    if (savedGame) {
      try {
        const game: GameSession = JSON.parse(savedGame);
        if (game.status === GameStatus.InProgress) {
          this.currentGame.set(game);
        }
      } catch (error) {
        localStorage.removeItem('currentGame');
      }
    }
  }

  startNewGame(): void {
    if (this.newGameForm.valid && !this.loading()) {
      this.loading.set(true);
      this.error.set(null);
      this.gameMessage.set('');

      const request: CreateGameRequest = {
        difficulty: this.newGameForm.value.difficulty
      };

      this.gameService.createGame(request).subscribe({
        next: (game) => {
          this.currentGame.set(game);
          this.gameService.setCurrentGame(game);
          this.saveGameToStorage(game);
          this.loading.set(false);
          this.gameMessage.set(`New ${this.getDifficultyName(game.difficulty)} game started! Guess a number between ${game.minRange} and ${game.maxRange}.`);
          
          // Update form validation based on the game range
          this.guessForm.get('guessedNumber')?.setValidators([
            Validators.required, 
            Validators.min(game.minRange), 
            Validators.max(game.maxRange)
          ]);
          this.guessForm.get('guessedNumber')?.updateValueAndValidity();
          
          this.guessForm.reset();
        },
        error: (error) => {
          this.loading.set(false);
          this.error.set(error.error?.message || 'Failed to start new game. Please try again.');
        }
      });
    }
  }

  makeGuess(): void {
    if (this.guessForm.valid && !this.loading() && this.isGameActive()) {
      this.loading.set(true);
      this.error.set(null);

      const request: MakeGuessRequest = {
        guessedNumber: parseInt(this.guessForm.value.guessedNumber)
      };

      const gameId = this.currentGame()!.id;

      this.gameService.makeGuess(gameId, request).subscribe({
        next: (response) => {
          this.loading.set(false);
          this.processGuessResponse(response);
          this.guessForm.reset();
          
          // Refresh game session to get updated attempts
          this.refreshGameSession();
        },
        error: (error) => {
          this.loading.set(false);
          this.error.set(error.error?.message || 'Failed to make guess. Please try again.');
        }
      });
    }
  }

  private processGuessResponse(response: MakeGuessResponse): void {
    // Display the hint from the response
    this.gameMessage.set(response.hint);

    // Check if the game is complete based on the result
    if (response.result === GuessResult.Correct) {
      this.showCelebration.set(true);
      setTimeout(() => this.showCelebration.set(false), 3000);
      localStorage.removeItem('currentGame');
      this.gameMessage.set(`Congratulations! You found the number ${response.guessedNumber} in ${response.attemptNumber} attempts!`);
    } else {
      // Continue playing - the hint helps guide the next guess
      // Check if max attempts reached after refreshing game session
    }
  }

  private refreshGameSession(): void {
    const gameId = this.currentGame()?.id;
    if (gameId) {
      this.gameService.getGameSession(gameId).subscribe({
        next: (game) => {
          this.currentGame.set(game);
          this.saveGameToStorage(game);
          
          // Check if game is complete after refresh
          if (game.status !== GameStatus.InProgress) {
            localStorage.removeItem('currentGame');
            
            if (game.status === GameStatus.Won) {
              this.gameMessage.set(`Congratulations! You won the game!`);
            } else if (game.status === GameStatus.Lost) {
              this.gameMessage.set(`Game over! You've reached the maximum number of attempts.`);
            } else if (game.status === GameStatus.Abandoned) {
              this.gameMessage.set(`Game was abandoned.`);
            }
          }
        },
        error: (error) => {
          console.error('Failed to refresh game session:', error);
        }
      });
    }
  }

  private saveGameToStorage(game: GameSession): void {
    if (game.status === GameStatus.InProgress) {
      localStorage.setItem('currentGame', JSON.stringify(game));
    } else {
      localStorage.removeItem('currentGame');
    }
  }

  getDifficultyName(difficulty: GameDifficulty): string {
    switch (difficulty) {
      case GameDifficulty.Easy:
        return 'Easy';
      case GameDifficulty.Medium:
        return 'Medium';
      case GameDifficulty.Hard:
        return 'Hard';
      default:
        return 'Unknown';
    }
  }

  getResultIcon(result: GuessResult): string {
    switch (result) {
      case GuessResult.TooLow:
        return '⬆️';
      case GuessResult.TooHigh:
        return '⬇️';
      case GuessResult.Correct:
        return '✅';
      default:
        return '❓';
    }
  }

  getResultColor(result: GuessResult): string {
    switch (result) {
      case GuessResult.TooLow:
        return 'warn';
      case GuessResult.TooHigh:
        return 'warn';
      case GuessResult.Correct:
        return 'primary';
      default:
        return 'basic';
    }
  }

  abandonGame(): void {
    if (this.currentGame() && this.isGameActive()) {
      if (confirm('Are you sure you want to abandon this game?')) {
        // Update game status locally
        const game = this.currentGame()!;
        game.status = GameStatus.Abandoned;
        this.currentGame.set(game);
        localStorage.removeItem('currentGame');
        this.gameMessage.set('Game abandoned. Start a new game when you\'re ready!');
      }
    }
  }

  navigateToStats(): void {
    this.router.navigate(['/game/stats']);
  }

  navigateToHistory(): void {
    this.router.navigate(['/game/history']);
  }
}
