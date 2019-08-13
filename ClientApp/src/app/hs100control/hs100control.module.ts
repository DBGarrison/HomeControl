import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HS100ControlRoutingModule } from './hs100control-routing.module';
import { HS100ControlComponent } from '../hs100-control/hs100-control.component';
 
@NgModule({
  imports: [
    CommonModule,
    HS100ControlRoutingModule
  ],
  declarations: [
    HS100ControlComponent
  ],
  exports: [
    HS100ControlComponent
  ]
})
export class HS100ControlModule { }
