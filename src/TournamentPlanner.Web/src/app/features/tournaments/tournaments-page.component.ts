import { ChangeDetectionStrategy, Component, DestroyRef, computed, inject } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { TournamentCreateFormComponent } from './components/tournament-create-form.component';
import { TournamentListComponent } from './components/tournament-list.component';
import { TournamentsStore } from './tournaments.store';
import { TournamentCreateFormValue } from './tournaments.models';

@Component({
  selector: 'app-tournaments-page',
  standalone: true,
  imports: [TournamentCreateFormComponent, TournamentListComponent],
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

  onLogout(): void {
    this.store.authStore
      .logout()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        error: () => undefined,
      });
  }
}
