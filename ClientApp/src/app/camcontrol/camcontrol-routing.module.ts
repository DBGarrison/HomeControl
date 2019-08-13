import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { CameraDashboardComponent } from '../camera-dashboard/camera-dashboard.component';
import { VideoPlaylistComponent } from '../video-playlist/video-playlist.component';

 
import { PiCam4Component } from '../pi-cam4/picam4.component';
import { PiCam5Component } from '../pi-cam5/picam5.component';
import { PiCam6Component } from '../pi-cam6/picam6.component';
import { PiCam7Component } from '../pi-cam7/picam7.component';
import { PiCam10Component } from '../pi-cam10/picam10.component';
import { PiCam11Component } from '../pi-cam11/picam11.component';

const routes: Routes = [
  { path: 'camera-dashboard', component: CameraDashboardComponent },
  { path: 'video-playlist', component: VideoPlaylistComponent },
  { path: 'pi-camera4', component: PiCam4Component },
  { path: 'pi-camera5', component: PiCam5Component },
  { path: 'pi-camera6', component: PiCam6Component },
  { path: 'pi-camera7', component: PiCam7Component },
  { path: 'pi-camera10', component: PiCam10Component },
  { path: 'pi-camera11', component: PiCam11Component },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CamControlRoutingModule { }
