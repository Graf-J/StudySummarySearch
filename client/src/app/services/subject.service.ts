import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SubjectService {

  constructor(private http: HttpClient) { }

  get(semester?: number): Observable<string[]> {
    let requestURL: string = `${ environment.serverURL }/api/Subject`;
    if (semester !== undefined) requestURL += `?semester=${ semester }`;
    return this.http.get<string[]>(requestURL);
  }
}
