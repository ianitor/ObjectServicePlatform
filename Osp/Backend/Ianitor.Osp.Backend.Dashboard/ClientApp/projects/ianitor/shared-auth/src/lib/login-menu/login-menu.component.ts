import { Component, OnInit } from '@angular/core';
import { AuthorizeService } from '../authorize.service';
import { Observable, BehaviorSubject  } from 'rxjs';
import { map, tap } from 'rxjs/operators';

@Component({
  selector: 'app-login-menu',
  templateUrl: './login-menu.component.html',
  styleUrls: ['./login-menu.component.css']
})
export class LoginMenuComponent implements OnInit {
  public isAuthenticated: BehaviorSubject<boolean>;
  public userName: Observable<string>;
  public isAdmin: Observable<boolean>;

  constructor(private authorizeService: AuthorizeService) { }

  ngOnInit() {
    const isIFrame = window.self !== window.top;

    console.log("app-login-menu::created");

    this.isAuthenticated = this.authorizeService.getIsAuthenticated();
    this.userName = this.authorizeService.getUser().pipe(map(u => u && u.name));
    this.isAdmin = this.authorizeService.getIsAdmin();

    this.isAuthenticated.subscribe(x => {

      console.log(`isAuthenticated changed to ${x} (iframe ${isIFrame})`);
    });
  }

  public login() {
    this.authorizeService.login();
  }

  public logout() {
    this.authorizeService.logout();
  }

  public register() {

  }
}
