import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpModule, JsonpModule } from '@angular/http';

import { ModalModule } from 'ngx-bootstrap';
import { NgxPaginationModule } from 'ngx-pagination';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';

import { StudioXModule } from '@studiox/studiox.module';

import { ServiceProxyModule } from '@shared/service-proxies/service-proxy.module';
import { SharedModule } from '@shared/shared.module';

import { HomeComponent } from '@app/pages/home/home.component';
import { AboutComponent } from '@app/pages/about/about.component';

import { UsersComponent } from '@app/system/users/users.component';
import { CreateUserComponent } from '@app/system/users/create-user.component';
import { EditUserComponent } from '@app/system/users/edit-user.component';

import { RolesComponent } from '@app/system/roles/roles.component';
import { CreateRoleComponent } from '@app/system/roles/create-role.component';
import { EditRoleComponent } from '@app/system/roles/edit-role.component';

import { TenantsComponent } from '@app/system/tenants/tenants.component';
import { CreateTenantComponent } from '@app/system/tenants/create-tenant.component';
import { EditTenantComponent } from '@app/system/tenants/edit-tenant.component';

import { TopBarComponent } from '@app/layout/topbar.component';
import { TopBarLanguageSwitchComponent } from '@app/layout/topbar-languageswitch.component';
import { SideBarUserAreaComponent } from '@app/layout/sidebar-user-area.component';
import { SideBarNavComponent } from '@app/layout/sidebar-nav.component';
import { SideBarFooterComponent } from '@app/layout/sidebar-footer.component';
import { RightSideBarComponent } from '@app/layout/right-sidebar.component';

import { PaginationModule } from 'ngx-bootstrap';
import { StudioXTableModule } from '@studiox-table/studiox-table-module';
import { TableDemoComponent } from '@app/demo/table/table.component';

@NgModule({
    declarations: [
        AppComponent,
        HomeComponent,
        AboutComponent,

        UsersComponent,
        CreateUserComponent,
        EditUserComponent,

        TenantsComponent,
        CreateTenantComponent,
        EditTenantComponent,

        RolesComponent,
        CreateRoleComponent,
        EditRoleComponent,

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
        NgxPaginationModule,
        PaginationModule.forRoot(),
        StudioXTableModule
    ],
    providers: [

    ]
})
export class AppModule { }