import {Component, OnInit} from '@angular/core';
import {UserDto} from "../../../models/userDto";
import {AbstractDetailsComponent} from "@ianitor/shared-ui";
import {ActivatedRoute} from "@angular/router";
import {IdentityBackendServices} from "../../../services/identity-backend.service";
import {MessageService, ObjectCloner} from "@ianitor/shared-services";
import {AbstractControl, FormControl, FormGroup, Validators} from "@angular/forms";
import {GraphQLCloneIgnoredProperties} from "@ianitor/osp-services";
import {ApiError} from "../../../models/apiError";

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.scss']
})
export class ResetPasswordComponent extends AbstractDetailsComponent<UserDto> implements OnInit {

  private readonly userName: string;
  private userDto: UserDto;
  public apiError : ApiError;

  constructor(route: ActivatedRoute, private identityBackendServices: IdentityBackendServices, private messageService: MessageService) {
    super();

    this.userName = route.snapshot.paramMap.get('userName');

    this._ownerForm = new FormGroup({
      'userName': new FormControl({value: null, disabled: true}, Validators.required),
      'password': new FormControl(null, Validators.required),
      'passwordConfirmation': new FormControl(null, [Validators.required, this.checkValue()]),
    });
  }

  ngOnInit(): void {

    this.identityBackendServices.getUserDetails(this.userName).subscribe(value => {
      this.userDto = value;

      this.ownerForm.setValue({
        userName: value.name,
        password: null,
        passwordConfirmation: null
      });
      this.loading = false;

    }, error => {

      this.apiError = error;
      this.loading = false;
    });
  }

  save(): void {

    this.apiError = null;

    this.ownerForm.disable();
    this.ownerForm.updateValueAndValidity();

    const password = this.ownerForm.get('password').value;

    this.identityBackendServices.resetPassword(this.userDto.name, password).subscribe(_ => {
      this.messageService.showInformation(`Password of user '${this.userDto.name}' has been changed.`);

      this.ownerForm.reset();
      this.ownerForm.enable();

    }, (error: ApiError) => {

      this.apiError = error;

      this.ownerForm.enable();
    })
  }

  checkValue() {

    return (control: AbstractControl) => {
      const value = control.value;
      return value === control.parent?.get('password').value ? null : {notSame: true}
    }
  }
}
