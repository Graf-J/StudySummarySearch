import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class KeywordService {

  constructor(private http: HttpClient) { }

  get(keywordStart?: string): Observable<string[]> {
    let requestURL: string = `${ environment.serverURL }/api/Keyword`;
    if (keywordStart !== undefined) requestURL += `?keywordStart=${ keywordStart }`;
    return this.http.get<string[]>(requestURL);
  }
}
