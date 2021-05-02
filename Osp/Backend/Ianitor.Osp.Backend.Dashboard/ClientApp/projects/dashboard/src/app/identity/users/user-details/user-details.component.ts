import {Component, OnInit} from '@angular/core';
import {UserDto} from "../../../models/userDto";
import {FormBuilder, Validators} from "@angular/forms";
import {ActivatedRoute} from "@angular/router";
import {IdentityBackendServices} from "../../../services/identity-backend.service";
import {MessageService, ObjectCloner} from "@ianitor/shared-services";
import {GraphQLCloneIgnoredProperties} from "@ianitor/osp-services";
import {RolesDataSource} from "../../../shared/rolesDataSource";
import {AbstractDetailsComponent} from "@ianitor/shared-ui";
import {RoleDto} from "../../../models/roleDto";
import {ApiError} from "../../../models/apiError";

@Component({
  selector: 'app-user-details',
  templateUrl: './user-details.component.html',
  styleUrls: ['./user-details.component.scss']
})
export class UserDetailsComponent extends AbstractDetailsComponent<UserDto> implements OnInit {

  private readonly userName: string;
  public apiError: ApiError;

  constructor(route: ActivatedRoute, private fb: FormBuilder, private identityBackendServices: IdentityBackendServices, private messageService: MessageService, public rolesDataSource: RolesDataSource) {
    super();

    this.userName = route.snapshot.paramMap.get('userName');

    this._ownerForm = this.fb.group({
      firstName: [''],
      lastName: [''],
      name: ['', [Validators.required]],
      email: ['', [Validators.required, Validators.email]],
      roles: ['', Validators.required]
    });
  }

  ngOnInit(): void {


    if (this.userName) {
      this.identityBackendServices.getUserDetails(this.userName).subscribe(value => {
        this.entity = value;

        this.ownerForm.setValue({
          firstName: value.firstName,
          lastName: value.lastName,
          name: value.name,
          email: value.email,
          roles: value.roles[0],
        });
        this.loading = false;

      }, error => {

        this.apiError = error;
        this.loading = false;
      })
    } else {
      this.entity = <UserDto>{};

      this.ownerForm.setValue(
        {
          firstName: null,
          lastName: null,
          name: null,
          email: null,
          roles: null
        }
      );
      this.loading = false;
    }
  }

  save(): void {

    this.apiError = null;

    this.ownerForm.disable();
    this.ownerForm.updateValueAndValidity();

    const userDto = ObjectCloner.cloneObject<UserDto, any>(this.ownerForm.value, [...GraphQLCloneIgnoredProperties, 'roles']);

    userDto.roles = new Array<RoleDto>();
    userDto.roles.push(this.ownerForm.get('roles').value);

    if (this.userName) {
      this.identityBackendServices.updateUser(this.userName, userDto).subscribe(_ => {
        this.messageService.showInformation(`User '${userDto.email}' has been updated.`);

        this.ownerForm.reset();
        this.ownerForm.enable();

      }, (error: ApiError) => {

        this.apiError = error;

        this.ownerForm.enable();
      })


    } else {
      this.identityBackendServices.createUser(userDto).subscribe(_ => {
        this.messageService.showInformation(`User '${userDto.email}' has been created.`);

        this.ownerForm.reset();
        this.ownerForm.enable();

      }, (error: ApiError) => {

        this.apiError = error;

        this.ownerForm.enable();
      })
    }
  }
}
