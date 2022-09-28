import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Summary } from '../models/Summary';
import { Observable, map, catchError, of, throwError } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SummaryService {

  constructor(private http: HttpClient) { }

  get(semester?: number, subject?: string, keyword?: string, name?: string, authorId?: number): Observable<Summary[]> {
    let url: string = `${ environment.serverURL }/api/Summary`

    let first: boolean = true;
    if (semester) {
      url += `${ first ? '?' : '&' }semester=${ semester }`;
      first = false;
    }
    if (subject) {
      url += `${ first ? '?' : '&' }subject=${ subject }`;
      first = false;
    }  
    if (keyword) {
      url += `${ first ? '?' : '&' }keyword=${ keyword }`;
      first = false;
    }  
    if (name) {
      url += `${ first ? '?' : '&' }name=${ name }`;
      first = false;
    } 
    if (authorId) {
      url += `${ first ? '?' : '&' }authorId=${ authorId }`;
      first = false;
    }  

    return this.http.get<Summary[]>(url)
      .pipe(
        map((summaries: Summary[]) => {
          return summaries.map((summary: Summary) => {
            summary.isImageLoading = true;
            return summary
          })
        })
      )
  }

  add(summary: Summary): Observable<number> {
    return this.http.post<number>(`${ environment.serverURL }/api/Summary`, summary);
  }

  update(id: number, summary: Summary): Observable<Summary> {
    return this.http.put<Summary>(`${ environment.serverURL }/api/Summary/${ id }`, summary);
  }

  delete(id: number): Observable<any> {
    return this.http.delete(`${ environment.serverURL }/api/Summary/${ id }`);
  }

  upload(id: number, formData: FormData): Observable<Summary> {
    return this.http.post<Summary>(`${ environment.serverURL }/api/Summary/upload/${ id }`, formData);
  }

  deleteImage(id: number): Observable<any> {
    return this.http.delete(`${ environment.serverURL }/api/Summary/image/${ id }`);
  }
}
