import axios from "axios";
import {Result} from "../Common";
import {
    ADD_DISCOUNT_TO_STORE_PATH,
    ADD_ITEM_TO_STORE_PATH,
    ADD_POLICY_TO_STORE_PATH,
    ADMIN_GET_STORE_PURCHASE_HISTORY,
    ADMIN_GET_USER_PURCHASE_HISTORY, APPROVE_OR_DISAPPROVE_BID,
    ASK_TO_BID_ITEM,
    EDIT_ITEM_IN_STORE_PATH, Get_All_Bid_Waiting,
    GET_ALL_ITEMS_IN_STORE_PATH,
    GET_HISTORY_OF_STORE_PATH,
    GET_ITEM_IN_STORE_PATH,
    GET_STORE_PERMISSION_FOR_USER_PATH,
    GET_USER_PURCHASE_HISTORY_PATH,
    OPEN_STORE_PATH,
    REMOVE_ITEM_FROM_STORE_PATH,
    SEARCH_ITEMS_PATH,
    SEARCH_STORE_PATH,
    STAFF_OF_STORE_PATH,
    STAFF_PERMISSIONS_OF_STORE_PATH,
    UPDATE_MANAGER_PERMISSIONS_IN_STORE_PATH
} from "./ApiPaths";
import {Item} from "../Data/Item";
import {StorePermission} from "../Data/StorePermission";
import {StaffPermission} from "../Data/StaffPermission";
import {PurchaseHistory} from "../Data/Purchase";
import {RuleNode} from "../Data/StorePolicies/RuleInfo";
import {DiscountNode} from "../Data/StorePolicies/DiscountInfoTree";
import {BidInfo} from "../Data/BidInfo";

const instance = axios.create(
    {withCredentials : true}
);

export class StoreApi {
    
    openStore(storeId: string) {
        return instance.post<Result<any>>(OPEN_STORE_PATH, 
            {},
            {
                params: {
                    storeId: storeId
                }
            })
            .then(res => {
                return res.data
            })
            .catch(res => undefined)
        }
        
   static async addItem(item: Item)
    {
        return instance.post<Result<any>>(ADD_ITEM_TO_STORE_PATH, item)
            .then(res => {
                return res.data
            })
            .catch(res => undefined)
    }

    removeItem(storeId: string, itemId: string)
    {
        return instance.post<Result<any>>(REMOVE_ITEM_FROM_STORE_PATH, 
            {
                storeId: storeId,
                itemId: itemId
            })
            .then(res => {
                return res.data
            })
            .catch(res => undefined)
    }

    editItem(item: Item)
    {
        return instance.post<Result<any>>(EDIT_ITEM_IN_STORE_PATH, item)
            .then(res => {
                return res.data
            })
            .catch(res => undefined)
    }
    
    //in Store component
    getPurchaseHistory(storeId: string){
        return instance.get<Result<PurchaseHistory>>(GET_HISTORY_OF_STORE_PATH(storeId))
            .then(res => {
                return res.data;
            })
            .catch(err => {
                return undefined
            })
    }
    
    //history for specific store - admin panel
    adminGetStorePurchaseHistory(storeId: string){
        return instance.get<Result<PurchaseHistory>>(ADMIN_GET_STORE_PURCHASE_HISTORY(storeId))
            .then(res => {
                return res.data;
            })
            .catch(err => {
                return undefined
            })
    }
    
    // ========== Store query ========== //

    getItem(storeId: string, itemId: string)
    {
        return instance.get<Result<Item>>(GET_ITEM_IN_STORE_PATH(storeId, itemId))
            .then(res => {
                return res.data
            })
            .catch(res => undefined)
    }

    getAllItems(storeId: string)
    {
        return instance.get<Result<Item[]>>(GET_ALL_ITEMS_IN_STORE_PATH(storeId))
            .then(res => {
                return new Result(res.data)
            }).catch(err => undefined)
    }
    
    searchItems(query: string){
        return instance.get<Result<Item[]>>(SEARCH_ITEMS_PATH,
            {
                params: {
                    query: query
                }
            })
            .then(res => {
                return new Result(res.data)
            }).catch(err => undefined)
    }

