import { RouterModule, Routes } from '@angular/router';

import { ProductDetailsComponent } from './details/product-details';
import { ProductListComponent } from './list/product-list.component';

const routes: Routes = [
    { path: 'list', component: ProductListComponent },
    { path: 'details', component: ProductDetailsComponent },
    { path: '', redirectTo: 'list', pathMatch: 'full' }
];

export const PRODUCT_ROUTES = RouterModule.forChild(routes);
