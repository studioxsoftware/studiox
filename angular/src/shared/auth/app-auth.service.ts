import { Injectable } from '@angular/core';
import { AppConsts } from '@shared/AppConsts';

@Injectable()
export class AppAuthService {

    logout(reload?: boolean): void {
        studiox.auth.clearToken();
        if (reload !== false) {
            location.href = AppConsts.appBaseUrl;
        }
    }
}