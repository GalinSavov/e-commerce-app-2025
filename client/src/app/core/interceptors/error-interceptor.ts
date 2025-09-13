import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { NavigationExtras, Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { SnackbarService } from '../services/snackbar.service';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const snackbar = inject(SnackbarService);
  return next(req).pipe(
    catchError((error:HttpErrorResponse) => {
      switch (error.status) {
        case 400:
          if(error.error.errors){
            const modelStateErrors = [];
            for(const key in error.error.errors){
              if(error.error.errors[key]){
                modelStateErrors.push(error.error.errors[key]);
              }
            }
            throw modelStateErrors.flat();
          }
          else{
            snackbar.error(error.error.title);
          }
          break;
        case 401:
          snackbar.error(error.error.title);
          break;
        case 404:
          router.navigateByUrl('/not-found'); 
          break;
        case 500:
          const navigationExtras:NavigationExtras = {state:{error:error.error}}
          router.navigateByUrl('/server-error',navigationExtras);  
          break;
        default:
          break;
      }
      return throwError(() => error);
    }) 
  )
};
