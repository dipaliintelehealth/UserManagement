import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { ResultModel } from '../models/result';
import {MemberBulkImportVM } from '../models/memberBulkImportVM';

@Injectable({
  providedIn: 'root'
})
export class BulkimportService {
  apiUrl = 'https://localhost:44324/api/BulkImport';
  constructor(private http:HttpClient) { }
  uploadFile(file: File): Observable<Array<ResultModel<MemberBulkImportVM>>>{
    let formData:FormData = new FormData();
        formData.append('formFile', file, file.name);
        let headers = new Headers();
        /** In Angular 5, including the header Content-Type can invalidate your request */
        headers.append('Content-Type', 'multipart/form-data');
    headers.append('Accept', 'application/json');
    
    const options = {
      headers: headers
     
    };

        
    return this.http.post<Array<ResultModel<MemberBulkImportVM>>>(this.apiUrl, formData);
}
}
