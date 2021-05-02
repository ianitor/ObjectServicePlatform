import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { NotificationTemplateDetailsComponent } from './notification-template-details.component';

describe('NotificationTemplateDetailsComponent', () => {
  let component: NotificationTemplateDetailsComponent;
  let fixture: ComponentFixture<NotificationTemplateDetailsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ NotificationTemplateDetailsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(NotificationTemplateDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
