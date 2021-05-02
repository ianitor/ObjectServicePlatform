import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UserDiagnosticsComponent } from './user-diagnostics.component';

describe('UserDiagnosticsComponent', () => {
  let component: UserDiagnosticsComponent;
  let fixture: ComponentFixture<UserDiagnosticsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ UserDiagnosticsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UserDiagnosticsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
