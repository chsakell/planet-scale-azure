export class ResultVM {
    public result: Result;
    public message: string;
    public data: any;
}

export enum Result {
    SUCCESS = 1,
    Error = 2
}