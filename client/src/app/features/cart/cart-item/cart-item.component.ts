import { Component, input } from '@angular/core';
import { CartItem } from '../../../shared/models/cartItem';
import { RouterLink } from "@angular/router";
import { MatButton, MatIconButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { CurrencyPipe } from '@angular/common';

@Component({
  selector: 'app-cart-item',
  imports: [RouterLink, MatButton, MatIcon, MatIconButton,CurrencyPipe],
  templateUrl: './cart-item.component.html',
  styleUrl: './cart-item.component.scss'
})
export class CartItemComponent {
  item = input.required<CartItem>();
}
