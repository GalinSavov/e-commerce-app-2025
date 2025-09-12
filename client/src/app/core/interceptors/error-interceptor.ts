import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { SnackbarService } from '../services/snackbar.service';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const snackbar = inject(SnackbarService);
  return next(req).pipe(
    catchError((error:HttpErrorResponse) => {
      switch (error.status) {
        case 400:
          snackbar.error(error.error.title);
          break;
        case 401:
          snackbar.error(error.error.title);
          break;
        case 404:
          router.navigateByUrl('/not-found'); 
          break;
        case 500:
          router.navigateByUrl('/server-error');  
          break;
        default:
          break;
      }
      return throwError(() => error);
    }) 
  )
};
