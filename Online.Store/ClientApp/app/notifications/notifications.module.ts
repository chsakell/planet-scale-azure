import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { StoreModule } from '@ngrx/store';
import { notificationsReducer } from './store/notifications.reducer';
import { NotifyService } from '../core/services/notifications.service';

@NgModule({
    imports: [
        CommonModule,
        StoreModule.forFeature('notifications', {
            notificationsState: notificationsReducer,
        })
    ],
    exports: [],
    declarations: [ ],
    providers: [
        NotifyService
    ],
})
export class NotificationsModule { }
