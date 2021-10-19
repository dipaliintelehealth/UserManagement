import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BulkimportSucessComponent } from './bulkimport-sucess.component';

describe('BulkimportSucessComponent', () => {
  let component: BulkimportSucessComponent;
  let fixture: ComponentFixture<BulkimportSucessComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ BulkimportSucessComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(BulkimportSucessComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
