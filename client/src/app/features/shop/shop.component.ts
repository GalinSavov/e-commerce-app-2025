import { Component, inject, OnInit, signal } from '@angular/core';
import { ShopService } from '../../core/services/shop.service';
import { Product } from '../../shared/models/product';
import { ProductItemComponent } from '../product-item/product-item.component';
import {MatDialog} from '@angular/material/dialog';
import { FiltersDialogComponent } from './filters-dialog/filters-dialog.component';
import { MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';

@Component({
  selector: 'app-shop',
  imports: [ProductItemComponent,MatButton,MatIcon],
  templateUrl: './shop.component.html',
  styleUrl: './shop.component.scss'
})
export class ShopComponent implements OnInit {
  private readonly shopService = inject(ShopService);
  private matDialogService = inject(MatDialog)
  protected products: Product[] = [];
  protected selectedBrands: string[] = [];
  protected selectedTypes: string[] = [];
  ngOnInit(): void 
  {
    this.getProducts();
    this.getProductBrands();
    this.getProductTypes();
  }
  getProducts(){
    this.shopService.getProducts().subscribe({
      next: response => this.products = response.data,
      error: error => console.log(error),
    });
  }
  getProductBrands(){
    this.shopService.getProductBrands();
  }
  getProductTypes(){
    this.shopService.getProductTypes();
  }
  openFiltersDialog(){
    const dialogRef = this.matDialogService.open(FiltersDialogComponent,{
      minWidth:'500px',
      data:{
        selectedBrands: this.selectedBrands,
        selectedTypes: this.selectedTypes
      }
    });
    dialogRef.afterClosed().subscribe({
      next: response => {
        if(response){
          this.selectedBrands = response.selectedBrands;
          this.selectedTypes = response.selectedTypes;
          this.shopService.getProducts(this.selectedBrands,this.selectedTypes).subscribe({
              next: response => this.products = response.data,
              error: error => console.log(error),
          })
        } 
      }
    })
  }
}
