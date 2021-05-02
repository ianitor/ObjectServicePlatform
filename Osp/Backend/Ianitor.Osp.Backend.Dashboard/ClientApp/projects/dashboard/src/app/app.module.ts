import {BrowserModule} from '@angular/platform-browser';
import {APP_INITIALIZER, LOCALE_ID, NgModule} from '@angular/core';

import {AppRoutingModule} from './app-routing.module';
import {AppComponent} from './app.component';
import {ConfigurationService} from "./services/configuration.service";
import {MAT_DIALOG_DEFAULT_OPTIONS} from "@angular/material/dialog";
import {AuthorizeInterceptor, AuthorizeService, SharedAuthModule} from "@ianitor/shared-auth";
import {defaultAuthorizeOptions} from "./config/defaultAuthorizeOptions";
import {HTTP_INTERCEPTORS, HttpClientModule} from "@angular/common/http";
import {HomeComponent} from "./home/home.component";
import {HeaderComponent} from "./navigation/header/header.component";
import {UserDiagnosticsComponent} from "./identity/user-diagnostics/user-diagnostics.component";
import {FooterComponent} from "./navigation/footer/footer.component";
import {SidenavListComponent} from "./navigation/sidenav-list/sidenav-list.component";
import {TenantsComponent} from "./tenants/tenants/tenants.component";
import {UsersComponent} from "./identity/users/users.component";
import {ServiceHooksComponent} from "./tenants/service-hooks/service-hooks.component";
import {NotificationTemplatesComponent} from "./tenants/notification-templates/notification-templates.component";
import {PageTemplatesComponent} from "./tenants/page-templates/page-templates.component";
import {ConstructionKitComponent} from "./tenants/construction-kit/construction-kit.component";
import {CkEntitiesDetailsComponent} from "./tenants/construction-kit/entities/details/details.component";
import {CkAttributeDetailsComponent} from "./tenants/construction-kit/attributes/details/ck-attribute-details.component";
import {CkEntitiesComponent} from "./tenants/construction-kit/entities/ck-entities.component";
import {AttributesComponent} from "./tenants/construction-kit/attributes/attributes.component";
import {CkEntitiesDetailsDerivedTypesComponent} from "./tenants/construction-kit/entities/details/derived-types/ck-entities-details-derived-types.component";
import {CkEntityAttributesComponent} from "./tenants/construction-kit/entity-attributes/ck-entity-attributes.component";
import {CkEntityAttributeDetailsComponent} from "./tenants/construction-kit/entity-attributes/details/ck-entity-attribute-details.component";
import {RtQueryBuilderComponent} from "./tenants/rt-query-builder/rt-query-builder.component";
import {ColumnSelectorComponent} from "./tenants/rt-query-builder/query-details/column-selector/column-selector.component";
import {CkSelectorComponent} from "./tenants/rt-query-builder/query-details/ck-selector/ck-selector.component";
import {QueryDetailsComponent} from "./tenants/rt-query-builder/query-details/query-details.component";
import {QuerySelectorComponent} from "./tenants/rt-query-builder/query-selector/query-selector.component";
import {QueryNameComponent} from "./tenants/rt-query-builder/query-details/query-name/query-name.component";
import {ResultDetailsComponent} from "./tenants/rt-query-builder/result-details/result-details.component";
import {AngularMarkdownEditorModule} from "angular-markdown-editor";
import {MarkdownModule, MarkedOptions} from "ngx-markdown";
import {AngularMaterialModule} from "./angular-material.module";
import {FormsModule} from "@angular/forms";
import {BrowserAnimationsModule} from "@angular/platform-browser/animations";
import {SortSelectorComponent} from "./tenants/rt-query-builder/query-details/sort-selector/sort-selector.component";
import {
  ConfirmationDialogModule, IaSharedUIModule,
  ProgressNotifierModule
} from "@ianitor/shared-ui";
import {HttpErrorInterceptor} from "./shared/httpErrorInterceptor";
import {ServiceHookDetailsComponent} from './tenants/service-hooks/service-hook-details/service-hook-details.component';
import {FieldFilterEditorComponent} from './tenants/rt-query-builder/query-details/field-filter-editor/field-filter-editor.component';
import {OspServicesModule} from "@ianitor/osp-services";
import {defaultOspServiceOptions} from "./config/defaultOspServiceOptions";
import {NotificationTemplateDetailsComponent} from "./tenants/notification-templates/notification-template-details/notification-template-details.component";
import {PageTemplateDetailsComponent} from "./tenants/page-templates/page-template-details/page-template-details.component";
import {UserDetailsComponent} from './identity/users/user-details/user-details.component';
import {TenantDetailsComponent} from './tenants/tenants/tenant-details/tenant-details.component';
import {MessageService, SharedServicesModule} from "@ianitor/shared-services";
import {TestAreaComponent} from './test-area/test-area.component';
import {ResetPasswordComponent} from './identity/users/reset-password/reset-password.component';
import {ClientsComponent} from './identity/clients/clients.component';
import {ClientDetailsComponent} from './identity/clients/client-details/client-details.component';
import {RolesComponent} from './identity/roles/roles.component';
import {RoleDetailsComponent} from './identity/roles/role-details/role-details.component';


