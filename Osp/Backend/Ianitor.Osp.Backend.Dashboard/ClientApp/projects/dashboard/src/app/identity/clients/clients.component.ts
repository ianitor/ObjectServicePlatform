import {Component, OnInit} from '@angular/core';
import {IdentityBackendServices} from "../../services/identity-backend.service";
import {ClientsDataSource} from "../../shared/clientsDataSource";
import {ClientDto} from "../../models/clientDto";

@Component({
  selector: 'app-clients',
  templateUrl: './clients.component.html',
  styleUrls: ['./clients.component.scss']
})
export class ClientsComponent implements OnInit {

  dataColumns: string[] = ['isEnabled', 'clientId', 'clientName', 'clientUri'];
  displayedColumns: string[] = [...this.dataColumns, 'actions'];

  constructor(private identityBackendServices: IdentityBackendServices, public dataSource: ClientsDataSource) {
  }

  ngOnInit() {

    this.dataSource.loadData();
  }

  onDelete(clientDto: ClientDto) {
    if(window.confirm('Are you sure to delete this item?')){
      this.identityBackendServices.deleteClient(clientDto.clientId).subscribe(_ => {
        this.dataSource.loadData();
      });
    }
  }

}
