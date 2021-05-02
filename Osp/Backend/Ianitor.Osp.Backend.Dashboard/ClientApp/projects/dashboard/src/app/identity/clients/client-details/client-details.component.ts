import {Component, OnInit} from '@angular/core';
import {ActivatedRoute} from "@angular/router";
import {IdentityBackendServices} from "../../../services/identity-backend.service";
import {MessageService} from "@ianitor/shared-services";
import {AbstractDetailsComponent} from "../../../../../../ianitor/shared-ui/src/lib/shared/abstractDetailsComponent";
import {ClientDto} from "../../../models/clientDto";
import {FormBuilder, FormControl, Validators} from "@angular/forms";
import {GrantTypes} from "../../../models/grantTypes";
import {CommonValidators} from "@ianitor/shared-ui";
import {ClientScope} from "../../../models/clientScope";

@Component({
  selector: 'app-client-details',
  templateUrl: './client-details.component.html',
  styleUrls: ['./client-details.component.scss']
})

export class ClientDetailsComponent extends AbstractDetailsComponent<ClientDto> implements OnInit {

  private readonly clientId: string;

  public readonly grantTypes: GrantTypes[] = [
    {id: "implicit", name: "implicit"},
    {id: "authorization_code", name: "authorization_code"},
    {id: "client_credentials", name: "client_credentials"},
    {id: "urn:ietf:params:oauth:grant-type:device_code", name: "device"}
  ];

  public readonly clientScopes: ClientScope[] = [
    {id: "systemAPI.full_access", name: "System API Read/Write"},
    {id: "systemAPI.read_only", name: "System API Read"},
    {id: "jobAPI.full_access", name: "Job API Read/Write"},
    {id: "jobAPI.read_only", name: "Job API Read"},
    {id: "policyAPI.full_access", name: "Policy API Read/Write"},
    {id: "policyAPI.read_only", name: "Policy API Read"},
    {id: "identityAPI.full_access", name: "Identity API Read/Write"},
    {id: "identityAPI.read_only", name: "Identity API Read"},
    {id: "openid", name: "Open ID"},
    {id: "profile", name: "User Profile"},
    {id: "email", name: "User E-Mail"},
    {id: "role", name: "User Roles"},
  ];

  public readonly redirectUriInput: FormControl;
  public readonly postLogoutRedirectUriInput: FormControl;
  public readonly allowedCorsOriginsInput: FormControl;

  constructor(route: ActivatedRoute, private fb: FormBuilder, private identityBackendServices: IdentityBackendServices, private messageService: MessageService) {

    super();

    this.redirectUriInput = new FormControl(null, [Validators.required, CommonValidators.httpUri()]);
    this.postLogoutRedirectUriInput = new FormControl(null, [Validators.required, CommonValidators.httpUri()]);
    this.allowedCorsOriginsInput = new FormControl(null, [Validators.required, CommonValidators.httpUri()]);

    this.clientId = route.snapshot.paramMap.get('id');

    this._ownerForm = this.fb.group({
      isEnabled: ['', Validators.required],
      clientId: ['', Validators.required],
      clientName: ['', Validators.required],
      clientUri: ['', null],
      clientSecret: ['', null],
      allowedGrantTypes: ['', Validators.required],
      redirectUris: ['', Validators.required],
      postLogoutRedirectUris: ['', Validators.required],
      allowedCorsOrigins: ['', Validators.required],
      allowedScopes: ['', Validators.required],
      isOfflineAccessEnabled: ['', Validators.required]
    });

  }

  ngOnInit(): void {

    if (this.clientId) {
      this.identityBackendServices.getClientDetails(this.clientId).subscribe(value => {
        this.entity = value;

        this.ownerForm.setValue({
          isEnabled: value.isEnabled,
          clientId: value.clientId,
          clientName: value.clientName,
          clientUri: value.clientUri,
          clientSecret: value.clientSecret,
          allowedGrantTypes: value.allowedGrantTypes,
          redirectUris: value.redirectUris,
          postLogoutRedirectUris: value.postLogoutRedirectUris,
          allowedCorsOrigins: value.allowedCorsOrigins,
          allowedScopes: value.allowedScopes,
          isOfflineAccessEnabled: value.isOfflineAccessEnabled
        });
        this.loading = false;

      })
    } else {
      this.entity = <ClientDto>{};

      this.ownerForm.setValue(
        {
          isEnabled: false,
          clientId: null,
          clientName: null,
          clientUri: null,
          clientSecret: null,
          allowedGrantTypes: null,
          redirectUris: new Array<string>(),
          postLogoutRedirectUris: new Array<string>(),
          allowedCorsOrigins: new Array<string>(),
          allowedScopes: null,
          isOfflineAccessEnabled: false
        }
      );
      this.loading = false;
    }
  }


  save() {
    this.loading = true;

    const transient = <ClientDto> this.ownerForm.value;

    Object.assign(this.entity, transient);

    if (this.clientId)
    {
      this.identityBackendServices.updateClient(this.clientId, transient).subscribe(value => {

        this.entity = value;
        this.loading = false;
        this.messageService.showInformation("Client saved.");
      }, error => {

        this.loading = false;
        this.messageService.showError(error, "Error during saving of client.");
      });
    }
    else {
      this.identityBackendServices.createClient(transient).subscribe(value => {

        this.entity = value;
        this.loading = false;

        this.messageService.showInformation("Client created.");

      }, error => {

        this.loading = false;
        this.messageService.showError(error, "Error during saving of client.");
      });
    }
  }

  add(input : FormControl, listName: string){
    const value = input.value;


   // this.ownerForm.get(inputName).updateValueAndValidity();
    if (input.invalid) {
      return;
    }

    const list = this.ownerForm.get(listName).value;
    if (list.indexOf(value) !== -1)
    {
      return;
    }

    list.push(value);
    input.reset(null);
    input.markAsUntouched();
    this.ownerForm.get(listName).updateValueAndValidity();
  }

  remove(listName: string, uri: string) {
    const list = this.ownerForm.get(listName).value;
    list.pop(uri);
    this.ownerForm.get(listName).updateValueAndValidity();

  }
}
