import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import {
  CreateTournamentRequest,
  CreateTournamentResponse,
  ScoreRoundRequest,
  TournamentDetailResponse,
  TournamentSummaryResponse,
} from '../tournaments.models';

@Injectable({ providedIn: 'root' })
export class TournamentService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = '/api/tournaments';

  getAll(): Observable<TournamentSummaryResponse[]> {
    return this.http.get<TournamentSummaryResponse[]>(`${this.baseUrl}`);
  }

  getDetail(tournamentId: string): Observable<TournamentDetailResponse> {
    return this.http.get<TournamentDetailResponse>(`${this.baseUrl}/${tournamentId}`);
  }

  create(request: CreateTournamentRequest): Observable<CreateTournamentResponse> {
    return this.http.post<CreateTournamentResponse>(`${this.baseUrl}`, request);
  }

  join(tournamentId: string): Observable<{ joined: boolean; message?: string }> {
    return this.http.post<{ joined: boolean; message?: string }>(
      `${this.baseUrl}/${tournamentId}/join`,
      {},
    );
  }

  addStaff(tournamentId: string, userId: string): Observable<{ added: boolean; message?: string }> {
    return this.http.post<{ added: boolean; message?: string }>(
      `${this.baseUrl}/${tournamentId}/staff`,
      {
        userId,
      },
    );
  }

  scoreRound(boutId: string, roundNumber: number, request: ScoreRoundRequest): Observable<unknown> {
    return this.http.post(`/api/bouts/${boutId}/rounds/${roundNumber}/score-events`, request);
  }
}
