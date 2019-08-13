import { Inject, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Injectable } from '@angular/core';
import { RecursiveTemplateAstVisitor } from '@angular/compiler';
import { forEach } from '@angular/router/src/utils/collection';

@Injectable()
export class RFSwitchService implements OnInit {
  public detections: RFSwitchDevice[];
  public lastUpdate = 'Unavailable';
  private _baseUrl: string;
  //private _basehs100Url: string;
  private _http: HttpClient;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this._baseUrl = baseUrl;
    this._http = http;
    console.log('RFSwitchService.constructor(): Entered');     
  }

  ngOnInit() {
    // this.callAPI('getStatus');
  }

  public callAPI(apiCall: string): RFSwitchDevice[] {
    console.log('RFSwitchService.callAPI() Entered. apiCall: ' + apiCall);
    let _url: string = this._baseUrl + 'api/rf/' + apiCall;

    this._http.get<RFSwitchDevice[]>(_url).subscribe(result => {
      this.lastUpdate = new Date().toLocaleTimeString();
      this.detections = result;
      /*
      if (this.detections == null) {
        this.detections = result;
      }
      else {
        //get the removed
        //const fresh = result.map(r => r.localDeviceId);
        //const stale = this.detections.map(r => r.localDeviceId);

        ////const removed = fresh.filter(r => !stale.includes(r)); 
        //const added = stale.filter(a => !fresh.includes(a));
        //const existing = fresh.filter(e => fresh.includes(e));

        //removed.forEach(r => {
        //  var idx = this.detections.findIndex(q => q.localDeviceId == r);
        //  this.detections.splice(idx, 1);
        //});

       let removed: RFSwitchDevice[] = new Array();
        this.detections.forEach(s => {
          var sw = result.find(q => q.localDeviceId == s.localDeviceId);
          if (sw == null) {
            removed.push(s);
          }
        });

        removed.forEach(r => {
          var idx = this.detections.findIndex(q => q.localDeviceId == r.localDeviceId);
          this.detections.splice(idx, 1);
        });

        result.forEach(r => {
          var sw = this.detections.find(q => q.localDeviceId == r.localDeviceId);
          if (sw == null) {
            this.detections.push(r);
          }
          else {
            var sw = this.detections.find(q => q.localDeviceId == r.localDeviceId);
             
            this.copySwitchProps(r, sw);
          }
        });
         
      }*/
    }, error => {
      console.error(apiCall + ' Failed!.');
      console.error(error);

    });

    return this.detections;
  }
  public setSwitch(localDeviceId: number, swState: number): RFSwitchDevice {
    console.log('RFSwitchService.setSwitch() Entered. localDeviceId: ' + localDeviceId.toString());
    let _url: string = this._baseUrl + 'api/rf/setswitch/' + localDeviceId.toString() + '/' + swState.toString();
    let retVal: RFSwitchDevice = null;

    this._http.get<RFSwitchDevice>(_url).subscribe(result => {
                       
      let retVal: RFSwitchDevice = this.detections.find(q => q.localDeviceId == result.localDeviceId);
      if (retVal) {
        this.copySwitchProps(result, retVal);
      }       
      return retVal;
                       
    }, error => {
      console.error(_url + ' Failed!.');
      console.error(error);

    });

    return null;
  }

  private copySwitchProps(from: RFSwitchDevice, to: RFSwitchDevice) {

    to.area = from.area;
    to.areaName = from.areaName;
    to.deviceName = from.deviceName;
    to.deviceType = from.deviceType;
    to.events = from.events;

    to.lastOffCode = from.lastOffCode;
    to.lastOnCode = from.lastOnCode;
    to.swState = from.swState;


    to.rfCodes = from.rfCodes;
    to.slaves = from.slaves;
  }
}

export interface RFSwitchDevice {
  area: number;
  areaName: string;
  localDeviceId: number;
  deviceType: number;
  deviceName: string;
  swState: number;

  lastOnCode: number;
  lastOffCode: number;
   
  events: AreaEvent[]; 
  rfCodes: RfCode[];
  slaves: MCU[];    
}

export interface MCU {
  ipAddress: string;
  remoteDeviceId: number;
  configId: number;
}

export interface RfCode {
  isOn: boolean;
  code: number;   
}

export interface AreaEvent {
  area: number;
  areaName: string;
  completer_IP: string;
  deviceName: string;
  eventStatus: number;
  eventType: number;
  initiator_IP: string;
}
