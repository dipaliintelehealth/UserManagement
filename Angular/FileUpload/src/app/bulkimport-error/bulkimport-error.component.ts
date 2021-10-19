import { Component, OnInit, Input, ViewChild } from '@angular/core';
import { DataTableDirective } from 'angular-datatables';
import { Subject } from 'rxjs';
import { MemberBulkImportVM } from '../models/memberBulkImportVM';
import { ResultModel } from '../models/result';

@Component({
  selector: 'app-bulkimport-error',
  templateUrl: './bulkimport-error.component.html',
  styleUrls: ['./bulkimport-error.component.css'],
})
export class BulkimportErrorComponent implements OnInit {
  @ViewChild(DataTableDirective)
  dtElement!: DataTableDirective;
  dtTrigger: Subject<any> = new Subject();

  @Input() data: Array<ResultModel<MemberBulkImportVM>> = [];
  dtOptions: any = {};
  constructor() {}

  ngOnInit(): void {
    this.dtOptions = {
      // Declare the use of the extension in the dom parameter
      dom: 'Bfrtip',
      // Configure the buttons
      buttons: ['copy', 'csv', 'excel'],
    };
  }
  ngAfterViewInit(): void {
    this.dtTrigger.next();
  }

  ngOnDestroy(): void {
    // Do not forget to unsubscribe the event
    this.dtTrigger.unsubscribe();
  }

  rerender(): void {
    this.dtElement.dtInstance.then((dtInstance: DataTables.Api) => {
      // Destroy the table first
      dtInstance.destroy();
      // Call the dtTrigger to rerender again
      this.dtTrigger.next();
    });
  }
}
