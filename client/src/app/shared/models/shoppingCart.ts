import { CartItem } from "./cartItem"
import {nanoid} from 'nanoid'
export type ShoppingCart = {
    id:string,
    items: CartItem[],
    deliveryMethodId?:number,
    paymentIntentId?:string,
    clientSecret?:string,
}
export function createNewCart(): ShoppingCart {
  return { id: nanoid(), 
    items: [],
    deliveryMethodId:undefined,
    paymentIntentId:undefined,
    clientSecret:undefined};
}