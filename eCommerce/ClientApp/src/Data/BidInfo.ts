import {Item} from "./Item";


export enum BidState {
    WaitingForApprove,
    Approved,
    NotApproved
}

export var BidStateNames = ["WaitingForApprove", "Approved", "NotApproved"]

export interface BidInfo {
    bidID: string,
    bidState: BidState,
    itemID : string
    price: number,
    amount: number,
    buyerID:string
}


