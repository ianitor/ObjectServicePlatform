import {Component, EventEmitter, OnInit, Output} from '@angular/core';
import {Observable} from "rxjs";
import {map} from "rxjs/operators";
import {ConfigurationService} from "../../services/configuration.service";
import {AuthorizeService} from "@ianitor/shared-auth";

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent implements OnInit {
  public isAdmin: Observable<boolean>;
  public isAuthenticated: Observable<boolean>;
  public userName: Observable<string>;
  public authority: Observable<string>;

  @Output() public sidenavToggle = new EventEmitter();

  constructor(private authorizeService: AuthorizeService, public configurationService: ConfigurationService) {

  }

  ngOnInit() {
    this.isAuthenticated = this.authorizeService.getIsAuthenticated();
    this.isAdmin = this.authorizeService.getIsAdmin();
    this.userName = this.authorizeService.getUser().pipe(map(u => u && u.name));
    this.authority = this.authorizeService.getAuthority();
  }

  public login() {
    this.authorizeService.login();
  }

  public logout() {
    this.authorizeService.logout();
  }

  onToggleSidenav()
  {
    this.sidenavToggle.emit();
  }
}
