import { Inject, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Injectable } from '@angular/core';
import { RecursiveTemplateAstVisitor } from '@angular/compiler';
import { retry } from 'rxjs/operator/retry';
//import { basename } from 'path';

@Injectable()
export class AppScenesService implements OnInit {
  public appScenes: AppScene[];
  public lastUpdate = 'Unavailable';
  private _baseUrl: string;
  private _http: HttpClient;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this._baseUrl = baseUrl;
    this._http = http;
    console.log('AppScenesService.constructor(): Entered');     
  }

  ngOnInit() {
    // this.callAPI('getStatus');
  }

  public getScenes(): AppScene[] {
    console.log('AppScenesService.getScenes() Entered.');
    let _url: string = this._baseUrl + 'api/AppScenes/';

    this._http.get<AppScene[]>(_url).subscribe(result => {
      this.lastUpdate = new Date().toLocaleTimeString();
      this.appScenes = result;
    }, error => {
      console.error('AppScenesService.getScenes() Failed!.');
      console.error(error);
    });

    return this.appScenes;
  } 
}

export enum EnumSceneType {
  Nothing = 0, CeilingFan = 1, SmartSwitch = 2, RfSwitch = 3, PiCamera = 4, Lamp = 5
}

export enum EnumSceneCondition {
  None = 0,
  /// <summary>
  /// Is the device available?
  /// </summary>
  Available = (1 << 0),
  /// <summary>
  /// If the the device can be switched on/off, is it ON?
  /// </summary>
  LightIsOn = (1 << 1),
  /// <summary>
  /// If the the device can be switched on/off, is it ON?
  /// </summary>
  FanIsOn = (1 << 2),
  /// <summary>
  /// If the device has a direction, is it reversed?
  /// </summary>
  IsReversed = (1 << 3),
  /// <summary>
  /// If the device has a speed setting, is the speed set to Low?
  /// </summary>
  LowSpeed = (1 << 4),
  /// <summary>
  /// If the device has a speed setting, is the speed set to Medium?
  /// </summary>
  MediumSpeed = (1 << 5),
  /// <summary>
  /// If the device has a speed setting, is the speed set to High?
  /// </summary>
  HighSpeed = (1 << 6),
  /// <summary>
  /// If the Device is a Camera, is It Streaming?
  /// </summary>
  Streaming = (1 << 7),
  /// <summary>
  /// If the Device is a Camera, is It Surveilling?
  /// </summary>
  Surveilling = (1 << 8),
  /// <summary>
  /// If the Device is a SmartSwitch or RfSwitch, is the RelayState On?
  /// </summary>
  RelayStateOn = (1 << 9)
}

export class AppScene {
  sceneId: number;
  deviceId: number;
  sceneType: EnumSceneType;
  sceneTypeDisplay: string;
  sceneConditions: EnumSceneCondition;
  imageName: string;
  sceneName: string;
  sceneStatus: string;   
}

