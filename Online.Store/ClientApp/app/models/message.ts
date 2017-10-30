export class Message {
    public type: MessageType;
    public message: string;
}

export enum MessageType {
    SUCCESS = 1,
    Error = 2,
    Info = 3
}