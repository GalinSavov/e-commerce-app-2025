import { Component, inject, OnInit } from '@angular/core';
import { OrderService } from '../../../core/features/order.service';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Order } from '../../../shared/models/order';
import { MatCardModule } from '@angular/material/card';
import { MatButton } from '@angular/material/button';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { AddressPipe } from '../../../shared/pipes/address-pipe';
import { PaymentPipe } from '../../../shared/pipes/payment-pipe';
import { AccountService } from '../../../core/services/account.service';
import { AdminService } from '../../../core/services/admin.service';

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
  protected accountService = inject(AccountService);
  protected adminService = inject(AdminService);
  private router = inject(Router);
  order?:Order;
  buttonText = this.accountService.isAdmin() ? 'Return To Admin' : 'Return To Orders';
  loadOrder(){
    const id = this.activatedRoute.snapshot.paramMap.get('id');
    if(!id) return;
    const loadOrderData = this.accountService.isAdmin() ? this.adminService.getOrder(+id) : this.orderService.getOrder(+id);
    loadOrderData.subscribe({
      next:order => this.order = order
    })
  }
  onReturnClick(){
    this.accountService.isAdmin() ? this.router.navigateByUrl('/admin') : this.router.navigateByUrl('/orders');
  }
}
