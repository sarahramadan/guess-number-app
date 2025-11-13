import { Component, signal, OnInit } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive, Router, NavigationEnd } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatMenuModule } from '@angular/material/menu';
import { filter } from 'rxjs/operators';
import { AuthService } from './core/services/auth';

@Component({
  selector: 'app-root',
  imports: [CommonModule, RouterOutlet, RouterLink, RouterLinkActive, MatButtonModule, MatMenuModule],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App implements OnInit {
  protected readonly title = signal('Guess Number Game');
  protected readonly isAuthPage = signal(false);
  
  constructor(
    private router: Router,
    private authService: AuthService
  ) {}
  
  ngOnInit() {
    // Update auth page status on route changes
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe((event: NavigationEnd) => {
      this.isAuthPage.set(event.url.startsWith('/auth'));
    });
    
    // Set initial state
    this.isAuthPage.set(this.router.url.startsWith('/auth'));
  }
  
  protected isAuthRoute(): boolean {
    return this.isAuthPage();
  }
  
  protected getCurrentUser() {
    return this.authService.currentUser;
  }
  
  protected logout(): void {
    this.authService.logout().subscribe({
      next: () => {
        // Successfully logged out
      },
      error: (error) => {
        console.error('Logout error:', error);
        // Logout will still complete due to error handling in service
      }
    });
  }
}
