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

@Component({
  selector: 'app-shop',
  imports: [ProductItemComponent,MatButton,MatIcon,MatMenu,MatSelectionList,MatListOption,MatMenuTrigger],
  templateUrl: './shop.component.html',
  styleUrl: './shop.component.scss'
})
export class ShopComponent implements OnInit {
  private readonly shopService = inject(ShopService);
  private matDialogService = inject(MatDialog)
  protected products: Product[] = [];
  protected selectedBrands: string[] = [];
  protected selectedTypes: string[] = [];
  protected selectedSorting: string = 'name';
  protected sortingOptions = [
    {name: 'A-Z', value:'name'},
    {name: 'Price: Low-High', value:'priceAsc'},
    {name: 'Price: High-Low', value:'priceDesc'},
  ]
  ngOnInit(): void 
  {
    this.getProducts();
    this.getProductBrands();
    this.getProductTypes();
  }
  getProducts(){
    this.shopService.getProducts(this.selectedBrands,this.selectedTypes,this.selectedSorting).subscribe({
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
          this.getProducts();
        } 
      }
    })
  }
  onSortChange(event: MatSelectionListChange){
    const selectedOption = event.options[0];
    if(selectedOption){
      this.selectedSorting = selectedOption.value;
      this.getProducts();
    }
  }
}
