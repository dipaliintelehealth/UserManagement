import { TestBed } from '@angular/core/testing';

import { BulkimportService } from './bulkimport.service';

describe('BulkimportService', () => {
  let service: BulkimportService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(BulkimportService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
