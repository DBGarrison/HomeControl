import { HomeComponent } from './home/home.component';
import { FanControlModule } from './fancontrol/fancontrol.module';
import { CamControlModule } from './camcontrol/camcontrol.module';
import { HS100ControlModule } from './hs100control/hs100control.module';
import { HS100ControlComponent } from './hs100-control/hs100-control.component';
 
import { Routes, RouterModule } from '@angular/router';
import { ModuleWithProviders } from '@angular/core';

export const routes: Routes = [
  { path: '', component: HomeComponent, pathMatch: 'full' },
  { path: 'home', component: HomeComponent, pathMatch: 'full' },
  { path: 'fan-control', loadChildren: './fancontrol/fancontrol.module#FanControlModule' },
  { path: 'camera-dashboard', loadChildren: './camcontrol/camcontrol.module#CamControlModule' },
  { path: 'video-playlist', loadChildren: './camcontrol/camcontrol.module#CamControlModule' },
  { path: 'pi-camera1', loadChildren:   './camcontrol/camcontrol.module#CamControlModule' },
  { path: 'pi-camera2', loadChildren:   './camcontrol/camcontrol.module#CamControlModule' },
  { path: 'pi-camera3', loadChildren:   './camcontrol/camcontrol.module#CamControlModule' },
  { path: 'pi-camera4', loadChildren: './camcontrol/camcontrol.module#CamControlModule' },
  { path: 'pi-camera5', loadChildren: './camcontrol/camcontrol.module#CamControlModule' },
  { path: 'pi-camera6', loadChildren: './camcontrol/camcontrol.module#CamControlModule' },
  { path: 'pi-camera7', loadChildren: './camcontrol/camcontrol.module#CamControlModule' },
  { path: 'hs100-control', loadChildren: './hs100control/hs100control.module#HS100ControlModule' }
];

export const routing: ModuleWithProviders = RouterModule.forRoot(routes);
