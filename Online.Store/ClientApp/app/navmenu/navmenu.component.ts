import { Component, Input } from '@angular/core';
import { User } from '../models/user';

@Component({
    selector: 'nav-menu',
    templateUrl: './navmenu.component.html',
    styleUrls: ['./navmenu.component.css']
})
export class NavMenuComponent {
    @Input() cart: any;
    @Input() user: User;
}
