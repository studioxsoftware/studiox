import { Component, Injector, ViewEncapsulation } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { MenuItem } from '@shared/layout/menu-item';

@Component({
    templateUrl: './sidebar-nav.component.html',
    selector: 'sidebar-nav',
    encapsulation: ViewEncapsulation.None
})
export class SideBarNavComponent extends AppComponentBase {

    menuItems: MenuItem[] = [
        new MenuItem(this.l("HomePage"), "", "home", "/app/home"),
        new MenuItem(this.l("Tenants"), "System.Administration.Tenants", "business", "/app/tenants"),
        new MenuItem(this.l("Users"), "System.Administration.Users", "people", "/app/users"),
        new MenuItem(this.l("Roles"), "System.Administration.Roles", "local_offer", "/app/roles"),
        new MenuItem(this.l("About"), "", "info", "/app/about"),

        new MenuItem(this.l("MultiLevelMenu"), "", "menu", "", [
            new MenuItem("Demo", "", "", "/app/demo", [
                new MenuItem("Table", "", "", "/app/demo")             
            ])            
        ])
    ];

    constructor(
        injector: Injector
    ) {
        super(injector);
    }

    showMenuItem(menuItem): boolean {
        if (menuItem.permissionName) {
            return this.permission.isGranted(menuItem.permissionName);
        }

        return true;
    }
}