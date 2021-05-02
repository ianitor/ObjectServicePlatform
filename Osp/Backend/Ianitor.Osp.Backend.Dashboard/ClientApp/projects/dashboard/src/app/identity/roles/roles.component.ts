import { Component, OnInit } from '@angular/core';
import {RolesDataSource} from "../../shared/rolesDataSource";
import {RoleDto} from "../../models/roleDto";
import {RolesBackendService} from "../../services/roles-backend.service";

@Component({
  selector: 'app-roles',
  templateUrl: './roles.component.html',
  styleUrls: ['./roles.component.scss']
})
export class RolesComponent implements OnInit {

  dataColumns: string[] = ['name'];
  displayedColumns: string[] = [...this.dataColumns, 'actions'];

  constructor(private rolesBackendService: RolesBackendService, public dataSource: RolesDataSource) { }

  ngOnInit(): void {
    this.dataSource.loadData();
  }

  onDelete(roleDto: RoleDto) {
    if (window.confirm('Are you sure to delete this item?')) {
      this.rolesBackendService.deleteRole(roleDto.name).subscribe(_ => {
        this.dataSource.loadData();
      });
    }
  }
}
