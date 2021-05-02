import { Component, OnInit } from '@angular/core';
import {MessageService} from "@ianitor/shared-services";
import {ErrorMessage} from "@ianitor/shared-services";
import {MatDialog} from "@angular/material/dialog";
import {MessageDetailsComponent} from "../message-details/message-details.component";

@Component({
  selector: 'ia-notification-bar',
  templateUrl: './ia-notification-bar.component.html',
  styleUrls: ['./ia-notification-bar.component.css']
})
export class IaNotificationBarComponent implements OnInit {

  public errorMessage : ErrorMessage;

  constructor(private messageService: MessageService, private dialog: MatDialog) {
  }

  ngOnInit() {

    this.messageService.getLatestErrorMessage().subscribe(value => {
      this.errorMessage = value;
    });
  }

  onHide()
  {
    this.errorMessage = null;
  }

  onShowDetails()
  {
    this.dialog.open(MessageDetailsComponent, {
      data:{
        errorMessage: this.errorMessage
      }
    });
  }
}
