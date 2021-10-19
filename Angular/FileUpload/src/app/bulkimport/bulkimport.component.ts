import { Component, OnInit } from '@angular/core';
import { MemberBulkImportVM } from '../models/memberBulkImportVM';
import { ResultModel } from '../models/result';
import {BulkimportService } from '../services/bulkimport.service';

@Component({
  selector: 'app-bulkimport',
  templateUrl: './bulkimport.component.html',
  styleUrls: ['./bulkimport.component.css']
})
export class BulkimportComponent implements OnInit {
  sucessData: Array<ResultModel<MemberBulkImportVM>> = [];
  errorData: Array<ResultModel<MemberBulkImportVM>> = [];
  file: File = new File([],'temp');
  constructor(private bulkimportServices:BulkimportService) { }

  fileChange(event:any): void {
    const fileList: FileList = event.target.files;
    if (fileList.length > 0) {
      this.file = fileList[0];
      console.log("File name : ", this.file.name);
    }
  }
  onUpload() {
    console.log("Upload called", this.file.name);
    this.bulkimportServices.uploadFile(this.file).subscribe((res) => {
      console.log("API data", res);
      this.sucessData = [];
      this.errorData = [];
      this.sucessData = res.filter(r => r.success);
      this.errorData = res.filter(r => !(r.success));
      console.log("Error data", this.errorData);
      console.log("Sucess data",  this.sucessData);
    });
    
}
  ngOnInit(): void {
  }

}
