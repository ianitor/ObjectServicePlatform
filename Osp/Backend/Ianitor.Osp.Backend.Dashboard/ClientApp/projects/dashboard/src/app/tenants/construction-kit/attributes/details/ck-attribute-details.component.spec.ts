import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CkAttributeDetailsComponent } from './ck-attribute-details.component';

describe('DetailsComponent', () => {
  let component: CkAttributeDetailsComponent;
  let fixture: ComponentFixture<CkAttributeDetailsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CkAttributeDetailsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CkAttributeDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
