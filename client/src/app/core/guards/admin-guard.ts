import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AccountService } from '../services/account.service';
import { SnackbarService } from '../services/snackbar.service';

export const adminGuard: CanActivateFn = (route, state) => {
  const accountService = inject(AccountService);
  const router = inject(Router);
  const snackbarService = inject(SnackbarService);
  if(accountService.isAdmin()){
    return true;
  }else{
    snackbarService.error('You do not have permission to access this page.');
    router.navigateByUrl('/shop');
    return false;
  }
};
