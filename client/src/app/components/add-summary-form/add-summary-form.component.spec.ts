import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddSummaryFormComponent } from './add-summary-form.component';

describe('AddSummaryFormComponent', () => {
  let component: AddSummaryFormComponent;
  let fixture: ComponentFixture<AddSummaryFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AddSummaryFormComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AddSummaryFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
