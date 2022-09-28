import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DropboxTreeComponent } from './dropbox-tree.component';

describe('DropboxTreeComponent', () => {
  let component: DropboxTreeComponent;
  let fixture: ComponentFixture<DropboxTreeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DropboxTreeComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DropboxTreeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
