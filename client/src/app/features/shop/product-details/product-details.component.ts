import { Component, inject, NgModule, OnInit } from '@angular/core';
import { ShopService } from '../../../core/services/shop.service';
import { Product } from '../../../shared/models/product';
import { ActivatedRoute } from '@angular/router';
import { CurrencyPipe } from '@angular/common';
import { MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import {MatFormField, MatLabel} from '@angular/material/form-field';
import{MatInput} from '@angular/material/input';
import { MatDivider } from "@angular/material/divider";
import { CartService } from '../../../core/services/cart.service';
import { FormsModule, NgForm } from '@angular/forms';
@Component({
  selector: 'app-product-details',
  imports: [CurrencyPipe, MatButton, MatIcon, MatInput, MatFormField, MatLabel, MatDivider,FormsModule],
  templateUrl: './product-details.component.html',
  styleUrl: './product-details.component.scss'
})
export class ProductDetailsComponent implements OnInit {
  private shopService = inject(ShopService);
  protected cartService = inject(CartService);
  private activatedRoute = inject(ActivatedRoute);
  protected product?: Product
  protected quantity = 1;
  
  ngOnInit(): void {
    this.getProductById();
  }
  addItemToCart(){
    if(this.product){
      this.cartService.addItemToCart(this.product,this.quantity)
    }
  }
  getProductById(){
    const id = this.activatedRoute.snapshot.paramMap.get('id');
    if(!id) return;
    this.shopService.getProductById(+id).subscribe({
      next: response => this.product = response,
      error: error => console.log(error),
    })
  }
  onQuantityChange(value:number){
    if(value < 1 || !value){
      this.quantity = 1;
    }
  }

}
