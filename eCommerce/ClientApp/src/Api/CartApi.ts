import axios from "axios";
import {Result} from "../Common";
import {
    ADD_ITEM_TO_CART_PATH,
    EDIT_ITEM_IN_CART_PATH,
    GET_CART_PATH,
    GET_PURCHASE_PRICE_CART_PATH,
    PURCHASE_CART_PATH
} from "./ApiPaths";
import {CartData, CartDataType} from "../Data/CartData";

const instance = axios.create(
    {withCredentials : true}
);

export class CartApi {

    getCart() {
        return instance.get<Result<CartDataType>>(GET_CART_PATH)
            .then(res => res.data)
            .catch(res => undefined)
    };

    AddItem(itemId: string, storeId: string, amount: number) {
        return instance.post<Result<any>>(ADD_ITEM_TO_CART_PATH, 
            {
                itemId: itemId,
                storeId: storeId,
                amount: amount
            })
            .then(res => {
                return res.data
            })
            .catch(res => undefined)
    };

    EditItemAmount(itemId: string, storeId: string, amount: number) {
        return instance.put<Result<any>>(EDIT_ITEM_IN_CART_PATH,
            {
                itemId: itemId,
                storeId: storeId,
                amount: amount
            })
            .then(res => {
                return res.data
            }).catch(res => undefined)
    };

    GetPurchasePrice() {
        return instance.get<Result<number>>(GET_PURCHASE_PRICE_CART_PATH)
            .then(res => {
                return res.data;
            })
            .catch(res => undefined)
    };

    PurchasePrice(userName: string, idNumber: string, creditCardNumber: string, 
                         creditCardExpirationDate: string, threeDigitsOnBackOfCard: string, 
                         fullAddress: string) {
        console.log(`username: ${userName}`)
        return instance.post<Result<any>>(PURCHASE_CART_PATH,
            {
                userName: userName,
                idNumber: idNumber,
                creditCardNumber: creditCardNumber,
                creditCardExpirationDate: creditCardExpirationDate,
                threeDigitsOnBackOfCard: threeDigitsOnBackOfCard,
                fullAddress: fullAddress
            })
            .then(res => {
                return res.data;
            })
            .catch(res => undefined)
    };
}