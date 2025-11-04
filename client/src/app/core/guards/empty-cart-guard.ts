import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { CartService } from '../services/cart.service';
import { SnackbarService } from '../services/snackbar.service';

export const emptyCartGuard: CanActivateFn = (route, state) => {
  const cartService = inject(CartService);
  const snackbarService = inject(SnackbarService);
  const router = inject(Router);

  if(!cartService.shoppingCart() || cartService.shoppingCart()?.items.length === 0){
    snackbarService.error("Shopping Cart is empty!");
    router.navigateByUrl('/cart');
    return false;
  }
  else{
    return true;
  }
};
