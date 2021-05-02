import {Component, OnInit} from '@angular/core';
import {RoleDto} from "../../../models/roleDto";
import {AbstractDetailsComponent} from "@ianitor/shared-ui";
import {FormBuilder, Validators} from "@angular/forms";
import {ActivatedRoute} from "@angular/router";
import {MessageService} from "@ianitor/shared-services";
import {RolesBackendService} from "../../../services/roles-backend.service";

@Component({
  selector: 'app-role-details',
  templateUrl: './role-details.component.html',
  styleUrls: ['./role-details.component.scss']
})
export class RoleDetailsComponent extends AbstractDetailsComponent<RoleDto> implements OnInit {

  private readonly roleId: string;

  constructor(route: ActivatedRoute, private fb: FormBuilder, private rolesBackendService: RolesBackendService, private messageService: MessageService) {
    super();

    this.roleId = route.snapshot.paramMap.get('roleId');

    this._ownerForm = this.fb.group({
      name: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    if (this.roleId) {
      this.rolesBackendService.getRoleDetails(this.roleId).subscribe(value => {
        this.entity = value;

        this.ownerForm.setValue({
          name: value.name,
        });
        this.loading = false;

      })
    } else {
      this.entity = <RoleDto>{};

      this.ownerForm.setValue(
        {
          name: null
        }
      );
      this.loading = false;
    }
  }

  save() {
    this.loading = true;

    const transient = <RoleDto> this.ownerForm.value;

    Object.assign(this.entity, transient);

    if (this.roleId)
    {
      this.rolesBackendService.updateRole(this.roleId, transient).subscribe(value => {

        this.entity = value;
        this.loading = false;
        this.messageService.showInformation("Role saved.");
      }, error => {

        this.loading = false;
        this.messageService.showError(error, "Error during saving of role.");
      });
    }
    else {
      this.rolesBackendService.createRole(transient).subscribe(value => {

        this.entity = value;
        this.loading = false;

        this.messageService.showInformation("Role created.");

      }, error => {

        this.loading = false;
        this.messageService.showError(error, "Error during saving of role.");
      });
    }
  }

}
