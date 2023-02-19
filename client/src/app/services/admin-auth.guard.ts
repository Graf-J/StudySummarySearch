import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class AdminAuthGuard implements CanActivate {

  constructor(private router: Router, private auth: AuthService) { }

  canActivate(): boolean {
    const token: string | null = sessionStorage.getItem('token');
    if (token) {
      const authRole: string = this.auth.getTokenAuthRole(token);
      if (authRole === 'Admin' || authRole === 'SuperUser') return true;
      this.router.navigate(['admin']);
      return false;
    }
    this.router.navigate(['login']);
    return false;
  }
  
}
