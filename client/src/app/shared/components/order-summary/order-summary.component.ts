import { Component, inject } from '@angular/core';
import { CartService } from '../../../core/services/cart.service';
import { CurrencyPipe, Location } from '@angular/common';
import { MatButton } from '@angular/material/button';
import { RouterLink } from '@angular/router';
import { MatFormField, MatLabel } from '@angular/material/form-field';
import {MatIconModule} from '@angular/material/icon';
import { MatInput } from '@angular/material/input';
import { FormsModule } from "@angular/forms";
import { A11yModule } from "@angular/cdk/a11y";
import { StripeService } from '../../../core/services/stripe.service';
import { MatIcon } from "@angular/material/icon";

@Component({
  selector: 'app-order-summary',
  imports: [CurrencyPipe, MatButton, RouterLink, MatFormField, MatLabel, MatInput, FormsModule, A11yModule, MatIconModule],
  templateUrl: './order-summary.component.html',
  styleUrl: './order-summary.component.scss'
})
export class OrderSummaryComponent {
  protected cartService = inject(CartService);
  protected stripeService = inject(StripeService);
  protected location = inject(Location);
  voucherCode:string = "";
  applyCouponCode(){
    const cart = this.cartService.shoppingCart();
    if(cart === null) return;
    this.cartService.validateCoupon(this.voucherCode).subscribe({
      next: async data =>{
        if(data){
          cart.coupon = data;
          console.log(cart.coupon);
          this.cartService.setCart(cart).subscribe({
            next: async () => {
              const stripe = await this.stripeService.getStripeInstance();
              if(stripe){
                this.stripeService.createOrUpdatePaymentIntent().subscribe({
                  error: error => console.log(error)
                });
              }
            }
          });
        }
      },
      error: error => console.log(error)
    });
  }
  removeCouponCode(){
    const cart = this.cartService.shoppingCart();
    if(cart === null) return;
    cart.coupon = undefined;
    this.cartService.setCart(cart).subscribe({
      next: async () => {
        const stripe = await this.stripeService.getStripeInstance();
        if(stripe){
          this.stripeService.createOrUpdatePaymentIntent().subscribe({
            error: error => console.log(error)
          });
        }
      },
      error: error => console.log(error)
    });
  }
}
