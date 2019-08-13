import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FanControlRoutingModule } from './fancontrol-routing.module';
import { FanControlComponent } from '../fan-control/fan-control.component';
//import { FanService } from './fan.service';

@NgModule({
  imports: [
    CommonModule,
    FanControlRoutingModule
  ],
  declarations: [
    FanControlComponent
  ],
  exports: [
    FanControlComponent
  ]//,
  //providers: [FanService]
})
export class FanControlModule { }
