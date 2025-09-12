import { Component, inject, OnInit, signal } from '@angular/core';
import { ShopService } from '../../core/services/shop.service';
import { Product } from '../../shared/models/product';
import { ProductItemComponent } from '../product-item/product-item.component';
import {MatDialog} from '@angular/material/dialog';
import { FiltersDialogComponent } from './filters-dialog/filters-dialog.component';
import { MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import {MatMenu, MatMenuTrigger} from '@angular/material/menu';
import { MatListOption, MatSelectionList, MatSelectionListChange } from '@angular/material/list';
import {MatPaginator, PageEvent} from '@angular/material/paginator';
import { ShopParams } from '../../shared/models/shopParams';
import { Pagination } from '../../shared/models/pagination';
import { FormsModule } from '@angular/forms';
import { MatIconButton } from "@angular/material/button";

@Component({
  selector: 'app-shop',
  imports: [ProductItemComponent, MatButton, MatIcon, MatMenu, MatSelectionList, MatListOption, MatMenuTrigger, MatPaginator, FormsModule, MatIconButton],
  templateUrl: './shop.component.html',
  styleUrl: './shop.component.scss'
})
export class ShopComponent implements OnInit {
  private readonly shopService = inject(ShopService);
  private matDialogService = inject(MatDialog)
  protected products?: Pagination<Product>;
  protected currentProduct?: Product;
  protected sortingOptions = [
    {name: 'A-Z', value:'name'},
    {name: 'Price: Low-High', value:'priceAsc'},
    {name: 'Price: High-Low', value:'priceDesc'},
  ]
  protected shopParams = new ShopParams();
  protected pageSizeOptions = [
    5,10,15,20
  ]
  ngOnInit(): void 
  {
    this.getProducts();
    this.getProductBrands();
    this.getProductTypes();
  }
  getProducts(){
    this.shopService.getProducts(this.shopParams).subscribe({
      next: response => this.products = response,
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
        selectedBrands: this.shopParams.brands,
        selectedTypes: this.shopParams.types
      }
    });
    dialogRef.afterClosed().subscribe({
      next: response => {
        if(response){
          this.shopParams.brands = response.selectedBrands;
          this.shopParams.types = response.selectedTypes;
          this.shopParams.pageNumber = 1;
          this.getProducts();
        } 
      }
    })
  }
  onSortChange(event: MatSelectionListChange){
    const selectedOption = event.options[0];
    if(selectedOption){
      this.shopParams.sort = selectedOption.value;
      this.shopParams.pageNumber = 1;
      this.getProducts();
    }
  }
  onPageChange(event: PageEvent){
    this.shopParams.pageNumber = event.pageIndex + 1;
    this.shopParams.pageSize = event.pageSize;
    this.getProducts();
  }
  onSearchChange(){
    this.shopParams.pageNumber = 1;
    this.getProducts();
  }
}
