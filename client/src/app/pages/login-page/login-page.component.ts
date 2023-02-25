import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-login-page',
  templateUrl: './login-page.component.html',
  styleUrls: ['./login-page.component.css']
})
export class LoginPageComponent implements OnInit {

  isLoading: boolean = false;
  loginError: boolean = false;
  loginErrorMsg: string = '';

  loginForm = new FormGroup({
    name: new FormControl('', [Validators.required]),
    password: new FormControl('', [Validators.required])
  });

  constructor(private router: Router, private auth: AuthService) { }

  ngOnInit(): void {
  }

  onSubmit(): void {
    if (this.loginForm.valid) {
      this.isLoading = true;
      
      this.auth.login(this.loginForm.value.name!, this.loginForm.value.password!).subscribe({
        next: res => {
          this.isLoading = false;
          this.auth.isLoggedIn = true;
          localStorage.setItem('token', res.jwt);
          this.router.navigate(['']);
        },
        error: err => {
          this.loginError = true;
          this.loginErrorMsg = err.error;
          this.isLoading = false;
        }
      });
    }
  }
}
