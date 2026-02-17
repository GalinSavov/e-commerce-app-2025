import { CartItem } from "./cartItem"
import {nanoid} from 'nanoid'
export type ShoppingCart = {
    id:string,
    items: CartItem[],
    deliveryMethodId?:number,
    paymentIntentId?:string,
    clientSecret?:string,
    coupon?:Coupon
}
export type Coupon = {
    name: string,
    amountOff: number,
    percentOff: number,
    promotionCode: string,
    couponId: string,
    id: number
}
export function createNewCart(): ShoppingCart {
  return { id: nanoid(), 
    items: [],
    deliveryMethodId:undefined,
    paymentIntentId:undefined,
    clientSecret:undefined};
}