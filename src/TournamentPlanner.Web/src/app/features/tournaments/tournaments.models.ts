export interface TournamentSummaryResponse {
  id: string;
  name: string;
  startDateUtc: string;
  endDateUtc: string;
  venue: string;
  disciplineCount: number;
  participantCount: number;
  status: string;
}

export interface CreateTournamentDisciplineRequest {
  code: string;
  name: string;
  roundCount: number;
  boutIntervalMinutes: number;
}

export interface CreateTournamentRequest {
  name: string;
  startDateUtc: string;
  endDateUtc: string;
  venueName: string;
  organizerUserIds: string[];
  staffUserIds: string[];
  participantUserIds: string[];
  disciplines: CreateTournamentDisciplineRequest[];
}

export interface CreateTournamentResponse {
  id: string;
}

export interface TournamentDisciplineFormValue {
  code: string;
  name: string;
  roundCount: number;
  boutIntervalMinutes: number;
}

export interface TournamentCreateFormValue {
  name: string;
  venueName: string;
  startDateUtc: string;
  endDateUtc: string;
  organizerUserIds: string[];
  staffUserIds: string[];
  disciplines: TournamentDisciplineFormValue[];
}
