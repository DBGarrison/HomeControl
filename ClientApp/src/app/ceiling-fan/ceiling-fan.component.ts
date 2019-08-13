import { Component, Inject, OnInit, OnDestroy } from '@angular/core'; 
import { Observable, Subscription } from 'rxjs/Rx';
import { CeilingFanService } from '../ceilingfan/ceilingfan.service';
import { delay } from 'q';
 

@Component({
  selector: 'app-ceiling-fan',
  templateUrl: './ceiling-fan.component.html'
})

export class CeilingFanComponent implements OnInit, OnDestroy {
  private sub: Subscription;
  fanService: CeilingFanService;
  public fanStatus: FanStatus;
  public lastUpdate: string = "Unavailable";
  public topicon: string;
  public fanlights: string;
  public fandirection: string;
  public speed0: string;
  public speed1: string;
  public speed2: string;
  public speed3: string;
  

  constructor(_fanService: CeilingFanService) {
    this.fanService = _fanService;
    console.log('CeilingFanComponent.constructor(): Entered');
  }

  ngOnInit() {
    //this.callAPI('getStatus');
    var sts = this.fanService.callAPI('getStatus');
    let cnt: number = 0;
    while (!sts && cnt++ < 3) {
      delay(1000);
      sts = this.fanService.callAPI('getStatus');
    }

    if (sts) {
      this.fanStatus = sts;
      this.setImgSrc(false);
    }

    var ticker = Observable.timer(1000, 30000);
    this.sub = ticker.subscribe(x => this.callAPI('getStatus'));
  }
 
  ngOnDestroy() {
    this.sub.unsubscribe();
  }
 
  setImgSrc(defaultImages: boolean) {
    
    console.log('setImgSrc() defaultImages = . ' + defaultImages);

    if (defaultImages) {
      let spd: string = "0";
      let dir: string = "0";
      let lgt: string = "0";

       
      this.lastUpdate = "(Not Available)";
      this.topicon = 'topicon00' + lgt;

      this.fanlights = 'fanlight' + lgt;
      this.fandirection = 'fandirection' + dir;

      this.speed0 = 'speed01';
      this.speed1 = 'speed10';
      this.speed2 = 'speed20';
      this.speed3 = 'speed30';   
    }
    else {
      let spd: string = this.fanStatus.fanSpeed.toString();
      let dir: string = this.fanStatus.isForward ? '0' : '1';
      let lgt: string = this.fanStatus.lightsIsOn ? '1' : '0'; 

      this.lastUpdate = this.fanService.lastUpdate;

      if (this.fanStatus.fanSpeed > 0)
        this.topicon = 'topicon' + spd + dir + lgt;
      else
        this.topicon = 'topicon00' + lgt;

      this.fanlights = 'fanlight' + lgt;
      this.fandirection = 'fandirection' + dir;

      this.speed0 = 'speed0' + ((this.fanStatus.fanSpeed == 0) ? '1' : '0');
      this.speed1 = 'speed1' + ((this.fanStatus.fanSpeed == 1) ? '1' : '0');
      this.speed2 = 'speed2' + ((this.fanStatus.fanSpeed == 2) ? '1' : '0');
      this.speed3 = 'speed3' + ((this.fanStatus.fanSpeed == 3) ? '1' : '0');
    }
  }

  handleClick(id: string) {
    //console.log('Click!', event);
    //var id = event.srcElement.id;
    switch (id) {
      case 'topicon': //Refresh
        this.callAPI('getStatus');
        break;
      case 'fanlights':
        this.callAPI('toggleLights');
        break;
      case 'fandirection':
        this.callAPI('toggleDirection');
        break;
      case 'speed0':
      case 'speed1':
      case 'speed2':
      case 'speed3':
        this.callAPI('setSpeed/' + id.substr(5, 1));
        break;      
    }
  }
 
  callAPI(apiCall: string) {
    //console.log('callAPI() Entered. apiCall: ' + apiCall);
    this.fanStatus = this.fanService.callAPI(apiCall);
    if (this.fanStatus) {
      this.lastUpdate = this.fanService.lastUpdate;
      this.setImgSrc(false);
    } else {
      this.setImgSrc(true);
    }

    //this._http.get<FanStatus>(this._baseFanUrl + apiCall).subscribe(result => {
    //let _url: string = this._baseUrl + 'api/Fan/' + apiCall;

    //this._http.get<FanStatus>(_url).subscribe(result => {
    //  this.fanStatus = result;
    //  this.setImgSrc(false);
    //}, error => {
    //  console.error(apiCall + ' Failed!.');
    //  console.error(error);
    //  this.setImgSrc(true);
    //});
  }
 
}

interface FanStatus {
  fanSpeed: number;
  lightsIsOn: boolean;
  isForward: boolean;
  chkInterval: number;
}
 
