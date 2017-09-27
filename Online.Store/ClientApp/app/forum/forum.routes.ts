import { RouterModule, Routes } from '@angular/router';

import { TopicDetailsComponent } from './details/topic-details.component';
import { TopicListComponent } from './list/topic-list.component';
import { ForumComponent } from "./forum.component";

const routes: Routes = [
    {
        path: '', component: ForumComponent, children: [
            { path: '', redirectTo: 'topics/list', pathMatch: 'full' },
            { path: 'topics/list', component: TopicListComponent },
            { path: 'topic/:id', component: TopicDetailsComponent }
        ]
    }
];

export const FORUM_ROUTES = RouterModule.forChild(routes);
