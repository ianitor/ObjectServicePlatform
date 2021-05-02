import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CkEntitiesComponent } from './ck-entities.component';

describe('EntitiesComponent', () => {
  let component: CkEntitiesComponent;
  let fixture: ComponentFixture<CkEntitiesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CkEntitiesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CkEntitiesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
