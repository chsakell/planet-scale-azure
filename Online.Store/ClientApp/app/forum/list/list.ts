import { Component, OnInit, Input, Output, EventEmitter, ChangeDetectionStrategy, ViewChild } from '@angular/core';
import { Topic } from "../../models/topic";
import { Reply } from '../../models/reply';
import { User } from '../../models/user';

@Component({
    selector: 'topic-list-presentation',
    templateUrl: './list.html',
    styleUrls: ['./list.css'],
    changeDetection: ChangeDetectionStrategy.OnPush
})

export class TopicListPresentationComponent {

    @Input() topics: Topic[];
    @Input() user: User;
    @Output() onCreateTopic: EventEmitter<Reply> = new EventEmitter();

    @ViewChild("fileInput") fileInput: any;
    viewCreateTopic: boolean = false;
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

        reply.title = this.title;
        reply.content = this.content;
        reply.mediaDescription = this.mediaDescription;
        reply.userId = this.user.id;
        reply.userName = this.user.username;

        if (fileToUpload) {
            reply.mediaFile = fileToUpload;
        }

        this.cleanReplyForm();

        this.onCreateTopic.emit(reply);
    }

    cleanReplyForm() {
        this.viewCreateTopic = false;
        this.title = '';
        this.content = '';
        this.mediaDescription = '';
    }
}