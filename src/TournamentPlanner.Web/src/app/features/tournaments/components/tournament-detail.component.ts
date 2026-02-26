import { ChangeDetectionStrategy, Component, computed, input, output } from '@angular/core';
import { DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthUserLookup } from '../../../core/auth/auth.models';
import { TournamentDetailResponse } from '../tournaments.models';

@Component({
  selector: 'app-tournament-detail',
  standalone: true,
  imports: [DatePipe, FormsModule],
  templateUrl: './tournament-detail.component.html',
  styleUrl: './tournament-detail.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TournamentDetailComponent {
  readonly detail = input<TournamentDetailResponse | null>(null);
  readonly loading = input(false);
  readonly actionLoading = input(false);
  readonly users = input<AuthUserLookup[]>([]);

  readonly join = output<void>();
  readonly addStaff = output<string>();
  readonly scoreRound = output<{ boutId: string; roundNumber: number; awardedToUserId: string }>();

  selectedStaffUserId: string | null = null;

  readonly userNameById = computed(() => {
    const map = new Map<string, string>();
    for (const participant of this.detail()?.participants ?? []) {
      map.set(participant.userId, participant.displayName);
    }
    for (const staff of this.detail()?.staff ?? []) {
      if (!map.has(staff.userId)) {
        map.set(staff.userId, staff.displayName);
      }
    }
    for (const organizer of this.detail()?.organizers ?? []) {
      if (!map.has(organizer.userId)) {
        map.set(organizer.userId, organizer.displayName);
      }
    }
    return map;
  });

  staffCandidates(): AuthUserLookup[] {
    const detail = this.detail();
    if (!detail) {
      return [];
    }

    const blocked = new Set([
      ...detail.organizers.map((m) => m.userId),
      ...detail.staff.map((m) => m.userId),
    ]);

    return this.users().filter((u) => !blocked.has(u.id));
  }

  onAddStaff(): void {
    if (!this.selectedStaffUserId) {
      return;
    }

    this.addStaff.emit(this.selectedStaffUserId);
    this.selectedStaffUserId = null;
  }

  resolveName(userId: string): string {
    return this.userNameById().get(userId) ?? userId;
  }
}
