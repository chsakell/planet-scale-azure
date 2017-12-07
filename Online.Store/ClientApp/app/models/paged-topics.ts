import { Topic } from "./topic";

export class PagedTopics {
    public topics: Topic[];
    public continuationToken: string;
}