import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ServiceHooksComponent } from './service-hooks.component';

describe('ServiceHooksComponent', () => {
  let component: ServiceHooksComponent;
  let fixture: ComponentFixture<ServiceHooksComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ServiceHooksComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ServiceHooksComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
