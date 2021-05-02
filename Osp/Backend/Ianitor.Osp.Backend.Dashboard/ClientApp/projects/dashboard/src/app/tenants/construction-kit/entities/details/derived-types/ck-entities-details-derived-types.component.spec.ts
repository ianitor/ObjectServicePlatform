import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CkEntitiesDetailsDerivedTypesComponent } from './ck-entities-details-derived-types.component';

describe('DerivedTypesComponent', () => {
  let component: CkEntitiesDetailsDerivedTypesComponent;
  let fixture: ComponentFixture<CkEntitiesDetailsDerivedTypesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CkEntitiesDetailsDerivedTypesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CkEntitiesDetailsDerivedTypesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
