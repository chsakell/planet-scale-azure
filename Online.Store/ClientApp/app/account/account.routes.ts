import { RouterModule, Routes } from '@angular/router';

import { AccountRegisterComponent } from './register/register.component';
import { AccountLoginComponent } from './login/login.component';
import { AccountComponent } from './account.component';

const routes: Routes = [
    {
        path: '', component: AccountComponent, children: [
            { path: '', redirectTo: 'login', pathMatch: 'full' },
            { path: 'login', component: AccountLoginComponent },
            { path: 'register', component: AccountRegisterComponent }
        ]
    }
];

export const ACCOUNT_ROUTES = RouterModule.forChild(routes);
