import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CkEntityAttributesComponent } from './ck-entity-attributes.component';

describe('EntityAttributesComponent', () => {
  let component: CkEntityAttributesComponent;
  let fixture: ComponentFixture<CkEntityAttributesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CkEntityAttributesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CkEntityAttributesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
