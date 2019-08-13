import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
 
import { AppScenesComponent } from '../app-scenes/app-scenes.component';


const routes: Routes = [{ path: 'app-scenes', component: AppScenesComponent }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AppScenesRoutingModule { }
