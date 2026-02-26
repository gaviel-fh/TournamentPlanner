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
      signupStartDateUtc: this.toUtcIso(formValue.signupStartDateUtc),
      signupEndDateUtc: this.toUtcIso(formValue.signupEndDateUtc),
      latitude: this.toNullableNumber(formValue.latitude),
      longitude: this.toNullableNumber(formValue.longitude),
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

  private toNullableNumber(value: number | null): number | null {
    if (value === null || Number.isNaN(value)) {
      return null;
    }

    return Number(value);
  }
}
