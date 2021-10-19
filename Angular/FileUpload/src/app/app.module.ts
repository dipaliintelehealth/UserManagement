import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HttpClientModule } from '@angular/common/http';
import { BulkimportComponent } from './bulkimport/bulkimport.component';
import { DataTablesModule } from "angular-datatables";
import { BulkimportSucessComponent } from './bulkimport-sucess/bulkimport-sucess.component';
import { BulkimportErrorComponent } from './bulkimport-error/bulkimport-error.component';

@NgModule({
  declarations: [
    AppComponent,
    BulkimportComponent,
    BulkimportSucessComponent,
    BulkimportErrorComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    DataTablesModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
