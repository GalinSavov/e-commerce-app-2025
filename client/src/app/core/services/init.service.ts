import { inject, Injectable } from '@angular/core';
import { CartService } from './cart.service';
import { forkJoin, of } from 'rxjs';
import { AccountService } from './account.service';

@Injectable({
  providedIn: 'root'
})
export class InitService {
  private cartService = inject(CartService);
  private accountService = inject(AccountService);
  init(){
    const cartId = localStorage.getItem('cart_id');
    const cart$ = cartId ? this.cartService.getCart(cartId) : of(null);
    const hasAuthCookie = document.cookie.includes('.AspNetCore.Identity.Application');
    const user$ = hasAuthCookie ? this.accountService.getUserInfo() : of(null);
    return forkJoin({
      cart:cart$,
      user:user$
    })
  }
}
