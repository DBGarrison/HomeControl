import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CeilingFanRoutingModule } from './ceilingfan-routing.module';
import { CeilingFanComponent } from '../ceiling-fan/ceiling-fan.component';

@NgModule({
  imports: [
    CommonModule,
    CeilingFanRoutingModule
  ],
  declarations: [
    CeilingFanComponent
  ],
  exports: [
    CeilingFanComponent
  ]  
})
export class FanControlModule { }
