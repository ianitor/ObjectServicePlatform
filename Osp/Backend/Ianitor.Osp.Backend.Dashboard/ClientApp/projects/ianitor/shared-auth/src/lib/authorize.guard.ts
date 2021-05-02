import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthorizeService } from './authorize.service';
import { tap } from 'rxjs/operators';

@Injectable()
export class AuthorizeGuard implements CanActivate {
  constructor(private authorize: AuthorizeService) {
  }
  canActivate(
    _next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean> | Promise<boolean> | boolean {
      return this.authorize.getIsAuthenticated()
        .pipe(tap(isAuthenticated => this.handleAuthorization(isAuthenticated)));
  }

  private handleAuthorization(isAuthenticated: boolean) {
    if (!isAuthenticated) {
      this.authorize.login();
    }
  }
}
