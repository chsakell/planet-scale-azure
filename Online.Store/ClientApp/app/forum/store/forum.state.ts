import { Topic } from './../../models/topic';
import { Post } from "../../models/post";

export interface ForumState {
    topics: Map<number, Topic[]>,
    selectedPage: number;
    continuationToken?: string,
    selectedTopic?: Topic
};