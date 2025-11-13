export enum GameStatus {
  InProgress = 1,
  Won = 2,
  Lost = 3,
  Abandoned = 4
}

export enum GameDifficulty {
  Easy = 1,
  Medium = 2,
  Hard = 3
}

export enum GuessResult {
  TooLow = 1,
  TooHigh = 2,
  Correct = 3
}

export interface CreateGameRequest {
  difficulty: GameDifficulty;
}

export interface GameSession {
  id: string;
  userId: string;
  attemptsCount: number;
  maxAttempts: number;
  status: GameStatus;
  score: number;
  startedAt: string;
  endedAt?: string | null;
  minRange: number;
  maxRange: number;
  difficulty: GameDifficulty;
  attempts: GameAttempt[];
}

export interface GameAttempt {
  id: string;
  guessedNumber: number;
  attemptNumber: number;
  result: GuessResult;
  hint: string;
  attemptedAt: string;
}

export interface MakeGuessRequest {
  guessedNumber: number;
}

export interface MakeGuessResponse {
  id: string;
  guessedNumber: number;
  attemptNumber: number;
  result: GuessResult;
  hint: string;
  attemptedAt: string;
}

export interface UserStats {
  totalGames: number;
  gamesWon: number;
  totalScore: number;
  winRate: number;
  bestAttempts: number;
  statsByDifficulty: { [key: string]: any };
}

export interface PagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasNext: boolean;
  hasPrevious: boolean;
}

export interface GameHistoryFilter {
  page?: number;
  pageSize?: number;
  status?: string;
  difficulty?: string;
}

// API Response Wrappers
export interface GameApiResponse {
  success: boolean;
  data: GameSession;
  message: string | null;
  errors: any;
  statusCode: number;
  timestamp: string;
}

export interface GuessApiResponse {
  success: boolean;
  data: MakeGuessResponse;
  message: string | null;
  errors: any;
  statusCode: number;
  timestamp: string;
}

export interface GameHistoryApiResponse {
  success: boolean;
  data: PagedResult<GameSession>;
  message: string | null;
  errors: any;
  statusCode: number;
  timestamp: string;
}

export interface UserStatsApiResponse {
  success: boolean;
  data: UserStats;
  message: string | null;
  errors: any;
  statusCode: number;
  timestamp: string;
}