export function initServices(configurationService: ConfigurationService, authorizeService: AuthorizeService) {
  return async () => {

    await configurationService.loadConfig();

    defaultAuthorizeOptions.wellKnownServiceUris = [
      configurationService.config.coreServices,
      configurationService.config.issuer,
      configurationService.config.jobServices
    ];

    defaultOspServiceOptions.coreServices = configurationService.config.coreServices;

    defaultAuthorizeOptions.issuer = configurationService.config.issuer;
    defaultAuthorizeOptions.scope = configurationService.config.scope;
    defaultAuthorizeOptions.redirectUri = configurationService.config.redirectUri;
    defaultAuthorizeOptions.postLogoutRedirectUri = configurationService.config.postLogoutRedirectUri;
    defaultAuthorizeOptions.clientId = configurationService.config.clientId;
    defaultAuthorizeOptions.showDebugInformation = true;

    await authorizeService.initialize();
  };
}

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    UserDiagnosticsComponent,


    HeaderComponent,
    SidenavListComponent,
    FooterComponent,
    TenantsComponent,
    UsersComponent,
    ServiceHooksComponent,
    NotificationTemplatesComponent,
    PageTemplatesComponent,
    NotificationTemplateDetailsComponent,
    ConstructionKitComponent,
    CkEntitiesDetailsComponent,
    CkEntitiesDetailsDerivedTypesComponent,
    AttributesComponent,
    CkAttributeDetailsComponent,
    CkEntitiesComponent,
    CkEntityAttributesComponent,
    CkEntityAttributeDetailsComponent,
    RtQueryBuilderComponent,
    ColumnSelectorComponent,
    CkSelectorComponent,
    QueryDetailsComponent,
    QuerySelectorComponent,
    QueryNameComponent,
    ResultDetailsComponent,
    SortSelectorComponent,
    ServiceHookDetailsComponent,
    FieldFilterEditorComponent,
    PageTemplateDetailsComponent,
    UserDetailsComponent,
    TenantDetailsComponent,
    TestAreaComponent,
    ResetPasswordComponent,
    ClientsComponent,
    ClientDetailsComponent,
    RolesComponent,
    RoleDetailsComponent
  ],
  imports: [
    BrowserAnimationsModule,
    BrowserModule.withServerTransition({appId: 'ng-cli-universal'}),
    HttpClientModule,
    FormsModule,
    AngularMaterialModule,
    SharedServicesModule.forRoot(),
    ProgressNotifierModule.forRoot(),
    ConfirmationDialogModule.forRoot(),
    MarkdownModule.forRoot({
      markedOptions: {
        provide: MarkedOptions,
        useValue: {
          gfm: true,
          tables: true,
          breaks: false,
          pedantic: false,
          sanitize: false,
          smartLists: true,
          smartypants: false,
        },
      },
    }),
    AngularMarkdownEditorModule.forRoot({
      // add any Global Options/Config you might want
      // to avoid passing the same options over and over in each components of your App
      iconlibrary: 'glyph'
    }),
    AppRoutingModule,
    OspServicesModule.forRoot(defaultOspServiceOptions),
    SharedAuthModule.forRoot(defaultAuthorizeOptions),
    IaSharedUIModule
  ],
  providers: [
    ConfigurationService,
    {provide: MAT_DIALOG_DEFAULT_OPTIONS, useValue: {hasBackdrop: false}},
    {provide: LOCALE_ID, useValue: "de-at"},
    {provide: APP_INITIALIZER, useFactory: initServices, deps: [ConfigurationService, AuthorizeService], multi: true},
    {provide: HTTP_INTERCEPTORS, useClass: AuthorizeInterceptor, multi: true},
    {provide: HTTP_INTERCEPTORS, useClass: HttpErrorInterceptor, deps: [MessageService], multi: true},
    {provide: MAT_DIALOG_DEFAULT_OPTIONS, useValue: {hasBackdrop: true}}
  ],
  bootstrap: [AppComponent]
})
export class AppModule {
}
