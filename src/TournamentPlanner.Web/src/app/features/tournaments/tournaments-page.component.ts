import { ChangeDetectionStrategy, Component, DestroyRef, computed, inject } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { TournamentCreateFormComponent } from './components/tournament-create-form.component';
import { TournamentDetailComponent } from './components/tournament-detail.component';
import { TournamentListComponent } from './components/tournament-list.component';
import { TournamentsStore } from './tournaments.store';
import { TournamentCreateFormValue } from './tournaments.models';

@Component({
  selector: 'app-tournaments-page',
  standalone: true,
  imports: [TournamentCreateFormComponent, TournamentListComponent, TournamentDetailComponent],
  templateUrl: './tournaments-page.component.html',
  styleUrl: './tournaments-page.component.scss',
  providers: [TournamentsStore],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TournamentsPageComponent {
  private readonly destroyRef = inject(DestroyRef);
  readonly store = inject(TournamentsStore);

  readonly displayName = computed(() => {
    const user = this.store.authStore.user();
    if (!user) {
      return 'Tournament planner user';
    }

    const fullName = [user.firstName, user.lastName]
      .filter((name): name is string => Boolean(name))
      .join(' ');

    return fullName || user.email || 'Tournament planner user';
  });

  constructor() {
    this.store
      .loadUsers()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        error: () => undefined,
      });

    this.store
      .loadTournaments()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        error: () => undefined,
      });
  }

  onCreateTournament(value: TournamentCreateFormValue): void {
    this.store
      .createTournament(value)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        error: () => undefined,
      });
  }

  onReload(): void {
    this.store
      .loadUsers()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        error: () => undefined,
      });

    this.store
      .loadTournaments()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        error: () => undefined,
      });
  }

  onSelectTournament(tournamentId: string): void {
    this.store
      .selectTournament(tournamentId)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({ error: () => undefined });
  }

  onJoinTournament(): void {
    this.store
      .joinSelectedTournament()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({ error: () => undefined });
  }

  onAddStaff(userId: string): void {
    this.store
      .addStaffToSelectedTournament(userId)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({ error: () => undefined });
  }

  onScoreRound(event: { boutId: string; roundNumber: number; awardedToUserId: string }): void {
    this.store
      .scoreRound(event.boutId, event.roundNumber, event.awardedToUserId)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({ error: () => undefined });
  }

  onLogout(): void {
    this.store.authStore
      .logout()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        error: () => undefined,
      });
  }
}
