import { ChangeDetectionStrategy, Component, DestroyRef, computed, inject } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { AuthStore } from '../../core/auth/auth.store';

@Component({
  selector: 'app-home-page',
  standalone: true,
  templateUrl: './home-page.component.html',
  styleUrl: './home-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HomePageComponent {
  private readonly destroyRef = inject(DestroyRef);
  protected readonly authStore = inject(AuthStore);

  protected readonly displayName = computed(() => {
    const user = this.authStore.user();
    if (!user) {
      return 'Tournament planner user';
    }

    const fullName = [user.firstName, user.lastName]
      .filter((name): name is string => Boolean(name))
      .join(' ');
    return fullName || user.email || 'Tournament planner user';
  });

  protected onLogout(): void {
    this.authStore
      .logout()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        error: () => undefined,
      });
  }
}
