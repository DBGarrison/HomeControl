import { enableProdMode } from '@angular/core';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';

import { AppModule } from './app/app.module';
import { environment } from './environments/environment';

export function getBaseUrl() {
  return document.getElementsByTagName('base')[0].href;
}

//function getBaseFanUrl() {
//  return location.protocol + "//Fan/"; //192.168.0.230
//}

function getBaseCam1Url() {
  return location.protocol + "//RpiZero1:8080/";
  }

  function getBaseCam2Url() {    
    return location.protocol + "//RpiZero2:8080/";
}

function getBaseCam3Url() {
  return location.protocol + "//RpiZero3:8080/";
}

function getBaseCam4Url() {
  return location.protocol + "//RpiZero4:8085/";
}

function getBaseCam5Url() {
  return location.protocol + "//RpiZero5:8085/";
}

function getBaseCam6Url() {
  return location.protocol + "//RpiZero6:8080/";
}

function getBaseCam7Url() {
  return location.protocol + "//RpiZero7:8080/";
}


function getBaseCam8Url() {
  return location.protocol + "//RpiZero8:8080/";
}


function getBaseCam9Url() {
  return location.protocol + "//RpiZero9:8080/";
}


function getBaseCam10Url() {
  return location.protocol + "//RpiZero10:8080/";
}


function getBaseCam11Url() {
  return location.protocol + "//RpiZero11:8080/";
}


function getBaseCam12Url() {
  return location.protocol + "//RpiZero12:8080/";
}

function getBaseCamOfficeUrl() {
  return location.protocol + "//Rpi1:8080/";
}

//function getBaseHS100Url() {
//  return location.protocol + "//192.168.0.62:2112/"; //RpiZero3
//}

const providers = [
  { provide: 'BASE_URL', useFactory: getBaseUrl, deps: [] },
  //{ provide: 'BASE_FAN_URL', useFactory: getBaseFanUrl, deps: [] },
  { provide: 'BASE_CAM1_URL', useFactory: getBaseCam1Url, deps: [] },
  { provide: 'BASE_CAM2_URL', useFactory: getBaseCam2Url, deps: [] },
  { provide: 'BASE_CAM3_URL', useFactory: getBaseCam3Url, deps: [] },
  { provide: 'BASE_CAM4_URL', useFactory: getBaseCam4Url, deps: [] },
  { provide: 'BASE_CAM5_URL', useFactory: getBaseCam5Url, deps: [] },
  { provide: 'BASE_CAM6_URL', useFactory: getBaseCam6Url, deps: [] },
  { provide: 'BASE_CAM7_URL', useFactory: getBaseCam7Url, deps: [] },
  { provide: 'BASE_CAM8_URL', useFactory: getBaseCam8Url, deps: [] },
  { provide: 'BASE_CAM9_URL', useFactory: getBaseCam9Url, deps: [] },
  { provide: 'BASE_CAM10_URL', useFactory: getBaseCam10Url, deps: [] },
  { provide: 'BASE_CAM11_URL', useFactory: getBaseCam11Url, deps: [] },
  { provide: 'BASE_CAM12_URL', useFactory: getBaseCam12Url, deps: [] },
  { provide: 'BASE_CAM_OFFICE_URL', useFactory: getBaseCamOfficeUrl, deps: [] }
];


if (environment.production) {
  enableProdMode();
}

platformBrowserDynamic(providers).bootstrapModule(AppModule)
  .catch(err => console.log(err));
