import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';




@Component({
  selector: 'camera-dashboard',
  templateUrl: './camera-dashboard.component2.html',
  styles: ['./camera-dashboard.component.css']
})
export class CameraDashboardComponent {
  title: string = 'Camera Dashboard';
  tiles: Tile[] = [];

  constructor(http: HttpClient,
    //@Inject('BASE_CAM1_URL') _baseCamUrl1: string,
    //@Inject('BASE_CAM2_URL') _baseCamUrl2: string,
    //@Inject('BASE_CAM3_URL') _baseCamUrl3: string,
    //@Inject('BASE_CAM4_URL') _baseCamUrl4: string,
    //@Inject('BASE_CAM5_URL') _baseCamUrl5: string,
    //@Inject('BASE_CAM6_URL') _baseCamUrl6: string,
    //@Inject('BASE_CAM7_URL') _baseCamUrl7: string,
    //@Inject('BASE_CAM8_URL') _baseCamUrl8: string,
    //@Inject('BASE_CAM9_URL') _baseCamUrl9: string,
    //@Inject('BASE_CAM10_URL') _baseCamUrl10: string,
    //@Inject('BASE_CAM11_URL') _baseCamUrl11: string,
    //@Inject('BASE_CAM12_URL') _baseCamUrl12: string,
    //@Inject('BASE_CAM_OFFICE_URL') _baseCamOffice: string

    @Inject('BASE_CAM4_URL') _baseCamUrl4: string,
    @Inject('BASE_CAM5_URL') _baseCamUrl5: string,
    @Inject('BASE_CAM6_URL') _baseCamUrl6: string,
    @Inject('BASE_CAM7_URL') _baseCamUrl7: string,
    @Inject('BASE_CAM9_URL') _baseCamUrl9: string,
    @Inject('BASE_CAM10_URL') _baseCamUrl10: string,
    @Inject('BASE_CAM11_URL') _baseCamUrl11: string) {

    console.log('CameraDashboardComponent::constructor() entered.');
 
    //this.tiles.push(new Tile('RpiZero1', _baseCamUrl1 + '?action=stream'));
    //this.tiles.push(new Tile('RpiZero2', _baseCamUrl2 + '?action=stream'));
    this.tiles.push(new Tile('RpiZero4', _baseCamUrl4 + '?action=stream'));
    this.tiles.push(new Tile('RpiZero5', _baseCamUrl5 + '?action=stream'));
    this.tiles.push(new Tile('RpiZero6', _baseCamUrl6 + '?action=stream'));
    this.tiles.push(new Tile('RpiZero7', _baseCamUrl7 + '?action=stream'));
    //this.tiles.push(new Tile('RpiZero8', _baseCamUrl8 + '?action=stream'));
    this.tiles.push(new Tile('RpiZero9', _baseCamUrl9 + '?action=stream'));
    this.tiles.push(new Tile('RpiZero10', _baseCamUrl10 + '?action=stream'));
    this.tiles.push(new Tile('RpiZero11', _baseCamUrl11 + '?action=stream'));
    //this.tiles.push(new Tile('RpiZero12', _baseCamUrl12 + '?action=stream'));
    //this.tiles.push(new Tile('Office', _baseCamOffice + '?action=stream'));
  }


}

export class Tile {
  cols: number;
  rows: number;
  text: string;
  url: string;
  constructor(_text: string, _url: string) {
    this.cols = 1;
    this.rows = 1;
    this.text = _text;
    this.url = _url;
  }
}