    searchStore(query: string){
        return instance.get<Result<string[]>>(SEARCH_STORE_PATH,
            {
                params: {
                    query: query
                }
            })
            .then(res => {
                return res.data
            }).catch(err => undefined)
    }

    getStorePermissionForUser(storeId: string)
    {
        return instance.get<Result<StorePermission[]>>(GET_STORE_PERMISSION_FOR_USER_PATH(storeId))
            .then(res => {
                return res.data
            }).catch(err => undefined)
    }

    // ========== Store staff ========== //
    
    appointOwner(storeId: string, appointedUserId: string){
        return instance.post<Result<any>>(STAFF_OF_STORE_PATH(storeId),
            {},
            {
                params: {
                    role: "owner",
                    userId: appointedUserId
                }
            })
            .then(res => {
                return res.data
            }).catch(err => undefined)
    }

    removeOwner(storeId: string, removedUserId: string){
        return instance.delete<Result<any>>(STAFF_OF_STORE_PATH(storeId), 
            {
                params: {
                    role: "owner",
                    userId: removedUserId
                }
            })
            .then(res => {
                return res.data
            }).catch(err => undefined)
    }

    appointManager(storeId: string, appointedUserId: string){
        return instance.post<Result<any>>(STAFF_OF_STORE_PATH(storeId),
            {},
            {
                params: {
                    role: "manager",
                    userId: appointedUserId
                }
            })
            .then(res => {
                return res.data
            }).catch(err => undefined)
    }

    removeManager(storeId: string, removedUserId: string){
        return instance.delete<Result<any>>(STAFF_OF_STORE_PATH(storeId),
            {
                params: {
                    role: "manager",
                    userId: removedUserId
                }
            })
            .then(res => {
                return res.data
            }).catch(err => undefined)
    }
    
    updateManagerPermissions(storeId: string, managerUserId: string,
                             permissions: StorePermission[]){
        return instance.put<Result<any>>(UPDATE_MANAGER_PERMISSIONS_IN_STORE_PATH(storeId),
            {
                storePermissions: permissions
            },
            {
                params: {
                    role: "manager",
                    userId: managerUserId
                }
            })
            .then(res => {
                return res.data
            }).catch(err => undefined)
    }
    
    getStoreStaffPermissions(storeId: string){
        return instance.get<Result<StaffPermission[]>>(STAFF_PERMISSIONS_OF_STORE_PATH(storeId))
            .then(res => {
                return res.data
            }).catch(err => undefined)
    }

    // ========== Store policy ========== //
    
    addRuleToStorePolicy(storeId: string, ruleNode: RuleNode){
        return instance.post<Result<any>>(ADD_POLICY_TO_STORE_PATH(storeId), ruleNode)
            .then(res => {
                return res.data
            }).catch(err => undefined)
    }

    addDiscountToStore(storeId: string, discountNode: DiscountNode){
        return instance.post<Result<any>>(ADD_DISCOUNT_TO_STORE_PATH(storeId), discountNode)
            .then(res => {
                return res.data
            }).catch(err => undefined)
    }

    // ========== Store bid ========== //
    askToBidOnItem(storeId:string, itemId:string , amount:number ,newPrice:number ){
        return instance.post<Result<any>>(ASK_TO_BID_ITEM(storeId),{},{
            params: {
                itemId: itemId,
                amount: amount,
                newPrice: newPrice
            }
        })
            .then(res => {
                return res.data
            }).catch(err => undefined)
    }
    
    getAllBidWaiting(storeId: string){
        return instance.get<Result<BidInfo>>(Get_All_Bid_Waiting(storeId))
            .then(res => {
                return res.data
            }).catch(err => undefined)
    }

    approveOrDisapproveBid(storeId:string, bidID:string ,shouldApprove:boolean ){
        return instance.post<Result<any>>(APPROVE_OR_DISAPPROVE_BID(storeId),{},{
            params: {
                bidID: bidID,
                shouldApprove: shouldApprove,
            }
        })
            .then(res => {
                return res.data
            }).catch(err => undefined)
    }


}