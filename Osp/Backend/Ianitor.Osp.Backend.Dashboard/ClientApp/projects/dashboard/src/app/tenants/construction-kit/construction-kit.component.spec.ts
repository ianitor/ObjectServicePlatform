import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ConstructionKitComponent } from './construction-kit.component';

describe('ConstructionKitComponent', () => {
  let component: ConstructionKitComponent;
  let fixture: ComponentFixture<ConstructionKitComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ConstructionKitComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ConstructionKitComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
