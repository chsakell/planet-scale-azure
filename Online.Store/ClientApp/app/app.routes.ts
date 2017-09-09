import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from "./home/home.component";
import { CounterComponent } from "./counter/counter.component";
import { FetchDataComponent } from "./fetchdata/fetchdata.component";

export const routes: Routes = [
    { path: '', redirectTo: 'home', pathMatch: 'full' },
    { path: 'home', component: HomeComponent },
    { path: 'counter', component: CounterComponent },
    { path: 'fetch-data', component: FetchDataComponent },
    { path: 'products', loadChildren: './product/product.module#ProductModule' },
    { path: '**', redirectTo: 'home' }
];

export const AppRoutes = RouterModule.forRoot(routes);
