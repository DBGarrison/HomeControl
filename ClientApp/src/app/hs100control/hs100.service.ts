import { Inject, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Injectable } from '@angular/core';
import { RecursiveTemplateAstVisitor } from '@angular/compiler';

@Injectable()
export class HS100Service implements OnInit {
  public statusList: HS100Status[];
  public lastUpdate = 'Unavailable';
  private _baseUrl: string;
  private _basehs100Url: string;
  private _http: HttpClient;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this._baseUrl = baseUrl;
    this._http = http;
    console.log('HS100Service.constructor(): Entered');     
  }

  ngOnInit() {
    // this.callAPI('getStatus');
  }

  public callAPI(apiCall: string): HS100Status[] {
    console.log('HS100Service.callAPI() Entered. apiCall: ' + apiCall);
    let _url: string = this._baseUrl + 'api/HS100/' + apiCall;

    this._http.get<HS100Status[]>(_url).subscribe(result => {
      this.lastUpdate = new Date().toLocaleTimeString();
      this.statusList = result;
    }, error => {
      console.error(apiCall + ' Failed!.');
      console.error(error);

    });

    return this.statusList;
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

