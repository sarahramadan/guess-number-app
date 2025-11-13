import { Component, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDividerModule } from '@angular/material/divider';
import { GameService } from '../../../core/services/game';
import { AuthService } from '../../../core/services/auth';
import { UserStats } from '../../../core/models/game.model';

@Component({
  selector: 'app-stats',
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatDividerModule
  ],
  templateUrl: './stats.html',
  styleUrl: './stats.scss',
})
export class StatsComponent implements OnInit {
  stats = signal<UserStats | null>(null);
  loading = signal(true);
  error = signal<string | null>(null);

  constructor(
    private gameService: GameService,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    if (!this.authService.isAuthenticated()) {
      this.router.navigate(['/auth/login']);
      return;
    }
    
    this.loadUserStats();
  }

  private loadUserStats(): void {
    this.loading.set(true);
    this.error.set(null);

    this.gameService.getUserStats().subscribe({
      next: (stats) => {
        this.stats.set(stats);
        this.loading.set(false);
      },
      error: (error) => {
        this.error.set(error.error?.message || 'Failed to load statistics');
        this.loading.set(false);
      }
    });
  }

  getWinRate(): number {
    const currentStats = this.stats();
    if (!currentStats) return 0;
    return Math.round(currentStats.winRate);
  }

  navigateToPlay(): void {
    this.router.navigate(['/game/play']);
  }

  navigateToHistory(): void {
    this.router.navigate(['/game/history']);
  }

  refresh(): void {
    this.loadUserStats();
  }
}
