import { Component, OnInit } from '@angular/core';
import { DiagnosticsModel } from '../../models/diagnosticsModel';
import { IdentityBackendServices } from '../../services/identity-backend.service';

@Component({
  selector: 'app-user-diagnostics',
  templateUrl: './user-diagnostics.component.html',
  styleUrls: ['./user-diagnostics.component.css']
})
export class UserDiagnosticsComponent implements OnInit {
  diagnostics: DiagnosticsModel;
  loading: boolean;

  constructor(private identityBackendServices: IdentityBackendServices) {
  }

  ngOnInit() {
    this.loading = true;
    this.identityBackendServices.userDiagnostics()
      .subscribe(dm => {
        this.diagnostics = dm;
        this.loading = false;
      });
  }

}
