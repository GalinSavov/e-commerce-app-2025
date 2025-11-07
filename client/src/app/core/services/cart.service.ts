import { computed, inject, Injectable, signal } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { createNewCart, ShoppingCart } from '../../shared/models/shoppingCart';
import { HttpClient } from '@angular/common/http';
import { CartItem } from '../../shared/models/cartItem';
import { Product } from '../../shared/models/product';
import { map } from 'rxjs';
import { DeliveryMethod } from '../../shared/models/deliveryMethod';

@Injectable({
  providedIn: 'root'
})
export class CartService {
  apiURL = environment.apiURL;
  shoppingCart = signal<ShoppingCart | null>(null);
  deliveryMethod = signal<DeliveryMethod | null>(null);
  itemCount = computed(() => {
    return this.shoppingCart()?.items.reduce((sum,item) => sum + item.quantity,0)
  });
  totals = computed(() => {
    const cart = this.shoppingCart();
    const deliveryMethod = this.deliveryMethod();
    if(!cart) return null;
    const subTotal = cart.items.reduce((sum,item) => sum + (item.price * item.quantity),0);
    const shipping = deliveryMethod ? deliveryMethod.price : 0;
    const discount = 0;
    return {
      subTotal,
      shipping,
      discount,
      total: (subTotal + shipping) - discount
    }
  })
  private http = inject(HttpClient);
  getCart(id:string){
    return this.http.get<ShoppingCart>(this.apiURL + 'cart?id='+ id).pipe(
      map(cart => {
        this.shoppingCart.set(cart);
        return cart;
      })
    )
  }
  deleteCart(id:string){
    return this.http.delete(this.apiURL+ 'cart?id=' + id).subscribe({
      next: () =>{
        localStorage.removeItem('cart_id');
        this.shoppingCart.set(null);
      }
    })
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
    this.setCart(cart);
  }
  removeItemFromCart(item:CartItem,quantity = 1){
    const cart = this.shoppingCart();
    if(!cart) return;
    const index = cart.items.findIndex(x=>x.productId === item.productId);
    if(index !== -1){
      if(cart.items[index].quantity > quantity){
        cart.items[index].quantity -= quantity;
      }
      else{
        cart.items.splice(index,1);
      }
      if(cart.items.length === 0) {
        this.deleteCart(cart.id);
      }
      else{
        this.setCart(cart);
      }
    }
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
