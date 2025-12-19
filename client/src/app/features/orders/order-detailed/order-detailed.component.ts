import { Component, inject, OnInit } from '@angular/core';
import { OrderService } from '../../../core/features/order.service';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { Order } from '../../../shared/models/order';
import { MatCardModule } from '@angular/material/card';
import { MatButton } from '@angular/material/button';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { AddressPipe } from '../../../shared/pipes/address-pipe';
import { PaymentPipe } from '../../../shared/pipes/payment-pipe';

@Component({
  selector: 'app-order-detailed',
  imports: [MatCardModule,MatButton,DatePipe,CurrencyPipe,AddressPipe,PaymentPipe,RouterLink],
  templateUrl: './order-detailed.component.html',
  styleUrl: './order-detailed.component.scss'
})
export class OrderDetailedComponent implements OnInit {
  ngOnInit(): void {
    this.loadOrder();
  }
  protected orderService = inject(OrderService);
  protected activatedRoute = inject(ActivatedRoute);
  order?:Order;
  loadOrder(){
    const id = this.activatedRoute.snapshot.paramMap.get('id');
    if(!id) return;
    this.orderService.getOrder(+id).subscribe({
      next:order => this.order = order
    })
  }
}
