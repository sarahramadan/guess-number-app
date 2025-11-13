import { Component, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatDividerModule } from '@angular/material/divider';
import { MatTableModule } from '@angular/material/table';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { GameDetailsDialogComponent } from './game-details-dialog.component';
import { GameService } from '../../../core/services/game';
import { AuthService } from '../../../core/services/auth';
import { 
  GameSession, 
  PagedResult, 
  GameStatus, 
  GameDifficulty, 
  GameHistoryFilter 
} from '../../../core/models/game.model';

@Component({
  selector: 'app-history',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatSelectModule,
    MatFormFieldModule,
    MatPaginatorModule,
    MatDividerModule,
    MatTableModule,
    MatDialogModule
  ],
  templateUrl: './history.html',
  styleUrl: './history.scss',
})
export class HistoryComponent implements OnInit {
  // Expose enums to template
  GameStatus = GameStatus;
  GameDifficulty = GameDifficulty;

  // State signals
  gameHistory = signal<PagedResult<GameSession> | null>(null);
  loading = signal(true);
  error = signal<string | null>(null);

  // Filter controls
  statusFilter = new FormControl<string>('');
  difficultyFilter = new FormControl<string>('');

  // Pagination
  currentPage = signal(0);
  pageSize = signal(10);

  // Table columns
  displayedColumns: string[] = ['difficulty', 'status', 'attempts', 'score', 'startedAt', 'endedAt', 'actions'];

  constructor(
    private gameService: GameService,
    private authService: AuthService,
    private router: Router,
    private dialog: MatDialog
  ) {}

  ngOnInit(): void {
    if (!this.authService.isAuthenticated()) {
      this.router.navigate(['/auth/login']);
      return;
    }
    
    this.loadGameHistory();
    
    // Subscribe to filter changes
    this.statusFilter.valueChanges.subscribe(() => {
      this.currentPage.set(0);
      this.loadGameHistory();
    });
    
    this.difficultyFilter.valueChanges.subscribe(() => {
      this.currentPage.set(0);
      this.loadGameHistory();
    });
  }

  private loadGameHistory(): void {
    this.loading.set(true);
    this.error.set(null);

    const filter: GameHistoryFilter = {
      page: this.currentPage() + 1, // API uses 1-based pagination
      pageSize: this.pageSize(),
      status: this.statusFilter.value || undefined,
      difficulty: this.difficultyFilter.value || undefined
    };

    this.gameService.getGameHistory(filter).subscribe({
      next: (history) => {
        this.gameHistory.set(history);
        this.loading.set(false);
      },
      error: (error) => {
        this.error.set(error.error?.message || 'Failed to load game history');
        this.loading.set(false);
      }
    });
  }

  onPageChange(event: PageEvent): void {
    this.currentPage.set(event.pageIndex);
    this.pageSize.set(event.pageSize);
    this.loadGameHistory();
  }

  viewGameDetails(gameSession: GameSession): void {
    const dialogRef = this.dialog.open(GameDetailsDialogComponent, {
      width: '800px',
      maxWidth: '90vw',
      data: gameSession,
      autoFocus: false
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result === 'playAgain') {
        this.playAgain(gameSession);
      }
    });
  }

  playAgain(gameSession: GameSession): void {
    // Create new game with same difficulty
    this.router.navigate(['/game/play'], { 
      queryParams: { difficulty: gameSession.difficulty } 
    });
  }

  clearFilters(): void {
    this.statusFilter.setValue('');
    this.difficultyFilter.setValue('');
    this.currentPage.set(0);
    this.loadGameHistory();
  }

  refresh(): void {
    this.loadGameHistory();
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
      case GameStatus.Abandoned:
        return 'Abandoned';
      case GameStatus.Lost:
        return 'Lost';
      default:
        return 'Unknown';
    }
  }

  getDifficultyColor(difficulty: GameDifficulty): string {
    switch (difficulty) {
      case GameDifficulty.Easy:
        return 'primary';
      case GameDifficulty.Medium:
        return 'accent';
      case GameDifficulty.Hard:
        return 'warn';
      default:
        return 'basic';
    }
  }

  getStatusColor(status: GameStatus): string {
    switch (status) {
      case GameStatus.InProgress:
        return 'accent';
      case GameStatus.Won:
        return 'primary';
      case GameStatus.Abandoned:
      case GameStatus.Lost:
        return 'warn';
      default:
        return 'basic';
    }
  }

  getAttemptsCount(gameSession: GameSession): number {
    return gameSession.attemptsCount || 0;
  }

  navigateToPlay(): void {
    this.router.navigate(['/game/play']);
  }

  navigateToStats(): void {
    this.router.navigate(['/game/stats']);
  }
}
