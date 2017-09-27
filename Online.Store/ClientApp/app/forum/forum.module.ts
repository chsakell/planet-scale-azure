import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TopicListComponent } from './list/topic-list.component';
import { TopicDetailsComponent } from './details/topic-details.component';
import { FORUM_ROUTES } from './forum.routes';
import { StoreModule } from '@ngrx/store';
import { EffectsModule } from '@ngrx/effects';
//import { productReducer } from './store/product.reducer';
//import { ProductEffects } from './store/product.effects';
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
        //StoreModule.forFeature('catalog', {
        //    productState: productReducer,
        //}),
        //EffectsModule.forFeature([ProductEffects])
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
