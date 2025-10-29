import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { ShoppingCart } from '../../shared/models/shoppingCart';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class CartService {
  apiURL = environment.apiURL;
  shoppingCart = signal<ShoppingCart | null>(null)
  private http = inject(HttpClient);
  getCart(id:number){
    return this.http.get<ShoppingCart>(this.apiURL + 'cart?id='+ id).subscribe({
      next: response => this.shoppingCart.set(response)
    })
  }
  deleteCart(id:number){
    return this.http.delete(this.apiURL+ 'cart?id=' + id)
  }
  setCart(shoppingCart:ShoppingCart){
    return this.http.post<ShoppingCart>(this.apiURL+'cart',shoppingCart).subscribe({
      next:response => this.shoppingCart.set(response)
    })
  }
}
