import { Message } from "../../models/message";

export interface NotificationsState {
    loading: boolean,
    message?: Message
};