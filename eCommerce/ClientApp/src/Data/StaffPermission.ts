import {StorePermission} from "./StorePermission";

export class StaffPermission {
    public userId: string;
    public permissions: StorePermission[];
    
    constructor(userId: string, permissions: StorePermission[]) {
        this.userId = userId;
        this.permissions = permissions;
    }
}