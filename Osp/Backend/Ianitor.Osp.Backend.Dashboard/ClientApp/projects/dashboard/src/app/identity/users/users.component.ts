import {Component, OnInit} from '@angular/core';
import {IdentityBackendServices} from "../../services/identity-backend.service";
import {UsersDataSource} from "../../shared/usersDataSource";
import {UserDto} from "../../models/userDto";

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.css']
})
export class UsersComponent implements OnInit {

  dataColumns: string[] = ['eMail', 'name', 'roles'];
  displayedColumns: string[] = [...this.dataColumns, 'actions'];

  constructor(private identityBackendServices: IdentityBackendServices, public dataSource: UsersDataSource) {
  }

  ngOnInit() {

    this.dataSource.loadData();
  }

  onDelete(userDto: UserDto) {
    if (window.confirm('Are you sure to delete this item?')) {
      this.identityBackendServices.deleteUser(userDto.name).subscribe(_ => {
        this.dataSource.loadData();
      });
    }
  }
}
