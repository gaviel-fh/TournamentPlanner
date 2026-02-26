import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';
import {
  AbstractControl,
  FormArray,
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  ValidationErrors,
  Validators,
} from '@angular/forms';
import { MatCheckboxChange, MatCheckboxModule } from '@angular/material/checkbox';
import { MatStepperModule } from '@angular/material/stepper';
import { TournamentCreateFormValue } from '../tournaments.models';
import { AuthUserLookup } from '../../../core/auth/auth.models';

const dateOrderValidator = (control: AbstractControl): ValidationErrors | null => {
  const start = control.get('startDateUtc')?.value as string | undefined;
  const end = control.get('endDateUtc')?.value as string | undefined;
  const signupStart = control.get('signupStartDateUtc')?.value as string | undefined;
  const signupEnd = control.get('signupEndDateUtc')?.value as string | undefined;

  if (!start || !end) {
    return null;
  }

  if (new Date(end) < new Date(start)) {
    return { invalidDateOrder: true };
  }

  if (signupStart && signupEnd && new Date(signupEnd) < new Date(signupStart)) {
    return { invalidSignupOrder: true };
  }

  if (signupStart && signupEnd) {
    if (new Date(signupStart) < new Date(start) || new Date(signupEnd) > new Date(end)) {
      return { invalidSignupWindow: true };
    }
  }

  return null;
};

@Component({
  selector: 'app-tournament-create-form',
  standalone: true,
  imports: [ReactiveFormsModule, MatStepperModule, MatCheckboxModule],
  templateUrl: './tournament-create-form.component.html',
  styleUrl: './tournament-create-form.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TournamentCreateFormComponent {
  readonly loading = input(false);
  readonly users = input<AuthUserLookup[]>([]);
  readonly usersLoading = input(false);

  readonly submitTournament = output<TournamentCreateFormValue>();
  readonly displayedUserColumns = ['select', 'name', 'email', 'nickname'];

  private readonly fb = new FormBuilder();

  readonly form = this.fb.group({
    basicInfo: this.fb.group(
      {
        name: this.fb.nonNullable.control('', [Validators.required]),
        venueName: this.fb.nonNullable.control('', [Validators.required]),
        startDateUtc: this.fb.nonNullable.control(this.defaultLocalDateTime(), [
          Validators.required,
        ]),
        endDateUtc: this.fb.nonNullable.control(this.defaultLocalDateTime(1), [
          Validators.required,
        ]),
        signupStartDateUtc: this.fb.nonNullable.control(this.defaultLocalDateTime(), [
          Validators.required,
        ]),
        signupEndDateUtc: this.fb.nonNullable.control(this.defaultLocalDateTime(1), [
          Validators.required,
        ]),
        latitude: this.fb.control<number | null>(null),
        longitude: this.fb.control<number | null>(null),
      },
      {
        validators: [dateOrderValidator],
      },
    ),
    organizers: this.fb.group({
      organizerUserIds: this.fb.nonNullable.control<string[]>([], [Validators.required]),
    }),
    staff: this.fb.group({
      staffUserIds: this.fb.nonNullable.control<string[]>([]),
    }),
    boutInfo: this.fb.group({
      disciplines: this.fb.array([this.createDisciplineGroup()]),
    }),
  });

  get basicInfoGroup(): FormGroup {
    return this.form.controls.basicInfo;
  }

  get organizersGroup(): FormGroup {
    return this.form.controls.organizers;
  }

  get staffGroup(): FormGroup {
    return this.form.controls.staff;
  }

  get boutInfoGroup(): FormGroup {
    return this.form.controls.boutInfo;
  }

  get organizerUserIds(): string[] {
    return this.form.controls.organizers.controls.organizerUserIds.value;
  }

  get staffUserIds(): string[] {
    return this.form.controls.staff.controls.staffUserIds.value;
  }

  get disciplines(): FormArray {
    return this.form.controls.boutInfo.controls.disciplines;
  }

  addDiscipline(): void {
    this.disciplines.push(this.createDisciplineGroup());
  }

  removeDiscipline(index: number): void {
    if (this.disciplines.length <= 1) {
      return;
    }

    this.disciplines.removeAt(index);
  }

  isOrganizerSelected(userId: string): boolean {
    return this.organizerUserIds.includes(userId);
  }

  isStaffSelected(userId: string): boolean {
    return this.staffUserIds.includes(userId);
  }

  onOrganizerToggle(userId: string, event: MatCheckboxChange): void {
    const next = event.checked
      ? [...this.organizerUserIds, userId]
      : this.organizerUserIds.filter((id) => id !== userId);

    this.form.controls.organizers.controls.organizerUserIds.setValue(next);
    this.form.controls.organizers.controls.organizerUserIds.markAsDirty();
    this.form.controls.organizers.controls.organizerUserIds.updateValueAndValidity();
  }

  onStaffToggle(userId: string, event: MatCheckboxChange): void {
    const next = event.checked
      ? [...this.staffUserIds, userId]
      : this.staffUserIds.filter((id) => id !== userId);

    this.form.controls.staff.controls.staffUserIds.setValue(next);
    this.form.controls.staff.controls.staffUserIds.markAsDirty();
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const raw = this.form.getRawValue();

    this.submitTournament.emit({
      name: raw.basicInfo.name,
      venueName: raw.basicInfo.venueName,
      startDateUtc: raw.basicInfo.startDateUtc,
      endDateUtc: raw.basicInfo.endDateUtc,
      signupStartDateUtc: raw.basicInfo.signupStartDateUtc,
      signupEndDateUtc: raw.basicInfo.signupEndDateUtc,
      latitude: raw.basicInfo.latitude,
      longitude: raw.basicInfo.longitude,
      organizerUserIds: raw.organizers.organizerUserIds,
      staffUserIds: raw.staff.staffUserIds,
      disciplines: raw.boutInfo.disciplines.map((discipline) => ({
        code: discipline.code,
        name: discipline.name,
        roundCount: discipline.roundCount,
        boutIntervalMinutes: discipline.boutIntervalMinutes,
      })),
    });
  }

  private createDisciplineGroup() {
    return this.fb.nonNullable.group({
      code: this.fb.nonNullable.control('', [Validators.required]),
      name: this.fb.nonNullable.control('', [Validators.required]),
      roundCount: this.fb.nonNullable.control(3, [Validators.required, Validators.min(1)]),
      boutIntervalMinutes: this.fb.nonNullable.control(30, [
        Validators.required,
        Validators.min(1),
      ]),
    });
  }

  private defaultLocalDateTime(offsetDays = 0): string {
    const value = new Date(Date.now() + offsetDays * 24 * 60 * 60 * 1000);
    value.setMinutes(0, 0, 0);

    const local = new Date(value.getTime() - value.getTimezoneOffset() * 60000);
    return local.toISOString().slice(0, 16);
  }

  displayUserName(user: AuthUserLookup): string {
    const fullName = `${user.firstName} ${user.lastName}`.trim();
    return fullName || user.email;
  }
}
