import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { QueryNameComponent } from './query-name.component';

describe('QueryNameComponent', () => {
  let component: QueryNameComponent;
  let fixture: ComponentFixture<QueryNameComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ QueryNameComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(QueryNameComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
