import { Component, ViewContainerRef, Injector, OnInit, AfterViewInit } from '@angular/core';
import { AppConsts } from '@shared/AppConsts';
import { AppComponentBase } from '@shared/app-component-base';

import { SignalRHelper } from '@shared/helpers/SignalRHelper';

@Component({
    templateUrl: './app.component.html'
})
export class AppComponent extends AppComponentBase implements OnInit, AfterViewInit {

    private viewContainerRef: ViewContainerRef;

    constructor(
        injector: Injector
    ) {
        super(injector);
    }

    ngOnInit(): void {
        if (this.appSession.application.features['SignalR']) {
            SignalRHelper.initSignalR();
        }

        studiox.event.on('studiox.notifications.received', userNotification => {
            studiox.notifications.showUiNotifyForUserNotification(userNotification);

            //Desktop notification
            Push.create("StudioXZeroTemplate", {
                body: userNotification.notification.data.message,
                icon: studiox.appPath + 'assets/app-logo-small.png',
                timeout: 6000,
                onClick: function () {
                    window.focus();
                    this.close();
                }
            });
        });
    }

    ngAfterViewInit(): void {
        ($ as any).AdminBSB.activateAll();
        ($ as any).AdminBSB.activateDemo();
    }
}