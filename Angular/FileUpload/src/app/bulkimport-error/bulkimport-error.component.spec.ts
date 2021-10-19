import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BulkimportErrorComponent } from './bulkimport-error.component';

describe('BulkimportErrorComponent', () => {
  let component: BulkimportErrorComponent;
  let fixture: ComponentFixture<BulkimportErrorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ BulkimportErrorComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(BulkimportErrorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
