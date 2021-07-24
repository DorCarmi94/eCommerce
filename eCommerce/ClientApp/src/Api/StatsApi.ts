import axios from "axios";
import {Result} from "../Common";
import {
    ADMIN_GET_LOGIN_STATS_PATH
} from "./ApiPaths";
import {LoginStats} from "../Data/LoginStats";


const instance = axios.create(
    {withCredentials : true}
);

export class StatsApi {
    
    loginStats(date: string) {
        return instance.get<Result<LoginStats>>(ADMIN_GET_LOGIN_STATS_PATH,
            {
                params: {
                    date: date
                }
            })
            .then(res => {
                return res.data
            })
            .catch(err => {
                return undefined
            })
    }
    
}