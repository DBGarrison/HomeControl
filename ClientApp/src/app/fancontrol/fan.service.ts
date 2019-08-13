import { Inject, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Injectable } from '@angular/core';

@Injectable()
export class FanService implements OnInit {
  public fanStatus: FanStatus;
  public lastUpdate = 'Unavailable';
  private _baseUrl: string;
  private _baseFanUrl: string;
  private _http: HttpClient;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this._baseUrl = baseUrl;
    this._http = http;
    console.log('FanService.constructor(): Entered');     
  }

  ngOnInit() {
    // this.callAPI('getStatus');
  }

  public callAPI(apiCall: string): FanStatus {
    console.log('FanService.callAPI() Entered. apiCall: ' + apiCall);     
    let _url: string = this._baseUrl + 'api/Fan/' + apiCall;

    this._http.get<FanStatus>(_url).subscribe(result => {
      this.lastUpdate = new Date().toLocaleTimeString();
      this.fanStatus = result;       
    }, error => {
      console.error(apiCall + ' Failed!.');
      console.error(error);
      
    });

    return this.fanStatus;
  }
}

export interface FanStatus {
  fanSpeed: number;
  lightsIsOn: boolean;
  isForward: boolean;
  chkInterval: number;
}
