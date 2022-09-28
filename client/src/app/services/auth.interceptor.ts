import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpEvent, HttpRequest, HttpHandler } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        const jwt: string | null = sessionStorage.getItem('token');
        if (jwt) {
            const clonedReq = req.clone({
                headers: req.headers.set('Authorization', `Bearer ${ jwt }`)
            })
            return next.handle(clonedReq);
        } else {
            return next.handle(req);
        }
    }
}