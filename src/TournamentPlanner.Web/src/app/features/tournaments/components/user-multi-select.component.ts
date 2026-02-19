import { ChangeDetectionStrategy, Component, computed, input, output, signal } from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import {
  MatAutocompleteModule,
  MatAutocompleteSelectedEvent,
} from '@angular/material/autocomplete';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatChipsModule } from '@angular/material/chips';
import { MatInputModule } from '@angular/material/input';
import { AuthUserLookup } from '../../../core/auth/auth.models';

@Component({
  selector: 'app-user-multi-select',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatAutocompleteModule,
    MatFormFieldModule,
    MatChipsModule,
    MatInputModule,
  ],
  templateUrl: './user-multi-select.component.html',
  styleUrl: './user-multi-select.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class UserMultiSelectComponent {
  readonly label = input.required<string>();
  readonly placeholder = input('Search user…');
  readonly users = input<AuthUserLookup[]>([]);
  readonly selectedIds = input<string[]>([]);
  readonly required = input(false);
  readonly loading = input(false);

  readonly selectedIdsChange = output<string[]>();

  readonly searchControl = new FormControl('', { nonNullable: true });
  private readonly searchTermSignal = signal('');

  readonly selectedUsers = computed(() => {
    const selected = new Set(this.selectedIds());
    return this.users().filter((user) => selected.has(user.id));
  });

  readonly filteredUsers = computed(() => {
    const selected = new Set(this.selectedIds());
    const searchTerm = this.searchTermSignal().trim().toLowerCase();

    return this.users().filter((user) => {
      if (selected.has(user.id)) {
        return false;
      }

      if (!searchTerm) {
        return true;
      }

      const fullName = `${user.firstName} ${user.lastName}`.toLowerCase();
      const nickname = (user.nickname ?? '').toLowerCase();

      return (
        fullName.includes(searchTerm) ||
        user.email.toLowerCase().includes(searchTerm) ||
        nickname.includes(searchTerm)
      );
    });
  });

  onSearchChange(): void {
    this.searchTermSignal.set(this.searchControl.value);
  }

  onOptionSelected(event: MatAutocompleteSelectedEvent): void {
    const userId = event.option.value as string;
    const next = [...this.selectedIds(), userId];
    this.selectedIdsChange.emit(next);
    this.searchControl.setValue('');
    this.searchTermSignal.set('');
  }

  remove(userId: string): void {
    this.selectedIdsChange.emit(this.selectedIds().filter((id) => id !== userId));
  }

  displayUser(user: AuthUserLookup): string {
    const fullName = `${user.firstName} ${user.lastName}`.trim();
    return fullName ? `${fullName} (${user.email})` : user.email;
  }
}
