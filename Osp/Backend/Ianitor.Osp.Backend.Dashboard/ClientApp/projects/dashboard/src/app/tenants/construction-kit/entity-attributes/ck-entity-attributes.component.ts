import {Component, Input, OnChanges, SimpleChanges} from '@angular/core';
import {CkEntityDetailDto} from "../../../models/ckEntityDetailDto";
import {CkEntityAttributeDetailDto} from "../../../models/ckEntityAttributeDetailDto";
import {GenericDataSource} from "@ianitor/shared-services";

@Component({
  selector: 'app-ck-entity-attributes',
  templateUrl: './ck-entity-attributes.component.html',
  styleUrls: ['./ck-entity-attributes.component.css']
})
export class CkEntityAttributesComponent implements OnChanges {

  @Input() constructionKitTypeDetail: CkEntityDetailDto;

  public dataSource: GenericDataSource<CkEntityAttributeDetailDto>;
  isMobile: boolean;
  dataColumns: string[] = ['attributeName', 'attributeId', 'attributeValueType'];
  displayedColumns: string[] = [...this.dataColumns, 'actions'];


  constructor() {
  }

  ngOnInit(): void {
  }

  ngOnChanges(changes: SimpleChanges): void {

    this.dataSource = new GenericDataSource<CkEntityAttributeDetailDto>(this.constructionKitTypeDetail?.attributes);

  }
}
