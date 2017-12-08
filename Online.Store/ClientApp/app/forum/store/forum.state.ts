import { Topic } from './../../models/topic';
import { Post } from "../../models/post";

export interface ForumState {
    topics: Topic[],
    selectedTopic?: Topic,
    previousContinuationToken?: string;
    nextContinuationToken?: string;
};