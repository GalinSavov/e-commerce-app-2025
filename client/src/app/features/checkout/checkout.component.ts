import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { OrderSummaryComponent } from "../../shared/components/order-summary/order-summary.component";
import {MatStepperModule} from '@angular/material/stepper';
import { RouterLink } from "@angular/router";
import { MatButton } from '@angular/material/button';
import { StripeService } from '../../core/services/stripe.service';
import { StripeAddressElement } from '@stripe/stripe-js';
import { SnackbarService } from '../../core/services/snackbar.service';
import {MatCheckboxChange, MatCheckboxModule} from '@angular/material/checkbox'
import { StepperSelectionEvent } from '@angular/cdk/stepper';
import { AccountService } from '../../core/services/account.service';
import { Address } from '../../shared/models/address';
import { firstValueFrom } from 'rxjs';

@Component({
  selector: 'app-checkout',
  imports: [OrderSummaryComponent, MatStepperModule, RouterLink,MatButton,RouterLink,MatCheckboxModule],
  templateUrl: './checkout.component.html',
  styleUrl: './checkout.component.scss'
})
export class CheckoutComponent implements OnInit,OnDestroy {
  
  protected stripeService = inject(StripeService);
  protected addressElement?:StripeAddressElement;
  protected snackbarService = inject(SnackbarService);
  protected saveAddress = false;
  protected accountService = inject(AccountService);
  async ngOnInit() {
    try {
      this.addressElement = await this.stripeService.createAddressElement();
      this.addressElement.mount('#address-element');
    } catch (error:any) {
      this.snackbarService.error(error.message)
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
