import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { createNewCart, ShoppingCart } from '../../shared/models/shoppingCart';
import { HttpClient } from '@angular/common/http';
import { CartItem } from '../../shared/models/cartItem';
import { Product } from '../../shared/models/product';

@Injectable({
  providedIn: 'root'
})
export class CartService {
  apiURL = environment.apiURL;
  shoppingCart = signal<ShoppingCart | null>(null)
  private http = inject(HttpClient);
  getCart(id:string){
    return this.http.get<ShoppingCart>(this.apiURL + 'cart?id='+ id).subscribe({
      next: response => this.shoppingCart.set(response)
    })
  }
  deleteCart(id:string){
    return this.http.delete(this.apiURL+ 'cart?id=' + id)
  }
  setCart(shoppingCart:ShoppingCart){
    return this.http.post<ShoppingCart>(this.apiURL+'cart',shoppingCart).subscribe({
      next:response => this.shoppingCart.set(response)
    })
  }
  addItemToCart(item:CartItem | Product, quantity = 1){
    const cart = this.shoppingCart() ?? this.createCart();
    if(this.isProduct(item)){
      item = this.mapProductToCartItem(item);
    }
    cart.items = this.addOrUpdateItem(cart.items,item,quantity);
    this.shoppingCart.set(cart);
  }
  private addOrUpdateItem(items: CartItem[], item: CartItem, quantity: number): CartItem[] {
    const index = items.findIndex(x=>x.productId === item.productId)
    if(index === -1){
      item.quantity = quantity;
      items.push(item);
    }
    else{
      items[index].quantity += quantity;
    }
    return items;
  }
  private mapProductToCartItem(product: Product): CartItem{
    return{
      productId: product.id,
      productName:product.name,
      price:product.price,
      quantity:0,
      pictureURL:product.pictureUrl,
      brand:product.brand,
      type:product.type
    }
  }
  //type guard
  private isProduct(item:CartItem | Product):item is Product{
    return (item as Product).id !== undefined;
  }
  private createCart():ShoppingCart{
    const cart = createNewCart();
    localStorage.setItem('cart_id',cart.id);
    return cart;
  }
}
