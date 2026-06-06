export interface CartItemDto {
  cartItemId: number;
  variantId: number;
  productId: number;
  productName: string;
  imageUrl?: string | null;
  sku: string;
  weight?: string | null;
  gripSize?: string | null;
  color?: string | null;
  price: number;
  quantity: number;
  stockQuantity: number;
  subTotal: number;
}

export interface CartDto {
  cartId: number;
  items: CartItemDto[];
  totalAmount: number;
}

export interface CartItemModel {
  id: number;
  variantId: number;
  productId: number;
  name: string;
  image: string;
  sku: string;
  variantLabel: string;
  price: number;
  quantity: number;
  stock: number;
  subTotal: number;
}

export interface CartModel {
  id: number;
  items: CartItemModel[];
  totalAmount: number;
  totalQuantity: number;
}

