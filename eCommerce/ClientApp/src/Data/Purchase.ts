import { Basket } from "../components/Cart/Basket";

export interface PurchaseRecordInterface {
    storeId: string,
    username: string,
    basket: Basket,
    purchaseTime: string,
}

export class PurchaseRecord implements PurchaseRecordInterface {
    basket: Basket;
    purchaseTime: string;
    storeId: string;
    username: string;
    
    constructor(data : PurchaseRecordInterface) {
        this.basket = data.basket;
        this.purchaseTime = data.purchaseTime;
        this.storeId = data.storeId;
        this.username = data.username;
    }
}

export interface PurchaseHistoryInterface {
    records: PurchaseRecord[]
}

export class PurchaseHistory implements PurchaseHistoryInterface {
    records: PurchaseRecord[];
    
    constructor(data : PurchaseHistoryInterface) {
        this.records = data.records;
    }
}