import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';
import { DatePipe } from '@angular/common';
import { TournamentSummaryResponse } from '../tournaments.models';

@Component({
  selector: 'app-tournament-list',
  standalone: true,
  imports: [DatePipe],
  templateUrl: './tournament-list.component.html',
  styleUrl: './tournament-list.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TournamentListComponent {
  readonly tournaments = input<TournamentSummaryResponse[]>([]);
  readonly loading = input(false);
  readonly selectedTournamentId = input<string | null>(null);
  readonly selectTournament = output<string>();
}
