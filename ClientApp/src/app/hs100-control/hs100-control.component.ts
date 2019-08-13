import { Component,  OnInit, OnDestroy } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, Subscription } from 'rxjs/Rx';
import { delay } from 'q';

import { HS100Service } from '../hs100control/hs100.service';

@Component({
  selector: 'app-hs100-control',
  templateUrl: './hs100-control.component.html'
})
export class HS100ControlComponent implements OnInit, OnDestroy {
  public lastUpdate = 'Unavailable'
  private sub: Subscription; 
  public statusList: HS100Status[];
  hs100Service: HS100Service;

  constructor(_hs100Service: HS100Service ) {
    this.hs100Service = _hs100Service;
    console.log('HS100ControlComponent.constructor(): Entered');
  }

  ngOnInit() {
    var sts = this.hs100Service.callAPI('');     
    let cnt: number = 0;

    while (!sts && cnt++ < 3) {
      delay(1000);
      sts = this.hs100Service.callAPI('');
    }

    if (sts) {
      this.statusList = sts;
      this.lastUpdate = this.hs100Service.lastUpdate;
    }

    var ticker = Observable.timer(5000, 30000);
    this.sub = ticker.subscribe(x => this.reload());
  }

  ngOnDestroy() {
    this.sub.unsubscribe();
  }

  reload() {
    this.callWebApi('');
  }

  handleToggle(event: Event) {      
    let str: string = event.srcElement.id.substring(6);   
    //let _url: string = this._baseUrl + 'api/HS100/Toggle/' + str;
    this.callWebApi('Toggle/' + str);
    //console.log('callWebApi() Entered. Calling: ' + _url);

    //this._http.get<HS100Status[]>(_url).subscribe(result => {
    //  this.callWebApi('');
    //}, error => {
    //  console.error(_url + ' Failed!.');
    //  console.error(error);

    //  });

  }

  callWebApi(apiCall: string) {
    //let _url: string = this._baseUrl + 'api/HS100/' + apiCall;
    //console.log('callWebApi() Entered. Calinng: ' + _url);
    this.statusList = this.hs100Service.callAPI(apiCall);
    this.lastUpdate = this.hs100Service.lastUpdate;
  }  
}

export interface HS100Status {
  deviceId: number;
  errorCode: number;
  errorMessage: string;
  softwareVersion: string;
  hardwareVersion: string;
  deviceType: string;
  model: string;
  mac: string;
  id: string;
  hardwareID: string;
  firmwareID: string;
  oemid: string;
  alias: string;
  deviceName: string;
  iconHash: string;
  relayState: number;
  onTime: number;
  activeMode: string;
  feature: string;
  updating: number;
  rssi: number;
  ledOffState: number;
  latitude: number;
  longitude: number;
}

/*
export interface HS100StatusRAW {
  err_code: number;
  sw_ver: string;
  hw_ver: string;
  type: string;
  model: string;
  mac: string;
  deviceId: string;
  hwId: string;
  fwId: string;
  oemId: string;
  alias: string;
  dev_name: string;
  icon_hash: string;
  relay_state: number;
  on_time: number;
  active_mode: string;
  feature: string;
  updating: number;
  rssi: number;
  led_off: number;
  latitude: number;
  longitude: number;
}*/

//export interface HS100Status {
//  err_code: number;
//  sw_ver: string;
//  hw_ver: string;
//  type: string;
//  model: string;
//  mac: string;
//  deviceId: string;
//  hwId: string;
//  fwId: string;
//  oemId: string;
//  alias: string;
//  dev_name: string;
//  icon_hash: string;
//  relay_state: number;
//  on_time: number;
//  active_mode: string;
//  feature: string;
//  updating: number;
//  rssi: number;
//  led_off: number;
//  latitude: number;
//  longitude: number;
//}

export interface HS100SysInfo {
  status: HS100Status;
}

export interface HS100Result {
  system: HS100SysInfo;
}
