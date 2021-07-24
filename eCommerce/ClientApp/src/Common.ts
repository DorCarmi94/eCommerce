interface ResultType<T> {
    error: string,
    value: T;
    isFailure: boolean,
    isSuccess: boolean
}

export class Result<T>{
    public error: string;
    public value: T;
    public isFailure: boolean;
    public isSuccess: boolean;
    
    constructor(resultData: ResultType<T>) {
        this.error = resultData.error;
        this.value = resultData.value;
        this.isFailure = resultData.isFailure;
        this.isSuccess = resultData.isSuccess;
    }
} 