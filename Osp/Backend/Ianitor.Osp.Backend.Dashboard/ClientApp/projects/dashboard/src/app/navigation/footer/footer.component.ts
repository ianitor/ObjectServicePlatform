import {Component, OnInit} from '@angular/core';
import {VERSION} from "../../../../../../environments/currentVersion";

@Component({
  selector: 'app-footer',
  templateUrl: './footer.component.html',
  styleUrls: ['./footer.component.css']
})
export class FooterComponent implements OnInit {
  version = VERSION;

  constructor() {
  }

  ngOnInit() {
  }

}
