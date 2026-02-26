export interface TournamentSummaryResponse {
  id: string;
  name: string;
  startDateUtc: string;
  endDateUtc: string;
  signupStartDateUtc: string;
  signupEndDateUtc: string;
  venue: string;
  latitude: number | null;
  longitude: number | null;
  disciplineCount: number;
  participantCount: number;
  status: string;
}

export interface TournamentMemberResponse {
  userId: string;
  displayName: string;
  email: string;
  nickname: string | null;
  role: string;
}

export interface TournamentRoundResponse {
  roundNumber: number;
  participantAScore: number;
  participantBScore: number;
}

export interface TournamentBoutResponse {
  id: string;
  scheduledStartUtc: string;
  status: string;
  participantAUserId: string;
  participantBUserId: string;
  participantATotalScore: number;
  participantBTotalScore: number;
  winnerUserId: string | null;
  rounds: TournamentRoundResponse[];
}

export interface TournamentDisciplineDetailResponse {
  tournamentDisciplineId: string;
  disciplineId: string;
  code: string;
  name: string;
  roundCount: number;
  boutIntervalMinutes: number;
  bouts: TournamentBoutResponse[];
}

export interface TournamentDetailResponse {
  id: string;
  name: string;
  startDateUtc: string;
  endDateUtc: string;
  signupStartDateUtc: string;
  signupEndDateUtc: string;
  venue: string;
  latitude: number | null;
  longitude: number | null;
  status: string;
  currentUserRole: string | null;
  canJoin: boolean;
  canManageStaff: boolean;
  canScore: boolean;
  organizers: TournamentMemberResponse[];
  staff: TournamentMemberResponse[];
  participants: TournamentMemberResponse[];
  disciplines: TournamentDisciplineDetailResponse[];
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
  signupStartDateUtc: string;
  signupEndDateUtc: string;
  venueName: string;
  latitude: number | null;
  longitude: number | null;
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
  signupStartDateUtc: string;
  signupEndDateUtc: string;
  latitude: number | null;
  longitude: number | null;
  organizerUserIds: string[];
  staffUserIds: string[];
  disciplines: TournamentDisciplineFormValue[];
}

export interface ScoreRoundRequest {
  awardedToUserId: string;
  awardedByUserId: string;
  points: number;
  reason: string | null;
}
