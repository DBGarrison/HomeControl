import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AppScenesRoutingModule } from './appscenes-routing.module';
import { AppScenesComponent } from '../app-scenes/app-scenes.component';
import { AppSceneComponent } from '../app-scene/app-scene.component';

@NgModule({
  imports: [ CommonModule, AppScenesRoutingModule ],
  declarations: [ AppScenesComponent, AppSceneComponent ],
  exports: [ AppScenesComponent, AppSceneComponent ]
})
export class AppScenesModule { }
