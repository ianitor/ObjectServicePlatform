import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PageTemplateDetailsComponent } from './page-template-details.component';

describe('PageTemplateDetailsComponent', () => {
  let component: PageTemplateDetailsComponent;
  let fixture: ComponentFixture<PageTemplateDetailsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PageTemplateDetailsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PageTemplateDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
