import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from "./home/home.component";
import { CounterComponent } from "./counter/counter.component";
import { FetchDataComponent } from "./fetchdata/fetchdata.component";
import { CheckoutComponent } from "./checkout/checkout.component";
import { UserComponent } from './user/user.component';

import { ProductModule } from './product/product.module';
import { ForumModule } from './forum/forum.module';

export const routes: Routes = [
    { path: '', redirectTo: 'home', pathMatch: 'full' },
    { path: 'home', component: HomeComponent },
    { path: 'counter', component: CounterComponent },
    { path: 'fetch-data', component: FetchDataComponent },
    { path: 'products', loadChildren: () => ProductModule },
    { path: 'forum', loadChildren: () => ForumModule },
    { path: 'checkout', component: CheckoutComponent },
    { path: '**', redirectTo: 'home' }
];

export const AppRoutes = RouterModule.forRoot(routes);
