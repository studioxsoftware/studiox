import { NgModule } from '@angular/core';
import { HttpModule, JsonpModule, Http, XHRBackend, RequestOptions } from '@angular/http';

import { StudioXSessionService } from './session/studiox-session.service';
import { PermissionCheckerService } from './auth/permission-checker.service';
import { FeatureCheckerService } from './features/feature-checker.service';
import { LocalizationService } from './localization/localization.service';
import { SettingService } from './settings/setting.service';
import { NotifyService } from './notify/notify.service';
import { MessageService } from './message/message.service';
import { LogService } from './log/log.service';
import { StudioXMultiTenancyService } from './multi-tenancy/studiox-multi-tenancy.service';
import { StudioXHttpConfiguration, StudioXHttp } from './studioxHttp';
import { StudioXUserConfigurationService } from './studiox-user-configuration.service';
import { TokenService } from './auth/token.service';
import { UtilsService } from './utils/utils.service';

export function studioxHttpFactory(xhrBackend: XHRBackend, requestOptions: RequestOptions, configuration: StudioXHttpConfiguration): Http {
    return new StudioXHttp(xhrBackend, requestOptions, configuration);
}

export let StudioXHttpProvider = {
    provide: Http,
    useFactory: studioxHttpFactory,
    deps: [XHRBackend, RequestOptions, StudioXHttpConfiguration]
};

@NgModule({
    imports: [
        HttpModule,
        JsonpModule
    ],

    declarations: [
    ],

    providers: [
        StudioXSessionService,
        PermissionCheckerService,
        FeatureCheckerService,
        LocalizationService,
        SettingService,
        NotifyService,
        MessageService,
        LogService,
        StudioXMultiTenancyService,
        StudioXUserConfigurationService,
        StudioXHttpConfiguration,
        StudioXHttpProvider,
        TokenService,
        UtilsService
    ]
})
export class StudioXModule { }