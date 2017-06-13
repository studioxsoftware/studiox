import { Component, OnInit, Injector } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';

@Component({
    selector: 'account-languages',
    templateUrl: './account-languages.component.html',
    styleUrls: [
        './account-languages.component.less'
    ]
})
export class AccountLanguagesComponent extends AppComponentBase implements OnInit {

    languages: studiox.localization.ILanguageInfo[];
    currentLanguage: studiox.localization.ILanguageInfo;

    constructor(
        injector: Injector
    ) { 
        super(injector);
    }

    ngOnInit() {
        this.languages = this.localization.languages;
        this.currentLanguage = this.localization.currentLanguage;
    }

    changeLanguage(languageName: string): void {
        studiox.utils.setCookieValue(
            "StudioX.Localization.CultureName",
            languageName,
            new Date(new Date().getTime() + 5 * 365 * 86400000), //5 year
            studiox.appPath
        );

        location.reload();
    }
}