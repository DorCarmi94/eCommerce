import axios from "axios";
import {Result} from "../Common";
import {
    GET_ALL_MANAGED_STORES,
    GET_ALL_OWNED_STORES,
    GET_USER_BASIC_INFO_PATH,
    GET_STORE_PERMISSION_FOR_USER_PATH, GET_USER_PURCHASE_HISTORY_PATH, ADMIN_GET_USER_PURCHASE_HISTORY
} from "./ApiPaths";
import {BasicUserInfo} from "../Data/BasicUserInfo";
import {StorePermission} from "../Data/StorePermission";
import {PurchaseHistory} from "../Data/Purchase";


const instance = axios.create(
    {withCredentials : true}
);

export class UserApi {
    
    getAllOwnedStoreIds() {
        return instance.get<Result<string[]>>(GET_ALL_OWNED_STORES)
            .then(res => {
                return res.data
            })
            .catch(err => {
                return undefined
            })
    }

    getAllManagedStoreIds() {
        return instance.get<Result<string[]>>(GET_ALL_MANAGED_STORES)
            .then(res => {
                return res.data
            })
            .catch(err => {
                return undefined
            })
    }
        
    getUserBasicInfo(){
        return instance.get<Result<BasicUserInfo>>(GET_USER_BASIC_INFO_PATH)
            .then(res => {
                return res.data.value;
            })
            .catch(err => {
                return undefined
            })
    }
    //current user history- member panel
    getPurchaseHistory(){
        return instance.get<Result<PurchaseHistory>>(GET_USER_PURCHASE_HISTORY_PATH)
            .then(res => {
                return res.data;
            })
            .catch(err => {
                return undefined
            })
    }

    //history of specific user - admin panel
    adminGetPurchaseHistory(userId: string){
        return instance.get<Result<PurchaseHistory>>(ADMIN_GET_USER_PURCHASE_HISTORY (userId))
            .then(res => {
                return res.data;
            })
            .catch(err => {
                return undefined
            })
    }
}