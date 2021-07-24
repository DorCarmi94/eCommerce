export interface Tuple<T1, T2> {
    item1: T1,
    item2: T2
}

export function makeTuple<T1, T2>(item1: T1 , item2: T2): Tuple<T1, T2> {
    return {item1: item1, item2: item2}
}

export interface LoginStatsInterface {
    stat: Tuple<string, number>[] 
}

export class LoginStats implements LoginStatsInterface{
    stat: Tuple<string, number>[];
    
    constructor(data: LoginStatsInterface) {
        this.stat = data.stat
    }
}