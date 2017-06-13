import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpModule, JsonpModule } from '@angular/http';

import { ModalModule } from 'ngx-bootstrap';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';

import { StudioXModule } from '@studiox/studiox.module';

import { ServiceProxyModule } from '@shared/service-proxies/service-proxy.module';
import { SharedModule } from '@shared/shared.module';

import { HomeComponent } from '@app/home/home.component';
import { AboutComponent } from '@app/about/about.component';
import { UsersComponent } from '@app/users/users.component';
import { CreateUserModalComponent } from '@app/users/create-user-modal.component';
import { TenantsComponent } from '@app/tenants/tenants.component';
import { CreateTenantModalComponent } from '@app/tenants/create-tenant-modal.component';
import { TopBarComponent } from '@app/layout/topbar.component';
import { TopBarLanguageSwitchComponent } from '@app/layout/topbar-languageswitch.component';
import { SideBarUserAreaComponent } from '@app/layout/sidebar-user-area.component';
import { SideBarNavComponent } from '@app/layout/sidebar-nav.component';
import { SideBarFooterComponent } from '@app/layout/sidebar-footer.component';
import { RightSideBarComponent } from '@app/layout/right-sidebar.component';

import { PaginationModule } from 'ngx-bootstrap';
import { Ng2TableModule } from '@ng-table/ng-table-module';
import { TableDemoComponent } from '@app/table-demo/table-demo.component';

@NgModule({
    declarations: [
        AppComponent,
        HomeComponent,
        AboutComponent,
        UsersComponent,
        CreateUserModalComponent,
        TenantsComponent,
        CreateTenantModalComponent,
        TopBarComponent,
        TopBarLanguageSwitchComponent,
        SideBarUserAreaComponent,
        SideBarNavComponent,
        SideBarFooterComponent,
        RightSideBarComponent,
        TableDemoComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        HttpModule,
        JsonpModule,
        ModalModule.forRoot(),
        StudioXModule,
        AppRoutingModule,
        ServiceProxyModule,
        SharedModule,
        PaginationModule.forRoot(),
        Ng2TableModule
    ],
    providers: [

    ]
})
export class AppModule { }