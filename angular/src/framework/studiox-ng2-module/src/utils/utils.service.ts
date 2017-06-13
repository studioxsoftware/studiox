import { Injectable } from '@angular/core';

@Injectable()
export class UtilsService {

    getCookieValue(key: string): string {
        return studiox.utils.getCookieValue(key);
    }

    setCookieValue(key: string, value: string, expireDate?: Date, path?: string): void {
        studiox.utils.setCookieValue(key, value, expireDate, path);
    }

    deleteCookie(key: string, path?: string): void {
        studiox.utils.deleteCookie(key, path);
    }

}