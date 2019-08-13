import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { routing } from './app.routing';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { VgCoreModule } from 'videogular2/core';
import { VgControlsModule } from 'videogular2/controls';
import { VgOverlayPlayModule } from 'videogular2/overlay-play';
import { VgBufferingModule } from 'videogular2/buffering';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { FanControlModule } from './fancontrol/fancontrol.module';
import { CamControlModule } from './camcontrol/camcontrol.module';
import { HS100ControlModule } from './hs100control/hs100control.module';
import { AppScenesModule } from './appscenes/appscenes.module';
import { RFSwitchControlModule } from './rfswitchcontrol/rfswitchcontrol.module';

import { FanService } from './fancontrol/fan.service';
import { HS100Service } from './hs100control/hs100.service';
import { RFSwitchService } from './rfswitchcontrol/rfswitch.service';
import { AppScenesService } from './appscenes/appscenes.service';
 
@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent
    //AppSceneComponent   
   // HS100ControlComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    BrowserAnimationsModule,
    FormsModule,
    VgCoreModule,
    VgControlsModule,
    VgOverlayPlayModule,
    VgBufferingModule,
    FanControlModule,
    CamControlModule,
    HS100ControlModule,
    AppScenesModule,
    RFSwitchControlModule,
    routing
  ],
  providers: [FanService, HS100Service, AppScenesService, AppScenesService, RFSwitchService],
  bootstrap: [AppComponent]
})
export class AppModule { }
