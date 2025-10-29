import { CartItem } from "./cartItem"
import {nanoid} from 'nanoid'
export type ShoppingCart = {
    id:string,
    cartItems: CartItem[]
}
export function createNewCart(): ShoppingCart {
  return { id: nanoid(), cartItems: [] };
}