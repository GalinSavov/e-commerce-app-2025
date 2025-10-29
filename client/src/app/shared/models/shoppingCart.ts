import { CartItem } from "./cartItem"
import {nanoid} from 'nanoid'
export type ShoppingCart = {
    id:string,
    items: CartItem[]
}
export function createNewCart(): ShoppingCart {
  return { id: nanoid(), items: [] };
}