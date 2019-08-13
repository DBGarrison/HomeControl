import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
 
import { RFSwitchControlComponent } from '../rfswitch-control/rfswitch-control.component';


const routes: Routes = [{ path: 'rfswitch-control', component: RFSwitchControlComponent }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class RFSwitchControlRoutingModule { }
