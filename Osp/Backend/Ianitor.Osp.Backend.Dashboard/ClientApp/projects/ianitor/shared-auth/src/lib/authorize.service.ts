import {Inject, Injectable} from '@angular/core';
import {BehaviorSubject} from 'rxjs';
import {filter} from 'rxjs/operators';
import {AuthConfig, OAuthService} from "angular-oauth2-oidc";

export interface IUser {
  name: string;
  role: string;
}

export class AuthorizeOptions {
  wellKnownServiceUris: string[];
  // Url of the Identity Provider
  issuer: string;
  // URL of the SPA to redirect the user to after login
  redirectUri: string;
  postLogoutRedirectUri: string;
  // The SPA's id. The SPA is registered with this id at the auth-server
  clientId: string;
  // set the scope for the permissions the client should request
  // The first three are defined by OIDC. The 4th is a use case-specific one
  scope: string;
  showDebugInformation: boolean;
  sessionChecksEnabled: boolean;
}

@Injectable()
export class AuthorizeService {
  private isAuthenticated: BehaviorSubject<boolean> = new BehaviorSubject(false);
  private isAdmin: BehaviorSubject<boolean> = new BehaviorSubject(false);
  private authority: BehaviorSubject<string> = new BehaviorSubject(null);
  private accessToken: BehaviorSubject<string> = new BehaviorSubject(null);
  private user: BehaviorSubject<IUser> = new BehaviorSubject(null);

  constructor(@Inject(AuthorizeOptions) private authorizeOptions: AuthorizeOptions, private oauthService: OAuthService) {
    console.debug("AuthorizeService::created");
    this.getUser().subscribe(s => {
      this.isAuthenticated.next(!!s);
      this.isAdmin.next(!!s && s.role === "Administrators");
    });
  }

  public getServiceUris(): Array<string> {
    return this.authorizeOptions.wellKnownServiceUris;
  }

  public getAuthority(): BehaviorSubject<string> {
    return this.authority;
  }

  public getIsAuthenticated(): BehaviorSubject<boolean> {
    return this.isAuthenticated;
  }

  public getIsAdmin(): BehaviorSubject<boolean> {
    return this.isAdmin;
  }

  public getAccessToken(): BehaviorSubject<string> {
    return this.accessToken;
  }

  public getUser(): BehaviorSubject<IUser> {
    return this.user;
  }

  public login() {
    this.oauthService.initImplicitFlow();
  }

  public logout() {
    this.oauthService.logOut(false);
  }


  public initialize() {

    console.debug("AuthorizeService::initialize::started");

    const config: AuthConfig = {
      responseType: 'code',
      issuer: this.authorizeOptions.issuer,
      redirectUri: this.authorizeOptions.redirectUri,
      postLogoutRedirectUri: this.authorizeOptions.postLogoutRedirectUri,
      clientId: this.authorizeOptions.clientId,
      scope: this.authorizeOptions.scope,
      showDebugInformation: this.authorizeOptions.showDebugInformation,
      sessionChecksEnabled: this.authorizeOptions.sessionChecksEnabled
    };

    this.oauthService.configure(config);
    this.oauthService.setStorage(localStorage);
    this.oauthService.loadDiscoveryDocumentAndTryLogin();

    this.oauthService.setupAutomaticSilentRefresh();

    this.oauthService.events.subscribe(e => {
      // tslint:disable-next-line:no-console
      console.debug('oauth/oidc event', e);
    });

    this.oauthService.events
      .pipe(filter(e => e.type === 'session_terminated'))
      .subscribe(e => {
        // tslint:disable-next-line:no-console
        console.debug('Your session has been terminated!');
      });

    this.oauthService.events
      .pipe(filter(e => e.type === 'token_received'))
      .subscribe(e => {
        this.loadUser();
      });

    this.oauthService.events
      .pipe(filter(e => e.type === 'logout'))
      .subscribe(e => {
        this.accessToken.next(null);
        this.user.next(null);
      });

    if (this.oauthService.hasValidAccessToken()) {
      this.loadUser();
    }

    this.authority.next(this.authorizeOptions.issuer);

    console.debug("AuthorizeService::initialize::done");

  }

  private loadUser() {
    const claims = this.oauthService.getIdentityClaims();
    if (!claims) {
      console.error("claims where null when loading identity claims");
      return;
    }

    const user = <IUser>claims;
    const accessToken = this.oauthService.getAccessToken();
    this.user.next(user);
    this.accessToken.next(accessToken);
  }
}
