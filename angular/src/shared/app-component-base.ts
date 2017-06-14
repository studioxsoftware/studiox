import { Injector, ElementRef} from '@angular/core';
import { AppConsts } from '@shared/AppConsts';
import { LocalizationService } from '@studiox/localization/localization.service';
import { PermissionCheckerService } from '@studiox/auth/permission-checker.service';
import { FeatureCheckerService } from '@studiox/features/feature-checker.service';
import { NotifyService } from '@studiox/notify/notify.service';
import { SettingService } from '@studiox/settings/setting.service';
import { MessageService } from '@studiox/message/message.service';
import { StudioXMultiTenancyService } from '@studiox/multi-tenancy/studiox-multi-tenancy.service';
import { AppSessionService } from '@shared/session/app-session.service';

export abstract class AppComponentBase {

    localizationSourceName = AppConsts.localization.defaultLocalizationSourceName;

    localization: LocalizationService;
    permission: PermissionCheckerService;
    feature: FeatureCheckerService;
    notify: NotifyService;
    setting: SettingService;
    message: MessageService;
    multiTenancy: StudioXMultiTenancyService;
    appSession: AppSessionService;
    domNode: HTMLElement = null;

    constructor(injector: Injector) {
        this.localization = injector.get(LocalizationService);
        this.permission = injector.get(PermissionCheckerService);
        this.feature = injector.get(FeatureCheckerService);
        this.notify = injector.get(NotifyService);
        this.setting = injector.get(SettingService);
        this.message = injector.get(MessageService);
        this.multiTenancy = injector.get(StudioXMultiTenancyService);
        this.appSession = injector.get(AppSessionService);
        this.domNode = injector.get(ElementRef).nativeElement;
    }

    ngAfterViewInit(): void {
         ($ as any).AdminBSB.input.activate($(this.domNode));
    }		      

    l(key: string, ...args: any[]): string {
        let localizedText = this.localization.localize(key, this.localizationSourceName);

        if (!localizedText) {
            localizedText = key;
        }

        if (!args || !args.length) {
            return localizedText;
        }

        args.unshift(localizedText);
        return studiox.utils.formatString.apply(this, args);
    }

    isGranted(permissionName: string): boolean {
        return this.permission.isGranted(permissionName);
    }
}