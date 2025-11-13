import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatDividerModule } from '@angular/material/divider';
import { MatChipsModule } from '@angular/material/chips';
import { GameSession, GameStatus, GameDifficulty, GuessResult } from '../../../core/models/game.model';

@Component({
  selector: 'app-game-details-dialog',
  standalone: true,
  imports: [
    CommonModule,
    MatDialogModule,
    MatCardModule,
    MatButtonModule,
    MatDividerModule,
    MatChipsModule
  ],
  template: `
    <div class="game-details-dialog">
      <div mat-dialog-title class="dialog-header">
        <h2>🎮 Game Details</h2>
        <p class="game-subtitle">{{ getDifficultyName(data.difficulty) }} • {{ getStatusName(data.status) }}</p>
      </div>
      
      <div mat-dialog-content class="dialog-content">
        <!-- Game Summary -->
        <div class="game-summary">
          <div class="summary-grid">
            <div class="summary-item">
              <span class="label">Game ID</span>
              <span class="value">{{ data.id }}</span>
            </div>
            <div class="summary-item">
              <span class="label">Difficulty</span>
              <span class="value difficulty-badge" [ngClass]="'difficulty-' + getDifficultyName(data.difficulty).toLowerCase()">
                {{ getDifficultyName(data.difficulty) }}
              </span>
            </div>
            <div class="summary-item">
              <span class="label">Status</span>
              <span class="value status-badge" [ngClass]="'status-' + getStatusName(data.status).toLowerCase().replace(' ', '-')">
                {{ getStatusName(data.status) }}
              </span>
            </div>
            <div class="summary-item">
              <span class="label">Score</span>
              <span class="value score-value" [class.score-zero]="data.score === 0">{{ data.score }}</span>
            </div>
            <div class="summary-item">
              <span class="label">Attempts</span>
              <span class="value">{{ data.attemptsCount }} / {{ data.maxAttempts }}</span>
            </div>
            <div class="summary-item">
              <span class="label">Number Range</span>
              <span class="value">{{ data.minRange }} - {{ data.maxRange }}</span>
            </div>
            <div class="summary-item">
              <span class="label">Started At</span>
              <span class="value">{{ data.startedAt | date:'medium' }}</span>
            </div>
            <div class="summary-item">
              <span class="label">{{ data.endedAt ? 'Ended At' : 'Duration' }}</span>
              <span class="value">
                @if (data.endedAt) {
                  {{ data.endedAt | date:'medium' }}
                } @else {
                  In Progress
                }
              </span>
            </div>
          </div>
        </div>

        <mat-divider class="section-divider"></mat-divider>

        <!-- Attempts History -->
        @if (data.attempts && data.attempts.length > 0) {
          <div class="attempts-section">
            <h3 class="section-title">📜 Attempt History</h3>
            <div class="attempts-list">
              @for (attempt of data.attempts; track attempt.id) {
                <div class="attempt-card">
                  <div class="attempt-header">
                    <div class="attempt-number">
                      <span class="attempt-badge">#{{ attempt.attemptNumber }}</span>
                    </div>
                    <div class="attempt-guess">
                      <span class="guess-number">{{ attempt.guessedNumber }}</span>
                    </div>
                    <div class="attempt-result">
                      <span class="result-icon" [ngClass]="getResultClass(attempt.result)">
                        {{ getResultIcon(attempt.result) }}
                      </span>
                      <span class="result-text" [ngClass]="getResultClass(attempt.result)">
                        {{ getResultName(attempt.result) }}
                      </span>
                    </div>
                    <div class="attempt-time">
                      {{ attempt.attemptedAt | date:'short' }}
                    </div>
                  </div>
                  @if (attempt.hint) {
                    <div class="attempt-hint">
                      💡 {{ attempt.hint }}
                    </div>
                  }
                </div>
              }
            </div>
          </div>
        } @else {
          <div class="no-attempts">
            <p>📝 No attempts recorded for this game.</p>
          </div>
        }
      </div>
      
      <div mat-dialog-actions class="dialog-actions">
        <button mat-button (click)="onClose()">Close</button>
      </div>
    </div>
  `,
  styles: [`
    .game-details-dialog {
      max-height: 80vh;
      overflow: hidden;
      display: flex;
      flex-direction: column;
    }

    .dialog-header {
      text-align: center;
      padding-bottom: 1rem;
      
      h2 {
        margin: 0;
        color: #667eea;
        font-size: 1.5rem;
      }
      
      .game-subtitle {
        margin: 0.5rem 0 0 0;
        color: #6c757d;
        font-size: 0.9rem;
      }
    }

    .dialog-content {
      flex: 1;
      overflow-y: auto;
      padding: 0 1rem;
    }

    .game-summary {
      margin-bottom: 1.5rem;
    }

    .summary-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
      gap: 1rem;
    }

    .summary-item {
      display: flex;
      flex-direction: column;
      padding: 0.75rem;
      background: #f8f9ff;
      border-radius: 8px;
      border-left: 4px solid #667eea;
      
      .label {
        font-size: 0.75rem;
        color: #6c757d;
        text-transform: uppercase;
        letter-spacing: 0.5px;
        margin-bottom: 0.25rem;
      }
      
      .value {
        font-weight: 600;
        color: #2d3748;
        font-size: 1rem;
      }
    }

    .difficulty-badge {
      padding: 0.25rem 0.75rem;
      border-radius: 12px;
      font-size: 0.75rem;
      font-weight: 600;
      text-transform: uppercase;
      
      &.difficulty-easy {
        background: #e6fffa;
        color: #00b894;
      }
      
      &.difficulty-medium {
        background: #fff4e6;
        color: #e17055;
      }
      
      &.difficulty-hard {
        background: #ffe6e6;
        color: #d63031;
      }
    }

    .status-badge {
      padding: 0.25rem 0.75rem;
      border-radius: 12px;
      font-size: 0.75rem;
      font-weight: 600;
      text-transform: uppercase;
      
      &.status-won {
        background: #e6fffa;
        color: #00b894;
      }
      
      &.status-in-progress {
        background: #fff4e6;
        color: #e17055;
      }
      
      &.status-abandoned {
        background: #ffe6e6;
        color: #d63031;
      }

      &.status-lost {
        background: #ffe6e6;
        color: #d63031;
      }
    }

    .score-value {
      font-weight: bold;
      color: #28a745;
      
      &.score-zero {
        color: #6c757d;
        font-style: italic;
      }
    }

    .section-divider {
      margin: 1.5rem 0;
    }

    .attempts-section {
      .section-title {
        color: #667eea;
        font-size: 1.1rem;
        margin-bottom: 1rem;
      }
    }

    .attempts-list {
      max-height: 300px;
      overflow-y: auto;
      padding-right: 0.5rem;
    }

    .attempt-card {
      background: #f8f9ff;
      border-radius: 8px;
      padding: 1rem;
      margin-bottom: 0.75rem;
      border-left: 4px solid #667eea;
      transition: transform 0.2s ease;
      
      &:hover {
        transform: translateY(-2px);
      }
    }

    .attempt-header {
      display: grid;
      grid-template-columns: auto 1fr auto auto;
      gap: 1rem;
      align-items: center;
    }

    .attempt-badge {
      background: #667eea;
      color: white;
      padding: 0.25rem 0.75rem;
      border-radius: 12px;
      font-size: 0.75rem;
      font-weight: 600;
    }

    .guess-number {
      font-size: 1.25rem;
      font-weight: bold;
      color: #2d3748;
    }

    .result-icon {
      font-size: 1.2rem;
      margin-right: 0.5rem;
    }

    .result-text {
      font-weight: 600;
      font-size: 0.875rem;
    }

    .result-correct {
      color: #28a745;
    }

    .result-too-low {
      color: #007bff;
    }

    .result-too-high {
      color: #dc3545;
    }

    .attempt-time {
      font-size: 0.75rem;
      color: #6c757d;
    }

    .attempt-hint {
      margin-top: 0.75rem;
      padding: 0.5rem;
      background: #e8f5e8;
      border-radius: 6px;
      font-style: italic;
      color: #2d3748;
      border-left: 3px solid #28a745;
    }

    .no-attempts {
      text-align: center;
      padding: 2rem;
      color: #6c757d;
      font-style: italic;
    }

    .dialog-actions {
      padding: 1rem;
      display: flex;
      gap: 0.5rem;
      justify-content: flex-end;
      border-top: 1px solid #e9ecef;
      margin-top: 1rem;
    }

    @media (max-width: 768px) {
      .summary-grid {
        grid-template-columns: 1fr;
      }
      
      .attempt-header {
        grid-template-columns: 1fr;
        gap: 0.5rem;
        text-align: center;
      }
    }
  `]
})
export class GameDetailsDialogComponent {
  gameStatus = GameStatus;

  constructor(
    private dialogRef: MatDialogRef<GameDetailsDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: GameSession
  ) {}

  onClose(): void {
    this.dialogRef.close();
  }

  onPlayAgain(): void {
    this.dialogRef.close('playAgain');
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

  getStatusName(status: GameStatus): string {
    switch (status) {
      case GameStatus.InProgress:
        return 'In Progress';
      case GameStatus.Won:
        return 'Won';
      case GameStatus.Lost:
        return 'Lost';
      case GameStatus.Abandoned:
        return 'Abandoned';
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

  getResultName(result: GuessResult): string {
    switch (result) {
      case GuessResult.TooLow:
        return 'Too Low';
      case GuessResult.TooHigh:
        return 'Too High';
      case GuessResult.Correct:
        return 'Correct!';
      default:
        return 'Unknown';
    }
  }

  getResultClass(result: GuessResult): string {
    switch (result) {
      case GuessResult.TooLow:
        return 'result-too-low';
      case GuessResult.TooHigh:
        return 'result-too-high';
      case GuessResult.Correct:
        return 'result-correct';
      default:
        return '';
    }
  }
}