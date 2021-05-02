import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CkEntityAttributeDetailsComponent } from './ck-entity-attribute-details.component';

describe('DetailsComponent', () => {
  let component: CkEntityAttributeDetailsComponent;
  let fixture: ComponentFixture<CkEntityAttributeDetailsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CkEntityAttributeDetailsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CkEntityAttributeDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
