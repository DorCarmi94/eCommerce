import {Item} from "./Item";

export interface BasketDataType {
    storeId: string
    items: Item[],
    totalPrice: number
}

export class BasketData implements BasketDataType{
    public storeId: string;
    public items: Item[];
    public totalPrice: number;
    
    constructor(data: BasketDataType) {
        this.storeId = data.storeId;
        this.items = data.items;
        this.totalPrice = data.totalPrice;
    }
}

export interface CartDataType {
    baskets: BasketData[]
}

export class CartData implements CartDataType{
    public baskets: BasketData[];
    
    constructor(data: CartDataType) {
        this.baskets = data.baskets;
    }
}