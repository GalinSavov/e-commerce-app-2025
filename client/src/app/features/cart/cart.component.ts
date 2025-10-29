import { Component, inject, OnInit } from '@angular/core';
import { CartService } from '../../core/services/cart.service';

@Component({
  selector: 'app-cart',
  imports: [],
  templateUrl: './cart.component.html',
  styleUrl: './cart.component.scss'
})
export class CartComponent implements OnInit {
  ngOnInit(): void {
    console.log(this.cartService.shoppingCart())
  }
  private cartService = inject(CartService);
  
}
