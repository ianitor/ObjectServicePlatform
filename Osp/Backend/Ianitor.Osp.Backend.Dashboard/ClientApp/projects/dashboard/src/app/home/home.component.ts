import { Component } from '@angular/core';
import {Observable} from "rxjs";
import {map} from "rxjs/operators";
import {AuthorizeService} from "@ianitor/shared-auth";

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent {

  public isAuthenticated: Observable<boolean>;
  public userName: Observable<string>;

  constructor(private authorizeService: AuthorizeService) {

  }

  ngOnInit() {
    this.isAuthenticated = this.authorizeService.getIsAuthenticated();
    this.userName = this.authorizeService.getUser().pipe(map(u => u && u.name));
  }

  login() {
    this.authorizeService.login();
  }

  logout() {
    this.authorizeService.logout();
  }
}
