import { Component, OnInit, Input, Output, EventEmitter, ViewChild, ChangeDetectionStrategy } from '@angular/core';
import { Topic } from "../../models/topic";
import { Post } from '../../models/post';
import { Reply } from '../../models/reply';
import { User } from '../../models/user';

@Component({
    selector: 'topic-details-presentation',
    templateUrl: './details.html',
    styleUrls: ['./details.css'],
    changeDetection: ChangeDetectionStrategy.OnPush
})

export class TopicDetailsPresentationComponent {

    @Input() topic: Topic;
    @Input() user: User;
    @Output() onReply: EventEmitter<Reply> = new EventEmitter();

    @ViewChild("fileInput") fileInput: any;

    viewReply: boolean = false;
    title: string = '';
    content: string = '';
    mediaDescription: string = '';

    constructor() { }

    addPost(): void {
        let fileToUpload;
        let fi = this.fileInput.nativeElement;

        if (fi.files && fi.files[0]) {
            fileToUpload = fi.files[0];
        }

        const reply: Reply = new Reply();

        reply.replyToPostId = this.topic.id;
        reply.title = this.title;
        reply.content = this.content;
        reply.mediaDescription = this.mediaDescription;
        reply.userId = this.user.id;
        reply.userName = this.user.username;

        if (fileToUpload) {
            reply.mediaFile = fileToUpload;
        }

        this.cleanReplyForm();

        this.onReply.emit(reply);
    }

    cleanReplyForm() {
        this.viewReply = false;
        this.title = '';
        this.content = '';
        this.mediaDescription = '';
    }

}