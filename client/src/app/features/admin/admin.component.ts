import { Component, inject, OnInit, ViewChild } from '@angular/core';
import { MatTableModule, MatTableDataSource } from '@angular/material/table';
import { Order } from '../../shared/models/order';
import { MatPaginator, MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { AdminService } from '../../core/services/admin.service';
import { OrderParams } from '../../shared/models/orderParams';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectChange, MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatTabsModule } from '@angular/material/tabs';
import { RouterLink } from '@angular/router';
import { DialogService } from '../../core/services/dialog.service';
@Component({
  selector: 'app-admin',
  imports: [
    MatTableModule,
    MatPaginatorModule,
    MatButtonModule,
    MatIconModule,
    MatSelectModule,
    MatFormFieldModule,
    DatePipe,
    CurrencyPipe,
    MatTooltipModule,
    MatTabsModule,
    RouterLink
  ],
  templateUrl: './admin.component.html',
  styleUrl: './admin.component.scss'
})
export class AdminComponent implements OnInit {

  displayedColumns: string[] = ['id', 'buyerEmail', 'orderDate','total', 'orderStatus','action'];
  dataSource = new MatTableDataSource<Order>([]);
  protected adminService = inject(AdminService);
  protected dialogService = inject(DialogService);
  orderParams = new OrderParams();
  totalItems = 0;
  statusOptions = ['All','Pending','PaymentReceived','PaymentMismatch','Refunded','Pending'];

  @ViewChild(MatPaginator) paginator!: MatPaginator;
    ngOnInit(): void {
    this.loadOrders();
  }

  loadOrders(){
    this.adminService.getOrders(this.orderParams).subscribe({
      next: response =>{
        if(response.data){
          this.dataSource.data = response.data;
          this.totalItems = response.count;
        }
      }
    })
  }
  onPageChange(event:PageEvent){
    this.orderParams.pageNumber = event.pageIndex + 1;
    this.orderParams.pageSize = event.pageSize;
    this.loadOrders();
  }
  onFilterSelect(event:MatSelectChange){
    this.orderParams.filter = event.value;
    this.orderParams.pageNumber = 1;
    this.loadOrders();
  }
  async openConfirmDialog(id:number){
    const confirmed = await this.dialogService.confirm('Confirm Refund','Are you sure you want to refund this order?');
    if(confirmed){
      this.refundOrder(id);
    }
  }
  refundOrder(id:number){
    this.adminService.refundOrder(id).subscribe({
      next: order => {
        this.dataSource.data = this.dataSource.data.map(o => o.id === id ? order : o);
      }
    })
  }

}
