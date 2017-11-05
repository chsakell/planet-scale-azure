import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from "./home/home.component";
import { CheckoutComponent } from "./checkout/checkout.component";
import { UserComponent } from './user/user.component';
import { OrdersComponent } from './orders/orders.component';

export const routes: Routes = [
    { path: '', redirectTo: 'home', pathMatch: 'full' },
    { path: 'home', component: HomeComponent },
    { path: 'products', loadChildren: './product/product.module#ProductModule' },
    { path: 'forum', loadChildren: './forum/forum.module#ForumModule' },
    { path: 'checkout', component: CheckoutComponent },
    { path: 'orders', component: OrdersComponent },
    { path: '**', redirectTo: 'home' }
];

export const AppRoutes = RouterModule.forRoot(routes);
