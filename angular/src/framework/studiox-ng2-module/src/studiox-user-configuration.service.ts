import { Injectable } from '@angular/core';
import { Http } from '@angular/http';

declare var jQuery: any;

@Injectable()
export class StudioXUserConfigurationService {

    constructor(private _http: Http) {

    }

    initialize(): void {
        this._http.get('/StudioXUserConfiguration/GetAll')
            .subscribe(result => {
                jQuery.extend(true, studiox, result.json());
            });
    }

}