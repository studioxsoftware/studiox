import { Injectable } from '@angular/core';

@Injectable()
export class LocalizationService {

    get languages(): studiox.localization.ILanguageInfo[] {
        return studiox.localization.languages;
    }

    get currentLanguage(): studiox.localization.ILanguageInfo {
        return studiox.localization.currentLanguage;
    }

    localize(key: string, sourceName: string): string {
        return studiox.localization.localize(key, sourceName);
    }
    
    getSource(sourceName: string): (key: string) => string {
        return studiox.localization.getSource(sourceName);
    }

}