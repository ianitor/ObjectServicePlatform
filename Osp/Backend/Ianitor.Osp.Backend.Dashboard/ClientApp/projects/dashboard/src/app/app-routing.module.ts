import {NgModule} from '@angular/core';
import {Routes, RouterModule} from '@angular/router';
import {HomeComponent} from "./home/home.component";
import {ConstructionKitComponent} from "./tenants/construction-kit/construction-kit.component";
import {CkEntitiesDetailsComponent} from "./tenants/construction-kit/entities/details/details.component";
import {CkAttributeDetailsComponent} from "./tenants/construction-kit/attributes/details/ck-attribute-details.component";
import {RtQueryBuilderComponent} from "./tenants/rt-query-builder/rt-query-builder.component";
import {CkEntityAttributeDetailsComponent} from "./tenants/construction-kit/entity-attributes/details/ck-entity-attribute-details.component";
import {UsersComponent} from "./identity/users/users.component";
import {UserDiagnosticsComponent} from "./identity/user-diagnostics/user-diagnostics.component";
import {TenantsComponent} from "./tenants/tenants/tenants.component";
import {NotificationTemplatesComponent} from "./tenants/notification-templates/notification-templates.component";
import {PageTemplatesComponent} from "./tenants/page-templates/page-templates.component";
import {ServiceHooksComponent} from "./tenants/service-hooks/service-hooks.component";
import {AuthorizeGuard} from "@ianitor/shared-auth";
import {ServiceHookDetailsComponent} from "./tenants/service-hooks/service-hook-details/service-hook-details.component";
import {NotificationTemplateDetailsComponent} from "./tenants/notification-templates/notification-template-details/notification-template-details.component";
import {PageTemplateDetailsComponent} from "./tenants/page-templates/page-template-details/page-template-details.component";
import {UserDetailsComponent} from "./identity/users/user-details/user-details.component";
import {TenantDetailsComponent} from "./tenants/tenants/tenant-details/tenant-details.component";
import {TestAreaComponent} from "./test-area/test-area.component";
import {ResetPasswordComponent} from "./identity/users/reset-password/reset-password.component";
import {ClientsComponent} from "./identity/clients/clients.component";
import {ClientDetailsComponent} from "./identity/clients/client-details/client-details.component";
import {RoleDetailsComponent} from "./identity/roles/role-details/role-details.component";
import {RolesComponent} from "./identity/roles/roles.component";


const routes: Routes = [
  {path: '', component: HomeComponent, pathMatch: 'full'},
  {path: 'tenants/constructionKit/:id', component: ConstructionKitComponent, canActivate: [AuthorizeGuard]},
  {
    path: 'tenants/constructionKit/:id/entities/:ckId',
    component: CkEntitiesDetailsComponent,
    canActivate: [AuthorizeGuard]
  },
  {
    path: 'tenants/constructionKit/:id/attributes/:attributeId',
    component: CkAttributeDetailsComponent,
    canActivate: [AuthorizeGuard]
  },
  {
    path: 'tenants/runtime/:id',
    component: RtQueryBuilderComponent,
    canActivate: [AuthorizeGuard]
  },
  {
    path: 'tenants/constructionKit/:id/entities/:ckId/attributes/:attributeName',
    component: CkEntityAttributeDetailsComponent,
    canActivate: [AuthorizeGuard]
  },
  {path: 'identity/clients', component: ClientsComponent, canActivate: [AuthorizeGuard]},
  {path: 'identity/clients/details/:id', component: ClientDetailsComponent, canActivate: [AuthorizeGuard]},
  {path: 'identity/clients/new', component: ClientDetailsComponent, canActivate: [AuthorizeGuard]},
  {path: 'identity/users', component: UsersComponent, canActivate: [AuthorizeGuard]},
  {
    path: 'identity/users/details/:userName',
    component: UserDetailsComponent,
    canActivate: [AuthorizeGuard]
  },
  {
    path: 'identity/users/new',
    component: UserDetailsComponent,
    canActivate: [AuthorizeGuard]
  },
  {path: 'identity/roles', component: RolesComponent, canActivate: [AuthorizeGuard]},
  {
    path: 'identity/roles/details/:roleId',
    component: RoleDetailsComponent,
    canActivate: [AuthorizeGuard]
  },
  {
    path: 'identity/roles/new',
    component: RoleDetailsComponent,
    canActivate: [AuthorizeGuard]
  },
  {
    path: 'identity/users/reset/:userName',
    component: ResetPasswordComponent,
    canActivate: [AuthorizeGuard]
  },
  {path: 'identity/userDiagnostics', component: UserDiagnosticsComponent, canActivate: [AuthorizeGuard]},
  {path: 'tenants', component: TenantsComponent, canActivate: [AuthorizeGuard]},
  {
    path: 'tenants/attach',
    component: TenantDetailsComponent,
    canActivate: [AuthorizeGuard]
  },
  {
    path: 'tenants/new',
    component: TenantDetailsComponent,
    canActivate: [AuthorizeGuard]
  },
  {
    path: 'tenants/notificationTemplates/:id',
    component: NotificationTemplatesComponent,
    canActivate: [AuthorizeGuard]
  },
  {
    path: 'tenants/notificationTemplates/:tenantId/details/:id',
    component: NotificationTemplateDetailsComponent,
    canActivate: [AuthorizeGuard]
  },
  {
    path: 'tenants/notificationTemplates/:tenantId/new',
    component: NotificationTemplateDetailsComponent,
    canActivate: [AuthorizeGuard]
  },
  {path: 'tenants/pageTemplates/:tenantId', component: PageTemplatesComponent, canActivate: [AuthorizeGuard]},
  {
    path: 'tenants/pageTemplates/:tenantId/details/:id',
    component: PageTemplateDetailsComponent,
    canActivate: [AuthorizeGuard]
  },
  {
    path: 'tenants/pageTemplates/:tenantId/new',
    component: PageTemplateDetailsComponent,
    canActivate: [AuthorizeGuard]
  },
  {path: 'tenants/serviceHooks/:tenantId', component: ServiceHooksComponent, canActivate: [AuthorizeGuard]},
  {path: 'tenants/serviceHooks/:tenantId/details/:serviceHookId', component: ServiceHookDetailsComponent, canActivate: [AuthorizeGuard]},
  {path: 'tenants/serviceHooks/:tenantId/new', component: ServiceHookDetailsComponent, canActivate: [AuthorizeGuard]},
  {path: 'test-area', component: TestAreaComponent, canActivate: [AuthorizeGuard]}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {
}
