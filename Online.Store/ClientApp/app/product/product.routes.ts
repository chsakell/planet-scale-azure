import { RouterModule, Routes } from '@angular/router';

import { ProductDetailsComponent } from './details/product-details';
import { ProductListComponent } from './list/product-list.component';
import { ProductComponent } from "./product.component";

const routes: Routes = [
    {
        path: '', component: ProductComponent, children: [
            { path: '', redirectTo: 'list', pathMatch: 'full' },
            { path: 'list', component: ProductListComponent },
            { path: 'details/:id', component: ProductDetailsComponent }
        ]
    }
];

export const PRODUCT_ROUTES = RouterModule.forChild(routes);
