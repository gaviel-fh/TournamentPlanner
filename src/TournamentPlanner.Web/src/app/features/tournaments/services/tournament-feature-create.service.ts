import { Injectable } from '@angular/core';
import {
  CreateTournamentRequest,
  TournamentCreateFormValue,
  TournamentDisciplineFormValue,
} from '../tournaments.models';

@Injectable({ providedIn: 'root' })
export class TournamentFeatureCreateService {
  toCreateRequest(formValue: TournamentCreateFormValue): CreateTournamentRequest {
    return {
      name: formValue.name.trim(),
      venueName: formValue.venueName.trim(),
      startDateUtc: this.toUtcIso(formValue.startDateUtc),
      endDateUtc: this.toUtcIso(formValue.endDateUtc),
      organizerUserIds: formValue.organizerUserIds,
      staffUserIds: formValue.staffUserIds,
      participantUserIds: [],
      disciplines: formValue.disciplines
        .map((discipline) => this.normalizeDiscipline(discipline))
        .filter((discipline) => Boolean(discipline.code && discipline.name)),
    };
  }

  private normalizeDiscipline(
    discipline: TournamentDisciplineFormValue,
  ): TournamentDisciplineFormValue {
    return {
      code: discipline.code.trim().toUpperCase(),
      name: discipline.name.trim(),
      roundCount: Math.max(1, Number(discipline.roundCount) || 1),
      boutIntervalMinutes: Math.max(1, Number(discipline.boutIntervalMinutes) || 30),
    };
  }

  private toUtcIso(localDateTime: string): string {
    return new Date(localDateTime).toISOString();
  }
}
