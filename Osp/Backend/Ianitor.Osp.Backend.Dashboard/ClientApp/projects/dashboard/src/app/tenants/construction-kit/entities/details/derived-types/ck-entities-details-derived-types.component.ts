import {Component, Input, OnChanges, SimpleChanges} from '@angular/core';
import {CkEntityDetailDto} from "../../../../../models/ckEntityDetailDto";
import {CkEntityTypeInfo} from "../../../../../models/ckEntityTypeInfo";
import {GenericDataSource} from "@ianitor/shared-services";

@Component({
  selector: 'app-ck-entities-details-derived-types',
  templateUrl: './ck-entities-details-derived-types.component.html',
  styleUrls: ['./ck-entities-details-derived-types.component.css']
})
export class CkEntitiesDetailsDerivedTypesComponent implements OnChanges {

  @Input() constructionKitTypeDetail : CkEntityDetailDto;

  public dataSource: GenericDataSource<CkEntityTypeInfo>;
  isMobile: boolean;
  dataColumns: string[] = ['ckId'];
  displayedColumns: string[] = [...this.dataColumns, 'actions'];

  constructor() {
  }

  ngOnInit(): void {


  }

  ngOnChanges(changes: SimpleChanges): void {

    this.dataSource = new GenericDataSource<CkEntityTypeInfo>(this.constructionKitTypeDetail?.derivedTypes);

  }

}
