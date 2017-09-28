import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TopicListComponent } from './list/topic-list.component';
import { TopicDetailsComponent } from './details/topic-details.component';
import { FORUM_ROUTES } from './forum.routes';
import { StoreModule } from '@ngrx/store';
import { EffectsModule } from '@ngrx/effects';
import { forumReducer } from './store/forum.reducer';
import { ForumEffects } from './store/forum.effects';
import { FormsModule } from '@angular/forms';
import { ForumComponent } from "./forum.component";
import { TopicListPresentationComponent } from './list/list';
import { TopicDetailsPresentationComponent } from "./details/details";

const FORUM_DIRECTIVES = [
    TopicListComponent,
    TopicListPresentationComponent,
    TopicDetailsComponent,
    TopicDetailsPresentationComponent,
    ForumComponent
];

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        FORUM_ROUTES,
        StoreModule.forFeature('community', {
            forumState: forumReducer,
        }),
        EffectsModule.forFeature([ForumEffects])
    ],
    exports: [
        ...FORUM_DIRECTIVES
    ],
    declarations: [
        ...FORUM_DIRECTIVES
    ],
    providers: [],
})
export class ForumModule { }
