import { HttpErrorResponse } from '@angular/common/http';
import { Component, EventEmitter, OnInit } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, FormGroupDirective, Validators } from '@angular/forms';
import { User } from 'src/app/models/User';
import { AuthService } from 'src/app/services/auth.service';
import { UserService } from 'src/app/services/user.service';

function ComparePasswordValidator(control: AbstractControl) {
  const passwordControl = control.get('password');
  const comparePasswordControl = control.get('comparePassword');
  if (passwordControl!.value !== comparePasswordControl!.value) {
    comparePasswordControl!.setErrors({ notEquivalentPasswords: true })
    return { notEquivalentPasswords: true };
  }
  return null;
}

@Component({
  selector: 'app-admin-users-page',
  templateUrl: './admin-users-page.component.html',
  styleUrls: ['./admin-users-page.component.css']
})
export class AdminUsersPageComponent implements OnInit {

  users: User[] = [];
  isUsersLoading: boolean = true;
  selectedUserId: number = 0;
  isUserDeleteLoading: number = 0;

  isDropboxTokenLoading: boolean = false;

  isRegisterLoading: boolean = false;
  registerError?: string;

  dropboxTokenForm = new FormGroup({
    dropboxToken: new FormControl('', Validators.required)
  });

  registerForm = new FormGroup({
    name: new FormControl('', [Validators.required]),
    password: new FormControl('', [Validators.required]),
    comparePassword: new FormControl('', [Validators.required])
  }, ComparePasswordValidator);

  constructor(private authService: AuthService, private userService: UserService) { }

  ngOnInit(): void {
    this.userService.get().subscribe((users: User[]) => {
      this.users = users;
      this.isUsersLoading = false;
    });
  }

  onSetDropboxToken(form: FormGroupDirective): void {
    if (this.dropboxTokenForm.valid) {
      this.isDropboxTokenLoading = true;
      this.authService.setDropboxToken(this.dropboxTokenForm.value.dropboxToken!, this.selectedUserId === 0 ? undefined : this.selectedUserId).subscribe(() => {
        this.dropboxTokenForm.reset();
        form.resetForm();
        this.isDropboxTokenLoading = false;
      })
    }
  }

  onUserClick(userId: number) {
    this.selectedUserId = userId;
  }

  onUserDelete(event: MouseEvent, userId: number) {
    event.stopPropagation();
    this.isUserDeleteLoading = userId;
    this.userService.delete(userId).subscribe(() => {
      this.users = this.users.filter((user: User) => user.id !== userId);
      this.isUserDeleteLoading = 0;
    });
  }

  onRegister(form: FormGroupDirective): void {
    if (this.registerForm.valid) {
      this.isRegisterLoading = true;
      this.authService.register(this.registerForm.value.name!, this.registerForm.value.password!, this.registerForm.value.comparePassword!).subscribe({
        next: (user: User) => {
          this.registerError = undefined;
          this.registerForm.reset();
          this.users.unshift(user);
          form.resetForm();
          this.isRegisterLoading = false;
        },
        error: (error: HttpErrorResponse) => {
          console.log(error);
          this.registerError = error.error;
        }
      })
    }
  }

}
