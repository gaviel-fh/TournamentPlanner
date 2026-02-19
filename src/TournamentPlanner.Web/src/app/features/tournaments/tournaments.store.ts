import { HttpErrorResponse } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { finalize, map, Observable, switchMap, tap } from 'rxjs';
import { AuthApiService } from '../../core/auth/auth-api.service';
import { AuthUserLookup } from '../../core/auth/auth.models';
import { AuthStore } from '../../core/auth/auth.store';
import { TournamentFeatureCreateService } from './services/tournament-feature-create.service';
import { TournamentService } from './services/tournament.service';
import { TournamentCreateFormValue, TournamentSummaryResponse } from './tournaments.models';

@Injectable()
export class TournamentsStore {
  private readonly tournamentService = inject(TournamentService);
  private readonly createMapper = inject(TournamentFeatureCreateService);
  private readonly authApi = inject(AuthApiService);
  readonly authStore = inject(AuthStore);

  private readonly tournamentsSignal = signal<TournamentSummaryResponse[]>([]);
  private readonly usersSignal = signal<AuthUserLookup[]>([]);

  readonly loadingList = signal(false);
  readonly loadingUsers = signal(false);
  readonly creating = signal(false);
  readonly error = signal<string | null>(null);
  readonly successMessage = signal<string | null>(null);

  readonly tournaments = this.tournamentsSignal.asReadonly();
  readonly users = this.usersSignal.asReadonly();
  readonly hasTournaments = computed(() => this.tournamentsSignal().length > 0);

  loadUsers(): Observable<void> {
    this.loadingUsers.set(true);
    this.error.set(null);

    return this.authApi.getUsers().pipe(
      tap((users) => this.usersSignal.set(users)),
      map(() => undefined),
      tap({
        error: (error) => this.error.set(this.toErrorMessage(error)),
      }),
      finalize(() => this.loadingUsers.set(false)),
    );
  }

  loadTournaments(): Observable<void> {
    this.loadingList.set(true);
    this.error.set(null);

    return this.tournamentService.getAll().pipe(
      tap((items) => this.tournamentsSignal.set(items)),
      map(() => undefined),
      tap({
        error: (error) => this.error.set(this.toErrorMessage(error)),
      }),
      finalize(() => this.loadingList.set(false)),
    );
  }

  createTournament(formValue: TournamentCreateFormValue): Observable<void> {
    this.creating.set(true);
    this.error.set(null);
    this.successMessage.set(null);

    const request = this.createMapper.toCreateRequest(formValue);

    return this.tournamentService.create(request).pipe(
      tap((response) => {
        this.successMessage.set(`Tournament created (${response.id}).`);
      }),
      switchMap(() => this.tournamentService.getAll()),
      tap((items) => this.tournamentsSignal.set(items)),
      map(() => undefined),
      tap({
        error: (error) => this.error.set(this.toErrorMessage(error)),
      }),
      finalize(() => this.creating.set(false)),
    );
  }

  clearMessages(): void {
    this.error.set(null);
    this.successMessage.set(null);
  }

  private toErrorMessage(error: unknown): string {
    if (error instanceof HttpErrorResponse) {
      if (typeof error.error?.message === 'string') {
        return error.error.message;
      }

      if (typeof error.error?.Message === 'string') {
        return error.error.Message;
      }

      return `Request failed (${error.status || 'unknown'}).`;
    }

    return 'Something went wrong. Please try again.';
  }
}
