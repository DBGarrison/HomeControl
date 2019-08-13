import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
 
import { MatGridListModule } from '@angular/material';
import { CamControlRoutingModule } from './camcontrol-routing.module';

import { CameraDashboardComponent } from '../camera-dashboard/camera-dashboard.component';
import { VideoPlaylistComponent } from '../video-playlist/video-playlist.component';

import { PiCam1Component } from '../pi-cam1/picam1.component';
import { PiCam2Component } from '../pi-cam2/picam2.component';
import { PiCam3Component } from '../pi-cam3/picam3.component';
import { PiCam4Component } from '../pi-cam4/picam4.component';
import { PiCam5Component } from '../pi-cam5/picam5.component';
import { PiCam6Component } from '../pi-cam6/picam6.component';
import { PiCam7Component } from '../pi-cam7/picam7.component';
import { PiCam10Component } from '../pi-cam10/picam10.component';
import { PiCam11Component } from '../pi-cam11/picam11.component';

@NgModule({
  imports: [
    CommonModule,
    MatGridListModule,
    CamControlRoutingModule
  ],
  declarations: [
    CameraDashboardComponent,
    VideoPlaylistComponent,
    PiCam1Component,
    PiCam2Component,
    PiCam3Component,
    PiCam4Component,
    PiCam5Component,
    PiCam6Component,
    PiCam7Component,
    PiCam10Component,
    PiCam11Component
  ],
  exports: [
    CameraDashboardComponent,
    VideoPlaylistComponent,
    PiCam4Component,
    PiCam5Component,
    PiCam6Component,
    PiCam7Component,
    PiCam10Component,
    PiCam11Component
  ]
})
export class CamControlModule { }
