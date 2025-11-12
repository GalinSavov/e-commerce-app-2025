import { Component, inject, OnInit, signal, Signal } from '@angular/core';
import { OrderService } from '../../core/features/order.service';
import { Order } from '../../shared/models/order';
import { RouterLink } from "@angular/router";
import { CurrencyPipe, DatePipe } from '@angular/common';

@Component({
  selector: 'app-order',
  imports: [RouterLink,DatePipe,CurrencyPipe],
  templateUrl: './order.component.html',
  styleUrl: './order.component.scss'
})
export class OrderComponent implements OnInit {
 
  protected orderService = inject(OrderService);
  protected orders:Order[] = [];
  ngOnInit(): void {
    this.getOrders();
  }
  getOrders(){
    this.orderService.getOrders().subscribe({
      next: response => this.orders = response
    })
  }
}
