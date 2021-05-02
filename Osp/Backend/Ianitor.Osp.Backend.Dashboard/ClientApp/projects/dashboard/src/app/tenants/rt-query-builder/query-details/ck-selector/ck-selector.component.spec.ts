import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CkSelectorComponent } from './ck-selector.component';

describe('CkSelectorComponent', () => {
  let component: CkSelectorComponent;
  let fixture: ComponentFixture<CkSelectorComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CkSelectorComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CkSelectorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
