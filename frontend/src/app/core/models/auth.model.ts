export interface User {
  id: string;
  email: string;
  displayName: string;
  roles: string[];
  stats?: {
    totalScore: number;
    gamesPlayed: number;
    gamesWon: number;
    averageScore: number;
    winRate: number;
    bestAttempts: number | null;
  };
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  FirstName: string;
  LastName: string;
  Email: string;
  Password: string;
  ConfirmPassword: string;
}

export interface AuthData {
  accessToken: string;
  refreshToken: string;
  expiresAt: string;
  user: User;
}

export interface AuthResponse {
  success: boolean;
  data: AuthData;
  message: string | null;
  errors: any;
  statusCode: number;
  timestamp: string;
}