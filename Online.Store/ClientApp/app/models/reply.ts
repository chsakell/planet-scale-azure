import { Post } from "./post";

export class Reply {
    public replyToPostId?: string;
    public title: string;
    public content: string;
    public mediaDescription: string;    
    public userId: string;    
    public userName: string;    
    public mediaFile?: File;
}