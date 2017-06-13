import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AppComponent } from './app.component';
import { AppRouteGuard } from '@shared/auth/auth-route-guard';
import { HomeComponent } from './home/home.component';
import { AboutComponent } from './about/about.component';
import { UsersComponent } from './users/users.component';
import { TenantsComponent } from './tenants/tenants.component';
import { TableDemoComponent } from '@app/table-demo/table-demo.component';

@NgModule({
    imports: [
        RouterModule.forChild([
            {
                path: '',
                component: AppComponent,
                children: [
                    { path: 'home', component: HomeComponent,  canActivate: [AppRouteGuard] },
                    { path: 'demo', component: TableDemoComponent},
                    {
                        path: 'users',
                        component: UsersComponent,
                        data: { permission: 'System.Administration.Users' },
                        canActivate: [AppRouteGuard]
                    },
                    {
                        path: 'tenants',
                        component: TenantsComponent,
                        data: { permission: 'System.Administration.Tenants' },
                        canActivate: [AppRouteGuard]
                    },
                    { path: 'about', component: AboutComponent }
                ]
            }
        ])
    ],
    exports: [RouterModule]
})
export class AppRoutingModule { }