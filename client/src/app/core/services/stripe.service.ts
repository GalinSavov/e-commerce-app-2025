import { inject, Injectable } from '@angular/core';
import {loadStripe, Stripe, StripeAddressElement, StripeAddressElementOptions, StripeElements} from '@stripe/stripe-js'
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { CartService } from './cart.service';
import { ShoppingCart } from '../../shared/models/shoppingCart';
import { firstValueFrom, map, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class StripeService {
  baseUrl = environment.apiURL;
  private cartService = inject(CartService);
  private stripePromise: Promise<Stripe | null>;
  private elements?: StripeElements;
  private addressElement?:StripeAddressElement;
  private http = inject(HttpClient);
  constructor(){
    this.stripePromise = loadStripe(environment.stripePublicKey);
  }
  getStripeInstance():Promise<Stripe | null>{
    return this.stripePromise;
  }
  async initializeElements(){
    if(!this.elements){
      const stripe = await this.getStripeInstance();
      if(stripe){
        const cart = await firstValueFrom(this.createOrUpdatePaymentIntent());
        this.elements = stripe.elements({clientSecret:cart.clientSecret,appearance:{labels:'floating'}})
      }else{
        throw new Error('Stripe has not been loaded');
      }
    }
    return this.elements;
  }
  async createAddressElement(){
    if(!this.addressElement){
      const elements = await this.initializeElements();
      if(elements){
        const options: StripeAddressElementOptions = {
          mode:'shipping'
        };
        this.addressElement = elements.create('address',options);
      }else{
        throw new Error('Elements instance has not been loaded');
      }
    }
    return this.addressElement;
  }
  createOrUpdatePaymentIntent():Observable<ShoppingCart>{
    const cart = this.cartService.shoppingCart();
    if(!cart) throw new Error('Problem with cart!');
    return this.http.post<ShoppingCart>(this.baseUrl+'payment/' + cart.id,{}).pipe(
      map(cart => {
        this.cartService.shoppingCart.set(cart);
        return cart;
      })
    );
  }
}
