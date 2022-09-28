import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class NameService {

  constructor(private http: HttpClient) { }

  get(semester: number, subject: string): Observable<string[]> {
    return this.http.get<string[]>(`${ environment.serverURL }/api/Name?semester=${ semester }&subject=${ subject }`);
  }
}
