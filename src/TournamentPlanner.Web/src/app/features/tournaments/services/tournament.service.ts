import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import {
  CreateTournamentRequest,
  CreateTournamentResponse,
  TournamentSummaryResponse,
} from '../tournaments.models';

@Injectable({ providedIn: 'root' })
export class TournamentService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = '/api/tournaments';

  getAll(): Observable<TournamentSummaryResponse[]> {
    return this.http.get<TournamentSummaryResponse[]>(`${this.baseUrl}`);
  }

  create(request: CreateTournamentRequest): Observable<CreateTournamentResponse> {
    return this.http.post<CreateTournamentResponse>(`${this.baseUrl}`, request);
  }
}
