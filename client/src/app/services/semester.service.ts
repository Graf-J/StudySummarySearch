import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class SemesterService {

  constructor(private http: HttpClient) { }

  get(): Observable<number[]> {
    return this.http.get<number[]>(`${ environment.serverURL }/api/Semester`);
  }
}
