import { Component, OnInit, Input } from '@angular/core';
import { CompleterService, CompleterData } from 'ng2-completer';

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

    constructor(private completerService: CompleterService) {
        this.suggestions = completerService.remote('/api/search/products?term=', 'description, title, image', 'title')
            .descriptionField('description')
            .titleField('title')
            .imageField('image');
    }

    ngOnInit() { }
}