export type Order = {
  id: number
  orderDate: string
  buyerEmail: string
  shippingAddress: ShippingAddress
  deliveryMethod: string
  shippingPrice: number
  paymentSummary: PaymentSummary
  orderItems: OrderItem[]
  subTotal: number
  orderStatus: string
  paymentIntentId: string,
  total:number
}

export type ShippingAddress = {
  name: string
  line1: string
  line2?: string
  city: string
  state: string
  postalCode: string
  country: string
}

export type PaymentSummary = {
  last4: number
  brand: string
  expMonth: number
  expYear: number
}

export type OrderItem = {
  productId: number
  productName: string
  pictureURL: string
  price: number
  quantity: number
}
export type OrderToCreate = {
    cartId:string,
    deliveryMethodId:number,
    shippingAddress:ShippingAddress,
    paymentSummary:PaymentSummary
}
