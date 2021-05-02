import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RtQueryBuilderComponent } from './rt-query-builder.component';

describe('RtQueryBuilderComponent', () => {
  let component: RtQueryBuilderComponent;
  let fixture: ComponentFixture<RtQueryBuilderComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RtQueryBuilderComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RtQueryBuilderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
