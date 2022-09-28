import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import jwt_decode from 'jwt-decode';
import { User } from '../models/User';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private _isLoggedIn: boolean = false;

  get isLoggedIn(): boolean {
    if (this._isLoggedIn) return true;
    const token: string | null = sessionStorage.getItem('token');
    if (token) {
      return !this.isTokenExpired(token);
    }
    return false;
  }
  set isLoggedIn(value: boolean) {
    this._isLoggedIn = value;
  }

  constructor(private http: HttpClient) { }

  login(name: string, password: string): Observable<any> {
    return this.http.post(`${ environment.serverURL }/api/Auth/login`, { userName: name, password: password });
  }

  register(userName: string, password: string, comparePassword: string): Observable<User> {
    return this.http.post<User>(`${ environment.serverURL }/api/Auth/register`, { userName, password, comparePassword });
  }

  setDropboxToken(token: string, userId?: number): Observable<any> {
    return this.http.post(`${ environment.serverURL }/api/Auth/dropbox-token${ userId ? `/${ userId }` : '' }`, { token });
  }

  getTokenUserId(jwt: string): string {
    const decodedToken: any = jwt_decode(jwt);
    return decodedToken['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'];
  }

  getTokenAuthRole(jwt: string): string {
    const decodedToken: any = jwt_decode(jwt);
    return decodedToken['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
  }

  isTokenExpired(jwt: string) {
    const decodedToken: any = jwt_decode(jwt);
    return (decodedToken.exp * 1000) < Date.now();
  }
}
