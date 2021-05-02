import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ServiceHookDetailsComponent } from './service-hook-details.component';

describe('ServiceHookDetailsComponent', () => {
  let component: ServiceHookDetailsComponent;
  let fixture: ComponentFixture<ServiceHookDetailsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ServiceHookDetailsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ServiceHookDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
