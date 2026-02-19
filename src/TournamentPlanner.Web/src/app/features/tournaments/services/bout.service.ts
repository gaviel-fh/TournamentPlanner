import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class BoutService {
  private readonly http = inject(HttpClient);
  private readonly tournamentsBaseUrl = '/api/tournaments';

  generateRoundRobinSchedule(
    tournamentId: string,
    tournamentDisciplineId: string,
    firstBoutStartUtc?: string,
  ): Observable<{ createdBouts: number; createdRounds: number }> {
    return this.http.post<{ createdBouts: number; createdRounds: number }>(
      `${this.tournamentsBaseUrl}/${tournamentId}/disciplines/${tournamentDisciplineId}/round-robin`,
      { firstBoutStartUtc: firstBoutStartUtc ?? null },
    );
  }
}
