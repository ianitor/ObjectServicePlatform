import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FieldFilterEditorComponent } from './field-filter-editor.component';

describe('FieldFilterEditorComponent', () => {
  let component: FieldFilterEditorComponent;
  let fixture: ComponentFixture<FieldFilterEditorComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FieldFilterEditorComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FieldFilterEditorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
