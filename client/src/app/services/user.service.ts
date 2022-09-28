import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { User } from '../models/User';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(private http: HttpClient) { }

  get(): Observable<User[]> {
    return this.http.get<User[]>(`${ environment.serverURL }/api/User`);
  }

  delete(userId: number): Observable<any> {
    return this.http.delete(`${ environment.serverURL }/api/User/${ userId }`);
  }
}
