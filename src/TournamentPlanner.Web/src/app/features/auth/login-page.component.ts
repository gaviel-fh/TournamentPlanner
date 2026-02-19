import { ChangeDetectionStrategy, Component, DestroyRef, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { AuthStore } from '../../core/auth/auth.store';

@Component({
  selector: 'app-login-page',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './login-page.component.html',
  styleUrl: './login-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LoginPageComponent {
  private readonly destroyRef = inject(DestroyRef);
  protected readonly authStore = inject(AuthStore);

  protected readonly submitted = signal(false);

  protected readonly emailControl = new FormControl('', {
    nonNullable: true,
    validators: [Validators.required, Validators.email],
  });

  protected readonly passwordControl = new FormControl('', {
    nonNullable: true,
    validators: [Validators.required, Validators.minLength(8)],
  });

  protected readonly form = new FormGroup({
    email: this.emailControl,
    password: this.passwordControl,
  });

  protected onSubmit(): void {
    this.submitted.set(true);

    if (this.emailControl.invalid || this.passwordControl.invalid) {
      this.emailControl.markAsTouched();
      this.passwordControl.markAsTouched();
      return;
    }

    this.authStore
      .login({
        email: this.emailControl.getRawValue().trim(),
        password: this.passwordControl.getRawValue(),
      })
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        error: () => undefined,
      });
  }

  protected shouldShowEmailError(): boolean {
    return this.emailControl.invalid && (this.emailControl.touched || this.submitted());
  }

  protected shouldShowPasswordError(): boolean {
    return this.passwordControl.invalid && (this.passwordControl.touched || this.submitted());
  }
}
