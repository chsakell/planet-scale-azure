import { Component, OnInit, Input } from '@angular/core';
import { CompleterService, CompleterData, CompleterItem } from 'ng2-completer';
import { Router } from '@angular/router';

@Component({
    selector: 'nav-bar',
    templateUrl: './navbar.component.html',
    styleUrls: ['./navbar.component.css']
})

export class NavBarComponent implements OnInit {
    @Input() user: any;

    public searchField = '';//bind to the search field text input
    suggestions: CompleterData;
    searchBy = 'term';//search by paraneter

    constructor(private completerService: CompleterService,
                private router: Router) {
        this.suggestions = completerService.remote('/api/search/products?term=', 'title', 'title')
            .titleField('title');
    }

    public viewProduct(item: CompleterItem) {
        if(item && item.originalObject) {
            console.log(item.originalObject);
            this.router.navigate(['/products/details/' + item.originalObject.id]);
        }
    }

    ngOnInit() { }
}