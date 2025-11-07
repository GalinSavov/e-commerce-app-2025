import { Component, inject, OnDestroy, OnInit, signal } from '@angular/core';
import { OrderSummaryComponent } from "../../shared/components/order-summary/order-summary.component";
import {MatStepperModule} from '@angular/material/stepper';
import { RouterLink } from "@angular/router";
import { MatButton } from '@angular/material/button';
import { StripeService } from '../../core/services/stripe.service';
import { ConfirmationToken, StripeAddressElement, StripeAddressElementChangeEvent, StripePaymentElement, StripePaymentElementChangeEvent } from '@stripe/stripe-js';
import { SnackbarService } from '../../core/services/snackbar.service';
import {MatCheckboxChange, MatCheckboxModule} from '@angular/material/checkbox'
import { StepperSelectionEvent } from '@angular/cdk/stepper';
import { AccountService } from '../../core/services/account.service';
import { Address } from '../../shared/models/address';
import { firstValueFrom } from 'rxjs';
import { CheckoutDeliveryComponent } from "./checkout-delivery/checkout-delivery.component";
import { CheckoutReviewComponent } from "./checkout-review/checkout-review.component";
import { CartService } from '../../core/services/cart.service';
import { CurrencyPipe, JsonPipe } from '@angular/common';

@Component({
  selector: 'app-checkout',
  imports: [OrderSummaryComponent, MatStepperModule, RouterLink, MatButton, RouterLink, MatCheckboxModule, CheckoutDeliveryComponent, CheckoutReviewComponent,CurrencyPipe,JsonPipe],
  templateUrl: './checkout.component.html',
  styleUrl: './checkout.component.scss'
})
export class CheckoutComponent implements OnInit,OnDestroy {
  
  protected stripeService = inject(StripeService);
  protected addressElement?:StripeAddressElement;
  protected paymentElement?:StripePaymentElement;
  protected snackbarService = inject(SnackbarService);
  protected saveAddress = false;
  protected accountService = inject(AccountService);
  protected cartService = inject(CartService);
  protected completionStatus = signal<{address:boolean,card:boolean,delivery:boolean}>({address:false,card:false,delivery:false});
  protected confirmationToken?:ConfirmationToken;

  async ngOnInit() {
    try {
      this.addressElement = await this.stripeService.createAddressElement();
      this.addressElement.mount('#address-element');
      this.addressElement.on('change',this.handleAddressChange)
      this.paymentElement = await this.stripeService.createPaymentElement();
      this.paymentElement.mount('#payment-element');
      this.paymentElement.on('change',this.handlePaymentChange)

    } catch (error:any) {
      this.snackbarService.error(error.message)
    }
  }
  handleAddressChange = (event:StripeAddressElementChangeEvent) => {
    this.completionStatus.update(state => {
      state.address = event.complete;
      return state;
    })
  }
  handleDeliveryMethodChange(event:boolean){
    this.completionStatus.update(state=>{
      state.delivery = event;
      return state;
    })
  }
  handlePaymentChange = (event:StripePaymentElementChangeEvent) => {
    this.completionStatus.update(state=>{
      state.card = event.complete
      return state;
    })
  }
  async getConfirmationToken(){
    try {
        if(Object.values(this.completionStatus()).every(status => status === true)){
        const result = await this.stripeService.createConfirmationToken();
        if(result.error) throw new Error(result.error.message);
        this.confirmationToken = result.confirmationToken;  
        }
    } catch (error:any) {
      this.snackbarService.error(error.message);
    }
    
  }
  async onStepChange(event:StepperSelectionEvent){
    if(event.selectedIndex === 1){
      if(this.saveAddress === true){
        const address = await this.getAddressFromStripeAddress();
        if(address){
          firstValueFrom(this.accountService.updateAddress(address));
        }
      }
    }
    if(event.selectedIndex === 2){
      await firstValueFrom(this.stripeService.createOrUpdatePaymentIntent());
    }
    if(event.selectedIndex === 3){
      await this.getConfirmationToken();
    }
  }
  private async getAddressFromStripeAddress():Promise<Address | null> {
    const result = await this.addressElement?.getValue();
    const address = result?.value.address;
    if(address){
      return{
        line1: address.line1,
        line2:address.line2 || undefined,
        city:address.city,
        country:address.country,
        state:address.state,
        postalCode:address.postal_code
      }
    }
    return null;
  }
  onSaveAddressChange(event:MatCheckboxChange){
    this.saveAddress = event.checked;
  }
  ngOnDestroy(): void {
    this.stripeService.disposeElements();
  }
}
