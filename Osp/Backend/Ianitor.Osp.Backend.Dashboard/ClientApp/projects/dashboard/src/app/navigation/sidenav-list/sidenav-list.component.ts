import {Component, EventEmitter, OnInit, Output} from '@angular/core';
import {Observable} from "rxjs";
import {map} from "rxjs/operators";
import {AuthorizeService} from "@ianitor/shared-auth";
import {ConfigurationService} from "../../services/configuration.service";

@Component({
  selector: 'app-sidenav-list',
  templateUrl: './sidenav-list.component.html',
  styleUrls: ['./sidenav-list.component.css']
})
export class SidenavListComponent implements OnInit {
  isAuthenticated: Observable<boolean>;
  userName: Observable<string>;
  authority: Observable<string>;

  @Output() sidenavClose = new EventEmitter();

  constructor(private authorizeService: AuthorizeService, public configurationService: ConfigurationService) {

  }

  ngOnInit() {
    this.isAuthenticated = this.authorizeService.getIsAuthenticated();
    this.userName = this.authorizeService.getUser().pipe(map(u => u && u.name));
    this.authority = this.authorizeService.getAuthority();
  }

  public login() {
    this.authorizeService.login();
  }

  public logout() {
    this.authorizeService.logout();
  }

  public onSidenavClose = () => {
    this.sidenavClose.emit();
  }

}
