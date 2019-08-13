import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
 
import { HS100ControlComponent } from '../hs100-control/hs100-control.component';


const routes: Routes = [{ path: 'hs100-control', component: HS100ControlComponent }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class HS100ControlRoutingModule { }
