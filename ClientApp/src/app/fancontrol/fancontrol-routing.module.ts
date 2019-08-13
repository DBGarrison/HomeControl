import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
 
import { FanControlComponent } from '../fan-control/fan-control.component';


const routes: Routes = [{ path: 'fan-control', component: FanControlComponent }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class FanControlRoutingModule { }
