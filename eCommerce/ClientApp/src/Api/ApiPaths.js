const API_PATH = "api"

// ========= Auth ========== //
export const AUTH_PATH = API_PATH + '/auth';
export const CONNECT_PATH = AUTH_PATH + '/connect';
export const LOGIN_PATH = AUTH_PATH + '/login';
export const LOGOUT_PATH = AUTH_PATH + '/logout';

export const REGISTER_PATH = AUTH_PATH + '/register';

// ========= Store ========== //

export const STORE_PATH = API_PATH + '/store';
export const OPEN_STORE_PATH = STORE_PATH + '/openStore';
export const ADD_ITEM_TO_STORE_PATH = STORE_PATH + '/addItem';
export const REMOVE_ITEM_FROM_STORE_PATH = STORE_PATH + '/removeItem';
export const EDIT_ITEM_IN_STORE_PATH = STORE_PATH + '/editItem';
export function GET_ITEM_IN_STORE_PATH(storeId, itemId) { return  STORE_PATH + `/${storeId}/${itemId}`; }
export function GET_ALL_ITEMS_IN_STORE_PATH(storeId){ return STORE_PATH + '/getAllItems/' + storeId }
export function GET_STORE_PERMISSION_FOR_USER_PATH(storeId) { return STORE_PATH + '/storePermissionForUser/' + storeId }
export const SEARCH_ITEMS_PATH = STORE_PATH + '/search';
export const SEARCH_STORE_PATH = STORE_PATH + '/searchStore';

export function STAFF_OF_STORE_PATH(storeId){ return STORE_PATH + `/${storeId}/staff` }
export function STAFF_PERMISSIONS_OF_STORE_PATH(storeId){ return STORE_PATH + `/${storeId}/staff` }
export function UPDATE_MANAGER_PERMISSIONS_IN_STORE_PATH(storeId){ return STORE_PATH + `/${storeId}/staff` }

export function GET_HISTORY_OF_STORE_PATH(storeId){ return STORE_PATH + `/${storeId}/history` }

export function ADD_POLICY_TO_STORE_PATH(storeId) { return STORE_PATH + `/${storeId}/policy`}
export function ADD_DISCOUNT_TO_STORE_PATH(storeId) { return STORE_PATH + `/${storeId}/discount`}

export function ASK_TO_BID_ITEM (storeId) { return STORE_PATH + `/${storeId}/askToBid`}

export function Get_All_Bid_Waiting (storeId) { return STORE_PATH + `/${storeId}/getAllBids`}

export function APPROVE_OR_DISAPPROVE_BID (storeId){ return STORE_PATH+`/${storeId}/approveOrDisapproveBid`}


// ========= Cart ========== //

export const CART_PATH = API_PATH + '/cart';
export const ADD_ITEM_TO_CART_PATH = CART_PATH + '/addItem';
export const EDIT_ITEM_IN_CART_PATH = CART_PATH + '/editItemAmount';
export const GET_CART_PATH = CART_PATH + '/getCart';
export const GET_PURCHASE_PRICE_CART_PATH = CART_PATH + "/getPurchasePrice";
export const PURCHASE_CART_PATH = CART_PATH + "/PurchaseCart";

// ========= User ========== //

export const USER_PATH = API_PATH + '/user';
export const GET_ALL_OWNED_STORES = USER_PATH + '/getALlStoreIds';
export const GET_ALL_MANAGED_STORES = USER_PATH + '/aLlManagedStoreIds'
export const GET_USER_BASIC_INFO_PATH = USER_PATH + '/getUserBasicInfo';
export const GET_USER_PURCHASE_HISTORY_PATH = USER_PATH + '/purchaseHistory'

// ========= Admin ========== //

export function ADMIN_GET_USER_PURCHASE_HISTORY(userId) {return USER_PATH + `/${userId}/userHistory`}
export function ADMIN_GET_STORE_PURCHASE_HISTORY(storeId) {return STORE_PATH + `/${storeId}/storeHistory`}
export const ADMIN_GET_LOGIN_STATS_PATH = API_PATH + '/stats/loginStats'