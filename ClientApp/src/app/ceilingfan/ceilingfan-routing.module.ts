import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
 
import { CeilingFanComponent } from '../ceiling-fan/ceiling-fan.component';

const routes: Routes = [{ path: 'ceiling-fan', component: CeilingFanComponent }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CeilingFanRoutingModule { }
