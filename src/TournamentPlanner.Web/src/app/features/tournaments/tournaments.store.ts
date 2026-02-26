import { HttpErrorResponse } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { finalize, map, Observable, of, switchMap, tap } from 'rxjs';
import { AuthApiService } from '../../core/auth/auth-api.service';
import { AuthUserLookup } from '../../core/auth/auth.models';
import { AuthStore } from '../../core/auth/auth.store';
import { TournamentFeatureCreateService } from './services/tournament-feature-create.service';
import { TournamentService } from './services/tournament.service';
import {
  TournamentCreateFormValue,
  TournamentDetailResponse,
  TournamentSummaryResponse,
} from './tournaments.models';

@Injectable()
export class TournamentsStore {
  private readonly tournamentService = inject(TournamentService);
  private readonly createMapper = inject(TournamentFeatureCreateService);
  private readonly authApi = inject(AuthApiService);
  readonly authStore = inject(AuthStore);

  private readonly tournamentsSignal = signal<TournamentSummaryResponse[]>([]);
  private readonly usersSignal = signal<AuthUserLookup[]>([]);
  private readonly selectedTournamentIdSignal = signal<string | null>(null);
  private readonly selectedTournamentDetailSignal = signal<TournamentDetailResponse | null>(null);

  readonly loadingList = signal(false);
  readonly loadingUsers = signal(false);
  readonly loadingDetail = signal(false);
  readonly actionLoading = signal(false);
  readonly creating = signal(false);
  readonly error = signal<string | null>(null);
  readonly successMessage = signal<string | null>(null);

  readonly tournaments = this.tournamentsSignal.asReadonly();
  readonly users = this.usersSignal.asReadonly();
  readonly selectedTournamentId = this.selectedTournamentIdSignal.asReadonly();
  readonly selectedTournamentDetail = this.selectedTournamentDetailSignal.asReadonly();
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
      tap((items) => {
        this.tournamentsSignal.set(items);

        const selected = this.selectedTournamentIdSignal();
        if (selected && !items.some((item) => item.id === selected)) {
          this.selectedTournamentIdSignal.set(null);
          this.selectedTournamentDetailSignal.set(null);
        }

        if (!this.selectedTournamentIdSignal() && items.length > 0) {
          this.selectedTournamentIdSignal.set(items[0].id);
        }
      }),
      switchMap(() => {
        const selected = this.selectedTournamentIdSignal();
        return selected ? this.selectTournament(selected) : of(undefined);
      }),
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
      tap((items) => {
        this.tournamentsSignal.set(items);
        if (items.length > 0) {
          this.selectedTournamentIdSignal.set(items[0].id);
        }
      }),
      switchMap(() => {
        const selected = this.selectedTournamentIdSignal();
        return selected ? this.selectTournament(selected) : of(undefined);
      }),
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

  selectTournament(tournamentId: string): Observable<void> {
    this.loadingDetail.set(true);
    this.error.set(null);
    this.selectedTournamentIdSignal.set(tournamentId);

    return this.tournamentService.getDetail(tournamentId).pipe(
      tap((detail) => this.selectedTournamentDetailSignal.set(detail)),
      map(() => undefined),
      tap({ error: (error) => this.error.set(this.toErrorMessage(error)) }),
      finalize(() => this.loadingDetail.set(false)),
    );
  }

  joinSelectedTournament(): Observable<void> {
    const tournamentId = this.selectedTournamentIdSignal();
    if (!tournamentId) {
      return of(undefined);
    }

    this.actionLoading.set(true);
    this.error.set(null);
    this.successMessage.set(null);

    return this.tournamentService.join(tournamentId).pipe(
      tap((result) => {
        this.successMessage.set(result.message ?? 'Joined tournament.');
      }),
      switchMap(() => this.selectTournament(tournamentId)),
      switchMap(() => this.tournamentService.getAll()),
      tap((items) => this.tournamentsSignal.set(items)),
      map(() => undefined),
      tap({ error: (error) => this.error.set(this.toErrorMessage(error)) }),
      finalize(() => this.actionLoading.set(false)),
    );
  }

  addStaffToSelectedTournament(userId: string): Observable<void> {
    const tournamentId = this.selectedTournamentIdSignal();
    if (!tournamentId) {
      return of(undefined);
    }

    this.actionLoading.set(true);
    this.error.set(null);
    this.successMessage.set(null);

    return this.tournamentService.addStaff(tournamentId, userId).pipe(
      tap((result) => {
        this.successMessage.set(result.message ?? 'Staff updated.');
      }),
      switchMap(() => this.selectTournament(tournamentId)),
      map(() => undefined),
      tap({ error: (error) => this.error.set(this.toErrorMessage(error)) }),
      finalize(() => this.actionLoading.set(false)),
    );
  }

  scoreRound(boutId: string, roundNumber: number, awardedToUserId: string): Observable<void> {
    const tournamentId = this.selectedTournamentIdSignal();
    const currentUserId = this.authStore.user()?.id;
    if (!tournamentId || !currentUserId) {
      return of(undefined);
    }

    this.actionLoading.set(true);
    this.error.set(null);
    this.successMessage.set(null);

    return this.tournamentService
      .scoreRound(boutId, roundNumber, {
        awardedToUserId,
        awardedByUserId: currentUserId,
        points: 1,
        reason: null,
      })
      .pipe(
        tap(() => this.successMessage.set('Point awarded.')),
        switchMap(() => this.selectTournament(tournamentId)),
        map(() => undefined),
        tap({ error: (error) => this.error.set(this.toErrorMessage(error)) }),
        finalize(() => this.actionLoading.set(false)),
      );
  }

  private toErrorMessage(error: unknown): string {
    if (error instanceof HttpErrorResponse) {
      if (Array.isArray(error.error?.errors)) {
        const first = error.error.errors.find((entry: unknown) => typeof entry === 'string');
        if (typeof first === 'string') {
          return first;
        }
      }

      if (typeof error.error?.message === 'string') {
        return error.error.message;
      }

      if (typeof error.error?.Message === 'string') {
        return error.error.Message;
      }

      if (error.status === 403) {
        return 'You do not have permission for this action.';
      }

      return `Request failed (${error.status || 'unknown'}).`;
    }

    return 'Something went wrong. Please try again.';
  }
}
