import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RFSwitchControlRoutingModule } from './rfswitchcontrol-routing.module';
import { RFSwitchControlComponent } from '../rfswitch-control/rfswitch-control.component';
 
@NgModule({
  imports: [
    CommonModule,
    RFSwitchControlRoutingModule
  ],
  declarations: [
    RFSwitchControlComponent
  ],
  exports: [
    RFSwitchControlComponent
  ]
})
export class RFSwitchControlModule { }
