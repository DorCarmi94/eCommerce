import {UserRole} from "./UserRole";

export interface IBasicUserInfo {
    username: string,
    isLoggedIn: boolean,
    userRole: UserRole
}

export class BasicUserInfo implements IBasicUserInfo {
    username: string;
    isLoggedIn: boolean;
    userRole: UserRole;
    
    constructor(data: IBasicUserInfo) {
        this.username = data.username;
        this.isLoggedIn = data.isLoggedIn;
        this.userRole = data.userRole

    }
    
    static create(username: string, isLoggedIn: boolean, userRole: UserRole) {
        return new BasicUserInfo({
            username: username,
            isLoggedIn: isLoggedIn,
            userRole: userRole
        })

    }
}
