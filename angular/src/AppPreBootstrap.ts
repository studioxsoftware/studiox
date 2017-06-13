import * as moment from 'moment';
import { AppConsts } from '@shared/AppConsts';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import { Type, CompilerOptions, NgModuleRef } from '@angular/core';

export class AppPreBootstrap {

    static run(callback: () => void): void {
        AppPreBootstrap.getApplicationConfig(() => {
            AppPreBootstrap.getUserConfiguration(callback);
        });
    }

    static bootstrap<TM>(moduleType: Type<TM>, compilerOptions?: CompilerOptions | CompilerOptions[]): Promise<NgModuleRef<TM>> {
        return platformBrowserDynamic().bootstrapModule(moduleType, compilerOptions);
    }

    private static getApplicationConfig(callback: () => void) {
        return studiox.ajax({
            url: '/assets/appconfig.json',
            method: 'GET',
            headers: {
                'StudioX.TenantId': studiox.multiTenancy.getTenantIdCookie()
            }
        }).done(result => {
            AppConsts.appBaseUrl = result.appBaseUrl;
            AppConsts.remoteServiceBaseUrl = result.remoteServiceBaseUrl;
            
            callback();
        });
    }

    private static getCurrentClockProvider(currentProviderName: string): studiox.timing.IClockProvider {
        if (currentProviderName === "unspecifiedClockProvider") {
            return studiox.timing.unspecifiedClockProvider;
        }

        if (currentProviderName === "utcClockProvider") {
            return studiox.timing.utcClockProvider;
        }

        return studiox.timing.localClockProvider;
    }

    private static getUserConfiguration(callback: () => void): JQueryPromise<any> {
        return studiox.ajax({
            url: AppConsts.remoteServiceBaseUrl + '/StudioXUserConfiguration/GetAll',
            method: 'GET',
            headers: {
                Authorization: 'Bearer ' + studiox.auth.getToken(),
                '.AspNetCore.Culture': studiox.utils.getCookieValue("StudioX.Localization.CultureName"),
                'StudioX.TenantId': studiox.multiTenancy.getTenantIdCookie()
            }
        }).done(result => {
            $.extend(true, studiox, result);

            studiox.clock.provider = this.getCurrentClockProvider(result.clock.provider);

            moment.locale(studiox.localization.currentLanguage.name);

            if (studiox.clock.provider.supportsMultipleTimezone) {
                moment.tz.setDefault(studiox.timing.timeZoneInfo.iana.timeZoneId);
            }

            callback();
        });
    }
}