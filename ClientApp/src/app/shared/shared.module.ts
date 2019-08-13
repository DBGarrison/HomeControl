import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpModule, Http, XHRBackend, RequestOptions } from '@angular/http';

import { PiCameraComponent } from './picamera/picamera.component'; 

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    HttpModule,
  ],
  declarations: [
    PiCameraComponent,
  ],
  exports: [PiCameraComponent, CommonModule, FormsModule, HttpModule, ReactiveFormsModule]
})
export class SharedModule { }
