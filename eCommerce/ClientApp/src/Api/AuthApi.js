import axios from "axios";
import {CONNECT_PATH, LOGIN_PATH, REGISTER_PATH,LOGOUT_PATH} from "./ApiPaths";
import {Result} from "../Common";

const instance = axios.create(
    {withCredentials : true}
);

export class RedirectWithData {
    constructor(data, redirectLocation) {
        this.data = data;
        this.redirect = redirectLocation
    }
}

export class AuthApi {
    Connect() {
        return instance.get(CONNECT_PATH)
            .then(res => {
                let data = res.data;
                return new RedirectWithData(data, res.headers['redirectto'])
            })
            .catch(res => undefined);
    }
    
    Login(username, password, role) {
        return instance.post(LOGIN_PATH,
            {
                username: username,
                password: password,
                role: role
            })
            .then(res => {
                return new RedirectWithData(
                    new Result(res.data),
                    res.headers['redirectto']
                );
            })
            .catch(res => undefined);
    }

    Logout() {
        return instance.get(LOGOUT_PATH)
            .then(res => {
                console.log(res)
                return new Result(res.data)
            })
            .catch(res => undefined);
    }
    
    Register(username, password, email, name, address, birthday) {
        return instance.post(REGISTER_PATH,
            {
                username: username,
                password: password,
                email: email,
                name: name,
                address: address,
                birthday: birthday
            })
            .then(res => {
                return new RedirectWithData(
                    new Result(res.data),
                    res.headers['redirectto']
                );
            })
            .catch(res => undefined);
    }
}